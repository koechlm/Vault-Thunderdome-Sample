using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;

namespace Thunderdome.Util
{
    /// <summary>
    /// Contains utility methods for folder operations
    /// </summary>
    internal static class FileUtil
    {
        private static readonly string NameOfFolder = "Thunderdome 2024";

        /// <summary>
        /// Returns path to assembly CodeBase folder
        /// </summary>
        internal static string GetAssemblyCodeBasePath()
        {
            const string prefix = "file:///";
            string codebase = Assembly.GetExecutingAssembly().CodeBase;
            if (codebase.StartsWith(prefix))
                codebase = codebase.Substring(prefix.Length);

            return Path.GetDirectoryName(codebase);
        }

        /// <summary>
        /// Returns path to executable assembly folder
        /// </summary>
        internal static string GetExetableDirectoryPath()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location);
        }

        /// <summary>
        /// Deletes file if it exists
        /// </summary>
        /// <param name="file">File to delete</param>
        internal static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }

        public static void DeleteDirectory(string path, bool recursive)
        {
            if (Directory.Exists(path) == false)
            {
                return;
            }

            if (recursive)
            {
                var subfolders = Directory.GetDirectories(path);
                foreach (var s in subfolders)
                {
                    DeleteDirectory(s, recursive);
                }
            }
            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {

                File.SetAttributes(f, FileAttributes.Normal);
                File.Delete(f);

            }

            Directory.Delete(path);
        }

        /// <summary>
        /// Returns path of new folder in 'My Documents' for current user
        /// </summary>
        /// <returns><c>folderPath</c> string for settings folder, <c>string.Empty</c> in case of error</returns>
        internal static string GetPathOfNewFolder()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrWhiteSpace(folderPath))
                return string.Empty;

            string pathOfNewFolder = Path.Combine(folderPath, NameOfFolder);
            try
            {
                Directory.CreateDirectory(pathOfNewFolder);
                return pathOfNewFolder;
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
        }
        internal static void RemoveReadOnly(IEnumerable<string> filePaths)
        {
            foreach (string file in filePaths)
            {
                File.SetAttributes(file, FileAttributes.Normal);

            }
        }

        internal static string TransformEnvironmentPath(string path)
        {
            var envVarNamePattern = @"\A%\w+%";
            var envVar = System.Text.RegularExpressions.Regex.Match(path, envVarNamePattern).Value;
            if (envVar == null)
            {
                return path;
            }

            var envPath = Environment.GetEnvironmentVariable(envVar.Trim(new char[] { '%' }));
            if (Directory.Exists(envPath) == false)
            {
                return path;
            }

            var subPath = path.Substring(envVar.Length).TrimStart(new char[] { '/', '\\' });
            return System.IO.Path.Combine(envPath, subPath);

        }
    }
}
