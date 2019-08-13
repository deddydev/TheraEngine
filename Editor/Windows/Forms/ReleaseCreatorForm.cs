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
            MainPanel.Enabled = true;
            Sln = slnTask.Result;
            cboConfiguration.Items.Clear();
            cboConfiguration.Items.AddRange(Sln.Configurations);
            CheckVersions();
        }

        public string SlnPath { get; }
        public string SlnDir { get; }
        public VisualStudioSolution Sln { get; private set; }

        private async void CheckVersions()
        {
            //Check if editor and/or engine are a newer version than the version on Github.
            //If not, disable their checkboxes.

            AssemblyName engineAssembly = typeof(Engine).Assembly.GetName();
            AssemblyName editorAssembly = Assembly.GetExecutingAssembly().GetName();
            var engineUpdate = Github.Updater.HasUpdate(engineAssembly);
            var editorUpdate = Github.Updater.HasUpdate(editorAssembly);
            await Task.WhenAll(engineUpdate, editorUpdate);

            bool hasEngineUpdate = engineUpdate.IsCompleted && engineUpdate.Result.hasUpdate;
            bool hasEditorUpdate = editorUpdate.IsCompleted && editorUpdate.Result.hasUpdate;
            if (!hasEngineUpdate && !hasEditorUpdate)
            {
                MessageBox.Show("No updates found.");
                return;
            }
            //chkEngine.Checked = chkEngine.Enabled = 
            //chkEditor.Checked = chkEditor.Enabled = 
        }

        private void ChkEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkEditor.Checked && !chkEngine.Checked)
                spltcReleaseNotes.Visible = false;
            else
            {
                spltcReleaseNotes.Visible = true;
                spltcReleaseNotes.Panel2Collapsed = !chkEditor.Checked;
            }
        }

        private void ChkEngine_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkEditor.Checked && !chkEngine.Checked)
                spltcReleaseNotes.Visible = false;
            else
            {
                spltcReleaseNotes.Visible = true;
                spltcReleaseNotes.Panel1Collapsed = !chkEngine.Checked;
            }
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
