using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Thunderdome.FileOperations;
using Thunderdome.Model;
using Thunderdome.Util;

namespace Thunderdome.DeploymentController
{
    public class DataStandardController : DeploymentContainerController
    {
        private const string FolderDataStandard = "DataStandard";
        private const string FolderVault = "Vault";
        private const string FolderCad = "CAD";
        private const string FolderVaultCustom = "Vault.Custom";
        private const string FolderCadCustom = "CAD.Custom";
        private const string FolderHelpFiles = "HelpFiles";

        public DataStandardController(Connection connection, string vaultName)
            : base(connection, vaultName, "Autodesk.dataStandard")
        { }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer(ExtensionRes.Category_DataStandardCustomizations, Key);
            try
            {
                string dsFolder = GetExtensionFolder(FolderDataStandard);
                Regex regex1 = new Regex(@"^\w\w-\w\w$");
                Regex regex2 = new Regex(@"^\w\w$");

                foreach (string folder in DirectoryUtil.GetDirectoriesOrEmpty(dsFolder))
                {
                    string folderName = Path.GetFileName(folder);
                    if (folderName.Equals(FolderVault, StringComparison.InvariantCultureIgnoreCase))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, ExtensionRes.DataStandardController_Vault));
                    else if (folderName.Equals(FolderCad, StringComparison.InvariantCultureIgnoreCase))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, ExtensionRes.DataStandardController_CAD));
                    else if (folderName.Equals(FolderVaultCustom, StringComparison.InvariantCultureIgnoreCase))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, ExtensionRes.DataStandardController_Vault_Custom));
                    else if (folderName.Equals(FolderCadCustom, StringComparison.InvariantCultureIgnoreCase))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, ExtensionRes.DataStandardController_CAD_Custom));
                    else if (folderName.Equals(FolderHelpFiles, StringComparison.InvariantCultureIgnoreCase))
                        container.DeploymentItems.Add(new DeploymentFolder(folder, ExtensionRes.DataStandardController_HelpFiles));
                    else if (regex1.IsMatch(folderName) || regex2.IsMatch(folderName))

                        container.DeploymentItems.Add(new DeploymentFolder(folder, string.Format(ExtensionRes.DataStandardController_0_LocalizationStrings, folderName)));
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
                string dsFolder = GetExtensionFolder(FolderDataStandard);
                foreach (string tempFolder in DirectoryUtil.GetDirectoriesOrEmpty(folder))
                {
                    string folderName = Path.GetFileName(tempFolder);
                    string localFolder = Path.Combine(dsFolder, folderName);

                    SetFolderMergeMoveOperations(tempFolder, localFolder, utilSettings, true);
                }
            }
            catch
            {
                throw new MoveOperationsException(ExtensionRes.Category_DataStandardCustomizations);
            }
        }
    }
}