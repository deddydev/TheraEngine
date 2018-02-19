using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    public enum UpdaterCode
    {
        UpdatedSuccessfully,
        UpdatedUnsuccessfully,
        NoUpdateFound,
        OctokitNotFound,
        NoInternetConnection,
    }
    class Program
    {
        const string Usage = @"
Usage:
-o = Install latest version
-u = Check for newer versions
";

        static int Main(string[] args)
        {
            string octokitPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Octokit.dll");

            //Prevent crash that occurs when this dll is not present
            if (!File.Exists(octokitPath))
            {
                Console.WriteLine("Octokit.dll not found.");
                return (int)UpdaterCode.OctokitNotFound;
            }

            bool somethingDone = false;
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-o": //Overwrite
                        somethingDone = true;
                        Task t = InstallLatestUpdate(true);
                        t.Wait();
                        break;
                    case "-u": //Update
                        somethingDone = true;
                        Task t2 = CheckUpdates(args[1], args[2], args[3] != "0");
                        t2.Wait();
                        break;
                }
            }
            else if (args.Length == 0)
            {
                somethingDone = true;
                Task t = InstallLatestUpdate();
                t.Wait();
            }

            if (!somethingDone)
                Console.WriteLine(Usage);

            return (int)UpdaterCode.NoUpdateFound;
        }

        public static readonly string GithubUrl = "www.github.com";
        public static readonly string BotUsername = "TheraEditorBot";
        public static readonly string RepoOwner = "BlackJax96";
        public static readonly string RepoName = "TheraEngine";

        public static readonly string ReleasesURL =
            $"https://{GithubUrl}/{RepoOwner}/{RepoName}/releases/download/";

        public static string UpdaterDirectoryPath =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static readonly byte[] BotAuthToken =
        {
            0x66, 0x38, 0x37, 0x31, 0x64, 0x65, 0x39, 0x31, 0x31, 0x39, 0x31, 0x36,
            0x36, 0x62, 0x61, 0x36, 0x33, 0x36, 0x61, 0x31, 0x32, 0x36, 0x31, 0x35,
            0x61, 0x35, 0x64, 0x31, 0x32, 0x35, 0x32, 0x33, 0x35, 0x36, 0x30, 0x34,
            0x62, 0x62, 0x34, 0x66
        };

        public static Credentials GetBotCredentials() =>
            new Credentials(Encoding.Default.GetString(BotAuthToken));

        public static async Task<UpdaterCode> InstallLatestUpdate(bool allowOverwrite = false)
        {
            try
            {
                //Check to see if the user is online and that github is up and running.
                Console.WriteLine("Checking connection to server...");
                using (Ping s = new Ping())
                {
                    PingReply reply = s.Send(GithubUrl);
                    if (reply.Status != IPStatus.Success)
                    {
                        Console.WriteLine($"Could not connect to {GithubUrl}: {reply.Status}");
                        return UpdaterCode.NoInternetConnection;
                    }
                }
                Console.WriteLine("Connected successfully.");

                // Initiate the github client.
                GitHubClient github = new GitHubClient(new ProductHeaderValue(RepoName));

                // get repo, Release, and release assets
                Repository repo;
                IReadOnlyList<Release> releases = null;

                repo = await github.Repository.Get(RepoOwner, RepoName);
                releases = await github.Repository.Release.GetAll(repo.Owner.Login, repo.Name);

                if (releases.Count == 0)
                    return UpdaterCode.NoUpdateFound;

                Release release = releases[0];
                ReleaseAsset asset = await github.Repository.Release.GetAsset(RepoOwner, repo.Name, release.Id);

                string installDirPath = UpdaterDirectoryPath;

                // Check if we were passed in the overwrite parameter, and if not create a new folder to extract in.
                if (!allowOverwrite)
                {
                    installDirPath = Path.Combine(UpdaterDirectoryPath, release.TagName);
                    Directory.CreateDirectory(installDirPath);
                }
                else
                {
                    //Find and close the editor application that will be overwritten
                    Process[] px = Process.GetProcessesByName("TheraEditor");
                    Process p = px.FirstOrDefault(x => x.MainModule.FileName.StartsWith(UpdaterDirectoryPath));
                    if (p != null && p != default(Process) && p.CloseMainWindow())
                        p.Close();
                }

                using (WebClient client = new WebClient())
                {
                    // Add the user agent header, otherwise we will get access denied.
                    client.Headers.Add("User-Agent: Other");

                    // Full asset streamed into a single string
                    string html = client.DownloadString(asset.Url);

                    // The browser download link to the self extracting archive, hosted on github
                    string URL = html.Substring(html.IndexOf(ReleasesURL)).TrimEnd(new char[] { '}', '"' });

                    string updatePath = Path.Combine(UpdaterDirectoryPath, "Update.exe");

                    Console.WriteLine("Downloading update...");
                    client.DownloadFile(URL, updatePath);
                    Console.WriteLine("Success!");

                    Console.WriteLine("Starting install...");

                    Process update = Process.Start(updatePath, "-o\"" + UpdaterDirectoryPath + "\" -y");
                    update.Exited += Update_Exited;
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Console.WriteLine("Unable to connect to the internet.");
                return UpdaterCode.NoInternetConnection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return UpdaterCode.NoUpdateFound;
        }

        private static void Update_Exited(object sender, EventArgs e)
        {

        }

        public static async Task CheckUpdates(string editorTag, string engineTag, bool manual = true)
        {
            try
            {
                var github = new GitHubClient(new ProductHeaderValue(RepoName));
                IReadOnlyList<Release> releases = null;
                
                releases = await github.Repository.Release.GetAll(RepoOwner, RepoName);

                if (releases == null || releases.Count == 0)
                    return;

                // Check if this is a known pre-release version
                bool isPreRelease = releases.Any(r => r.Prerelease
                    && string.Equals(releases[0].TagName, editorTag, StringComparison.InvariantCulture)
                    && r.Name.IndexOf(RepoName, StringComparison.InvariantCultureIgnoreCase) >= 0);

                // If this is not a known pre-release version, remove all pre-release versions from the list
                if (!isPreRelease)
                    releases = releases.Where(r => !r.Prerelease).ToList();
                
                if (!String.Equals(releases[0].TagName, editorTag, StringComparison.InvariantCulture) && //Make sure the most recent version is not this version
                    releases[0].Name.IndexOf(RepoName, StringComparison.InvariantCultureIgnoreCase) >= 0) //Make sure this is a BrawlBox release
                {
                    DialogResult UpdateResult = MessageBox.Show(releases[0].Name + " is available! Update now?", "Update", MessageBoxButtons.YesNo);
                    if (UpdateResult == DialogResult.Yes)
                    {
                        DialogResult OverwriteResult = MessageBox.Show("Overwrite current installation?", "", MessageBoxButtons.YesNoCancel);
                        if (OverwriteResult != DialogResult.Cancel)
                        {
                            Task t = InstallLatestUpdate(OverwriteResult == DialogResult.Yes);
                            t.Wait();
                        }
                    }
                }
                else if (manual)
                    MessageBox.Show("No updates found.");
            }
            catch (System.Net.Http.HttpRequestException)
            {
                Console.WriteLine("Unable to connect to the internet.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
