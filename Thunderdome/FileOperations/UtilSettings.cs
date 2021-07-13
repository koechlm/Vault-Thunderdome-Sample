using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Thunderdome.Util;

namespace Thunderdome.FileOperations
{
    /// <summary>
    /// Settings for the Vault client to pass to the utility.
    /// </summary>
    [XmlRoot("UtilSettings")]
    public class UtilSettings
    {
        public const string FileName = "utilSettings.xml";

        private string _vaultClient;
        private string _tempFolder;
        private VaultEntry _vaultEntry;
        private DateTime _deploymentCheckInDate;
        private List<FileMove> _fileMoveOperations;
        private List<FolderMove> _folderMoveOperations;
        private List<string> _deleteOperations;

        /// <summary>
        /// Gets or sets Vault executable path
        /// </summary>
        [XmlElement("VaultClient")]
        public string VaultClient
        {
            get { return _vaultClient; }
            set { _vaultClient = value; }
        }

        /// <summary>
        /// Gets or sets temporary folder path for file operations
        /// </summary>
        [XmlElement("TempFolder")]
        public string TempFolder
        {
            get { return _tempFolder; }
            set { _tempFolder = value; }
        }

        /// <summary>
        /// Gets or sets vault entry for changing last update time after packaged was deployed
        /// </summary>
        [XmlElement("VaultEntry")]
        public VaultEntry VaultEntry
        {
            get { return _vaultEntry; }
            set { _vaultEntry = value; }
        }

        /// <summary>
        /// Gets or sets deployment file check in date for changing last update time after packaged was deployed
        /// </summary>
        [XmlElement("DeploymentCheckInDate")]
        public DateTime DeploymentCheckInDate
        {
            get { return _deploymentCheckInDate; }
            set { _deploymentCheckInDate = value; }
        }

        /// <summary>
        /// Gets or sets list of FileMove operations
        /// </summary>
        [XmlElement("FileMoveOperations")]
        public List<FileMove> FileMoveOperations
        {
            get { return _fileMoveOperations; }
            set { _fileMoveOperations = value; }
        }

        /// <summary>
        /// Gets or sets list of FolderMove operations
        /// </summary>
        [XmlElement("FolderMoveOperations")]
        public List<FolderMove> FolderMoveOperations
        {
            get { return _folderMoveOperations; }
            set { _folderMoveOperations = value; }
        }

        /// <summary>
        /// Gets or sets list of files to be deleted
        /// </summary>
        [XmlElement("DeleteOperations")]
        public List<string> DeleteOperations
        {
            get { return _deleteOperations; }
            set { _deleteOperations = value; }
        }

        /// <summary>
        /// Creates instance of UtilSettings with no operations
        /// </summary>
        public UtilSettings()
        {
            _fileMoveOperations = new List<FileMove>();
            _folderMoveOperations = new List<FolderMove>();
            _deleteOperations = new List<string>();
        }

        private static string GetXmlFileName()
        {
            return Path.Combine(FileUtil.GetPathOfNewFolder(), FileName);
        }

        /// <summary>
        /// Serializes current instance to file
        /// </summary>
        /// <returns><c>true</c> if data has been saved successfully, <c>false</c> otherwise</returns>
        public bool Save()
        {
            try
            {

                using (StreamWriter writer = new StreamWriter(GetXmlFileName()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UtilSettings));
                    serializer.Serialize(writer, this);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads instance of <c>UtilSettings</c> from file
        /// </summary>
        /// <returns>Instance of <c>UtilSettings</c> if data has been read successfully, <c>null</c> otherwise</returns>
        public static UtilSettings Load()
        {
            try
            {
                using (StreamReader reader = new StreamReader(GetXmlFileName()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UtilSettings));
                    return (UtilSettings)serializer.Deserialize(reader);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes saved file if it exists
        /// </summary>
        public void Delete()
        {
            FileUtil.DeleteFile(GetXmlFileName());
        }
    }
}
