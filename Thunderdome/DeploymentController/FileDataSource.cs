using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System;

namespace Thunderdome.DeploymentController
{
    /// <summary>
    /// Represents file provided data source
    /// </summary>
    public class FileDataSource : IStaticDataSource
    {
        private string _path;

        /// <summary>
        /// Creates and instance of <c>FileDataSource</c> for specific file path
        /// </summary>
        /// <param name="path">Name of file to read data from</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is <c>null</c></exception>
        public FileDataSource(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            _path = path;
        }

        public Stream GetSource()
        {
            return new FileStream(_path, FileMode.Open, FileAccess.Read);
        }
    }
}