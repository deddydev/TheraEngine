﻿using Octokit;
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
using System.Text;
using System.IO.Compression;
using System.ComponentModel;
using System.Threading;
using TheraEditor.Core.Extensions;
using TheraEngine.Core;
using System.Net.Http.Headers;
using System.Globalization;
using TheraEngine.Core.Files.Serialization;
using TheraEditor.Windows.Forms;

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
        
        public static readonly byte[] BotAuthToken =
        {
            0x66, 0x38, 0x37, 0x31, 0x64, 0x65, 0x39, 0x31, 0x31, 0x39, 0x31, 0x36,
            0x36, 0x62, 0x61, 0x36, 0x33, 0x36, 0x61, 0x31, 0x32, 0x36, 0x31, 0x35,
            0x61, 0x35, 0x64, 0x31, 0x32, 0x35, 0x32, 0x33, 0x35, 0x36, 0x30, 0x34,
            0x62, 0x62, 0x34, 0x66
        };

        public static string GetBotAuthToken() => Encoding.Default.GetString(BotAuthToken);
        public static Credentials GetBotCredentials() => new Credentials(GetBotAuthToken());
        
        public static class ReleaseCreator
        {
            private static string CreateReleaseTagName(AssemblyName name)
            {
                Version ver = name.Version;
                return $"{name.Name.ReplaceWhitespace("_")}_v{ver.ToString()}";
            }
            public static async Task CreateNewRelease(Assembly assembly, string postBody, string zipFilePath)
            {
                try
                {
                    if (!CheckGithubConnection(out GitHubClient client))
                        return;

                    AssemblyName name = assembly.GetName();
                    string tagName = CreateReleaseTagName(name);
                    NewRelease newRelease = new NewRelease(tagName)
                    {
                        Name = name.Name + " v" + name.Version.ToString(),
                        Body = postBody,
                        Draft = true, //Need to upload zip file
                        Prerelease = false
                    };
                    
                    Release release = await client.Repository.Release.Create(RepoOwner, RepoName, newRelease);
                    using (var archiveContents = File.OpenRead(zipFilePath))
                    {
                        var assetUpload = new ReleaseAssetUpload(
                            Path.GetFileName(zipFilePath),
                            "application/zip",
                            archiveContents,
                            null);

                        var asset = await client.Repository.Release.UploadAsset(release, assetUpload);
                    }
                    ReleaseUpdate updateRelease = release.ToUpdate();
                    updateRelease.Draft = false;
                    release = await client.Repository.Release.Edit(RepoOwner, RepoName, release.Id, updateRelease);
                    
                    Engine.PrintLine("New release created successfully! Find it at " + release.HtmlUrl);
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
        }
        private static bool CheckGithubConnection(out GitHubClient client)
        {
            //Check to see if the user is online and that github is up and running.
            Engine.PrintLine("Checking connection to Github...");
            try
            {
                using (Ping s = new Ping())
                {
                    PingReply reply = s.Send(GithubUrl);
                    if (reply.Status != IPStatus.Success)
                    {
                        Engine.LogWarning($"Could not connect to {GithubUrl}: {reply.Status}");
                        client = null;
                        return false;
                    }
                }
            }
            catch
            {
                Engine.PrintLine("Unable to connect.");
                client = null;
                return false;
            }

            Engine.PrintLine("Connected successfully.");
            client = new GitHubClient(new Octokit.ProductHeaderValue(RepoName))
            {
                Credentials = GetBotCredentials()
            };
            return true;
        }
        public static class Updater
        {
            public static async Task CheckUpdates(params AssemblyName[] assemblies)
            {
                try
                {
                    if (!CheckGithubConnection(out GitHubClient client))
                        return;
                    
                    IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(RepoOwner, RepoName);
                    if (releases == null || releases.Count == 0)
                    {
                        Engine.PrintLine("No updates found.");
                        return;
                    }
                    
                    foreach (AssemblyName name in assemblies)
                    {
                        Version ver = name.Version;
                        Version releaseVer = null;
                        Release newestRelease = null;
                        string tagName;
                        string assemblyName;
                        string versionStr;
                        string[] verParts;

                        foreach (Release release in releases)
                        {
                            tagName = release.TagName;

                            //Searching for the _v backwards will always work 
                            //because version numbers don't contain underscores or letters
                            int verIndex = tagName.FindFirstReverse("_v");
                            if (verIndex > 0)
                            {
                                assemblyName = tagName.Substring(0, verIndex);
                                versionStr = tagName.Substring(verIndex + 2);
                                verParts = versionStr.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                if (verParts.Length == 4)
                                {
                                    Version thisVer = new Version(
                                        int.Parse(verParts[0]),
                                        int.Parse(verParts[1]),
                                        int.Parse(verParts[2]),
                                        int.Parse(verParts[3]));

                                    if (releaseVer == null || thisVer.CompareTo(releaseVer) > 0)
                                    {
                                        releaseVer = thisVer;
                                        newestRelease = release;
                                    }
                                }
                            }
                        }

                        if (newestRelease == null)
                        {
                            Engine.PrintLine("No updates found.");
                            return;
                        }

                        int comp = releaseVer.CompareTo(ver);
                        if (comp <= 0)
                        {
                            Engine.PrintLine("You are running the most recent version.");
                            return;
                        }

                        DialogResult update = MessageBox.Show(newestRelease.Name + " is available! Update now?", "Update", MessageBoxButtons.YesNo);
                        if (update != DialogResult.Yes)
                        {
                            Engine.PrintLine("Update canceled.");
                            return;
                        }
                                
                        DialogResult overwrite = MessageBox.Show("Overwrite current installation?", "Overwrite", MessageBoxButtons.YesNoCancel);
                        if (overwrite == DialogResult.Cancel)
                        {
                            Engine.PrintLine("Update canceled.");
                            return;
                        }
                            
                        Repository repo = await client.Repository.Get(RepoOwner, RepoName);
                        ReleaseAsset asset = (await client.Repository.Release.GetAllAssets(RepoOwner, repo.Name, newestRelease.Id))[0];
                                        
                        string exeDir = System.Windows.Forms.Application.StartupPath;
                        string zipDir = Path.Combine(exeDir, newestRelease.TagName);
                        Directory.CreateDirectory(exeDir);

                        string token = GetBotAuthToken();
                        string credentials = string.Format(CultureInfo.InvariantCulture, ":{0}", token);
                        credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));

                        string localUpdateZipPath = Path.Combine(exeDir, newestRelease.TagName + ".zip");
                        string localUpdateUnzipPath = Path.Combine(exeDir, newestRelease.TagName);
                        string zipUrl = $"https://{token}:@api.github.com/repos/{RepoOwner}/{RepoName}/releases/assets/{asset.Id}";

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.Headers.Add(HttpRequestHeader.UserAgent, BotUsername);
                            webClient.Headers.Add(HttpRequestHeader.Authorization, "Basic " + credentials);
                            webClient.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
                            
                            int op = Editor.Instance.BeginOperation("Downloading zip file...", out Progress<float> progress, out CancellationTokenSource cancel);
                            IProgress<float> iProg = progress;
                            void downloadFileProgressChanged(object sender, DownloadProgressChangedEventArgs args)
                                => iProg.Report((float)args.BytesReceived / args.TotalBytesToReceive);
                            webClient.DownloadProgressChanged += downloadFileProgressChanged;
                            await webClient.DownloadFileTaskAsync(zipUrl, localUpdateZipPath);
                            webClient.DownloadProgressChanged -= downloadFileProgressChanged;
                            Editor.Instance.EndOperation(op);

                            Engine.PrintLine("Success!");
                            Engine.PrintLine("Starting install...");

                            Directory.CreateDirectory(localUpdateUnzipPath);

                            op = Editor.Instance.BeginOperation("Extracting new update...", out progress, out cancel);
                            iProg = progress;
                            await Task.Run(() => ZipFileWithProgress.ExtractToDirectory(localUpdateZipPath, localUpdateUnzipPath, iProg));
                            Editor.Instance.EndOperation(op);

                            if (overwrite == DialogResult.Yes)
                            {
                                string exePath = System.Windows.Forms.Application.ExecutablePath;
                                string localUpdateDirName = Path.GetFileName(localUpdateUnzipPath);
                                Process currentProcess = Process.GetCurrentProcess();
                                string pid = currentProcess.Id.ToString();
                                string tempPath = Environment.GetEnvironmentVariable("TMP");
                                string batName = SerializationCommon.ResolveFileName(tempPath, "TheraEngineUpdate", "bat");
                                string batPath = tempPath + Path.DirectorySeparatorChar + batName;

                                string batchCode =
                                    $"@echo off" + Environment.NewLine +
                                    $"taskkill /pid {pid} /f" + Environment.NewLine +
                                    $"for /d %%I in ({exeDir}\\*) do (" + Environment.NewLine +
                                    $"if /i not \"%%~nxI\" equ \"{localUpdateDirName}\" rmdir /q /s \"%%~I\"" + Environment.NewLine +
                                    $")" + Environment.NewLine +
                                    $"del /q {exeDir}\\*" + Environment.NewLine +
                                    $"xcopy \"{localUpdateUnzipPath}\" \"{exeDir}\" /E /y" + Environment.NewLine +
                                    $"rmdir /S /Q \"{localUpdateUnzipPath}\"" + Environment.NewLine +
                                    $"Start \"\"  \"{exePath}\"" + Environment.NewLine +
                                    $"del %0";

                                File.WriteAllText(batPath, batchCode);

                                ProcessStartInfo info = new ProcessStartInfo(batPath)
                                {
                                    CreateNoWindow = false,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    UseShellExecute = true,
                                };
                                Process.Start(info);
                            }

                            return;
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

            private static void BatProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
                => Engine.PrintLine(e.Data);
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
                //string octokitPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Octokit.dll");

                ////Prevent crash that occurs when this dll is not present
                //if (!File.Exists(octokitPath))
                //{
                //    Engine.LogWarning("Octokit.dll not found.");
                //    return;
                //}

                try
                {
                    var github = new GitHubClient(new Octokit.ProductHeaderValue(RepoName))
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

                                        Issue x = await github.Issue.Update(RepoOwner, RepoName, i.Number, update);
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
                            Issue x = await github.Issue.Create(RepoOwner, RepoName, issue);
                        }
                    }
                }
                catch
                {
                    Engine.LogWarning("The application was unable to retrieve permission to send this issue.");
                }
            }
        }
    }
}