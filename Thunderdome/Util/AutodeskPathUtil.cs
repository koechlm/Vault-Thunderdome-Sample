using System;
using System.Diagnostics;
using System.IO;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;

namespace Thunderdome.Util
{
    /// <summary>
    /// Contains utility methods for %AppData%\Autodesk path operations
    /// </summary>
    internal static class AutodeskPathUtil
    {
        //private const string CommonFolder1 = "Services_Security_1_6_2017";
        //private const string CommonFolder2 = "Services_Security_6_29_2011";
        private const string VaultProFolderName = "Autodesk Vault Professional 2022";
        private const string VaultWgFolderName = "Autodesk Vault Workgroup 2022";

        /// <summary>
        /// Returns path to VaultCommon folder
        /// </summary>
        internal static string GetLocalVaultCommonFolder()
        {
            return GetAutodeskAppDataSubFolder("VaultCommon");
        }

        /// <summary>
        /// Returns path common vault folder specified by server and vault name under Services_Security_2_3_2016 folder
        /// </summary>
        /// <param name="server">Server to get folder for</param>
        /// <param name="vault">Vault name to get folder for</param>
        internal static string GetCurrentVaultCommonFolder(Connection conn, string server, string vault)
        {
            //string commonRoot = GetLocalVaultCommonFolder();
            //return Path.Combine(commonRoot, "Servers", CommonFolder1, server, "Vaults", vault);
            Autodesk.DataManagement.Client.Framework.Currency.FolderPathAbsolute folderRoot = 
                Autodesk.DataManagement.Client.Framework.Vault.Library.LocalFileLocation.GetVaultCommonConnectionPath(conn, false, CommonPreferenceVersionRequirements.VersionSpecific, false);
            return Path.Combine(folderRoot.FullPath, server, "Vaults", vault);
        }

        /// <summary>
        /// Returns path common vault folder specified by server and vault name under Services_Security_6_29_2011 folder
        /// </summary>
        /// <param name="server">Server to get folder for</param>
        /// <param name="vault">Vault name to get folder for</param>
        internal static string GetCurrentVaultCommonFolder2(Connection conn, string server, string vault)
        {
            //string commonRoot = GetLocalVaultCommonFolder();
            //return Path.Combine(commonRoot, "Servers", CommonFolder2, server, "Vaults", vault);
            Autodesk.DataManagement.Client.Framework.Currency.FolderPathAbsolute folder2Root = 
                Autodesk.DataManagement.Client.Framework.Vault.Library.LocalFileLocation.GetVaultCommonConnectionPath(conn, false, CommonPreferenceVersionRequirements.VersionIndependent, false);
            return Path.Combine(folder2Root.FullPath, server, "Vaults", vault);
        }

        /// <summary>
        /// Returns vault settings folder path
        /// </summary>
        internal static string GetLocalVaultSettingsFolder()
        {
            switch (Process.GetCurrentProcess().ProcessName)
            {
                case "Connectivity.VaultPro":
                    return GetAutodeskAppDataSubFolder(VaultProFolderName);

                case "Connectivity.VaultWkg":
                    return GetAutodeskAppDataSubFolder(VaultWgFolderName);

                default:
                    return null;
            }
        }

        internal static string GetServerSettingsFolderName(Connection connection)
        {
            return connection.Server.Replace(@"/", "_").Replace(@":", "_");
        }

        private static string GetAutodeskAppDataSubFolder(string subFolder)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Autodesk", subFolder);
        }

        /// <summary>
        /// Returns settings folder path specified by server and vault name
        /// </summary>
        /// <param name="server">Server to get folder for</param>
        /// <param name="vault">Vault name to get folder for</param>
        public static string GetCurrentVaultSettingsFolder(string server, string vault)
        {
            string root = GetLocalVaultSettingsFolder();
            return Path.Combine(root, "Servers", server, "Vaults", vault);
        }
    }
}