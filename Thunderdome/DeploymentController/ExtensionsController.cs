using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.Connectivity.JobProcessor.Extensibility;
using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Thunderdome.FileOperations;
using Thunderdome.Model;
using Thunderdome.Util;
using File = System.IO.File;

namespace Thunderdome.DeploymentController
{
    public class ExtensionsController : DeploymentContainerController
    {
        private const string ThunderdomeExtensionName = "Thunderdome";

        public ExtensionsController(Connection mgr, string vaultName)
            : base(mgr, vaultName, "Autodesk.Extensions")
        { }

        public override DeploymentContainer DetectItems()
        {
            DeploymentContainer container = new DeploymentContainer(ExtensionRes.ExtensionsController_PlugIns, Key);
            try
            {
                ExtensionLoader loader = new ExtensionLoader();
                HashSet<string> folders = new HashSet<string>();

                foreach (Extension<IExplorerExtension> ext in loader.FindExtensions<IExplorerExtension>())
                    AddExtension(container, ext, folders);

                foreach (Extension<IWebServiceExtension> ext in loader.FindExtensions<IWebServiceExtension>())
                    AddExtension(container, ext, folders);

                foreach (Extension<IJobHandler> ext in loader.FindExtensions<IJobHandler>())
                    AddExtension(container, ext, folders);

                return container;
            }
            catch
            {
                throw new DeploymentContainerException(container);
            }
        }

        private void AddExtension(DeploymentContainer container, Extension ext, HashSet<string> folders)
        {
            string[] tokens = ext.ExtensionTypeString.Split(',');
            string assemblyName = tokens[1].Trim();

            if (assemblyName == ThunderdomeExtensionName)
                return;

            string location = Path.GetDirectoryName(ext.Location);
            location = Path.Combine(location, assemblyName + ".dll");

            if (!IsAllowedExtension(location))
                return;

            folders.Add(location);
            string name = Path.GetFileName(location);
            string folder = Path.GetDirectoryName(location);
            container.DeploymentItems.Add(new DeploymentFolder(folder, name));
        }

        public override void SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    ExtensionLoader loader = new ExtensionLoader();
                    ExtensionFolder loaderFolder =
                        new ExtensionFolder(folder, ExtensionFolder.SearchTypeEnum.OneLevelOnly, false);
                    loader.SetExtensionFolders(ArrayUtil.FromSingle(loaderFolder));

                    List<Extension> validExtensions = new List<Extension>();

                    validExtensions.AddRange(loader.FindExtensions<IExplorerExtension>());
                    validExtensions.AddRange(loader.FindExtensions<IWebServiceExtension>());
                    validExtensions.AddRange(loader.FindExtensions<IJobHandler>());

                    foreach (string subFolder in validExtensions.Select(e => Directory.GetParent(e.Location).FullName))
                    {
                        string folderName = Path.GetFileName(subFolder);
                        string targetFolder = Path.Combine(ExtensionLoader.DefaultExtensionsFolder, folderName);
                        utilSettings.FolderMoveOperations.Add(new FolderMove
                        {
                            From = subFolder,
                            To = targetFolder
                        });
                    }
                }
            }
            catch
            {
                throw new MoveOperationsException(ExtensionRes.ExtensionsController_PlugIns);
            }
        }

        /// <summary>
        /// Check to make sure it's an allowed plug-in
        /// </summary>
        private bool IsAllowedExtension(string assemblyFile)
        {
            try
            {
                if (!File.Exists(assemblyFile))
                    return false;

                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
                if (assembly == null)
                    return false;

                byte[] assemblyKey = assembly.GetName().GetPublicKeyToken();

                byte[] onesAndZerosKey = { 0x0c, 0xc2, 0x9e, 0x10, 0x18, 0x77, 0x03, 0x14 };
                byte[] tdKey = { 0xbd, 0xd0, 0x06, 0xd5, 0x31, 0x5f, 0x17, 0x27 };

                return ArrayUtil.Equal(onesAndZerosKey, assemblyKey) || ArrayUtil.Equal(tdKey, assemblyKey);
            }
            catch
            {
                return false;
            }
        }
    }
}