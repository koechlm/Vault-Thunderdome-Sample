using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Currency;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using Autodesk.DataManagement.Client.Framework.Vault.Results;
using Autodesk.DataManagement.Client.Framework.Vault.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AWS = Autodesk.Connectivity.WebServices;

using IO = System.IO;

namespace Thunderdome.Util
{
    /// <summary>
    /// Contains Vault API utilities
    /// </summary>
    public static class VaultUtil
    {
        private const int ConfigAdminRoleId = 22; //Configuration Administrator Role

        /// <summary>
        /// Tells if the logged in user is an admin or not.
        /// </summary>
        /// <returns>True if the user is an administrator. Otherwise false.</returns>
        public static bool IsAdmin(Connection conn)
        {
            long userId = conn.WebServiceManager.SecurityService.Session.User.Id;
            if (userId > 0)
            {
                AWS.Permis[] ConfigAdminPermis;
                try
                {
                    ConfigAdminPermis = conn.WebServiceManager.AdminService.GetPermissionsByRoleId(ConfigAdminRoleId);
                }
                catch (System.Exception ex)
                {

                    if (ex.Message=="303")
                    {
                        return false;
                    }

                    throw ex;
                }
                AWS.Permis[] permissions = conn.WebServiceManager.AdminService.GetPermissionsByUserId(userId);

                //Check if the user has the the Admin Config Role
                foreach (AWS.Permis perm in ConfigAdminPermis)
                {
                    if (permissions.FirstOrDefault(p => p.Id == perm.Id) == null)
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds file to vault if it is new, or updates if the file already exists in vault
        /// </summary>
        /// <param name="localPath">Local file path</param>
        /// <param name="fileName">File name to be added</param>
        /// <param name="vaultFolder">Target vault folder</param>
        /// <param name="conn">Vault connection</param>
        /// <returns>Reference to vault file</returns>
        public static async Task<Autodesk.Connectivity.WebServices.File> AddOrUpdateFileAsync(string localPath,
            string fileName, Autodesk.Connectivity.WebServices.Folder vaultFolder, Connection conn)
        {
            return await Task.Run<Autodesk.Connectivity.WebServices.File>(() =>
            {
                string vaultPath = vaultFolder.FullName + "/" + fileName;

                Autodesk.Connectivity.WebServices.File[] result = conn.WebServiceManager.DocumentService.FindLatestFilesByPaths(
                    ArrayUtil.FromSingle(vaultPath));

                if (result == null || result.Length == 0 || result[0].Id < 0)
                {
                    var vdfFolder = new Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities.Folder(conn, vaultFolder);

                    // using a stream so that we can set a different file name
                    using (IO.FileStream stream = new IO.FileStream(localPath, IO.FileMode.Open, IO.FileAccess.Read))
                    {
                        FileIteration newFile = conn.FileManager.AddFile(
                            vdfFolder, fileName, ExtensionRes.VaultUtil_ThunderdomeDeployment,
                            System.IO.File.GetLastWriteTime(localPath), null, null,
                            AWS.FileClassification.None, false, stream);

                        return newFile;
                    }
                }
                else
                {
                    FileIteration vdfFile = new FileIteration(conn, result[0]);

                    AcquireFilesSettings settings = new AcquireFilesSettings(conn);
                    settings.AddEntityToAcquire(vdfFile);
                    settings.DefaultAcquisitionOption = AcquireFilesSettings.AcquisitionOption.Checkout;
                    AcquireFilesResults results = conn.FileManager.AcquireFiles(settings);

                    if (results.FileResults.First().Exception != null)
                        throw results.FileResults.First().Exception;

                    vdfFile = results.FileResults.First().File;
                    vdfFile = conn.FileManager.CheckinFile(vdfFile, ExtensionRes.VaultUtil_ThunderdomeDeployment, false,
                        null, null, false, null, AWS.FileClassification.None, false, new FilePathAbsolute(localPath));

                    return vdfFile;
                }
            });
        }

        public async static Task<AcquireFilesResults> DownloadFolderAsync(Connection connection, AWS.Folder folder, bool includeReferences, bool includeHidden, string destLocalPath = "")
        {
            var folders = new List<AWS.Folder>() { folder };

            AWS.Folder[] subFolders = connection.WebServiceManager.DocumentService.GetFoldersByParentId(folder.Id, true);

            if (subFolders != null)
            {
                folders.AddRange(subFolders);
            }

            AWS.FileArray[] filesPerFolder = connection.WebServiceManager.DocumentService.GetLatestFilesByFolderIds(folders.Select(f => f.Id).ToArray(), includeHidden);

            if (filesPerFolder == null)
            {
                return null;
            }

            var allFiles=new List<(AWS.File file, bool includeReferences, string destLocalPath)> ();
            var destLocalPathLength = destLocalPath.Length;
            for (int i = 0; i < folders.Count; i++)
            {
                if (filesPerFolder[i]?.Files==null)
                {
                    continue;
                }

                foreach (var file in filesPerFolder[i].Files)
                {
                    var destPathChuncks = folders[i].FullName.Substring(folder.FullName.Length).Split(new char[]{'/'}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    destPathChuncks.Insert(0, destLocalPath);

                    var destPath = IO.Path.Combine(destPathChuncks.ToArray());

                    allFiles.Add((file, false, destPath));
                }

            }

            return await DownloadFilesAsync(connection, allFiles);

        }

        /// <summary>
        /// Downloads files from Vault in the local folder. It can optionally download references.
        /// </summary>
        /// <param name="connection">Vault Connection</param>
        /// <param name="files">Files to download</param>
        /// <param name="includeReferences">Download files references by 'files'</param>
        /// <returns></returns>
        public async static Task<AcquireFilesResults> DownloadFilesAsync(Connection connection, List<(AWS.File file, bool includeReferences, string destLocalPath)> files)
        {
            var settings = new AcquireFilesSettings(connection);

            foreach (var file in files)
            {
                FolderPathAbsolute destPath = null;
                if (file.destLocalPath != "")
                {
                    destPath = new FolderPathAbsolute(file.destLocalPath);
                }

                AcquireFilesSettings.AcquisitionOption downloadOption = AcquireFilesSettings.AcquisitionOption.Download;


                settings.DefaultAcquisitionOption = downloadOption;
                settings.OptionsResolution.SyncWithRemoteSiteSetting = AcquireFilesSettings.SyncWithRemoteSite.Always;
                settings.OptionsResolution.OverwriteOption = AcquireFilesSettings.AcquireFileResolutionOptions.OverwriteOptions.ForceOverwriteAll;

                if (file.includeReferences)
                {
                    settings.OptionsRelationshipGathering.FileRelationshipSettings.IncludeChildren = true;
                    settings.OptionsRelationshipGathering.FileRelationshipSettings.IncludeLibraryContents = true;
                    settings.OptionsRelationshipGathering.FileRelationshipSettings.RecurseChildren = true;
                }

                var fileIteration = new FileIteration(connection, file.file);
                if (destPath == null)
                {
                    settings.AddFileToAcquire(fileIteration, downloadOption);
                }
                else
                {
                    settings.AddFileToAcquire(fileIteration, downloadOption, destPath);
                }
            }

            return await connection.FileManager.AcquireFilesAsync(settings);
        }

        public static string NormalizePath(string folderPath)
        {
            string normalizedPath;

            if (string.IsNullOrEmpty(folderPath))
                return string.Empty;

            if (!folderPath.EndsWith("/"))
                normalizedPath = $"{folderPath}/";
            else
                normalizedPath = folderPath;

            return normalizedPath.Replace("\\", "/");
        }

        public static File FindLatestFile(Connection conn, string vaultFilePath)
        {
            if (string.IsNullOrEmpty(vaultFilePath))
                return null;

            File[] files = conn.WebServiceManager.DocumentService.FindLatestFilesByPaths(ArrayUtil.FromSingle(vaultFilePath));
            if (files == null || files.Length == 0 || files[0].Id <= 0 || files[0].Cloaked)
                return null;
            return files[0];
        }
    }
}