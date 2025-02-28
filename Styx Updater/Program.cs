using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Styx_Updater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Styx Updater");
            Thread.Sleep(3000);

            Console.ForegroundColor = ConsoleColor.White;
            var remoteVersionUrl = "http://alphahub.org/styx/version2.txt";

            var webClient = new WebClient();
            try
            {
                var remoteVersionText = webClient.DownloadString(remoteVersionUrl).Trim();
                var remoteVersionParts = new Regex(@"\,\s+").Split(remoteVersionText);
                var remoteVersion = new Version(remoteVersionParts[0]);
                var tempPathCore = Path.GetTempPath() + "Styx\\";
                var tempPathStyx = tempPathCore + "Update\\";
                var fullPathWhereToSave = Directory.GetCurrentDirectory() + "\\";

                if (Directory.Exists(tempPathStyx)) DeleteDirectory(tempPathStyx);

                while (FileDownloader.DownloadFile(remoteVersionParts[1], tempPathStyx + remoteVersionParts[2], 60000))
                    if (File.Exists(tempPathStyx + remoteVersionParts[2]))
                    {
                        ZipFile.ExtractToDirectory(tempPathStyx + remoteVersionParts[2], tempPathStyx);
                        File.Delete(tempPathStyx + remoteVersionParts[2]);
                        DirectoryCopy(tempPathStyx, fullPathWhereToSave, true);
                        Console.WriteLine("Update Complete");
                        var gameProcess = new Process();
                        Console.WriteLine("Closing Styx Updater...");
                        gameProcess.StartInfo.FileName = fullPathWhereToSave + "Styx.exe";
                        gameProcess.Start();
                        Environment.Exit(1);
                    }
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to connect to updates server!");
                Thread.Sleep(10000);
                Process.Start("https://styx-bot.com/");
                Environment.Exit(1);
            }
        }

        public static void DeleteDirectory(string targetDir)
        {
            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs) DeleteDirectory(dir);

            Directory.Delete(targetDir, false);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " +
                                                     sourceDirName);

            var dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName)) Directory.CreateDirectory(destDirName);

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            if (copySubDirs)
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
        }
    }
}