using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace Thunderdome.Model
{
    /// <summary>
    /// Something that gets packaged and deployed
    /// </summary>
    [XmlRoot]
    public abstract class DeploymentItem
    {
        private string _displayName;

        /// <summary>
        /// Gets or sets name to be displayed in configuration window
        /// </summary>
        [XmlAttribute("DisplayName")]
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }


        /// <summary>
        /// Adds files and folders to zip archive
        /// </summary>
        /// <param name="zip">Target zip file to add items to</param>
        /// <param name="key">Key to group files in zip archive</param>
        public abstract void Zip(ZipFile zip, string key);

        /// <summary>
        /// Returns string representation of this instance
        /// </summary>
        /// <returns>String representation of this instance</returns>
        public override string ToString()
        {
            return DisplayName;
        }

        public virtual void CleanUp()
        {
            return;
        }

    }
}