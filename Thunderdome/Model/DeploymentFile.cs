using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using Thunderdome.DeploymentController;

namespace Thunderdome.Model
{
    [XmlRoot("DeploymentFile")]
    public class DeploymentFile : DeploymentItem
    {
        private readonly string _path;
        private readonly string _subFolder;

        /// <summary>
        /// Gets deployment file path
        /// </summary>
        [XmlIgnore]
        public string Path { get { return _path; } }

        /// <summary>
        /// Gets deployment file sub-folder
        /// </summary>
        [XmlIgnore]
        public string SubFolder { get { return _subFolder; } }

        /// <summary>
        /// Creates empty instance of <c>DeploymentFile</c>
        /// </summary>
        public DeploymentFile()
        { }

        public DeploymentFile(string path, string displayName, string subFolder = null)
        {
            _path = path;
            DisplayName = displayName;
            _subFolder = subFolder;
        }

        /// <summary>
        /// Adds files and folders to zip archive
        /// </summary>
        /// <param name="zip">Target zip file to add items to</param>
        /// <param name="key">Key to group files in zip archive</param>
        public override void Zip(ZipFile zip, string key)
        {
            string zipPath;
            if (SubFolder == null)
                zipPath = key + "/" + System.IO.Path.GetFileName(Path);
            else
                zipPath = key + "/" + SubFolder + "/" + System.IO.Path.GetFileName(Path);

            zip.Add(new FileDataSource(Path), zipPath);
        }
    }
}