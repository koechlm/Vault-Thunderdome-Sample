using System.Xml.Serialization;

namespace Thunderdome.FileOperations
{
    /// <summary>
    /// Specifies folder move operation
    /// </summary>
    [XmlRoot("FolderMove")]
    public class FolderMove
    {
        private string _from;
        private string _to;

        /// <summary>
        /// Gets or sets folder path to move from
        /// </summary>
        [XmlElement("From")]
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        /// <summary>
        /// Gets or sets folder path to move to
        /// </summary>
        [XmlElement("To")]
        public string To
        {
            get { return _to; }
            set { _to = value; }
        }
    }
}