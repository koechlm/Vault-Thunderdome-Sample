using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using Thunderdome.DeploymentController;
using Thunderdome.Util;

namespace Thunderdome.Model
{
    [XmlRoot("DeploymentFolder")]
    public class DeploymentFolder : DeploymentItem
    {
        private readonly string _path;

        /// <summary>
        /// Gets deployment folder path
        /// </summary>
        [XmlIgnore]
        public string Path { get { return _path; } }

        /// <summary>
        /// Creates empty instance of <c>DeploymentFolder</c>
        /// </summary>
        public DeploymentFolder()
        { }

        /// <summary>
        /// Creates instance of <c>DeploymentFolder</c> with specified path and displayName
        /// </summary>
        /// <param name="path">Deployment folder path</param>
        /// <param name="displayName">Name to be displayed in configuration window</param>
        public DeploymentFolder(string path, string displayName)
        {
            _path = path.Trim('\\');
            DisplayName = displayName;
        }

        /// <summary>
        /// Adds files and folders to zip archive
        /// </summary>
        /// <param name="zip">Target zip file to add items to</param>
        /// <param name="key">Key to group files in zip archive</param>
        public override void Zip(ZipFile zip, string key)
        {
            ZipFolder(zip, key, Path);
        }

        private void ZipFolder(ZipFile zip, string key, string currentPath)
        {
            string folderName = System.IO.Path.GetFileName(Path);

            foreach (string file in DirectoryUtil.GetFilesOrEmpty(currentPath))
            {
                if (file.EndsWith(".v"))
                {
                    continue;
                }
                else
                {
                    string subPath = System.IO.Path.GetDirectoryName(file);
                    subPath = subPath.Remove(0, Path.Length);
                    subPath = subPath.Replace('\\', '/');
                    subPath = subPath.TrimStart('/');

                    string zipPath;

                    if (subPath.Length > 0)
                        zipPath = key + "/" + folderName + "/" + subPath + "/" + System.IO.Path.GetFileName(file);
                    else
                        zipPath = key + "/" + folderName + "/" + System.IO.Path.GetFileName(file);

                    zip.Add(new FileDataSource(file), zipPath);
                }

            }

            foreach (string folder in DirectoryUtil.GetDirectoriesOrEmpty(currentPath))
                if (!folder.Contains("_V"))
                    ZipFolder(zip, key, folder);
        }
    }
}