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
        
        private async void SlnLoaded(Task<VisualStudioSolution> slnTask)
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

            await CheckVersionsAsync();
        }

        public string SlnPath { get; }
        public string SlnDir { get; }
        public VisualStudioSolution Sln { get; private set; }

        private async Task CheckVersionsAsync()
        {
            //Check if editor and/or engine are a newer version than the version on Github.
            //If not, disable their checkboxes.

            string engineName = typeof(Engine).Assembly.GetName().Name;
            string editorName = Assembly.GetExecutingAssembly().GetName().Name;

            //var engine64Newer = CheckIfNewerBuild(engineName, "ReleaseGame", true);
            var editor64Newer = CheckIfNewerBuild(editorName, "ReleaseEditor", true);
            //var engine86Newer = CheckIfNewerBuild(engineName, "ReleaseGame", false);
            //var editor86Newer = CheckIfNewerBuild(editorName, "ReleaseEditor", false);

            await Task.WhenAll(editor64Newer);

            //await Task.WhenAll(engine64Newer, editor64Newer, engine86Newer, editor86Newer);

            //Engine64IsNewer = engine64Newer.Result;
            Editor64IsNewer = editor64Newer.Result;
            //Engine86IsNewer = engine86Newer.Result;
            //Editor86IsNewer = editor86Newer.Result;

            if (!Engine64IsNewer && !Editor64IsNewer && !Engine86IsNewer && !Editor86IsNewer)
            {
                MessageBox.Show("All builds are up to date.");
                Close();
            }
            else
                SetEnabled();
        }

        public bool Engine64IsNewer { get; private set; }
        public bool Editor64IsNewer { get; private set; }
        public bool Engine86IsNewer { get; private set; }
        public bool Editor86IsNewer { get; private set; }
        
        private void SetEnabled()
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action)SetEnabled);
                return;
            }

            bool engine, editor;
            if (rdx86.Checked)
            {
                engine = Engine86IsNewer;
                editor = Editor86IsNewer;
            }
            else
            {
                engine = Engine64IsNewer;
                editor = Editor64IsNewer;
            }

            chkEngine.Enabled = engine;
            chkEditor.Enabled = editor;
        }

        private void Rdx64_CheckedChanged(object sender, EventArgs e) => SetEnabled();
        private void Rdx86_CheckedChanged(object sender, EventArgs e) => SetEnabled();

        public async Task<bool> CheckIfNewerBuild(string name, string globalConfiguration, bool x64)
        {
            string globalPlatform = x64 ? "x64" : "x86";
            foreach (var project in Sln.Projects)
            {
                //var project = Sln.Projects.FirstOrDefault(x => x.Name.EqualsInvariantIgnoreCase(name));
                project.GetConfigPlatform(
                    globalConfiguration, 
                    globalPlatform,
                    out string localConfiguration,
                    out string localPlatform);

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

                var testGrps = pGrps.Where(pGrp => ConfigConditionMatches(pGrp, localConfiguration, localPlatform));
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
                            Build(globalConfiguration, globalPlatform, Sln.PathForProject(project));
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

        public static bool Build(string configuration, string platform, string projectPath)
        {
            //var instances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

            Dictionary<string, string> props = new Dictionary<string, string>
            {
                { "Configuration",  configuration  },
                { "Platform",       platform       },
            };
            EngineBuildLogger logger = new EngineBuildLogger();
            BuildRequestData request = new BuildRequestData(projectPath, props, null, new[] { "Build" }, null);
            ProjectCollection pc = new ProjectCollection();
            BuildParameters buildParams = new BuildParameters(pc)
            {
                Loggers = new ILogger[] { logger }
            };
            BuildResult result = BuildManager.DefaultBuildManager.Build(buildParams, request);
            if (logger.Errors.Count > 0)
                logger.Display();

            return result.OverallResult == BuildResultCode.Success;
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

        private void BtnPost_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(this, 
                "This will upload to Github and be distributed to all users. Is all information correct?", 
                "Are you sure?", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                Editor.DomainProxy.PostNewReleases(
                    chkEditor.Checked,
                    chkEngine.Checked,
                    rdx86.Checked,
                    SlnDir,
                    txtEditorNotes.Text,
                    txtEngineNotes.Text);

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
