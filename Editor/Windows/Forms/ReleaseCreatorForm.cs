using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.ThirdParty;
using Extensions;
using static TheraEngine.ThirdParty.MSBuild;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Evaluation;
using static TheraEditor.TProject;

namespace TheraEditor.Windows.Forms
{
    public partial class ReleaseCreatorForm : TheraForm
    {
        public ReleaseCreatorForm()
        {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;
            string path = Assembly.GetExecutingAssembly().Location;
            for (int i = 0; i < 5; ++i)
                path = Path.GetDirectoryName(path);
            SlnDir = path;
            SlnPath = Path.Combine(path, "Thera.sln");
            MainPanel.Enabled = false;
            TFileObject.LoadAsync<VisualStudioSolution>(SlnPath).ContinueWith(SlnLoaded);
        }
        
        private void SlnLoaded(Task<VisualStudioSolution> slnTask)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<Task<VisualStudioSolution>>)SlnLoaded, slnTask);
                return;
            }

            MainPanel.Enabled = true;
            Sln = slnTask.Result;
            cboConfiguration.Items.Clear();
            cboConfiguration.Items.AddRange(Sln.Configurations);
            if (cboConfiguration.Items.Count > 0)
                cboConfiguration.SelectedIndex = 0;
            CheckVersions();
        }

        public string SlnPath { get; }
        public string SlnDir { get; }
        public VisualStudioSolution Sln { get; private set; }

        private void CheckVersions()
        {
            //Check if editor and/or engine are a newer version than the version on Github.
            //If not, disable their checkboxes.

            string engineName = typeof(Engine).Assembly.GetName().Name;
            string editorName = Assembly.GetExecutingAssembly().GetName().Name;

            var engineNewer = CheckIfNewerBuild(engineName, "ReleaseGame", "x64");
            var editorNewer = CheckIfNewerBuild(editorName, "ReleaseEditor", "x64");
            Task.WhenAll(engineNewer, editorNewer).ContinueWith(SetEnabled);
        }

        private void SetEnabled(Task<bool[]> result)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<Task<bool[]>>)SetEnabled, result);
                return;
            }
            chkEngine.Checked = chkEngine.Enabled = result.Result[0];
            chkEditor.Checked = chkEditor.Enabled = result.Result[1];
        }

        public async Task<bool> CheckIfNewerBuild(string name, string configuration, string platform)
        {
            foreach (var project in Sln.Projects)
            {
                //var project = Sln.Projects.FirstOrDefault(x => x.Name.EqualsInvariantIgnoreCase(name));
                var msbuild = await Sln.ReadProjectAsync(project);
                //msbuild.Variables["Configuration"] = configuration;
                //msbuild.Variables["Platform"] = platform;

                var pGrps = msbuild.PropertyGroupElements;
                if (!pGrps.Any(pGrp => GroupHasAssemblyName(pGrp, name)))
                    continue;

                var typeProp = pGrps.Select(pGrp => pGrp.PropertyElements.
                    FirstOrDefault(prop => string.Equals(prop.ElementName, "OutputType", StringComparison.InvariantCultureIgnoreCase))).
                    FirstOrDefault(prop => prop != null);

                string typeExt = ".exe";
                if (typeProp != null)
                {
                    switch (typeProp.StringContent.Value)
                    {
                        default:
                        case "WinExe":
                            typeExt = ".exe";
                            break;
                        case "Library":
                            typeExt = ".dll";
                            break;
                    }
                }

                var testGrps = pGrps.Where(pGrp => ConfigConditionMatches(pGrp, configuration, platform));
                foreach (var pGrp in testGrps)
                {
                    var outputPathProp = pGrp.PropertyElements.FirstOrDefault(prop =>
                        string.Equals(prop.ElementName, "OutputPath", StringComparison.InvariantCultureIgnoreCase));

                    if (outputPathProp is null)
                        continue;

                    string path = outputPathProp?.StringContent?.Value;
                    if (!path.IsAbsolutePath())
                    {
                        string projPath = Sln.PathForProject(project);
                        string projDirPath = Path.GetDirectoryName(projPath);
                        path = Path.GetFullPath(Path.Combine(projDirPath, path));
                    }
                    path += name + typeExt;
                    if (path.IsValidPath())
                    {
                        if (!File.Exists(path))
                        {
                            Dictionary<string, string> props = new Dictionary<string, string>
                            {
                                { "Configuration",  configuration  },
                                { "Platform",       platform       },
                            };
                            EngineBuildLogger logger = new EngineBuildLogger();
                            BuildRequestData request = new BuildRequestData(Sln.PathForProject(project), props, null, new[] { "Build" }, null);
                            ProjectCollection pc = new ProjectCollection();
                            BuildParameters buildParams = new BuildParameters(pc)
                            {
                                Loggers = new ILogger[] { logger }
                            };
                            BuildManager.DefaultBuildManager.Build(buildParams, request);
                            if (logger.Errors.Count > 0)
                                logger.Display();
                        }
                        if (File.Exists(path))
                        {
                            AssemblyName assembly = AssemblyName.GetAssemblyName(path);
                            if (assembly != null)
                            {
                                var (result, newestRelease) = await Github.Updater.HasUpdate(assembly);
                                return result == Github.Updater.EVersionRelation.IsNewerBuild;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool GroupHasAssemblyName(PropertyGroup group, string assemblyName)
            => group.GetChildren<PropertyGroup.Property>().Any(prop =>
                string.Equals(prop.ElementName, "AssemblyName", StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(prop.StringContent.Value, assemblyName, StringComparison.InvariantCultureIgnoreCase));
        
        private bool ConfigConditionMatches(PropertyGroup group, string configuration, string platform)
        {
            string cond = group?.Condition?.Trim();
            return string.IsNullOrWhiteSpace(cond) || string.Equals(cond,
                $"'$(Configuration)|$(Platform)' == '{configuration}|{platform}'",
                StringComparison.InvariantCultureIgnoreCase);
        }

        private void ChkEditor_CheckedChanged(object sender, EventArgs e) => CheckBoxesChanged();
        private void ChkEngine_CheckedChanged(object sender, EventArgs e) => CheckBoxesChanged();
        private void CheckBoxesChanged()
        {
            if (!chkEditor.Checked && !chkEngine.Checked)
                spltcReleaseNotes.Visible = false;
            else
            {
                spltcReleaseNotes.Visible = true;
                spltcReleaseNotes.Panel1Collapsed = !chkEngine.Checked;
                spltcReleaseNotes.Panel2Collapsed = !chkEditor.Checked;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void BtnPost_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, 
                "This will upload to Github and be distributed to all users. Is all information correct?", 
                "Are you sure?", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                List<Task> tasks = new List<Task>();

                if (chkEditor.Checked)
                {
                    string path = ResolveEditorPath(rdx86.Checked);
                    tasks.Add(Github.ReleaseCreator.New(path, txtEditorNotes.Text));
                }

                if (chkEngine.Checked)
                {
                    string path = ResolveEnginePath(rdx86.Checked);
                    tasks.Add(Github.ReleaseCreator.New(path, txtEngineNotes.Text));
                }

                await Task.WhenAll(tasks);

                DialogResult = DialogResult.OK;
                Close();
            }
            DialogResult = DialogResult.Cancel;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Instance = null;
        }

        private string GetPlatform(bool x86) => x86 ? "x86" : "x64";
        //TODO: read sln and csproj files to get exact directory
        //in case build config changes in the future
        private string ResolveEnginePath(bool x86)
            => Path.Combine(SlnDir, "Build", "Release", "Game", GetPlatform(x86), "TheraEngine.dll");
        private string ResolveEditorPath(bool x86)
            => Path.Combine(SlnDir, "Build", "Release", "Editor", GetPlatform(x86), "TheraEditor.exe");
        
        public static ReleaseCreatorForm Instance { get; private set; }
        public static void ShowInstance()
        {
            if (Instance != null && !(Instance.IsDisposed || Instance.Disposing))
                Instance.Focus();
            else
            {
                Instance = new ReleaseCreatorForm();
                Instance.Show(Editor.Instance);
            }
        }
        private void BtnSaveDefaultEngine_Click(object sender, EventArgs e)
        {
            var dref = Editor.GetSettingsRef();
            
        }
        private void BtnSaveDefaultEditor_Click(object sender, EventArgs e)
        {

        }
    }
}
