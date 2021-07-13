using System.Collections.Generic;
using System.Xml.Serialization;

namespace Thunderdome.Model
{
    /// <summary>
    /// A collection of similar DeploymentItems
    /// </summary>
    [XmlRoot("DeploymentContainer")]
    [XmlInclude(typeof(DeploymentFile)), XmlInclude(typeof(DeploymentFolder)), XmlInclude(typeof(DeploymentVaultFolder))]
    public class DeploymentContainer
    {
        private string _displayName;
        private List<DeploymentItem> _deploymentItems;
        private string _key;

        /// <summary>
        /// Creates instance of <c>DeploymentContainer</c> with empty deployment items
        /// </summary>
        public DeploymentContainer()
        {
            _deploymentItems = new List<DeploymentItem>();
        }

        /// <summary>
        /// Creates instance of DeploymentContainer using specified displayName and key
        /// </summary>
        /// <param name="displayName">Name to be displayed in configuration window</param>
        /// <param name="key">Container identifier</param>
        public DeploymentContainer(string displayName, string key)
            :this()
        {
            _displayName = displayName;
            _key = key;
        }

        /// <summary>
        /// Gets or sets container identifier
        /// </summary>
        [XmlAttribute("Key")]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

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
        /// Gets or sets list of deployment items
        /// </summary>
        [XmlElement("DeploymentItems")]
        public List<DeploymentItem> DeploymentItems
        {
            get { return _deploymentItems; }
            set { _deploymentItems = value; }
        }

        /// <summary>
        /// Returns string representation of this instance
        /// </summary>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}