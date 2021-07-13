using System;
using Thunderdome.Model;

namespace Thunderdome.DeploymentController
{
    /// <summary>
    /// Represents error occurred when detecting deployment items
    /// </summary>
    internal class DeploymentContainerException : Exception
    {
        private readonly DeploymentContainer _container;

        /// <summary>
        /// Container that has failed to detect some configuration deployment items
        /// </summary>
        public DeploymentContainer Container { get { return _container; } }


        /// <summary>
        /// Creates instance <c>DeploymentContainerException</c>
        /// </summary>
        /// <param name="container">Container that has failed to detect some configuration deployment items</param>
        public DeploymentContainerException(DeploymentContainer container)
            : base(ExtensionRes.DeploymentContainerExceptionMessage)
        {
            _container = container;
        }
    }
}
