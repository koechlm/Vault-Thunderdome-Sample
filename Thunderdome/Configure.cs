using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thunderdome.DeploymentController;
using Thunderdome.Model;
using Thunderdome.Util;
using IO = System.IO;

namespace Thunderdome
{
    /// <summary>
    /// Deployment configuration form
    /// </summary>
    public partial class Configure : Form
    {
        private Connection _conn;
        private bool _isInitializing;

        private TreeView _deploymentTreeView = new DoubleClickCheckFixTreeView();

        //private Folder _deploymentFolder;
        private DeploymentSet _deployments = new DeploymentSet();

        private DeploymentModel _selectedDeployment;

        private List<DeploymentContainerException> _containerExceptions = new List<DeploymentContainerException>();
        private TreeNode _vaultFolderContainerNode;
        private VaultFoldersController _vaultFoldersController;

        public string VaultName { get; set; }

        /// <summary>
        /// Gets user's configuration result
        /// </summary>
        public List<ConfigurationResult> ConfigurationResults
        {
            get
            {
                return _deployments.DeploymentModels.Select(m => new ConfigurationResult() { SelectedDeployments = m }).ToList();
            }
        }

        /// <summary>
        /// Creates instance of deployment configuration form
        /// </summary>
        /// <param name="deployments">Selected configuration items to be deployed</param>
        /// <param name="vaultName">Vault name to deploy package</param>
        /// <param name="conn">Vault connection</param>
        public Configure()
        {

            InitializeComponent();

        }

        public async Task<DialogResult> ShowDialog(DeploymentSet deployments, string vaultName, Connection conn)
        {
            VaultName = vaultName;

            InitializeTreeView();

            _isInitializing = true;
            _conn = conn;

            CleanDeploymentList(deployments, conn);

            _deployments = deployments;
            var defaultDeployment = deployments?.GetDeploymentByDisplayName(ThunderdomeExplorerExtension.DefaultPackageDisplayName);


            if (defaultDeployment == null)
            {
                _deployments.DeploymentModels.Add(new DeploymentModel(ThunderdomeExplorerExtension.DefaultPackageDisplayName));

            }

            _selectedDeployment = _deployments.DeploymentModels.First();

            InitializeDeploymentsList();

            await InitializeTreeViewDataAsync(_selectedDeployment);

            InitializeWarningsButton();
            _isInitializing = false;

            return this.ShowDialog();
        }

        private void CleanDeploymentList(DeploymentSet deployments, Connection conn)
        {
            if (deployments==null)
                return;

            var i = 0;
            while (i<deployments.DeploymentModels.Count)
            {
                var deploymentPackage = VaultUtil.FindLatestFile(conn,deployments.DeploymentModels[i].DeploymentLocation);
                if (deploymentPackage==null)
                {
                    deployments.DeploymentModels.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }
        }

        private void InitializeDeploymentsList()
        {
            _deploymentTypeComboBox.Items.Insert(0, ThunderdomeExplorerExtension.DefaultPackageDisplayName);

            var customDeployments = _deployments.DeploymentModels.Where(m => m.DisplayName.Equals(ThunderdomeExplorerExtension.DefaultPackageDisplayName) == false).Select(m=>m.DisplayName).ToArray();

            _deploymentTypeComboBox.Items.AddRange(customDeployments);
            _deploymentTypeComboBox.SelectedIndex = 0;
        }

        private void InitializeWarningsButton()
        {
            _warningsButton.Text = $"Warnings ({_containerExceptions.Count})";
            _warningsButton.Visible = _containerExceptions.Count > 0;
            _warningsButton.Enabled = _containerExceptions.Count > 0;
        }

        private async Task InitializeTreeViewDataAsync(DeploymentModel deploymentModel)
        {
            _deploymentTreeView.Nodes.Clear();

            MasterController mc = new MasterController(_conn, VaultName);

            foreach (DeploymentContainerController controller in mc.ContainerControllers)
            {
                DeploymentContainer container = DetectItems(controller);
                TreeNode containerNode = new TreeNode();

                if (container.DeploymentItems.Any())
                {
                    containerNode = AddContainerNode(container);

                    AddItemNodes(container, containerNode);
                }
                else if (controller is VaultFoldersController)
                {
                    _vaultFoldersController = controller as VaultFoldersController;

                    containerNode = AddContainerNode(container);
                    _vaultFolderContainerNode = containerNode;

                    var vaultFolderDeployments = deploymentModel.Containers.FirstOrDefault(c => c.Key == controller.Key);

                    if (vaultFolderDeployments != null)
                    {
                        this.Enabled = false;
                        var nbItems = vaultFolderDeployments.DeploymentItems.Count;
                        var waitingForm = new WaitingForm(this,nbItems);
                        waitingForm.Show();

                        for (int i = 0; i < nbItems; i++)
                        {
                            DeploymentVaultFolder item = (DeploymentVaultFolder) vaultFolderDeployments.DeploymentItems[i];
                        waitingForm.SetProgress($"Adding Vault Folder '{item.VaultPath}'...", i+1);
                            await AddVaultItemNodeAsync(item.VaultPath, item.LocalDestinationPath);


                        }

                        waitingForm.Close();
                        this.Enabled = true;

                    }
                }

                if (_vaultFolderContainerNode != null)
                {
                    _vaultFolderContainerNode.Checked = (_vaultFolderContainerNode.Nodes.Cast<TreeNode>().Any(t=>t.Checked));

                }

                if (_isInitializing)
                {
                    SetParentCheckedByChildrenState(containerNode.Nodes.Cast<TreeNode>().FirstOrDefault());
                }

                containerNode.ExpandAll();
            }
        }

        #region Deployment Treeview

        private void InitializeTreeView()
        {
            // Initialization logic moved here, since there is a bug with tree view double click
            // and visual studio x32 can not work with controls compiled in x64 library (which Thunderdome is);
            // The reason of creation of the DoubleClickCheckFixTreeView described in DoubleClickCheckFixTreeView.cs
            _deploymentTreeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _deploymentTreeView.CheckBoxes = true;
            _deploymentTreeView.Location = new Point(128, 120);
            _deploymentTreeView.Name = "_deploymentTreeView";
            _deploymentTreeView.Size = new Size(353, 284);
            _deploymentTreeView.TabIndex = 9;
            _deploymentTreeView.BeforeCheck += DeploymentTreeView__BeforeCheck;
            _deploymentTreeView.AfterCheck += DeploymentTreeView_AfterCheck;
            tableLayoutPanel1.Controls.Add(_deploymentTreeView, 1, 4);
            //tableLayoutPanel1.SetColumnSpan(_deploymentTreeView, 3);

        }

        private void DeploymentTreeView__BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            
            if (e.Node.Tag.GetType()==typeof(DeploymentVaultFolder))
            {
            e.Cancel = ((DeploymentVaultFolder)e.Node.Tag).VaultPath==null;

            }
        }

        private TreeNode AddContainerNode(DeploymentContainer container)
        {
            TreeNode containerNode = _deploymentTreeView.Nodes.Add(container.DisplayName);
            containerNode.Tag = container;
            containerNode.Checked = true;
            containerNode.ExpandAll();
            return containerNode;
        }

        private void AddItemNodes(DeploymentContainer container, TreeNode containerNode)
        {
            foreach (DeploymentItem item in container.DeploymentItems)
            {
                AddItemNode(item, containerNode);
            }
        }

        private TreeNode AddItemNode(DeploymentItem item, TreeNode containerNode)
        {
            TreeNode deploymentItemNode = containerNode.Nodes.Add(item.DisplayName);
            deploymentItemNode.Tag = item;

            var containerKey = ((DeploymentContainer)(containerNode.Tag)).Key;
            deploymentItemNode.Checked = IsItemInDeploymentModel(_selectedDeployment, containerKey, item.DisplayName);

            return deploymentItemNode;
        }

        private async Task AddVaultItemNodeAsync(string vaultPath,string localDestinationPath)
        {

            DeploymentVaultFolder deploymentItem =await  _vaultFoldersController.AddFolderAsync(vaultPath,localDestinationPath);


            var node = AddItemNode(deploymentItem, _vaultFolderContainerNode);
            if (deploymentItem.VaultPath == null)
            {
                node.Checked = false;
                node.ForeColor = Color.Red;

                node.Text = $"{vaultPath} Not found";
            }
            else
            {
                node.Checked = true;
            }

            var extendedDescription = $" => '{localDestinationPath}'";
            if (string.IsNullOrEmpty(localDestinationPath))
            {
                extendedDescription=" => Vault working folder";
            }

            node.Text += extendedDescription;

            _vaultFolderContainerNode.Expand();

        }

        private void DeploymentTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || _isInitializing)
                return;

            _deploymentTreeView.AfterCheck -= DeploymentTreeView_AfterCheck;

            SetParentCheckedByChildrenState(e.Node);
            SetChildNotesChecked(e.Node);

            _deploymentTreeView.AfterCheck += DeploymentTreeView_AfterCheck;
        }

        private static void SetParentCheckedByChildrenState(TreeNode childNode)
        {
            if (childNode==null)
            {
                return;
            }
            TreeNode parent = childNode.Parent;
            if (parent != null)
                parent.Checked = parent.Nodes.Cast<TreeNode>().All(n => n.Checked);
        }

        private static void SetChildNotesChecked(TreeNode root)
        {
            foreach (TreeNode node in root.Nodes)
            {
                node.Checked = root.Checked;
                if (node.Nodes.Count > 0)
                    SetChildNotesChecked(node);
            }
        }

        #endregion Deployment Treeview

        #region DeploymentType ComboBox

        private int _lastSelectedDeploymentTypeId = -1;
        private async void DeploymentType_Changed(object sender, EventArgs e)
        {
            //Do not save if Initialization or not a saved Config
            if (_isInitializing==false && _lastSelectedDeploymentTypeId!=1)
            {

                var saveResult = TransientSave();

                if (saveResult==false)
                {
                    //If error restore the last selection
                    _deploymentTypeComboBox.SelectedIndexChanged -= DeploymentType_Changed;
                    _deploymentTypeComboBox.SelectedIndex = _lastSelectedDeploymentTypeId;
                    _deploymentTypeComboBox.SelectedIndexChanged += DeploymentType_Changed;
                    return;
                }    

                _lastSelectedDeploymentTypeId = _deploymentTypeComboBox.SelectedIndex;
                
            }


            switch (_deploymentTypeComboBox.SelectedIndex)
            {
                case 0: //Default deployment selected
                    _deploymentNameTextBox.Text = ThunderdomeExplorerExtension.DefaultPackageFileName;
                    _deploymentNameTextBox.Enabled = false;

                    _selectedDeployment = _deployments.GetDeploymentByDisplayName(ThunderdomeExplorerExtension.DefaultPackageDisplayName);

                    _deployFolderTextBox.Text = IO.Path.GetDirectoryName(_selectedDeployment.DeploymentLocation);

                    this._forceDeploymentCheckBox.Enabled = true;
                    this._forceDeploymentCheckBox.Checked = _selectedDeployment.ForceUpdate;

                    EnableDeploymentContentsUI(true);

                    break;

                case 1:// New deployment selected
                    _deploymentNameTextBox.Text = string.Empty;
                    _deploymentNameTextBox.Enabled = true;

                    this._forceDeploymentCheckBox.Enabled = false;
                    this._forceDeploymentCheckBox.Checked = false;

                    _deploymentTreeView.Nodes.Clear();


                    EnableDeploymentContentsUI(false);

                    break;

                default:
                    var selectedDeploymentName = _deploymentTypeComboBox.Text;
                    _deploymentNameTextBox.Text = $"{selectedDeploymentName}.td";
                    _deploymentNameTextBox.Enabled = false;

                    _selectedDeployment = _deployments.GetDeploymentByDisplayName(selectedDeploymentName);

                    _deployFolderTextBox.Text = IO.Path.GetDirectoryName(_selectedDeployment.DeploymentLocation);

                    this._forceDeploymentCheckBox.Enabled = false;
                    this._forceDeploymentCheckBox.Checked = false;

                    EnableDeploymentContentsUI(true);

                    break;
            }

            _createButton.Enabled = _deploymentNameTextBox.Enabled;

            _lastSelectedDeploymentTypeId = _deploymentTypeComboBox.SelectedIndex;

            if (_isInitializing==false && _deploymentTypeComboBox.SelectedIndex!=1)
            {
                await InitializeTreeViewDataAsync(_selectedDeployment);
            }


        }

        private void EnableDeploymentContentsUI(bool enable )
        {
            _deploymentTreeView.Enabled = enable;
            _deployFolderTextBox.Enabled = enable;
            _addFolderButton.Enabled = enable;
            _allUsersButton.Enabled = enable;

          
        }


        private void DeploymentType_DrawItem(object sender, DrawItemEventArgs e)

        {
            e.DrawBackground();

            if (e.Index == 1)
            {
                e.Graphics.DrawLine(Pens.Gray, new Point(e.Bounds.Left, e.Bounds.Bottom - 1),

                  new Point(e.Bounds.Right, e.Bounds.Bottom - 1));
            }

            TextRenderer.DrawText(e.Graphics, _deploymentTypeComboBox.Items[e.Index].ToString(),

              _deploymentTypeComboBox.Font, e.Bounds, _deploymentTypeComboBox.ForeColor, TextFormatFlags.Left);

            e.DrawFocusRectangle();
        }

        #endregion DeploymentType ComboBox

        private DeploymentContainer DetectItems(DeploymentContainerController controller)
        {
            try
            {
                return controller.DetectItems();
            }
            catch (DeploymentContainerException e)
            {
                _containerExceptions.Add(e);
                return e.Container;
            }
        }

        private bool IsItemInDeploymentModel(DeploymentModel deploymentModel, string containerKey, string deploymentItemDispName)
        {
            var container = deploymentModel.Containers.FirstOrDefault(c => c.Key == containerKey);
            if (container == null)
            {
                return false;
            }

            var item = container.DeploymentItems.FirstOrDefault(i => i.DisplayName == deploymentItemDispName);

            return (item == null) == false;
        }

        private bool TransientSave(bool setDirty=false)
        {

            DeploymentModel selectedDeployment = GetSelectedDataModel();


            selectedDeployment.ForceUpdate = _forceDeploymentCheckBox.Checked;

                selectedDeployment.Updated = setDirty;

           var deploymentPath = _deployFolderTextBox.Text;

            var deploymentFolder = GetDeploymentFolder(deploymentPath);

            if (deploymentFolder == null)
            {
                return false;
            }

            selectedDeployment.DeploymentFolder = deploymentFolder;

            var deploymentModelName = _deploymentTypeComboBox.Items[_lastSelectedDeploymentTypeId].ToString();

            string deploymentFileName= _deploymentNameTextBox.Text;

            selectedDeployment.DeploymentLocation = VaultUtil.NormalizePath(deploymentFolder.FullName) + deploymentFileName;

            var deploymentId=_deployments.DeploymentModels.FindIndex(m => m.DisplayName == deploymentModelName);

            _deployments.DeploymentModels[deploymentId] = selectedDeployment;

            return true;
        }

        private DeploymentModel GetSelectedDataModel()
        {
            var deploymentModelName = _deploymentTypeComboBox.Items[_lastSelectedDeploymentTypeId].ToString();

            DeploymentModel model = new DeploymentModel(deploymentModelName)
            {
                Containers = new List<DeploymentContainer>()
            };
            foreach (TreeNode containerNode in _deploymentTreeView.Nodes)
            {
                DeploymentContainer container = containerNode.Tag as DeploymentContainer;
                if (container == null)
                    continue;

                DeploymentContainer newContainer = new DeploymentContainer(container.DisplayName, container.Key);
                foreach (TreeNode itemNode in containerNode.Nodes.Cast<TreeNode>().Where(n => n.Checked))
                {
                    DeploymentItem item = itemNode.Tag as DeploymentItem;
                    if (item != null)
                        newContainer.DeploymentItems.Add(item);
                }

                if (newContainer.DeploymentItems.Any())
                    model.Containers.Add(newContainer);
            }

            return model;
        }

        private void AllUsersButton_Click(object sender, EventArgs e)
        {
            SelectFolder(_deployFolderTextBox);
        }

        private void SelectFolder(TextBox textbox)
        {
            FolderBrowseDialog dialog = new FolderBrowseDialog(_conn,false);
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (dialog.SelectedFolder==null)
                {
                    ThunderdomeExplorerExtension.ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_NoFolderSelected);
                    return;
                }

                textbox.Text = dialog.SelectedFolder.FullName;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var setDirty = MessageBox.Show(ExtensionRes.ThunderdomeExplorerExtension_ForceDeploymentUpdate, ExtensionRes.Info,MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK;

            var saveResult =TransientSave(setDirty);

            if (saveResult==false)
            {
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {

            string newDeploymentName= _deploymentNameTextBox.Text;
            if (string.IsNullOrEmpty(newDeploymentName.Trim()))
            {
                ThunderdomeExplorerExtension.ShowErrorMessage(string.Format(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_InvalidDeploymentNmae, newDeploymentName));
                return;
            }

            if (newDeploymentName.EndsWith(".td",StringComparison.InvariantCultureIgnoreCase))
            {
                newDeploymentName = IO.Path.GetFileNameWithoutExtension(_deploymentNameTextBox.Text);

            }

            var isDeploymentExists = (_deployments.GetDeploymentByDisplayName(newDeploymentName)!=null);

            if (isDeploymentExists)
            {
                ThunderdomeExplorerExtension.ShowErrorMessage(string.Format(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_DeploymentNameUsed,newDeploymentName));
                return;
            }

            var isDefaultDeploymentName = newDeploymentName.Equals(System.IO.Path.GetFileNameWithoutExtension(ThunderdomeExplorerExtension.DefaultPackageFileName), StringComparison.InvariantCultureIgnoreCase);

            if (isDefaultDeploymentName)
            {
                ThunderdomeExplorerExtension.ShowErrorMessage(string.Format(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_DeploymentNameUsedForDefault, newDeploymentName));
                return;
            }

            _deployments.DeploymentModels.Add(new DeploymentModel(newDeploymentName));

            this._deploymentTypeComboBox.Items.Add(newDeploymentName);
            this._deploymentTypeComboBox.SelectedIndex = this._deploymentTypeComboBox.Items.Count - 1;
        }

        private async void AddFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowseDialog dialog = new FolderBrowseDialog(_conn,true);
            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            this.Enabled = false;
            var waitingForm = new WaitingForm(this, 2);
            waitingForm.Show();
            waitingForm.SetProgress($"Downloading '{dialog.SelectedFolder.FullName}'...",1);
            await AddVaultItemNodeAsync(dialog.SelectedFolder.FullName,dialog.LocalMappedFolder);
            waitingForm.Close();
            this.Enabled = true;
        }

        private Folder GetDeploymentFolder(string deployFolder)
        {
            if (deployFolder.Length == 0)
            {
                ThunderdomeExplorerExtension.ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_NoDeploymentFolderDefined);
                return null;
            }

            Folder[] results = _conn.WebServiceManager.DocumentService.FindFoldersByPaths(new string[] { VaultUtil.NormalizePath(deployFolder) });
            if (!IsValidFolder(results))
            {
                ThunderdomeExplorerExtension.ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_InvalidDeploymentFolderDefined);
                return null;
            }

            return results[0];
        }

        private bool IsValidFolder(Folder[] results)
        {
            if (results == null || results.Length == 0)
                return false;

            Folder result = results[0];
            if (result == null || result.Cloaked || result.Id < 0)
                return false;

            return true;
        }

        private void WarningsButton_Click(object sender, EventArgs e)
        {
            string message = CreateWarningMessage();
            MessageBox.Show(this, message, "Warnings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private string CreateWarningMessage()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine("Not all deployment data has been collected for following categories");

            foreach (DeploymentContainerException exception in _containerExceptions)
                builder.Append("   ")
                    .AppendLine(exception.Container.DisplayName);

            return builder.ToString();
        }


    }
}