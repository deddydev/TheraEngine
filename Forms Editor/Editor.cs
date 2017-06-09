using CustomEngine;
using CustomEngine.Files;
using CustomEngine.Tests;
using CustomEngine.Worlds;
using CustomEngine.Worlds.Actors;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheraEditor.Editors;

namespace TheraEditor
{
    public partial class Editor : Form
    {
        private static Editor _instance;
        public static Editor Instance => _instance ?? (_instance = new Editor());

        private SingleFileRef<Project> _project;

        public Project Project
        {
            get => _project;
            set => _project = value;
        }

        public Editor()
        {
            InitializeComponent();
            DoubleBuffered = false;
            renderPanel1.GlobalHud = new EditorHud(renderPanel1);
            renderPanel1.GlobalHud.QueuePossession(PlayerIndex.One);
            //Engine.Settings.OpeningWorld = typeof(TestWorld);
            Engine.Initialize();

            actorPropertyGrid.SelectedObject = Engine.World?.Settings;
            SpawnedActors_Modified();
            if (Engine.World != null)
                Engine.World.State.SpawnedActors.PostModified += SpawnedActors_Modified;
            renderPanel1.RegisterTick();
        }

        private void SpawnedActors_Modified()
        {
            actorTree.Nodes.Clear();
            if (Engine.World != null)
                actorTree.Nodes.AddRange(Engine.World.State.SpawnedActors.Select(x => new TreeNode(((ObjectBase)x).Name) { Tag = x }).ToArray());
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.UnregisterTick();
            Engine.ShutDown();
        }

        private void BtnOpenWorld_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileManager.GetCompleteFilter(typeof(World)),
                Multiselect = false
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                World world = FileObject.Import<World>(ofd.FileName);
                CurrentWorld = world;
            }
        }

        private void BtnNewMaterial_Click(object sender, EventArgs e)
        {
            new MaterialEditorForm().Show();
        }

        private void BtnNewWorld_Click(object sender, EventArgs e)
        {
            CurrentWorld = new World();
        }

        public World CurrentWorld
        {
            get => Engine.World;
            set
            {
                if (Engine.World != null && 
                    Engine.World.UserData is EditorState s && 
                    s._changedFields.Count > 0)
                {
                    DialogResult r = MessageBox.Show(this, "Save changes to current world?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (r == DialogResult.Cancel)
                        return;
                    else if (r == DialogResult.Yes)
                        Engine.World.Export();
                    Engine.World.UserData = null;
                }
                value.UserData = new EditorState();
                value.PropertyChanged += Value_PropertyChanged;
                Engine.World = value;
            }
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((EditorState)((ObjectBase)sender).UserData)._changedFields.Add(e.PropertyName);
        }

        private void ActorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            actorPropertyGrid.SelectedObject = actorTree.SelectedNode == null ? Engine.World.Settings : actorTree.SelectedNode.Tag;
        }

        private bool CloseProject()
        {
            if (Project != null && Project.UserData is EditorState s && s._changedFields.Count > 0)
            {
                DialogResult r = MessageBox.Show(this, "Save changes to current project?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (r == DialogResult.Cancel)
                    return false;
                else if (r == DialogResult.Yes)
                    Project.Export();
                Project.UserData = null;
                return true;
            }
            Project = null;
            return true;
        }
        private void OpenProject(Project p)
        {
            if (p == null)
                return;
            p.UserData = new EditorState();
            p.PropertyChanged += Value_PropertyChanged;
            Project = p;

            contentTree.DisplayProject(p);
        }
        private void btnNewProject_Click(object sender, EventArgs e)
        {
            if (CloseProject())
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = true,
                    Description = "",
                };
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    OpenProject(Project.New(fbd.SelectedPath, "NewProject"));
                }
            }

            bool projectOpened = Project != null;
            btnEngineSettings.Enabled = projectOpened;
            btnProjectSettings.Enabled = projectOpened;
            btnUserSettings.Enabled = projectOpened;
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileObject.GetFileHeader(typeof(Project)).GetFilter(),
            };
            if (ofd.ShowDialog() == DialogResult.OK && CloseProject())
                OpenProject(FileObject.Import<Project>(ofd.FileName));

            bool projectOpened = Project != null;
            btnEngineSettings.Enabled = projectOpened;
            btnProjectSettings.Enabled = projectOpened;
            btnUserSettings.Enabled = projectOpened;
        }

        private void btnSaveProject_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_project.RefPathAbsolute))
            {
                btnSaveProjectAs_Click(sender, e);
                return;
            }
            _project.ExportReference();
        }

        private void btnSaveProjectAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = FileObject.GetFileHeader(typeof(Project)).GetFilter(),
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileObject.GetDirNameFmt(sfd.FileName, out string dir, out string name, out FileFormat fmt);
                _project.ExportReference(dir, name, fmt, true);
            }
        }

        private void btnProjectSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Project;
        }

        private void btnEngineSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Project?.EngineSettings;
        }

        private void btnEditorSettings_Click(object sender, EventArgs e)
        {

        }

        private void btnUserSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Project?.UserSettings;
        }

        private void btnWorldSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Engine.World?.Settings;
        }
    }
}
