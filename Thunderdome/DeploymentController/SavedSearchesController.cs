using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.IO;
using Thunderdome.FileOperations;
using Thunderdome.Model;
using Thunderdome.Util;

namespace Thunderdome.DeploymentController
{
    public class SavedSearchesController : DeploymentContainerController
    {
        private const string FolderSearches = "Searches";

        public SavedSearchesController(Connection connection, string vaultName)
            : base(connection, vaultName, "Autodesk.SavedSearch")
        { }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer(ExtensionRes.SavedSearchesController_SavedSearches, Key);
            try
            {
                string serverName = AutodeskPathUtil.GetServerSettingsFolderName(Connection);
                string localSettingFolder = AutodeskPathUtil.GetCurrentVaultCommonFolder(Connection, serverName, VaultName);
                string searchesPath = Path.Combine(localSettingFolder, FolderSearches);
                foreach (string searchPath in DirectoryUtil.GetFilesOrEmpty(searchesPath))
                {
                    string name = string.Format(ExtensionRes.SavedSearchesController_SavedSearch_0, Path.GetFileName(searchPath));
                    container.DeploymentItems.Add(new DeploymentFile(searchPath, name));
                }
                return container;
            }
            catch
            {
                throw new DeploymentContainerException(container);
            }
        }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                string serverName = AutodeskPathUtil.GetServerSettingsFolderName(Connection);
                string localSettingFolder = AutodeskPathUtil.GetCurrentVaultCommonFolder(Connection, serverName, VaultName);
                string searchesPath = Path.Combine(localSettingFolder, FolderSearches);
                foreach (string searchPath in DirectoryUtil.GetFilesOrEmpty(folder))
                {
                    string filename = Path.GetFileName(searchPath);
                    utilSettings.FileMoveOperations.Add(new FileMove
                    {
                        From = searchPath,
                        To = Path.Combine(searchesPath, filename)
                    });
                }
            }
            catch
            {
                throw new MoveOperationsException(ExtensionRes.SavedSearchesController_SavedSearches);
            }
        }
    }
}