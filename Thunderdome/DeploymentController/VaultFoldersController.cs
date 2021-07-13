using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.Linq;
using System.Threading.Tasks;
using Thunderdome.FileOperations;
using Thunderdome.Model;
using Thunderdome.Util;
using IO = System.IO;

namespace Thunderdome.DeploymentController
{
    public class VaultFoldersController : DeploymentContainerController
    {
        private static readonly string[] ConfigNames = { ".thunderdome", "_thunderdome" };

        private static string RootVaultLocalFolder { get; set; }

        public VaultFoldersController(Connection connection, string vaultName) : base(connection, vaultName, "Autodesk.vaultFolder")
        {
            RootVaultLocalFolder = connection.WorkingFoldersManager.GetWorkingFolder("$").FullPath;
        }

        private DeploymentContainer p_Container = null;

        private DeploymentContainer Container
        {
            get
            {
                if (p_Container == null)
                {
                    p_Container = new DeploymentContainer(ExtensionRes.Category_VaultFoldersCustomizations, Key);
                }
                return p_Container;
            }
        }

        public override DeploymentContainer DetectItems()
        {
            return Container;
        }

        private static string GetTmpRootFolder()
        {
            return IO.Path.Combine(RootVaultLocalFolder, ConfigNames[0]);
        }

        public static void CleanUpTmpRootFolder()
        {
            var tmpFolder = GetTmpRootFolder();
            if (IO.Directory.Exists(tmpFolder))
            {
                try
                {
                    IO.Directory.Delete(tmpFolder, true);
                }
                catch { }
            }
        }

        public async Task<DeploymentVaultFolder> AddFolderAsync(string vaultFolder, string localDestinationPath)
        {
            Folder folder = null;
            try
            {
                folder = Connection.WebServiceManager.DocumentService.GetFolderByPath(vaultFolder);

            }
            catch 
            {
                //Fails if the folder not found
           }

            if (folder==null)
            {
                return new DeploymentVaultFolder();
            }

            var tmpLocalFolder = GetUniqueLocalFolder(vaultFolder);

            FileUtil.DeleteDirectory(tmpLocalFolder, true);
            IO.Directory.CreateDirectory(tmpLocalFolder);

            var downloadResults = await Util.VaultUtil.DownloadFolderAsync(Connection, folder, true, false, tmpLocalFolder);
            FileUtil.RemoveReadOnly(downloadResults.FileResults.Select(f => f.LocalPath.FullPath));

            var configFilePath = IO.Path.Combine(tmpLocalFolder, ConfigNames[0]);
            IO.File.WriteAllLines(configFilePath, new string[] { $"{vaultFolder};{localDestinationPath}" });

            var displayName = vaultFolder.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();

            var item = new DeploymentVaultFolder(vaultFolder, tmpLocalFolder, localDestinationPath, displayName);

            Container.DeploymentItems.Add(item);

            return item;
        }

        private string GetUniqueLocalFolder(string vaultFolder)
        {
            var tmpRootFolder = GetTmpRootFolder();

            return IO.Path.Combine(tmpRootFolder, IO.Path.GetRandomFileName());
        }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                var subFolders = IO.Directory.GetDirectories(folder);
                foreach (var subFolder in subFolders)
                {
                    MoveFolder(subFolder, utilSettings);
                }
            }
            catch
            {
                throw new MoveOperationsException(ExtensionRes.Category_DataStandardCustomizations);
            }
        }

        private void MoveFolder(string folder, UtilSettings utilSettings)
        {
            try
            {
                var configFilePath = IO.Path.Combine(folder, ConfigNames[0]);
                if (IO.File.Exists(configFilePath) == false)
                {
                    configFilePath = IO.Path.Combine(folder, ConfigNames[1]);
                }

                string[] config = IO.File.ReadAllLines(configFilePath);

                string[] vaultFolderMapping = config.First().Split(';');

                string vaultFolderSrc = vaultFolderMapping[0];

                //if no folder mapped, then used the default local Vault path in the working folder
                string destFolder = vaultFolderMapping[1];
                if (string.IsNullOrEmpty(destFolder))
                {
                    destFolder = Connection.WorkingFoldersManager.GetWorkingFolder(vaultFolderSrc).FullPath;
                }
                else
                {
                    destFolder= FileUtil.TransformEnvironmentPath(destFolder);
                }

                SetFolderMergeMoveOperations(folder, destFolder, utilSettings, true);

                //Exclude Vault folder config files for the deployments
                utilSettings.FileMoveOperations = utilSettings.FileMoveOperations.Where(f => f.From.EndsWith(ConfigNames[0])==false && f.From.EndsWith(ConfigNames[1])==false).ToList();

            }
            catch
            {
                throw new MoveOperationsException(ExtensionRes.Category_DataStandardCustomizations);
            }
        }
    }
}