using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Thunderdome;
using Thunderdome.FileOperations;
using Thunderdome.Util;

namespace Deploy
{
    internal static class Program
    {
        private static readonly string LogFilePath = Path.Combine(FileUtil.GetExetableDirectoryPath(), "deployErrorLog.txt");

        private static readonly UtilSettings DeploymentSettings = UtilSettings.Load();

        private static void Main()
        {
            try
            {
                if (DeploymentSettings == null)
                    return;

                Console.CancelKeyPress += ConsoleOnCancelKeyPress;
                PerformDeployment();
            }
            catch (Exception e)
            {
                Log(e);
                Console.WriteLine($"\r\nPlease contact your administrator. Log file: {LogFilePath} \r\n");

                Console.WriteLine("Press a key to close.");
                Console.ReadKey();
            }
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            DeleteTempFiles();
        }

        private static void Log(Exception e)
        {
            Console.WriteLine($"Error: {e.ToString()}");
            Console.WriteLine($"Error message saved in the log file: {LogFilePath} ");
            File.AppendAllText(LogFilePath, e.ToString());
        }

        private static void PerformDeployment()
        {
            Console.Out.WriteLine("Waiting for Vault client to exit...");
            WaitUntilVaultExplorerExit();

            Console.Out.WriteLine("Performing Vault updates...");
            DeleteFiles(DeploymentSettings.DeleteOperations);
            MoveFolders(DeploymentSettings.FolderMoveOperations);
            MoveFiles(DeploymentSettings.FileMoveOperations);
            ReplaceLastUpdateTime();
            DeleteTempFiles();
            Process.Start(DeploymentSettings.VaultClient);
        }

        private static void WaitUntilVaultExplorerExit()
        {
            string vaultExeName = Path.GetFileNameWithoutExtension(DeploymentSettings.VaultClient);
            bool vaultClosed = false;
            while (!vaultClosed)
            {
                Thread.Sleep(5000);
                Process[] processes = Process.GetProcessesByName(vaultExeName);
                vaultClosed = processes.Length == 0;
            }
        }

        private static void DeleteFiles(IEnumerable<string> files)
        {
            foreach (string file in files.Where(File.Exists))
                FileUtil.DeleteFile(file);
        }

        private static void MoveFolders(IEnumerable<FolderMove> folderMoveOperations)
        {
            FolderMove[] folderExists = folderMoveOperations.Where(move => Directory.Exists(move.To)).ToArray();
            foreach (FolderMove move in folderExists)
                MoveDirectory(move);
        }

        private static void MoveDirectory(FolderMove move)
        {

                var sourcePath = move.From.TrimEnd('\\', ' ');
                var targetPath = move.To.TrimEnd('\\', ' ');
                var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories).GroupBy(s => Path.GetDirectoryName(s));
                foreach (var folder in files)
                {
                    var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                    Directory.CreateDirectory(targetFolder);
                    foreach (var file in folder)
                    {
                        var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                        if (File.Exists(targetFile))
                        {
                            File.SetAttributes(targetFile, FileAttributes.Normal);
                            File.Delete(targetFile);
                        }
                        File.Move(file, targetFile);
                    }
                }
                Directory.Delete(move.From, true);

        }

        private static void MoveFiles(IEnumerable<FileMove> fileMoveOperations)
        {
            foreach (FileMove move in fileMoveOperations)
            {
                string toDir = Path.GetDirectoryName(move.To);
                if (!Directory.Exists(toDir))
                    Directory.CreateDirectory(toDir);

                if (File.Exists(move.From))
                {
                    if (File.Exists(move.To))
                    {
                        File.SetAttributes(move.To, FileAttributes.Normal);

                        File.Delete(move.To);
                    }

                    File.Copy(move.From, move.To);

                    File.Delete(move.From);
                }
            }
        }

        private static void DeleteTempFiles()
        {

                DeploymentSettings.Delete();
                Directory.Delete(DeploymentSettings.TempFolder, true);

        }

        private static void ReplaceLastUpdateTime()
        {
            if (DeploymentSettings.VaultEntry != null)
            {
                Settings addInSettings = Settings.Load();
                VaultEntry entry = DeploymentSettings.VaultEntry;
                addInSettings.SetLastUpdate(entry.ServerName, entry.VaultName, entry.DeploymentName,DeploymentSettings.DeploymentCheckInDate);
                addInSettings.Save();
            }
        }
    }
}
