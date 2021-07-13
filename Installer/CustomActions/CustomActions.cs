using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.IO;

namespace Thunderdome.Installer.CustomActions
{
    public static class InstallDirectory
    {
        private const string VaultAssemblyVersion = "26";
        private const string VaultVersion = "2021";

        [CustomAction]
        public static ActionResult InitializeInstallDirectories(Session session)
        {
            if (VaultInstalled())
            {
                string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                session["InstallDirectory"] = Path.Combine(commonAppData, $"Autodesk\\Vault {VaultVersion}\\Extensions\\");
                session["VAULT_INSTALLED"] = "1";
            }
            else
            {
                session["VAULT_INSTALLED"] = "0";
            }

            return ActionResult.Success;
        }

        private static bool VaultInstalled()
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                            .OpenSubKey(@"Software\Autodesk");
            if (key == null)
                return false;

            foreach (string subKeyName in key.GetSubKeyNames())
            {
                if (subKeyName.ToLower().Contains("vault"))
                {
                    RegistryKey vaultKey = key.OpenSubKey(subKeyName);
                    foreach (string vaultVersionKey in vaultKey.GetSubKeyNames())
                    {
                        if (vaultVersionKey.Contains(VaultAssemblyVersion))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
