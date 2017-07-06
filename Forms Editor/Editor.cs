using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Tests;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheraEditor.Editors;
using TheraEngine.Timers;

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
            string lastOpened = Properties.Settings.Default.LastOpened;
            if (!string.IsNullOrEmpty(lastOpened))
                OpenProject(FileObject.FromXML<Project>(lastOpened));
            else
            {
                Project = new Project()
                {
                    OpeningWorld = typeof(TestWorld),
                    UserSettings = new UserSettings(),
                    EngineSettings = new EngineSettings()
                    {
                        CapFPS = true,
                        TargetFPS = 60.0f,
                    }
                };
            }

            Engine.SetGame(Project);
            InitializeComponent();
            DoubleBuffered = false;
            Engine.Initialize(renderPanel1);

            GenerateInitialActorList();
            if (Engine.World != null)
            {
                Engine.World.State.SpawnedActors.PostAdded += SpawnedActors_PostAdded;
                Engine.World.State.SpawnedActors.PostRemoved += SpawnedActors_PostRemoved;
            }
        }

        private void GenerateInitialActorList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(GenerateInitialActorList));
                return;
            }
            actorTree.Nodes.Clear();
            if (Engine.World != null)
                actorTree.Nodes.AddRange(Engine.World.State.SpawnedActors.Select(x => x.EditorState.TreeNode = new TreeNode(x.ToString()) { Tag = x }).ToArray());
        }
        private void SpawnedActors_PostAdded(IActor item)
        {
            if (Engine.World != null)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<IActor>(SpawnedActors_PostAdded), item);
                    return;
                }
                TreeNode t = new TreeNode(item.ToString()) { Tag = item };
                item.EditorState.TreeNode = t;
                actorTree.Nodes.Add(t);
            }
        }
        private void SpawnedActors_PostRemoved(IActor item)
        {
            if (Engine.World != null)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<IActor>(SpawnedActors_PostRemoved), item);
                    return;
                }
                actorTree.Nodes.Remove(item.EditorState.TreeNode);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OnRedrawn = Redraw;
            Engine.RegisterRenderTick(RenderTick);
            actorPropertyGrid.SelectedObject = Engine.World?.Settings;
            Engine.Run();

            FlyingCameraPawn editorCamera = new FlyingCameraPawn(PlayerIndex.One)
            {
                Hud = new EditorHud(renderPanel1.ClientSize, this)
            };

            Engine.World.SpawnActor(editorCamera);
            //Engine.ActivePlayers[0].ControlledPawn = editorCamera;
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            Engine.UnregisterRenderTick(RenderTick);
            Engine.ShutDown();
            base.OnClosing(e);
        }
        public Action OnRedrawn;
        private void Redraw()
        {
            renderPanel1.Invalidate();
            Application.DoEvents();
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            try
            {
                Invoke(OnRedrawn);
            }
            catch { }
        }
        protected override void OnResizeBegin(EventArgs e)
        {
            renderPanel1.BeginResize();
            base.OnResizeBegin(e);
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            renderPanel1.EndResize();
            base.OnResizeEnd(e);
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
                if (Engine.World != null && Engine.World.EditorState != null && Engine.World.EditorState.HasChanges)
                {
                    DialogResult r = MessageBox.Show(this, "Save changes to current world?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (r == DialogResult.Cancel)
                        return;
                    else if (r == DialogResult.Yes)
                        Engine.World.Export();
                    Engine.World.EditorState = null;
                }
                value.EditorState = new EditorState();
                Engine.World = value;
            }
        }
        
        private void ActorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            actorPropertyGrid.SelectedObject = actorTree.SelectedNode == null ? Engine.World.Settings : actorTree.SelectedNode.Tag;
        }

        private bool CloseProject()
        {
            if (Project != null && Project.EditorState != null && Project.EditorState.HasChanges)
            {
                DialogResult r = MessageBox.Show(this, "Save changes to current project?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (r == DialogResult.Cancel)
                    return false;
                else if (r == DialogResult.Yes)
                    Project.Export();
                Project.EditorState = null;
                return true;
            }
            Project = null;
            return true;
        }
        private void OpenProject(Project p)
        {
            if (p == null)
                return;
            p.EditorState = new EditorState();
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
                    OpenProject(Project.Create(fbd.SelectedPath, "NewProject"));
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
