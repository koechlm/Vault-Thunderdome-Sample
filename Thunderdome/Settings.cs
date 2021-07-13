using System;
using System.Collections.Generic;
using IO=System.IO;
using System.Linq;
using System.Xml.Serialization;
using Thunderdome.Util;
using AWS = Autodesk.Connectivity.WebServices;

namespace Thunderdome
{
    /// <summary>
    /// Settings storage model
    /// </summary>
    [XmlRoot("Settings")]
    public class Settings
    {
        private const string SettingsFile = "settings.xml";

        private bool _neverDownload;
        private List<VaultEntry> _vaultEntries;

        /// <summary>
        /// Gets or sets list of vault entries containing information about last 
        /// configuration update time for particular server and vault on that server
        /// </summary>
        [XmlElement("VaultEntry")]
        public List<VaultEntry> VaultEntries
        {
            get { return _vaultEntries; }
            set { _vaultEntries = value; }
        }

        /// <summary>
        /// Creates instance with default settings
        /// </summary>
        public Settings()
        {
            NeverDownload = false;
            VaultEntries = new List<VaultEntry>();
        }

        /// <summary>
        /// Gets or sets value indicating that user never wants to download new configuration packages
        /// </summary>
        [XmlElement("NeverDownload")]
        public bool NeverDownload
        {
            get { return _neverDownload; }
            set { _neverDownload = value; }
        }

        /// <summary>
        /// Gets <c>VaultEntry</c> be server and vault name.
        /// </summary>
        /// <returns>Existing instance of <c>VaultEntry</c> if there is one, or creates a new instance if there is no existing</returns>
        public VaultEntry GetOrCreateEntry(string serverName, string vaultName, string deploymentName)
        {
            VaultEntry entry = null;

            var deploymentLatestUpdates = VaultEntries.Where(n =>
                    string.Equals(n.VaultName, vaultName, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(n.DeploymentName, deploymentName, StringComparison.InvariantCultureIgnoreCase));


            if (deploymentLatestUpdates.Any())
            {
                var lastUpdate = deploymentLatestUpdates.Max(n => n.LastUpdate);
                deploymentLatestUpdates = deploymentLatestUpdates.Where(n => n.LastUpdate == lastUpdate);
            }

           
            if (serverName==string.Empty)
            {

                entry = deploymentLatestUpdates.FirstOrDefault();
            }
            else
            {
                entry = deploymentLatestUpdates.FirstOrDefault(n =>
                    string.Equals(n.ServerName, serverName, StringComparison.InvariantCultureIgnoreCase));
            }


            if (entry == null)
            {
                entry = new VaultEntry
                {
                    ServerName = serverName,
                    VaultName = vaultName,
                    DeploymentName=deploymentName,
                    LastUpdate = DateTime.MinValue
                };
                VaultEntries.Add(entry);
            }
            return entry;
        }

        /// <summary>
        /// Sets last update time for given server and vault
        /// </summary>
        public void SetLastUpdate(string serverName, string vaultName, string deploymentName, DateTime dateTime)
        {
            VaultEntry entry = this.VaultEntries.FirstOrDefault(e => e.ServerName == serverName && e.VaultName == vaultName && e.DeploymentName == deploymentName);
            if (entry==null)
            {
                entry = GetOrCreateEntry(serverName, vaultName, deploymentName);
            }
            //VaultEntry entry = GetOrCreateEntry(serverName, vaultName, deploymentName);
            entry.LastUpdate = dateTime;
        }

        /// <summary>
        /// Saves data of this instance to file
        /// </summary>
        /// <returns>Value indicating that data has been saved successfully</returns>
        public bool Save()
        {
            try
            {
                using (IO.StreamWriter writer = new IO.StreamWriter(GetSettingsFileName()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
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
        /// Loads instance from file
        /// </summary>
        /// <returns>If file exists, instance of <c>Settings</c> loaded from file is returned, otherwise new empty instance is created</returns>
        public static Settings Load()
        {
            try
            {
                using (IO.StreamReader reader = new IO.StreamReader(GetSettingsFileName()))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    return (Settings)serializer.Deserialize(reader);
                }
            }
            catch
            {
                return new Settings();
            }
        }

        private static string GetSettingsFileName()
        {
            string newFolder = FileUtil.GetPathOfNewFolder();
            return IO.Path.Combine(newFolder, SettingsFile);
        }
    }

    /// <summary>
    /// Contains update information for particular server and vault on that server
    /// </summary>
    public class VaultEntry
    {
        private DateTime _lastUpdate;
        private string _serverName;
        private string _vaultName;

        [XmlElement("LastUpdate")]
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        /// <summary>
        /// Ser
        /// </summary>
        [XmlElement("ServerName")]
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        /// <summary>
        /// Name of vault
        /// </summary>
        [XmlElement("VaultName")]
        public string VaultName
        {
            get { return _vaultName; }
            set { _vaultName = value; }
        }

        /// <summary>
        /// Name of deployment
        /// </summary>
        [XmlElement("DeploymentName")]
        public string DeploymentName { get; set; }

        /// <summary>
        /// Name of deployment
        /// </summary>
        [XmlIgnore]
        public AWS.File DeploymentPackage { get; set; }

    }
}
