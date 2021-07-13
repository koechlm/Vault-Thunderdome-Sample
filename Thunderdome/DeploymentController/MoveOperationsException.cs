using System;

namespace Thunderdome.DeploymentController
{
    /// <summary>
    /// Describes error occurred when setting up move operations in deployment controller
    /// </summary>
    internal class MoveOperationsException: Exception
    {
        private readonly string _category;

        /// <summary>
        /// Gets name of configuration category where controller has failed to setup move operations
        /// </summary>
        public string Category { get { return _category; } }

        /// <summary>
        /// Creates instance of <c>MoveOperationException</c>
        /// </summary>
        /// <param name="category"></param>
        public MoveOperationsException(string category)
            :base(ExtensionRes.MoveOperationsExceptionMessage)
        {
            _category = category;
        }
    }
}
