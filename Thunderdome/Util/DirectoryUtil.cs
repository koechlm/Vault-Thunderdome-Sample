using System.IO;

namespace Thunderdome.Util
{
    /// <summary>
    /// Contains utility methods for folder operations
    /// </summary>
    internal static class DirectoryUtil
    {
        /// <summary>
        /// Empty string array specifying no paths
        /// </summary>
        internal static readonly string[] Empty = new string[0];

        /// <summary>
        /// Returns array of file names under <paramref name="path"/>.
        /// If there was any error accessing the path, empty array is returned
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Non-null array of file names under <paramref name="path"/></returns>
        internal static string[] GetFilesOrEmpty(string path)
        {
            try
            {
                return Directory.GetFiles(path);
            }
            catch
            {
                return Empty;
            }
        }

        /// <summary>
        /// Returns array of directory names under <paramref name="path"/>.
        /// If there was any error accessing the path, empty array is returned
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Non-null array of directory names under <paramref name="path"/></returns>
        internal static string[] GetDirectoriesOrEmpty(string path)
        {
            try
            {
                return Directory.GetDirectories(path);
            }
            catch
            {
                return Empty;
            }
        }

        /// <summary>
        /// Recursively deletes files and directories under <paramref name="path"/> and the <paramref name="path"/> directory.
        /// </summary>
        /// <param name="path">Directory path to be deleted</param>
        internal static void DeleteRecursivelly(string path)
        {
            foreach (var dir in GetDirectoriesOrEmpty(path))
                DeleteRecursivelly(dir);

            foreach (var file in GetFilesOrEmpty(path))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            
            Directory.Delete(path);
        }
    }
}
