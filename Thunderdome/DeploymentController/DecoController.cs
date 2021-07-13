using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.IO;
using System.Linq;
using Thunderdome.FileOperations;
using Thunderdome.Model;
using Thunderdome.Util;

namespace Thunderdome.DeploymentController
{
    public class DecoController : DeploymentContainerController
    {
        private const string FolderDecoExtension = "Deco";
        private const string FolderControls = "Controls";

        private const string FileSettings = "Settings.xml";

        public DecoController(Connection connection, string vaultName)
            : base(connection, vaultName, "Autodesk.Deco")
        { }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer(ExtensionRes.DecoController_DecoFiles, Key);
            try
            {
                string decoFolder = GetExtensionFolder(FolderDecoExtension);
                string decoSettingsPath = Path.Combine(decoFolder, FileSettings);
                if (File.Exists(decoSettingsPath))
                    container.DeploymentItems.Add(new DeploymentFile(decoSettingsPath, ExtensionRes.DecoController_DecoSettings));

                string decoControlsPath = Path.Combine(decoFolder, FolderControls);
                foreach (string controlPath in DirectoryUtil.GetFilesOrEmpty(decoControlsPath))
                {
                    string name = string.Format(ExtensionRes.DecoController_DecoFile_0, Path.GetFileName(controlPath));
                    container.DeploymentItems.Add(new DeploymentFile(controlPath, name));
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
                string decoFolder = GetExtensionFolder(FolderDecoExtension);
                string decoControlsPath = Path.Combine(decoFolder, FolderControls);

                foreach (string filename in DirectoryUtil.GetFilesOrEmpty(folder).Select(Path.GetFileName))
                {
                    string localFolder = decoControlsPath;
                    if (IsSettingsFile(filename))
                        localFolder = decoFolder;

                    SetFileMoveOperation(filename, folder, localFolder, utilSettings);
                }
            }
            catch
            {
                throw new MoveOperationsException(ExtensionRes.DecoController_DecoFiles);
            }
        }

        private static bool IsSettingsFile(string filename)
        {
            return string.Equals(filename, FileSettings, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}