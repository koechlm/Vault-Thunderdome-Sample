using Autodesk.Connectivity.WebServices;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Thunderdome.Model
{
    /// <summary>
    /// General deployment model containing configuration items to be deployed
    /// </summary>
    [XmlRoot("DeploymentModel")]
    public class DeploymentModel
    {
        private DeploymentModel() { }

        /// <summary>
        /// Creates instance of <c>DeploymentModel</c>
        /// </summary>
        public DeploymentModel(string displayName)
        {
            Containers = new List<DeploymentContainer>();
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Gets or sets the location of the deployment in Vault
        /// </summary>
        [XmlElement("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the location of the deployment in Vault
        /// </summary>
        [XmlElement("DeploymentLocation")]
        public string DeploymentLocation { get; set; }


        [XmlIgnore]
        public Folder DeploymentFolder { get; set; }

        /// <summary>
        /// Gets or sets list of deployment containers
        /// </summary>
        [XmlElement("Containers")]
        public List<DeploymentContainer> Containers { get; set; }

        /// <summary>
        /// Gets or sets if the end can discard the deployemnt
        /// </summary>
        [XmlElement("ForceUpdate")]
        public bool ForceUpdate { get; set; }

        /// <summary
        /// Gets or sets if the deployment was updated by the user
        /// </summary>
        [XmlIgnore]
        public bool Updated { get; set; }

    }
}