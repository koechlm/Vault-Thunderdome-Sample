using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Thunderdome.FileOperations;

namespace Thunderdome.DeploymentController
{
    /// <summary>
    /// Controls the entire data model
    /// </summary>
    public class MasterController
    {
        private readonly List<DeploymentContainerController> _containerControllers;

        /// <summary>
        /// Gets list of <c>DeploymentContainerController</c> instances
        /// </summary>
        public List<DeploymentContainerController> ContainerControllers { get { return _containerControllers; } }

        /// <summary>
        /// Creates instance of <c>MasterController</c> using connection and vault name
        /// </summary>
        /// <param name="connection">Vault server connection</param>
        /// <param name="vaultName">Vault name</param>
        public MasterController(Connection connection, string vaultName)
        {
            _containerControllers = new List<DeploymentContainerController>()
            {
                new ConfigFilesController(connection, vaultName),
                new SavedSearchesController(connection, vaultName),
                new ExtensionsController(connection, vaultName),
                new DecoController(connection, vaultName),
                new DataStandardController(connection, vaultName),
                new VaultFoldersController(connection,vaultName)
            };
        }

        /// <summary>
        /// Sets all move operations to <paramref name="utilSettings"/>
        /// </summary>
        /// <param name="folder">Root folder for move operations</param>
        /// <param name="utilSettings">Target instance to set move operations to</param>
        /// <returns></returns>
        public MoveOperationsConfigurationResult SetMoveOperations(string folder, UtilSettings utilSettings)
        {
            List<string> failedCategories = new List<string>();
            foreach (string subFolder in Directory.GetDirectories(folder))
            {
                string subName = Path.GetFileName(subFolder);
                DeploymentContainerController controller = FindControllder(subName);
                try
                {
                    controller?.SetMoveOperations(subFolder, utilSettings);
                }
                catch (MoveOperationsException e)
                {
                    failedCategories.Add(e.Category);
                }
            }
            return new MoveOperationsConfigurationResult(failedCategories);
        }

        private DeploymentContainerController FindControllder(string key)
        {
            StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
            return ContainerControllers.SingleOrDefault(c => string.Equals(c.Key, key, comparison));
        }
    }

    /// <summary>
    /// Describes result of <c>MasterController.SetMoveOperations()</c> method
    /// </summary>
    public class MoveOperationsConfigurationResult
    {
        private IEnumerable<string> _failedCategories;

        /// <summary>
        /// Get collection of names configuration categories where controller has failed to setup move operations
        /// </summary>
        public IEnumerable<string> FailedCategories { get { return _failedCategories; } }

        /// <summary>
        /// Creates instance of <c>MoveOperationsConfigurationResult</c>
        /// </summary>
        /// <param name="failedCategories">Collection of categories where controller has failed to setup move operations</param>
        public MoveOperationsConfigurationResult(IEnumerable<string> failedCategories)
        {
            _failedCategories = failedCategories;
        }

        /// <summary>
        /// Gets value indicating that all controllers has succeed to setup move operations
        /// </summary>
        public bool IsFullySuccessful { get { return !_failedCategories.Any(); } }

        /// <summary>
        /// Returns string representation of this instance
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach(string categoryName in _failedCategories)
                builder.Append("   ").AppendLine(categoryName);

            return builder.ToString();
        }
    }
}