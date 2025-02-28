using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using StyxLauncher.Properties;

namespace StyxLauncher
{
    internal class Program
    {
        private static readonly string RemoteVersionUrl = "http://alphahub.org/styx/version2.txt";
        private static readonly string UpdaterUrl = "http://alphahub.org/styx/updater.zip";

        private static bool _mpgh = false;
        private static string _remoteVersionText = "1.1.1";
        private static readonly string Version = "1.1.1";
        private static string _gameFilePath = "steam://rungameid/429790";
        private static readonly string Uri = Directory.GetCurrentDirectory() + "\\settings.xml";
        private static readonly string TempPathCore = Path.GetTempPath() + "Styx\\";
        private static readonly string TempPath = TempPathCore + "StyxUpdater\\";

        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (!_mpgh)
            {
                Console.WriteLine($"Styx Bot v{Version}");
            }
            else
            {
                Console.WriteLine($"Styx Bot v{Version} (MPGH VERSION)");
                Console.WriteLine($"Check STYX-BOT.COM or Discord for latest release");
            }

            Thread.Sleep(3000);

            Console.ForegroundColor = ConsoleColor.White;
            if (!_mpgh) CheckUpdate();

            Console.ForegroundColor = ConsoleColor.Cyan;
            var gameProcess = new Process();
            if (File.Exists(Uri) && Process.GetProcessesByName("aq3d").FirstOrDefault() == null)
                using (var reader = XmlReader.Create(Uri))
                {
                    while (reader.Read())
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Settings")
                            if (reader.HasAttributes)
                            {
                                if (File.Exists(reader.GetAttribute("GameFilePath")))
                                    _gameFilePath = reader.GetAttribute("GameFilePath");
                                Console.WriteLine("Launching AdventureQuest 3D...");
                                gameProcess.StartInfo.FileName = _gameFilePath;
                                gameProcess.Start();
                            }
                }

            Console.WriteLine("Searching for AdventureQuest 3D...");
            Thread.Sleep(10000);
            AwaitProcess().Wait();
        }

        private static async Task AwaitProcess()
        {
            var target = await GetTargetProcess();
            var mono = await GetMonoModule(target);

            var monoHandle = UnsafeNativeMethods.LoadLibrary(mono.FileName);
            /* Load mono.dll so that we can obtain the addresses f its exported functions */

            if (monoHandle == IntPtr.Zero)
            {
                Console.WriteLine("Unable to obtain mono functions (failed to load mono.dll)");
                Console.ReadLine();
                Environment.Exit(0);
            }

            var injector = new Injector(target.Handle, monoHandle);

            try
            {
                injector.Inject(Resources.Styx, "Styx.Core", "Loader", "Load");
            }
            catch (ApplicationException ae)
            {
                Console.WriteLine($"Injection failed: {ae.Message}");
                Console.ReadKey();
            }
            finally
            {
                UnsafeNativeMethods.FreeLibrary(monoHandle);
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

            foreach (var dir in dirs)
                DeleteDirectory(dir);


            Directory.Delete(targetDir, false);
        }

        private static void CheckUpdate()
        {
            var webClient = new WebClient();
            try
            {
                Console.WriteLine("Checking for updates...");
                _remoteVersionText = webClient.DownloadString(new Uri(RemoteVersionUrl));

                var remoteVersionParts = new Regex(@"\,\s+").Split(_remoteVersionText);
                var localVersion = new Version(Version.Trim());
                var remoteVersion = new Version(remoteVersionParts[0]);
                var forced = remoteVersionParts[3];
                var fullPathWhereToSave = Directory.GetCurrentDirectory() + "\\";
                if (Directory.Exists(TempPathCore)) DeleteDirectory(TempPathCore);

                if (remoteVersion > localVersion)
                {
                    Console.WriteLine("Styx v" + remoteVersion + " is available.");
                    while (true)
                    {
                        if (forced == "true")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Mandatory update... Press any key to continue");
                            Console.ForegroundColor = ConsoleColor.White;
                            Thread.Sleep(5000);
                        }
                        else
                        {
                            Console.Write("Perform update? (y / n): ");
                        }

                        if (forced == "true" || Console.ReadLine().Trim().ToLower().StartsWith("y"))
                            while (FileDownloader.DownloadFile(UpdaterUrl, TempPath + "updater.zip", 120000))
                                if (File.Exists(TempPath + "updater.zip"))
                                {
                                    Console.WriteLine("Opening Updater");
                                    ZipFile.ExtractToDirectory(TempPath + "updater.zip", TempPath);
                                    DirectoryCopy(TempPath, fullPathWhereToSave, true);

                                    if (File.Exists(fullPathWhereToSave + "updater.exe"))
                                    {
                                        var gameProcess = new Process();
                                        gameProcess.StartInfo.FileName = fullPathWhereToSave + "updater.exe";
                                        gameProcess.Start();
                                        Environment.Exit(1);
                                    }
                                }
                                else
                                    break;
                    }
                }
                else
                {
                    if (File.Exists(fullPathWhereToSave + "updater.exe"))
                        File.Delete(fullPathWhereToSave + "updater.exe");
                    if (File.Exists(fullPathWhereToSave + "updater.zip"))
                        File.Delete(fullPathWhereToSave + "updater.zip");
                    if (Directory.Exists(TempPath)) Directory.Delete(TempPath);
                }
            }
            catch (Exception)
            {
                OnError("Was not able to download update!");
            }
        }

        private static void OnError(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(10000);
            Environment.Exit(1);
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

        private static async Task<Process> GetTargetProcess()
        {
            Process target;
            while ((target = Process.GetProcessesByName("aq3d").FirstOrDefault()) == null) await Task.Delay(5000);
            return target;
        }

        private static async Task<ProcessModule> GetMonoModule(Process p)
        {
            ProcessModule mono;
            while ((mono = p.Modules.Cast<ProcessModule>().FirstOrDefault(pm => pm.ModuleName == "mono.dll")) == null)
                await Task.Delay(1000);
            return mono;
        }
    }
}