using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Thunderdome
{
    /// <summary>
    /// A dialog which allows the user to select a Vault folder.
    /// </summary>
    public partial class FolderBrowseDialog : Form
    {
        private FolderBrowseControl _folderBrowseControl;
     
        /// <summary>
        /// Gets the currently selected folder. The value will be null if no folder is selected.
        /// </summary>
        public Folder SelectedFolder
        {
            get
            {
                return _folderBrowseControl.SelectedFolder;
            }
        }

        public string LocalMappedFolder
        {
            get
            {
                return _localDestFolderTextBox.Text;
            }
        }

        /// <summary>
        /// Creates instance of FolderBrowseDialog
        /// </summary>
        /// <param name="connection">Vault server connection</param>
        public FolderBrowseDialog(Connection connection, bool isVaultMapping)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            InitializeComponent();

            _folderBrowseControl = new FolderBrowseControl(connection);
            _folderBrowseControl.Location = new Point(10, 10);

           EnableVaultMappingUI(isVaultMapping);

           if (isVaultMapping)
            {
                _folderBrowseControl.Size = new Size(Size.Width - 40, _setCustomDestCheckBox.Top - 20);

            }
            else
            {
                _folderBrowseControl.Size = new Size(Size.Width - 40, _cancelButton.Top - 20);
            }

            _folderBrowseControl.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            Controls.Add(_folderBrowseControl);
            
        }

        private void EnableVaultMappingUI(bool enable)
        {
            _setCustomDestCheckBox.Visible = enable;
            _folderBrowserButton.Visible = enable;
            _localDestFolderTextBox.Visible = enable;


        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            var localFolder = _localDestFolderTextBox.Text;

            localFolder = Util.FileUtil.TransformEnvironmentPath(localFolder);

            if (string.IsNullOrEmpty(localFolder)==false && Directory.Exists(localFolder)==false)
            {
                ThunderdomeExplorerExtension.ShowErrorMessage(string.Format(ExtensionRes.ThunderdomeExplorerExtension_MappedFolderNotExist, _localDestFolderTextBox.Text));
                return;
            }

            DialogResult = DialogResult.OK;
        }



        private void _folderBrowserButton_Click(object sender, EventArgs e)
        {
            var localFolder = _localDestFolderTextBox.Text;
            if (Directory.Exists(localFolder))
            {
                _folderBrowserDialog.SelectedPath = localFolder;
            }

            if (_folderBrowserDialog.ShowDialog()!= DialogResult.OK)
            {
                return;
            }

            _localDestFolderTextBox.Text = _folderBrowserDialog.SelectedPath;
        }

        private void _setCustomDestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _folderBrowserButton.Enabled = _setCustomDestCheckBox.Checked;
            _localDestFolderTextBox.Enabled = _setCustomDestCheckBox.Checked;

            if (_setCustomDestCheckBox.Checked)
            {
                _localDestFolderTextBox.Text = string.Empty;
            }

        }
    }
}
