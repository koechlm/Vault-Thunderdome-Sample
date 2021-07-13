using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System.IO;
using Thunderdome.FileOperations;
using Thunderdome.Model;

namespace Thunderdome.DeploymentController
{
    /// <summary>
    /// controls how things are packaged, deployed for a set of items
    /// </summary>
    public abstract class DeploymentContainerController
    {
        private Connection _connection;
        private string _vaultName;
        private string _key;


        protected DeploymentContainerController(Connection connection, string vaultName, string key)
        {
            _connection = connection;
            _vaultName = vaultName;
            _key = key;
        }

        public string Key
        {
            get { return _key; }
            protected set { _key = value; }
        }

        protected Connection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        protected string VaultName
        {
            get { return _vaultName; }
            set { _vaultName = value; }
        }


        protected static string GetExtensionFolder(string subfolder)
        {
            return Path.Combine(ExtensionLoader.DefaultExtensionsFolder, subfolder);
        }


        protected void SetFileMoveOperation(string filename, string tempFolder, string localFolder, UtilSettings utilSettings)
        {
            // if the file exists in the temp location, mark it for move
            string tempFile = Path.Combine(tempFolder, filename);
            if (File.Exists(tempFile))
                utilSettings.FileMoveOperations.Add(new FileMove
                {
                    From = tempFile,
                    To = Path.Combine(localFolder, filename)
                });
        }


        /// <param name="recurseFolders">If false, only get the files within the folder.
        /// If true, get the files in the folder and any sub-folders</param>
        protected void SetFolderMergeMoveOperations(string tempFolder, string localFolder, UtilSettings utilSettings, bool recurseFolders)
        {
            if (Directory.Exists(tempFolder))
            {
                string[] controlPaths = GetControlPaths(tempFolder, recurseFolders);
                foreach (string controlPath in controlPaths)
                {
                    string fileName = controlPath.Substring(tempFolder.Length + 1);
                    SetFileMoveOperation(fileName, tempFolder, localFolder, utilSettings);
                }
            }
        }

        private static string[] GetControlPaths(string tempFolder, bool recurseFolders)
        {
            if (!recurseFolders)
                return Directory.GetFiles(tempFolder);

            return Directory.GetFiles(tempFolder, "*", SearchOption.AllDirectories);
        }

        public abstract DeploymentContainer DetectItems();
        public abstract void SetMoveOperations(string folder, UtilSettings utilSettings);
    }
}
