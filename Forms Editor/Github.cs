using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using TheraEngine;

namespace TheraEditor
{
    public static class Github
    {
        public enum UpdaterCode
        {
            NoError,
            OctokitNotFound,
            NoInternetConnection,
            NoReleasesAvailable,
            UnhandledErrorOccurred,
            NoArguments,
        }

        public static readonly string GithubUrl = "www.github.com";
        public static readonly string BotUsername = "TheraEditorBot";
        public static readonly string RepoOwner = "BlackJax96";
        public static readonly string RepoName = "TheraEngine";

        public static readonly string ReleasesURL =
            $"https://{GithubUrl}/{RepoOwner}/{RepoName}/releases/download/";

        public static readonly string UpdaterDirectoryPath =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static readonly byte[] BotAuthToken =
        {
            0x66, 0x38, 0x37, 0x31, 0x64, 0x65, 0x39, 0x31, 0x31, 0x39, 0x31, 0x36,
            0x36, 0x62, 0x61, 0x36, 0x33, 0x36, 0x61, 0x31, 0x32, 0x36, 0x31, 0x35,
            0x61, 0x35, 0x64, 0x31, 0x32, 0x35, 0x32, 0x33, 0x35, 0x36, 0x30, 0x34,
            0x62, 0x62, 0x34, 0x66
        };

        public static Credentials GetBotCredentials() =>
            new Credentials(System.Text.Encoding.Default.GetString(BotAuthToken));

        public class AssemblyVersion
        {
            public AssemblyVersion(string ver)
            {
                string[] verParts = ver.Split('.');
                Major = int.Parse(verParts[0]);
                Minor = int.Parse(verParts[1]);
                Build = int.Parse(verParts[2]);
                Revision = int.Parse(verParts[3]);
            }
            public AssemblyVersion(int major, int minor, int build, int revision)
            {
                Major = major;
                Minor = minor;
                Build = build;
                Revision = revision;
            }

            int Major { get; set; }
            int Minor { get; set; }
            int Build { get; set; }
            int Revision { get; set; }

            public static AssemblyVersion FromTagName(string tagName)
            {
                string[] parts = tagName.Split(' ');
                string ver = parts[parts.Length - 1];
                return new AssemblyVersion(ver);
            }
            public AssemblyVersion Clone()
            {
                return new AssemblyVersion(Major, Minor, Build, Revision);
            }
            public static bool operator >(AssemblyVersion left, AssemblyVersion right)
            {
                if (left.Major > right.Major)
                    return true;
                else if (left.Major == right.Major)
                {
                    if (left.Minor > right.Minor)
                        return true;
                    else if (left.Minor == right.Minor)
                    {
                        if (left.Build > right.Build)
                            return true;
                        else if (left.Build == right.Build)
                        {
                            if (left.Revision > right.Revision)
                                return true;
                        }
                    }
                }
                return false;
            }
            public static bool operator <(AssemblyVersion left, AssemblyVersion right)
            {
                if (left.Major < right.Major)
                    return true;
                else if (left.Major == right.Major)
                {
                    if (left.Minor < right.Minor)
                        return true;
                    else if (left.Minor == right.Minor)
                    {
                        if (left.Build < right.Build)
                            return true;
                        else if (left.Build == right.Build)
                        {
                            if (left.Revision < right.Revision)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public static class Updater
        {
            public static async Task CheckUpdates(params string[] tagNames)
            {
                try
                {
                    //Check to see if the user is online and that github is up and running.
                    Engine.PrintLine("Checking connection to Github...");
                    using (Ping s = new Ping())
                    {
                        PingReply reply = s.Send(GithubUrl);
                        if (reply.Status != IPStatus.Success)
                        {
                            Engine.LogWarning($"Could not connect to {GithubUrl}: {reply.Status}");
                            return;
                        }
                    }
                    Engine.PrintLine("Connected successfully.");

                    GitHubClient github = new GitHubClient(new ProductHeaderValue(RepoName)) { Credentials = GetBotCredentials() };

                    IReadOnlyList<Release> releases = await github.Repository.Release.GetAll(RepoOwner, RepoName);
                    if (releases == null || releases.Count == 0)
                    {
                        Engine.PrintLine("No updates found.");
                        return;
                    }

                    //// Check if this is a known pre-release version
                    //bool isPreRelease = releases.Any(r => r.Prerelease
                    //    && string.Equals(releases[0].TagName, editorTag, StringComparison.InvariantCulture)
                    //    && r.Name.IndexOf(RepoName, StringComparison.InvariantCultureIgnoreCase) >= 0);

                    //// If this is not a known pre-release version, remove all pre-release versions from the list
                    //if (!isPreRelease)
                    //    releases = releases.Where(r => !r.Prerelease).ToList();

                    foreach (string tagName in tagNames)
                    {
                        string currentReleaseName = tagName.Substring(0, tagName.LastIndexOf(" "));

                        AssemblyVersion newestVer = AssemblyVersion.FromTagName(tagName);
                        Release newerRelease = null;
                        foreach (Release r in releases)
                        {
                            string releaseName = tagName.Substring(0, tagName.LastIndexOf(" "));
                            if (string.Equals(currentReleaseName, releaseName, StringComparison.InvariantCulture))
                            {
                                AssemblyVersion repoVer = AssemblyVersion.FromTagName(r.TagName);
                                if (repoVer > newestVer)
                                {
                                    newerRelease = r;
                                    newestVer = repoVer;
                                }
                            }
                        }

                        if (newerRelease != null)
                        {
                            DialogResult UpdateResult = MessageBox.Show(newerRelease.Name + " is available! Update now?", "Update", MessageBoxButtons.YesNo);
                            if (UpdateResult == DialogResult.Yes)
                            {
                                DialogResult OverwriteResult = MessageBox.Show("Overwrite current installation?", "Overwrite", MessageBoxButtons.YesNoCancel);
                                if (OverwriteResult != DialogResult.Cancel)
                                {
                                    Repository repo = await github.Repository.Get(RepoOwner, RepoName);
                                    ReleaseAsset asset = await github.Repository.Release.GetAsset(RepoOwner, repo.Name, newerRelease.Id);

                                    string installDirPath = UpdaterDirectoryPath;

                                    // Check if we were passed in the overwrite parameter, and if not create a new folder to extract in.
                                    if (OverwriteResult == DialogResult.No)
                                    {
                                        installDirPath = Path.Combine(UpdaterDirectoryPath, newerRelease.TagName);
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

                                        Engine.PrintLine("Downloading update...");
                                        client.DownloadFile(URL, updatePath);
                                        Engine.PrintLine("Success!");

                                        Engine.PrintLine("Starting install...");

                                        Process installer = Process.Start(updatePath, "-o\"" + UpdaterDirectoryPath + "\" -y");
                                        installer.Exited += Update_Exited;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Engine.PrintLine("No updates found.");
                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    Engine.LogWarning("Unable to connect to the internet.");
                }
                catch (Exception e)
                {
                    Engine.LogException(e);
                }
            }

            private static void Update_Exited(object sender, EventArgs e)
            {

            }
        }

        public static class IssueReporter
        {
            public static async Task PostIssue(
                string TagName,
                string ExceptionMessage,
                string StackTrace,
                string Title,
                string Description)
            {
                string octokitPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Octokit.dll");

                //Prevent crash that occurs when this dll is not present
                if (!File.Exists(octokitPath))
                {
                    Engine.LogWarning("Octokit.dll not found.");
                    return;
                }

                try
                {
                    var github = new GitHubClient(new ProductHeaderValue(RepoName))
                    {
                        Credentials = GetBotCredentials()
                    };

                    IReadOnlyList<Release> releases = null;
                    IReadOnlyList<Issue> issues = null;
                    try
                    {
                        releases = await github.Repository.Release.GetAll(RepoOwner, RepoName);
                        issues = await github.Issue.GetAllForRepository(RepoOwner, RepoName);
                    }
                    catch (System.Net.Http.HttpRequestException)
                    {
                        Engine.LogWarning("Unable to connect to the internet.");
                        return;
                    }

                    //if (releases != null && releases.Count > 0 && releases[0].TagName != TagName)
                    //{
                    //    //This build's version tag does not match the latest release's tag on the repository.
                    //    //This bug may have been fixed by now. Tell the user to update to be allowed to submit bug reports.

                    //    DialogResult UpdateResult = MessageBox.Show(releases[0].Name + " is available!\nYou cannot submit bug reports using an older version of the editor.\nUpdate now?", "An update is available", MessageBoxButtons.YesNo);
                    //    if (UpdateResult == DialogResult.Yes)
                    //    {
                    //        DialogResult OverwriteResult = MessageBox.Show("Overwrite current installation?", "", MessageBoxButtons.YesNoCancel);
                    //        if (OverwriteResult != DialogResult.Cancel)
                    //        {
                    //            Task t = Updater.UpdateCheck(OverwriteResult == DialogResult.Yes);
                    //            t.Wait();
                    //        }
                    //    }
                    //}
                    //else
                    {
                        bool found = false;
                        if (issues != null && !String.IsNullOrEmpty(StackTrace))
                            foreach (Issue i in issues)
                                if (i.State == ItemState.Open)
                                {
                                    string desc = i.Body;
                                    if (desc.Contains(StackTrace) &&
                                        desc.Contains(ExceptionMessage) &&
                                        desc.Contains(TagName))
                                    {
                                        found = true;
                                        IssueUpdate update = i.ToUpdate();

                                        update.Body =
                                            Title + Environment.NewLine +
                                            Description + Environment.NewLine +
                                            Environment.NewLine +
                                            i.Body;

                                        Issue x = await github.Issue.Update("BrawlBox", "BrawlBoxIssues", i.Number, update);
                                    }
                                }

                        if (!found)
                        {
                            NewIssue issue = new NewIssue(Title)
                            {
                                Body =
                                Description + Environment.NewLine +
                                Environment.NewLine +
                                TagName + Environment.NewLine +
                                ExceptionMessage + Environment.NewLine +
                                StackTrace
                            };
                            Issue x = await github.Issue.Create("BrawlBox", "BrawlBoxIssues", issue);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("The application was unable to retrieve permission to send this issue.");
                }
            }
        }
    }
}