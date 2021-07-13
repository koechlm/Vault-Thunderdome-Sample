using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Thunderdome.Model
{
    /// <summary>
    /// All deployment models
    /// </summary>
    [XmlRoot("Deployments")]
    public partial class DeploymentSet
    {
        [XmlElement("DeploymentModels")]
        public List<DeploymentModel> DeploymentModels { get; set; } = new List<DeploymentModel>();


    }
    public partial class DeploymentSet
    {
        public DeploymentSet() { }

        internal DeploymentModel GetDeploymentByDisplayName(string deploymentName)
        {
            return this.DeploymentModels.FirstOrDefault(m => m.DisplayName.Equals(deploymentName, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Serializes current instance of XML string
        /// </summary>
        /// <returns>XML of current instance serialized, or <c>string.Empty</c> if an error occurred during serialization</returns>
        public string ToXml()
        {
            try
            {
                using (StringWriter writer = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DeploymentSet));
                    serializer.Serialize(writer, this);

                    return writer.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Deserializes <c>DeploymentModel</c> from given XML string.
        /// </summary>
        /// <param name="xml">XML string to deserialize <c>DeploymentModel</c> from </param>
        /// <returns>Deserialized model in case of success. If any error occurred during serialization empty model is returned</returns>
        public static DeploymentSet Load(string xml)
        {
            try
            {
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DeploymentSet));
                    var deploymentSet = serializer.Deserialize(reader) as DeploymentSet;
                    return deploymentSet;
                }
            }
            catch
            {
                return new DeploymentSet();
            }
        }
    }

}
