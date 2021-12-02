using Autodesk.Connectivity.Explorer.Extensibility;
using Autodesk.Connectivity.Extensibility.Framework;
using Autodesk.DataManagement.Client.Framework.Currency;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using Autodesk.DataManagement.Client.Framework.Vault.Results;
using Autodesk.DataManagement.Client.Framework.Vault.Settings;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thunderdome.DeploymentController;
using Thunderdome.FileOperations;
using Thunderdome.Model;
using Thunderdome.Util;
using File = Autodesk.Connectivity.WebServices.File;

[assembly: ApiVersion("15.0")]
[assembly: ExtensionId("2AAE56F1-3E44-4B69-8AF0-15566D7A2E49")]

namespace Thunderdome
{
    /// <summary>
    /// Represents thunderdome application extension for Vault
    /// </summary>
    public class ThunderdomeExplorerExtension : IExplorerExtension
    {
        public const string DeploymentConfigOption = "autodesk.thunderdome.deploymentConfig";

        private const string BackupFileNameFormat = "VaultSettingsBackup-{0}-{1}-{2}.zip";

        public const string DefaultPackageDisplayName = "Default";
        public const string DefaultPackageFileName = "Deployment.td";
        private const string TempDeploymentFolder = "Thunderdome";

        private const string ThunderdomeMenuSite = "autodesk.thunderdome.site";
        private const string ThunderdomeBackupCommandId = "autodesk.thunderdome.command.backup";
        private const string ThunderdomeConfigCommandId = "autodesk.thunderdome.command.config";
        private const string ThunderdomeDeployCommandId = "autodesk.thunderdome.command.deploy";

        private const string DeployExecutable = "deploy.exe";

        private readonly HashSet<string> _excludedDeploymentFiles = new HashSet<string>()
        {
            "ApplicationPreferences.xml".ToLower(),
            "LoginHistory.xml".ToLower(),
            "Login.xml".ToLower()
        };

        private bool _restartPending;

        private DeploymentSet _deployments { get; set; } = null;
       
        #region IExtension Members

        /// <summary>
        /// Callback to get commands
        /// </summary>
        public IEnumerable<CommandSite> CommandSites()
        {
            //ExtensionRes.Culture = new System.Globalization.CultureInfo("de");

            SetLanguage();

            CommandSite site = new CommandSite(ThunderdomeMenuSite, ExtensionRes.ThunderdomeExplorerExtension_MessageBoxCaption)
            {
                DeployAsPulldownMenu = true,
                Location = CommandSiteLocation.ToolsMenu
            };

            CommandItem backupCmd = new CommandItem(ThunderdomeBackupCommandId, ExtensionRes.ThunderdomeExplorerExtension_BackupVaultSettings)
            {
                Description = ExtensionRes.ThunderdomeExplorerExtension_CommandSites_BackupCommand,
                Hint = ExtensionRes.ThunderdomeExplorerExtension_CommandSites_BackupCommand,
                Image = ExtensionRes._000275_computer_save,
                ToolbarPaintStyle = PaintStyle.TextAndGlyph
            };
            backupCmd.Execute += BackupCmd_Execute;
            site.AddCommand(backupCmd);

            CommandItem deployCmd = new CommandItem(ThunderdomeDeployCommandId, ExtensionRes.ThunderdomeExplorerExtension_DeployCommand)
            {
                Description = ExtensionRes.ThunderdomeExplorerExtension_CommandSites_DeployCommandDescription,
                Hint = ExtensionRes.ThunderdomeExplorerExtension_DeployCommand,
                Image = ExtensionRes._000260_computer_export,
                ToolbarPaintStyle = PaintStyle.TextAndGlyph
            };
            deployCmd.Execute += DeployCmd_Execute;
            site.AddCommand(deployCmd);

            CommandItem configCmd = new CommandItem(ThunderdomeConfigCommandId, ExtensionRes.ThunderdomeExplorerExtension_ConfigureationCommand)
            {
                Description = ExtensionRes.ThunderdomeExplorerExtension_CommandSites_ConfigureationCommandDescription,
                Hint = ExtensionRes.ThunderdomeExplorerExtension_ConfigureationCommand,
                Image = ExtensionRes._000276_computer_settings,
                ToolbarPaintStyle = PaintStyle.TextAndGlyph
            };
            configCmd.Execute += ConfigCmd_Execute;
            site.AddCommand(configCmd);

            return new CommandSite[] { site };
        }

        /// <summary>
        /// Callback to get detail tabs
        /// </summary>
        public IEnumerable<DetailPaneTab> DetailTabs()
        {
            return null;
        }

        /// <summary>
        /// Callback to get hidden commands
        /// </summary>
        public IEnumerable<string> HiddenCommands()
        {
            return null;
        }

        /// <summary>
        /// Callback to get custom entity handlers
        /// </summary>
        public IEnumerable<CustomEntityHandler> CustomEntityHandlers()
        {
            return null;
        }

        /// <summary>
        /// Called on Vault application logged off a vault server
        /// </summary>
        /// <param name="application">Vault application context</param>
        public void OnLogOff(IApplication application)
        {
            // no need to do something on log off
        }

        /// <summary>
        /// Called on Vault application logged on a vault server
        /// </summary>
        /// <param name="application">Vault application context</param>
        public void OnLogOn(IApplication application)
        {
            try
            {
                this._deployments = LoadSavedDeployments(application.Connection);

                FindAndApplyUpdates(application.Connection);
            }
            catch (Exception ex)
            {
                string message = "\n " + Autodesk.DataManagement.Client.Framework.Library.ExceptionParser.GetMessage(ex);
                ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_OnLogOn_ThunderdomeCouldNotUpdate + message);
            }
        }


        /// <summary>
        /// Called on Vault application shutdown
        /// </summary>
        /// <param name="application">Vault application context</param>
        public void OnShutdown(IApplication application)
        {
            try
            {
                if (_restartPending)
                {
                    string codeFolder = FileUtil.GetAssemblyCodeBasePath();
                    string exePath = Path.Combine(codeFolder, DeployExecutable);

                    Process.Start(exePath);
                }
            }
            catch
            {
                ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_OnShutdow_CouldNotTriggerDeployment);
            }
        }

        /// <summary>
        /// Callback called on Vault application startup
        /// </summary>
        /// <param name="application">Vault application context</param>
        public void OnStartup(IApplication application)
        {
            // no need to do something on startup
        }

        #endregion IExtension Members

        private void SetLanguage()
        {
            string languageSettingLocation = FileUtil.GetPathOfNewFolder();
            string languageSettingFullFileName = Path.Combine(languageSettingLocation, "language.config");

            if (System.IO.File.Exists(languageSettingFullFileName) == false)
                return;

            string[] lines = System.IO.File.ReadAllLines(languageSettingFullFileName);
            if (lines.Any() == false)
                return;

            if (lines[0].ToLower().Trim() == "de")
            {
                ExtensionRes.Culture = new System.Globalization.CultureInfo("de");
                Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("de");
            }
        }

        private void FindAndApplyUpdates(Connection conn)
        {
            Settings settings = Settings.Load();
            if (IsUpdateRequired(settings) == false) return;

            FindUpdateFileAndApply(conn, settings);
        }

        private bool IsUpdateRequired(Settings settings)
        {
            if (_restartPending)
                return false;

            if (_deployments == null)
            {
                return false;
            }

            // Admin set a deployment as 'forced' the user cannot skip it
            if (_deployments.DeploymentModels.Any(d => d.ForceUpdate == true) == true)
                return true;

            // user previously indicated not to download updates and not to be prompted
            if (settings.NeverDownload)
                return false;
            return true;
        }

        private void FindUpdateFileAndApply( Connection conn, Settings settings)
        {
            var entries = new List<VaultEntry>();
            var updatedDeployments = new List<DeploymentModel>();

            foreach (var depolyment in _deployments.DeploymentModels)
            {
                var isServerSpecific = depolyment.Containers.Any(c => c.Key == ConfigFilesController.ControllerKey);
                var specificServer= string.Empty;
                if (isServerSpecific)
                {
                    specificServer = conn.Server;
                }

                VaultEntry entry = CreateEntry(specificServer, conn.Vault, depolyment.DisplayName, conn, settings);

                if (entry == null)
                {
                    continue;
                }

                if (entry.LastUpdate >= entry.DeploymentPackage.CkInDate)
                    continue;

                entries.Add(entry);

                updatedDeployments.Add(depolyment);
            }

            if (entries.Any() == false)
            {
                return;
            }

            var defaultDeployment = updatedDeployments.FirstOrDefault(d => d.DisplayName == ThunderdomeExplorerExtension.DefaultPackageDisplayName);

            var isDefaultDeploymentUpdated = (defaultDeployment != null);

            var userWantsUpdate = false;
            if (isDefaultDeploymentUpdated == true)
            {
                userWantsUpdate = VerifyUserWantsUpdate(conn, settings, defaultDeployment);
            }

            var otherDeployments = string.Join("\r\n", updatedDeployments.Where(d => d.DisplayName != ThunderdomeExplorerExtension.DefaultPackageDisplayName).Select(d => $"- {d.DisplayName}"));

            if (otherDeployments.Any())
            {
                ShowInfoMessage($"{ExtensionRes.ThunderdomeExplorerExtension_PopUpCustomDeploymentUpdatesAvailable}\r\n{otherDeployments}");
            }

            if (userWantsUpdate == false)
                return;

            var defaultentry = settings.VaultEntries.First(e => e.VaultName == conn.Vault && e.DeploymentName == DefaultPackageDisplayName && e.DeploymentPackage!=null);

            MoveOperationsConfigurationResult moveOperationsConfigResult = SetUpMoveOperations(conn, defaultentry);

            DisplayResultMessage(moveOperationsConfigResult);
            _restartPending = true;
        }

        private VaultEntry CreateEntry(string serverName, string vaultName, string deploymentName, Connection conn, Settings settings)
        {
            VaultEntry entry = settings.GetOrCreateEntry(serverName, vaultName, deploymentName);

            string deploymentPath = GetDeploymentPath(deploymentName);

            File deploymentPackage = VaultUtil.FindLatestFile(conn, deploymentPath);
            if (deploymentPackage == null)
                return null;

            entry.DeploymentPackage = deploymentPackage;

            return entry;
        }

        private void PrepareCustomDeployment(string deploymentName, Connection conn)
        {
            _deployments = LoadSavedDeployments(conn);

            string deploymentPath = GetDeploymentPath(deploymentName);

            File deploymentPackage = VaultUtil.FindLatestFile(conn, deploymentPath);
            if (deploymentPackage == null)
                return;

            VaultEntry entry = new VaultEntry()
            {
                ServerName = conn.Server,
                VaultName = conn.Vault,
                DeploymentName = deploymentName,
                DeploymentPackage = deploymentPackage,
                LastUpdate = DateTime.MinValue
            };

            MoveOperationsConfigurationResult moveOperationsConfigResult = SetUpMoveOperations(conn, entry);

            DisplayResultMessage(moveOperationsConfigResult);
            _restartPending = true;
        }

        private static void DisplayResultMessage(MoveOperationsConfigurationResult moveOperationsConfigResult)
        {
            StringBuilder messageBuilder =
                new StringBuilder(ExtensionRes.ThunderdomeExplorerExtension_DisplayResultMessage_UpdatesDownloaded);

            if (!moveOperationsConfigResult.IsFullySuccessful)
                messageBuilder.AppendLine(ExtensionRes.ThunderdomeExplorerExtension_DisplayResultMessage_FollowingConfigurationsFailedToSetup)
                    .Append(moveOperationsConfigResult);

            MessageBox.Show(messageBuilder.ToString(), ExtensionRes.ThunderdomeExplorerExtension_DisplayResultMessage_ExitRequired,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static MoveOperationsConfigurationResult SetUpMoveOperations(Connection conn, VaultEntry entry)
        {
            string tempPath = CreateTempDirectory();
            File file = entry.DeploymentPackage;

            UtilSettings utilSettings = new UtilSettings
            {
                TempFolder = tempPath,
                VaultClient = Environment.GetCommandLineArgs()[0], // get vault .exe file name
                VaultEntry = entry,
                DeploymentCheckInDate = file.CkInDate
            };

            string zipPath = Path.Combine(tempPath, file.Name);
            DownloadDeploymentFile(conn, file, zipPath);
            ExtractDeploymentFile(tempPath, zipPath);

            MasterController mc = new MasterController(conn, entry.VaultName);
            MoveOperationsConfigurationResult result = mc.SetMoveOperations(tempPath, utilSettings);
            utilSettings.Save();
            return result;
        }

        private static void ExtractDeploymentFile(string tempPath, string zipPath)
        {
            System.IO.File.SetAttributes(zipPath, FileAttributes.Normal);
            var zfe = new ZipEntryFactory { IsUnicodeText = true };
            FastZip zip = new FastZip { EntryFactory = zfe }; 
            zip.ExtractZip(zipPath, tempPath, null);
        }

        private static void DownloadDeploymentFile(Connection conn, File file, string zipPath)
        {
            FileIteration vdfFile = new FileIteration(conn, file);
            FilePathAbsolute vdfPath = new FilePathAbsolute(zipPath);
            AcquireFilesSettings acquireSettings = new AcquireFilesSettings(conn, false);
            acquireSettings.AddFileToAcquire(vdfFile, AcquireFilesSettings.AcquisitionOption.Download, vdfPath);
            AcquireFilesResults acquireResults = conn.FileManager.AcquireFiles(acquireSettings);
            foreach (FileAcquisitionResult acquireResult in acquireResults.FileResults)
            {
                if (acquireResult.Exception != null)
                    throw acquireResult.Exception;
            }
        }

        private static string CreateTempDirectory()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), TempDeploymentFolder);
            if (Directory.Exists(tempPath))
                DirectoryUtil.DeleteRecursivelly(tempPath);

            Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        private string GetDeploymentPath(string deploymentName)
        {
            return _deployments.GetDeploymentByDisplayName(deploymentName).DeploymentLocation;
        }

        private bool VerifyUserWantsUpdate(Connection conn, Settings settings, DeploymentModel deployment)
        {
            UpdateDialogResult dialogResult = AskDialog.ShowDialog(deployment);
            if (dialogResult == UpdateDialogResult.No)
                return false;

            if (dialogResult == UpdateDialogResult.Never)
            {
                settings.NeverDownload = true;
                settings.Save();
                return false;
            }

            return true;
        }

        private void BackupCmd_Execute(object sender, CommandItemEventArgs e)
        {
            try
            {
                Backup(e.Context.Application.Connection);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(ExtensionRes.ThunderdomeExplorerExtension_BackupCmdExecute_UnableToCreateBackup_0, ex.Message));
            }
        }

        private void Backup(Connection conn)
        {
            //string commonPath = AutodeskPathUtil.GetLocalVaultCommonFolder();
            FolderPathAbsolute path =
                Autodesk.DataManagement.Client.Framework.Vault.Library.LocalFileLocation.GetVaultCommonConnectionPath(conn, false, CommonPreferenceVersionRequirements.None, false);
            string commonPath = Path.GetDirectoryName(path.FullPath);
            string settingsPath = AutodeskPathUtil.GetLocalVaultSettingsFolder();
            if (settingsPath == null)
            {
                ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_Backup_ErrorUnknownVaultProduct);
                return;
            }

            if (!Directory.Exists(settingsPath))
            {
                ShowErrorMessage(string.Format(ExtensionRes.ThunderdomeExplorerExtension_BackupFolder_0_DoesNotExist, settingsPath));
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                DateTime now = DateTime.Now;
                dlg.FileName = string.Format(BackupFileNameFormat, now.Year, now.Month, now.Day);
                dlg.Title = ExtensionRes.ThunderdomeExplorerExtension_Backup_BackupVaultUserSettings;

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                string backupPath = dlg.FileName;

                CreateSettingsBackup(commonPath, settingsPath, backupPath);
            }
            ShowInfoMessage(ExtensionRes.ThunderdomeExplorerExtension_BackupCompleted);
        }

        private void CreateSettingsBackup(string commonPath, string settingsPath, string backupPath)
        {
            ZipFile zip = ZipFile.Create(backupPath);
            zip.BeginUpdate();

            ZipFolder(zip, commonPath, commonPath);
            ZipFolder(zip, settingsPath, settingsPath);

            zip.CommitUpdate();
            zip.Close();
        }

        private void ZipFolder(ZipFile zip, string rootPath, string currentPath)
        {
            string rootFolderName = Path.GetFileName(rootPath);
            foreach (string file in Directory.GetFiles(currentPath).Where(f => !IsExcludedDeploymentFile(f)))
            {
                string zipPath = rootFolderName + "\\" + file.Remove(0, rootPath.Length + 1);
                zipPath = zipPath.Replace('\\', '/');
                zip.Add(new FileDataSource(file), zipPath);
            }
            foreach (string folder in Directory.GetDirectories(currentPath))
            {
                ZipFolder(zip, rootPath, folder);
            }
        }

        private bool IsExcludedDeploymentFile(string file)
        {
            return _excludedDeploymentFiles.Contains(Path.GetFileName(file).ToLower());
        }

        private async void ConfigCmd_Execute(object sender, CommandItemEventArgs e)
        {
            try
            {
                await ConfigureThunderdome(e.Context);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.ToString());
            }
        }

        private async Task ConfigureThunderdome(ICommandContext context)
        {
            Connection conn = context.Application.Connection;
            string vaultName = conn.Vault;
            string serverName = conn.Server;
            if (!VaultUtil.IsAdmin(conn))
            {
                ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_OnlyAdminCanUseThisFunction);
                return;
            }

            DeploymentSet initialDeploymentModel = LoadSavedDeployments(conn);

            ManageDeployments cfgDialog = new ManageDeployments();
            if (await cfgDialog.ShowDialog(initialDeploymentModel, vaultName, conn) != DialogResult.OK)
            {
                VaultFoldersController.CleanUpTmpRootFolder();
                return;
            }
            List<ConfigurationResult> configurationResults = cfgDialog.ConfigurationResults;

            //Read again, because initialdeploymentModel was changed by the user configuration
            initialDeploymentModel = LoadSavedDeployments(conn);

            List<ConfigurationResult> updatedConfigurations = GetUpdatedConfigurations(conn, configurationResults, initialDeploymentModel);

            WaitingForm waitingForm = new WaitingForm(cfgDialog, 2 * updatedConfigurations.Count + 1);

            waitingForm.Show();

            var step = 1;

            foreach (var configurationResult in updatedConfigurations)
            {

                waitingForm.SetProgress($"Building deployment '{configurationResult.SelectedDeployments.DisplayName}' : Collecting files...", step);
                step += 1;

                // zip up the files and upload to Vault
                string tempFile = Path.GetTempFileName();
                if (configurationResult.AnyItemSelected)
                {
                    await ArchiveDeploymentModel(configurationResult.SelectedDeployments, tempFile);
                }

                waitingForm.SetProgress($"Building deployment '{configurationResult.SelectedDeployments.DisplayName}' : Uploading the package in Vault...", step);
                step += 1;

                var deploymentFileName = Path.GetFileName(configurationResult.SelectedDeployments.DeploymentLocation);
                File vaultPackage = await VaultUtil.AddOrUpdateFileAsync(tempFile, deploymentFileName, configurationResult.SelectedDeployments.DeploymentFolder, conn);

                System.IO.File.Delete(tempFile);

                SetUpToDateState(vaultName, serverName, configurationResult.SelectedDeployments.DisplayName, vaultPackage);

                CleanUp(configurationResult.SelectedDeployments.Containers);
            }

            VaultFoldersController.CleanUpTmpRootFolder();

            waitingForm.SetProgress($"Updating metadata...", step);

            var deploymentSet = new DeploymentSet();
            deploymentSet.DeploymentModels.AddRange(configurationResults.Select(c => c.SelectedDeployments));

            conn.WebServiceManager.KnowledgeVaultService.SetVaultOption(DeploymentConfigOption, deploymentSet.ToXml());

            waitingForm.Close();

            ShowInfoMessage(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_DeploymentCreated);
            context.ForceRefresh = true;

            var defaultConfigLocation = deploymentSet.GetDeploymentByDisplayName(DefaultPackageDisplayName);
            context.GoToLocation = new LocationContext(SelectionTypeId.File, defaultConfigLocation.DeploymentLocation);
        }

        private DeploymentSet LoadSavedDeployments(Connection conn)
        {
            var savedData = conn.WebServiceManager.KnowledgeVaultService.GetVaultOption(DeploymentConfigOption);
            DeploymentSet parsedData = new DeploymentSet(); ;
            if (savedData != null)

            {
                parsedData = DeploymentSet.Load(savedData);
            }

            return parsedData;
        }

        private List<ConfigurationResult> GetUpdatedConfigurations(Connection conn, List<ConfigurationResult> configurationResults, DeploymentSet initialDeploymentModel)
        {
            var currentDeploymentFiles = conn.WebServiceManager.DocumentService.FindLatestFilesByPaths(configurationResults.Select(c => c.SelectedDeployments.DeploymentLocation).ToArray()).Where(f => f.Id != -1);

            var updatedConfig = new List<ConfigurationResult>();
            foreach (ConfigurationResult config in configurationResults)
            {
                if (config.SelectedDeployments.Updated)
                {
                    updatedConfig.Add(config);
                    continue;
                }

                var isDeploymentFileExists = currentDeploymentFiles.FirstOrDefault(f => f.Name == System.IO.Path.GetFileName(config.SelectedDeployments.DeploymentLocation));

                if (isDeploymentFileExists == null)
                {
                    updatedConfig.Add(config);
                    continue;
                }

                var previousConfig = initialDeploymentModel.GetDeploymentByDisplayName(config.SelectedDeployments.DisplayName);

                if (previousConfig == null)
                {
                    updatedConfig.Add(config);
                    continue;
                }

                var isSameContent = IsSameDeploymentContent(config, previousConfig);

                if (isSameContent == false)
                {
                    updatedConfig.Add(config);
                }
            }

            return updatedConfig;
        }

        private static bool IsSameDeploymentContent(ConfigurationResult config, DeploymentModel previousConfig)
        {

            var newContainerNames = config.SelectedDeployments.Containers.Select(c => c.DisplayName).ToList();
            var previousContainerNames = previousConfig.Containers.Select(c => c.DisplayName).ToList();

            var isContainerChanges = IsDifferentStringList(newContainerNames, previousContainerNames);

            if (isContainerChanges)
            {
                return false;
            }

            foreach (var container in config.SelectedDeployments.Containers)
            {
                var previousContainer = previousConfig.Containers.FirstOrDefault(c => c.DisplayName == container.DisplayName);

                var newItemNames = container.DeploymentItems.Select(i => i.DisplayName).ToList();
                var previousItemNames = previousContainer.DeploymentItems.Select(i => i.DisplayName).ToList();

                var isItemChanges = IsDifferentStringList(newItemNames, previousItemNames);

                if (isItemChanges)
                {
                    return false;
                }

            }

            return true;
        }

        private static bool IsDifferentStringList(List<String> list1, List<string> list2)
        {
            return (list1.Except(list2).Any() == true || list2.Except(list1).Any() == true);
            //return list1.Intersect(list2).Except(list1).Any();
        }

        private void DeployCmd_Execute(object sender, CommandItemEventArgs e)
        {
            try
            {
                Deploy(e.Context);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(ExtensionRes.ThunderdomeExplorerExtension_DeployCmdExecute_UnableToDeploy_0, ex.Message));
            }
        }

        private void Deploy(ICommandContext context)
        {
            if (_restartPending)
            {
                ShowErrorMessage(ExtensionRes.ThunderdomeExplorerExtension_ConfigureThunderdome_PendingDeployment);
                return;
            }

            Connection conn = context.Application.Connection;

            DeploymentSet initialDeploymentModel = DeploymentSet.Load(conn.WebServiceManager.KnowledgeVaultService.GetVaultOption(DeploymentConfigOption));

            var deployFrm = new _deployForm();
            deployFrm.Configure(initialDeploymentModel.DeploymentModels);
            deployFrm.ShowDialog();

            if (string.IsNullOrEmpty(deployFrm.SelectedDeployment))
                return;

            PrepareCustomDeployment(deployFrm.SelectedDeployment, context.Application.Connection);
        }

        private static async Task ArchiveDeploymentModel(DeploymentModel deploymentModel, string tempFile)
        {
            await Task.Run(() =>
            {
                using (ZipFile zip = ZipFile.Create(tempFile))
                {
                    zip.EntryFactory = new ZipEntryFactory { IsUnicodeText = true };
                    zip.BeginUpdate();
                    foreach (DeploymentContainer container in deploymentModel.Containers)
                    {
                        foreach (DeploymentItem item in container.DeploymentItems)
                        {
                            item.Zip(zip, container.Key);
                        }
                    }
                    zip.CommitUpdate();
                    zip.Close();
                }
            });
        }

        private static void CleanUp(List<DeploymentContainer> containers)
        {
            foreach (DeploymentContainer container in containers)
            {
                foreach (DeploymentItem item in container.DeploymentItems)
                {
                    item.CleanUp();
                }
            }
        }

        private static void SetUpToDateState(string vaultName, string serverName, string deploymentName, File vaultPackage)
        {
            Settings settings = Settings.Load();
            VaultEntry entry = settings.GetOrCreateEntry(serverName, vaultName, deploymentName);
            entry.LastUpdate = vaultPackage.CkInDate;
            settings.Save();
        }

        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, ExtensionRes.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult ShowInfoMessage(string message)
        {
            return MessageBox.Show(message, ExtensionRes.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}