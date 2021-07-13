using System;
using System.IO;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Thunderdome.FileOperations;
using Thunderdome.Model;
using Thunderdome.Util;

namespace Thunderdome.DeploymentController
{
    public class ConfigFilesController : DeploymentContainerController
    {
        private const string ShortcutsFileName = "Shortcuts.xml";

        private const string FolderObjects = "Objects";

        private const string FileWorkingFolders = "WorkingFolders.xml";
        private const string FileFilterConfig = "FilterConfig.xml";
        private const string FileGridConfiguration = "GridConfiguration.xml";
        private const string FileGridState = "GridState.xml";
        private const string FileViewStyles = "ViewStyles.xml";
        public const string ControllerKey = "Autodesk.ConfigXML";

        public ConfigFilesController(Connection connection, string vaultName)
            : base(connection, vaultName, ControllerKey)
        { }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                string serverName =AutodeskPathUtil.GetServerSettingsFolderName(Connection);

                string localSettingFolder = AutodeskPathUtil.GetCurrentVaultCommonFolder(Connection, serverName, VaultName);
                SetFileMoveOperation(ShortcutsFileName, folder, Path.Combine(localSettingFolder, FolderObjects),
                    utilSettings);

                localSettingFolder = AutodeskPathUtil.GetCurrentVaultCommonFolder2(Connection, serverName, VaultName);
                SetFileMoveOperation(FileWorkingFolders, folder, Path.Combine(localSettingFolder, FolderObjects),
                    utilSettings);

                localSettingFolder = AutodeskPathUtil.GetCurrentVaultSettingsFolder(serverName, VaultName);
                SetFileMoveOperation(FileFilterConfig, folder, localSettingFolder, utilSettings);
                SetFileMoveOperation(FileGridConfiguration, folder, localSettingFolder, utilSettings);

                localSettingFolder = Path.Combine(localSettingFolder, FolderObjects);
                SetFileMoveOperation(FileGridState, folder, localSettingFolder, utilSettings);
                SetFileMoveOperation(FileViewStyles, folder, localSettingFolder, utilSettings);
            }
            catch
            {
                throw new MoveOperationsException(ExtensionRes.ConfigFilesController_ConfigurationFiles);
            }
        }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer(ExtensionRes.ConfigFilesController_ConfigurationFiles, Key);
            try
            {
                string serverName = AutodeskPathUtil.GetServerSettingsFolderName(Connection);

                string localSettingFolder = AutodeskPathUtil.GetCurrentVaultCommonFolder(Connection, serverName, VaultName);

                string shortcutsPath = Path.Combine(localSettingFolder, FolderObjects, ShortcutsFileName);
                if (File.Exists(shortcutsPath))
                    container.DeploymentItems.Add(new DeploymentFile(shortcutsPath, ExtensionRes.ConfigFilesController_Shortcuts));

                localSettingFolder = AutodeskPathUtil.GetCurrentVaultCommonFolder2(Connection, serverName, VaultName);
                string workingFolder = Path.Combine(localSettingFolder, FolderObjects, FileWorkingFolders);
                if (File.Exists(workingFolder))
                    container.DeploymentItems.Add(new DeploymentFile(workingFolder, ExtensionRes.ConfigFilesController_WorkingFolders));

                localSettingFolder = AutodeskPathUtil.GetCurrentVaultSettingsFolder(serverName, VaultName);

                string filterConfigPath = Path.Combine(localSettingFolder, FileFilterConfig);
                if (File.Exists(filterConfigPath))
                    container.DeploymentItems.Add(new DeploymentFile(filterConfigPath, ExtensionRes.ConfigFilesController_FilterConfiguration));

                string gridConfigPath = Path.Combine(localSettingFolder, FileGridConfiguration);
                if (File.Exists(gridConfigPath))
                    container.DeploymentItems.Add(new DeploymentFile(gridConfigPath, ExtensionRes.ConfigFilesController_GrdiConfiguration));

                string gridStatePath = Path.Combine(localSettingFolder, FolderObjects, FileGridState);
                if (File.Exists(gridStatePath))
                    container.DeploymentItems.Add(new DeploymentFile(gridStatePath, ExtensionRes.ConfigFilesController_GridState));

                string viewStylesPath = Path.Combine(localSettingFolder, FolderObjects, FileViewStyles);
                if (File.Exists(viewStylesPath))
                    container.DeploymentItems.Add(new DeploymentFile(viewStylesPath, ExtensionRes.ConfigFilesController_ViewStyles));

                return container;
            }
            catch
            {
                throw new DeploymentContainerException(container);
            }
        }
    }
}