using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Serialization;
using IO = System.IO;
namespace Thunderdome.Model
{
    [XmlRoot("DeploymentVaultFolder")]
    public class DeploymentVaultFolder : DeploymentFolder
    {
        public DeploymentVaultFolder() { }
        public string VaultPath { get; set; }
        public string LocalDestinationPath { get; set; }
        public DeploymentVaultFolder(string vaultPath, string srcLocalPath, string destLocalPath, string displayName) : base(srcLocalPath.Trim('\\'), displayName)
        {
            VaultPath = vaultPath;
            LocalDestinationPath = destLocalPath;
        }

        public override void Zip(ZipFile zip, string key)
        {
            ZipFolder(zip, key, Path);
        }

        private void ZipFolder(ZipFile zip, string key, string currentPath)
        {

            base.Zip(zip, key);
        }

        public override void CleanUp()
        {
            if (IO.Directory.Exists(Path))
            {
                try
                {
                    IO.Directory.Delete(Path, true);
                }
                catch { }
            }
        }
    }
}
