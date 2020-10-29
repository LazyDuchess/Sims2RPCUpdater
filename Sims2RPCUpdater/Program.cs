using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Data;
using System.IO.Compression;

namespace Sims2RPCUpdater
{
    public static class Program
    {
        public enum UpdateResult { FAILED, SUCCESS, ALREADY, PROBLEMS };

        public static ZipArchive OpenRead(string filename)
        {
            return new ZipArchive(File.OpenRead(filename), ZipArchiveMode.Read);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static UpdateResult Update(bool manual, ref string exceptions)
        {
            var problems = false;
            var versionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Sims2RPC");
            var versionFile = Path.Combine(versionFolder, "version.txt");
            if (!Directory.Exists(versionFolder))
                Directory.CreateDirectory(versionFolder);
            string s = "";
            WebClient client = new WebClient();
            //client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
            s = client.DownloadString("https://lazyduchess.github.io/Sims2RPC.com/version.html");
            if (File.Exists(versionFile))
            {
                if (File.ReadAllText(versionFile) != s || manual == false)
                {
                    client.DownloadFile("https://github.com/LazyDuchess/Sims2RPC.com/releases/download/" + s + "/Sims2RPC.zip", "Sims2RPC.zip");
                    //ZipFile.ExtractToDirectory("Sims2RPC.zip", Directory.GetCurrentDirectory());
                    ZipArchive zipArchive = OpenRead("Sims2RPC.zip");
                    foreach (ZipArchiveEntry entry in zipArchive.Entries)
                    {
                        if (entry.Name != "")
                        {
                            try
                            {
                                var path = Path.GetDirectoryName(entry.FullName);
                                if (path != "")
                                {
                                    if (!Directory.Exists(path))
                                    {
                                        Directory.CreateDirectory(path);
                                    }
                                }
                                entry.ExtractToFile(entry.FullName, true);
                            }
                            catch(Exception e)
                            {
                                problems = true;
                                exceptions += entry.Name + ":" + Environment.NewLine + e.ToString() + Environment.NewLine;
                            }
                        }
                    }
                    zipArchive.Dispose();
                    File.Delete("Sims2RPC.zip");
                    File.WriteAllText(versionFile, s);
                    return UpdateResult.SUCCESS;
                }
                else
                    return UpdateResult.ALREADY;
            }
            else
            {
                client.DownloadFile("https://github.com/LazyDuchess/Sims2RPC.com/releases/download/" + s + "/Sims2RPC.zip", "Sims2RPC.zip");
                //ZipFile.ExtractToDirectory("Sims2RPC.zip", Directory.GetCurrentDirectory());
                ZipArchive zipArchive = OpenRead("Sims2RPC.zip");
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    if (entry.Name != "")
                    {
                        try { 
                        var path = Path.GetDirectoryName(entry.FullName);
                        if (path != "")
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                        }
                        entry.ExtractToFile(entry.FullName, true);
                    }
                            catch (Exception e)
                    {
                        problems = true;
                        exceptions += entry.Name + ":" + Environment.NewLine + e.ToString() + Environment.NewLine;
                    }
                }
                }
                zipArchive.Dispose();
                File.Delete("Sims2RPC.zip");
                File.WriteAllText(versionFile, s);
                if (problems)
                    return UpdateResult.PROBLEMS;
                return UpdateResult.SUCCESS;
            }
            return UpdateResult.FAILED;
        }
    }
}
