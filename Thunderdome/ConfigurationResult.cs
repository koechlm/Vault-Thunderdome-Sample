using System.Collections;
using System.Linq;
using Autodesk.Connectivity.WebServices;
using Thunderdome.Model;

namespace Thunderdome
{
    /// <summary>
    /// Contains user's configuration
    /// </summary>
    public class ConfigurationResult
    {
        private DeploymentModel _selectedDeployments;

        /// <summary>
        /// Selected configuration items to be deployed
        /// </summary>
        public DeploymentModel SelectedDeployments
        {
            get { return _selectedDeployments; }
            set { _selectedDeployments = value; }
        }

        /// <summary>
        /// Indicates if user has selected at least one configuration item to deploy
        /// </summary>
        public bool AnyItemSelected
        {
            get
            {
                if (_selectedDeployments?.Containers == null)
                    return false;

                return _selectedDeployments.Containers.Any();
            }
        }

    }
}
