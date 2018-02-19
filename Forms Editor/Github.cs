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
        public static readonly string GithubUrl = "www.github.com";
        public static readonly string BotUsername = "TheraEditorBot";
        public static readonly string RepoOwner = "BlackJax96";
        public static readonly string RepoName = "TheraEngine";

        public static readonly string BaseURL = 
            $"https://{GithubUrl}/{RepoOwner}/{RepoName}/releases/download/";

        public static string AppPath =
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

        public static class Updater
        {
            public static async Task UpdateCheck() => await UpdateCheck(false);
            public static async Task UpdateCheck(bool allowOverwrite)
            {
                //Check to see if the user is online and that github is up and running.
                Engine.PrintLine("Checking connection to server...");
                using (Ping s = new Ping())
                {
                    PingReply reply = s.Send(GithubUrl);
                    if (reply.Status != IPStatus.Success)
                    {
                        Engine.LogWarning($"Could not connect to {GithubUrl}: {reply.Status}");
                        return;
                    }
                }

                // Initiate the github client.
                GitHubClient github = new GitHubClient(new ProductHeaderValue(RepoName));

                // get repo, Release, and release assets
                Repository repo;
                IReadOnlyList<Release> releases = null;
                try
                {
                    repo = await github.Repository.Get(RepoOwner, RepoName);
                    releases = await github.Repository.Release.GetAll(repo.Owner.Login, repo.Name);
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    Engine.LogWarning("Unable to connect to the internet.");
                    return;
                }

                if (releases.Count == 0)
                    return;

                Release release = releases[0];
                ReleaseAsset Asset = await github.Repository.Release.GetAsset(RepoOwner, repo.Name, release.Id);

                // Check if we were passed in the overwrite parameter, and if not create a new folder to extract in.
                if (!allowOverwrite)
                {
                    Directory.CreateDirectory(AppPath + "/" + release.TagName);
                    AppPath += "/" + release.TagName;
                }
                else
                {
                    //Find and close the editor application that will be overwritten
                    //Process[] px = Process.GetProcessesByName("TheraEditor");
                    //Process p = px.FirstOrDefault(x => x.MainModule.FileName.StartsWith(AppPath));
                    //if (p != null && p != default(Process) && p.CloseMainWindow())
                    //    p.Close();
                }

                using (WebClient client = new WebClient())
                {
                    // Add the user agent header, otherwise we will get access denied.
                    client.Headers.Add("User-Agent: Other");

                    // Full asset streamed into a single string
                    string html = client.DownloadString(Asset.Url);

                    // The browser download link to the self extracting archive, hosted on github
                    string URL = html.Substring(html.IndexOf(BaseURL)).TrimEnd(new char[] { '}', '"' });

                    Engine.PrintLine("Downloading update...");
                    client.DownloadFile(URL, AppPath + "/Update.exe");
                    Engine.PrintLine("Success!");
                    
                    Engine.PrintLine("Starting install...");

                    Process update = Process.Start(AppPath + "/Update.exe", "-o\"" + AppPath + "\" -y");
                    update.Exited += Update_Exited;
                }
            }

            private static void Update_Exited(object sender, EventArgs e)
            {
                DialogResult result = MessageBox.Show("Install completed. Restart now?", "Install complete", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }

            public static async Task CheckUpdates(string releaseTag, bool manual = true)
            {
                try
                {
                    var github = new GitHubClient(new ProductHeaderValue(RepoName));
                    IReadOnlyList<Release> releases = null;
                    try
                    {
                        releases = await github.Repository.Release.GetAll(RepoOwner, RepoName);

                        // Check if this is a known pre-release version
                        bool isPreRelease = releases.Any(r => r.Prerelease
                            && string.Equals(releases[0].TagName, releaseTag, StringComparison.InvariantCulture)
                            && r.Name.IndexOf(RepoName, StringComparison.InvariantCultureIgnoreCase) >= 0);

                        // If this is not a known pre-release version, remove all pre-release versions from the list
                        if (!isPreRelease)
                        {
                            releases = releases.Where(r => !r.Prerelease).ToList();
                        }
                    }
                    catch (System.Net.Http.HttpRequestException)
                    {
                        MessageBox.Show("Unable to connect to the internet.");
                        return;
                    }

                    if (releases != null &&
                        releases.Count > 0 &&
                        !String.Equals(releases[0].TagName, releaseTag, StringComparison.InvariantCulture) && //Make sure the most recent version is not this version
                        releases[0].Name.IndexOf("BrawlBox", StringComparison.InvariantCultureIgnoreCase) >= 0) //Make sure this is a BrawlBox release
                    {
                        DialogResult UpdateResult = MessageBox.Show(releases[0].Name + " is available! Update now?", "Update", MessageBoxButtons.YesNo);
                        if (UpdateResult == DialogResult.Yes)
                        {
                            DialogResult OverwriteResult = MessageBox.Show("Overwrite current installation?", "", MessageBoxButtons.YesNoCancel);
                            if (OverwriteResult != DialogResult.Cancel)
                            {
                                Task t = UpdateCheck(OverwriteResult == DialogResult.Yes);
                                t.Wait();
                            }
                        }
                    }
                    else if (manual)
                        MessageBox.Show("No updates found.");
                }
                catch (Exception e)
                {
                    if (manual)
                        MessageBox.Show(e.Message);
                }
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

                    if (releases != null && releases.Count > 0 && releases[0].TagName != TagName)
                    {
                        //This build's version tag does not match the latest release's tag on the repository.
                        //This bug may have been fixed by now. Tell the user to update to be allowed to submit bug reports.

                        DialogResult UpdateResult = MessageBox.Show(releases[0].Name + " is available!\nYou cannot submit bug reports using an older version of the editor.\nUpdate now?", "An update is available", MessageBoxButtons.YesNo);
                        if (UpdateResult == DialogResult.Yes)
                        {
                            DialogResult OverwriteResult = MessageBox.Show("Overwrite current installation?", "", MessageBoxButtons.YesNoCancel);
                            if (OverwriteResult != DialogResult.Cancel)
                            {
                                Task t = Updater.UpdateCheck(OverwriteResult == DialogResult.Yes);
                                t.Wait();
                            }
                        }
                    }
                    else
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