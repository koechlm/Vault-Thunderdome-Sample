using System.Xml.Serialization;

namespace Thunderdome.FileOperations
{
    /// <summary>
    /// Specifies file move operation
    /// </summary>
    [XmlRoot("FileMove")]
    public class FileMove
    {
        private string _from;
        private string _to;

        /// <summary>
        /// Specifies file name to move from
        /// </summary>
        [XmlElement("From")]
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        /// <summary>
        /// Specifies file name to move to
        /// </summary>
        [XmlElement("To")]
        public string To
        {
            get { return _to; }
            set { _to = value; }
        }
    }
}