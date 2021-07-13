using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.Windows.Forms;

namespace Thunderdome
{
    /// <summary>
    /// A control that displays a tree view of the Vault folder structure.
    /// </summary>
    /// <remarks>The control dynamicly builds the tree level-by-level as the user expands the tree.
    /// This way only the needed Folder objects are downloaded from the server.</remarks>
    public partial class FolderBrowseControl : UserControl
    {
        private Connection _vaultConnection;

        /// <summary>
        /// Gets the currently selected folder.  The value will be null if no folder is selected.
        /// </summary>
        public Folder SelectedFolder
        {
            get
            {
                TreeNode node = _folderTreeView.SelectedNode;
                return (Folder) node?.Tag;
            }
        }

        /// <summary>
        /// Initializes the control with Vault data.
        /// Must be called after DocumentService is set.
        /// </summary>
        public FolderBrowseControl(Connection vaultConnection)
        {
            if (vaultConnection == null)
                throw new ArgumentNullException(nameof(vaultConnection));
            InitializeComponent();

            _vaultConnection = vaultConnection;

            Folder root = vaultConnection.WebServiceManager.DocumentService.GetFolderRoot();
            TreeNode rootNode = new TreeNode(root.FullName);
            rootNode.Tag = root;

            _folderTreeView.Nodes.Add(rootNode);
            AddChildFolders(rootNode);
        }

        private void m_folderTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // get the next level in the tree
            _folderTreeView.BeginUpdate();
            foreach (TreeNode node in e.Node.Nodes)
                AddChildFolders(node);
            _folderTreeView.EndUpdate();
        }


        /// <summary>
        /// Make a server call and populate the folder tree 1 level deep
        /// if the folders are already there, no call to the server is made.
        /// </summary>
        private void AddChildFolders(TreeNode parentNode)
        {
            Folder parentFolder = (Folder)parentNode.Tag;

            if (parentFolder.NumClds == parentNode.Nodes.Count)
                return;  // we already have the child nodes

            parentNode.Nodes.Clear();

            Folder[] childFolders = _vaultConnection.WebServiceManager.DocumentService.GetFoldersByParentId(parentFolder.Id, false);
            if (childFolders != null)
            {
                foreach (Folder folder in childFolders)
                {
                    TreeNode childNode = new TreeNode(folder.Name);
                    childNode.Tag = folder;
                    parentNode.Nodes.Add(childNode);
                }
            }
        }
    }
}
