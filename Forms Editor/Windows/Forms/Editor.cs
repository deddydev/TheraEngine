using BorderlessForm;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Input.Devices;
using TheraEngine.Tests;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor : TheraForm
    {
        private static Editor _instance;
        public static Editor Instance => _instance ?? (_instance = new Editor());
        
        private Project _project;
        private Assembly _gameProgram;
        
        public DockableRenderForm RenderForm1 = new DockableRenderForm(PlayerIndex.One);
        public DockableRenderForm RenderForm2;
        public DockableRenderForm RenderForm3;
        public DockableRenderForm RenderForm4;
        public DockableActorTree ActorsForm = new DockableActorTree();
        public DockableFileTree FileForm = new DockableFileTree();
        public DockablePropertyGrid PropForm = new DockablePropertyGrid();
        
        public ResourceTree ContentTree => FileForm.ContentTree;
        public TreeView ActorTree => ActorsForm.ActorTree;
        
        public Project Project
        {
            get => _project;
            set
            {
                if (_project == value)
                    return;
                if (_project != null && !CloseProject())
                    return;
                _project = value;
                ContentTree?.DisplayProject(_project);
                if (_project != null)
                {
                    _project.EditorState = new EditorState();
                    if (string.IsNullOrEmpty(_project.FilePath))
                        Text = "Thera Editor";
                    else
                        Text = "Thera Editor - " + _project.FilePath;
                }
                else
                    Text = "Thera Editor";
            }
        }
        
        public Editor() : base()
        {
            _instance = this;
            string lastOpened = Properties.Settings.Default.LastOpened;
            if (!string.IsNullOrEmpty(lastOpened))
                Project = FileObject.FromFile<Project>(lastOpened);
            else
            {
                Project = new Project()
                {
                    OpeningWorld = typeof(TestWorld),
                    UserSettings = new UserSettings(),
                    EngineSettings = new EngineSettings(),
                };
            }

            Engine.EditorState.InGameMode = false;
            Engine.SetGame(Project);
            InitializeComponent();
            menuStrip1.Renderer = new TheraToolstripRenderer();

            DockPanel.Theme = new TheraEditorTheme();

            DockPanel.SuspendLayout(true);
            ActorsForm.Show(DockPanel, DockState.DockRight);
            FileForm.Show(DockPanel, DockState.DockLeft);
            PropForm.Show(ActorsForm.Pane, DockAlignment.Bottom, 0.5);
            RenderForm1.Show(DockPanel, DockState.Document);
            DockPanel.ResumeLayout(true, true);

            DoubleBuffered = false;
            Engine.SetGamePanel(RenderForm1.RenderPanel, false);
            Engine.Initialize(true);

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
            ActorsForm.ActorTree.Nodes.Clear();
            if (Engine.World != null)
                ActorsForm.ActorTree.Nodes.AddRange(Engine.World.State.SpawnedActors.Select(x => x.EditorState.TreeNode = new TreeNode(x.ToString()) { Tag = x }).ToArray());
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
                ActorsForm.ActorTree.Nodes.Add(t);
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
                ActorsForm.ActorTree.Nodes.Remove(item.EditorState.TreeNode);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            OnRedrawn = Redraw;
            Engine.RegisterRenderTick(RenderTick);
            PropForm.PropertyGrid.SelectedObject = Engine.World?.Settings;
            //Engine.SetPaused(true, PlayerIndex.One, true);
            Engine.Run();
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            Engine.UnregisterRenderTick(RenderTick);
            foreach (IDockContent document in DockPanel.DocumentsToArray())
            {
                document.DockHandler.DockPanel = null;
                document.DockHandler.Close();
            }
            Engine.ShutDown();
            base.OnClosing(e);
        }
        public Action OnRedrawn;
        private void Redraw()
        {
            if (!RenderForm1.IsDisposed)
                RenderForm1.RenderPanel.Invalidate();
            if (RenderForm2 != null && !RenderForm2.IsDisposed)
                RenderForm2.RenderPanel.Invalidate();
            if (RenderForm3 != null && !RenderForm3.IsDisposed)
                RenderForm3.RenderPanel.Invalidate();
            if (RenderForm4 != null && !RenderForm4.IsDisposed)
                RenderForm4.RenderPanel.Invalidate();
            Application.DoEvents();
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            try { Invoke(OnRedrawn); } catch { }
        }
        //protected override void OnResizeBegin(EventArgs e)
        //{
        //    RenderForm.RenderPanel.BeginResize();
        //    base.OnResizeBegin(e);
        //}
        //protected override void OnResizeEnd(EventArgs e)
        //{
        //    RenderForm.RenderPanel.EndResize();
        //    base.OnResizeEnd(e);
        //}
        private void BtnOpenWorld_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileObject.GetFileHeader(typeof(World)).GetFilter(),
                Multiselect = false
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                World world = FileObject.FromFile<World>(ofd.FileName);
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
            PropForm.PropertyGrid.SelectedObject = ActorsForm.ActorTree.SelectedNode == null ? Engine.World.Settings : ActorsForm.ActorTree.SelectedNode.Tag;
        }

        private bool CloseProject()
        {
            if (_project != null && _project.EditorState != null && _project.EditorState.HasChanges)
            {
                DialogResult r = MessageBox.Show(this, "Save changes to current project?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (r == DialogResult.Cancel)
                    return false;
                else if (r == DialogResult.Yes)
                    _project.Export();
                _project.EditorState = null;
                _project = null;
                return true;
            }
            _project = null;
            return true;
        }
        private void BtnNewProject_Click(object sender, EventArgs e)
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
                    Project = Project.Create(fbd.SelectedPath, "NewProject");
                }
            }

            bool projectOpened = Project != null;
            btnEngineSettings.Enabled = projectOpened;
            btnProjectSettings.Enabled = projectOpened;
            btnUserSettings.Enabled = projectOpened;
        }

        private void BtnOpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileObject.GetFileHeader(typeof(Project)).GetFilter(),
            };

            if (ofd.ShowDialog() == DialogResult.OK && CloseProject())
                Project = FileObject.FromFile<Project>(ofd.FileName);

            bool projectOpened = Project != null;
            btnEngineSettings.Enabled = projectOpened;
            btnProjectSettings.Enabled = projectOpened;
            btnUserSettings.Enabled = projectOpened;
        }

        private void BtnSaveProject_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_project.FilePath))
            {
                BtnSaveProjectAs_Click(sender, e);
                return;
            }
            _project.Export();
        }

        private void BtnSaveProjectAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = FileObject.GetFileHeader(typeof(Project)).GetFilter(),
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileObject.GetDirNameFmt(sfd.FileName, out string dir, out string name, out FileFormat fmt);
                _project.Export(dir, name, fmt);
            }
        }

        private void BtnProjectSettings_Click(object sender, EventArgs e)
        {
            PropForm.PropertyGrid.SelectedObject = Project;
        }

        private void BtnEngineSettings_Click(object sender, EventArgs e)
        {
            PropForm.PropertyGrid.SelectedObject = Project?.EngineSettings;
        }

        private void BtnEditorSettings_Click(object sender, EventArgs e)
        {

        }

        private void BtnUserSettings_Click(object sender, EventArgs e)
        {
            PropForm.PropertyGrid.SelectedObject = Project?.UserSettings;
        }

        private void BtnWorldSettings_Click(object sender, EventArgs e)
        {
            PropForm.PropertyGrid.SelectedObject = Engine.World?.Settings;
        }

        private void CboContentViewTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BtPlay_Click(object sender, EventArgs e)
        {
            //RenderForm.RenderPanel.Focus();
            //RenderForm.RenderPanel.Capture = true;
            //Cursor.Clip = RenderForm.RenderPanel.RectangleToScreen(RenderForm.RenderPanel.ClientRectangle);
            Cursor.Hide();
            InputInterface.GlobalRegisters.Add(RegisterInput);
            //Engine.World.DespawnActor(_editorCameraPawn);
            Engine.World.BeginPlay();
        }

        private void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, EndGameplay, InputPauseType.TickAlways);
        }

        private void EndGameplay()
        {
            Cursor.Show();
            Cursor.Clip = new Rectangle();
            InputInterface.GlobalRegisters.Remove(RegisterInput);
            //Engine.World.SpawnActor(_editorCameraPawn);
            Engine.World.EndPlay();
        }

        private void ToolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            //if (float.TryParse(toolStripTextBox1.Text, out float fps))
            //    Engine.TargetRenderFreq = fps;
        }
        
        //private void BtnMaximize_Click(object sender, EventArgs e)
        //{
        //    //if (WindowState == FormWindowState.Normal)
        //    //{
        //    //    WindowState = FormWindowState.Maximized;
        //    //    btnMaximize.Text = "🗗";
        //    //}
        //    //else
        //    //{
        //    //    WindowState = FormWindowState.Normal;
        //    //    btnMaximize.Text = "🗖";
        //    //}
        //}

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (CloseProject())
                Engine.ShutDown();
            else
                e.Cancel = true;
            base.OnFormClosing(e);
        }

        private void ActorPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (PropForm.PropertyGrid.SelectedObject is ObjectBase obj)
            {
                obj.OnPropertyChanged(e.OldValue, e.ChangedItem.PropertyDescriptor.Name);
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            TheraEngineText.ForeColor = InactiveTextColor;
            base.OnDeactivate(e);
        }
        protected override void OnActivated(EventArgs e)
        {
            TheraEngineText.ForeColor = ActiveBorderColor;
            base.OnActivated(e);
        }

        private void BtnViewActorTree_Click(object sender, EventArgs e)
        {
            if (!ActorsForm.IsDisposed)
                ActorsForm.Focus();
            else
                (ActorsForm = new DockableActorTree()).Show(DockPanel);
        }

        private void Viewport1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm1 != null && !RenderForm1.IsDisposed)
                RenderForm1.Focus();
            else
                (RenderForm1 = new DockableRenderForm(PlayerIndex.One)).Show(DockPanel);
        }

        private void viewport2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm2 != null && !RenderForm2.IsDisposed)
                RenderForm2.Focus();
            else
                (RenderForm2 = new DockableRenderForm(PlayerIndex.One)).Show(DockPanel);
        }

        private void viewport3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm3 != null && !RenderForm3.IsDisposed)
                RenderForm3.Focus();
            else
                (RenderForm3 = new DockableRenderForm(PlayerIndex.One)).Show(DockPanel);
        }

        private void viewport4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm4 != null && !RenderForm4.IsDisposed)
                RenderForm4.Focus();
            else
                (RenderForm4 = new DockableRenderForm(PlayerIndex.One)).Show(DockPanel);
        }

        private void btnViewFileTree_Click(object sender, EventArgs e)
        {
            if (!FileForm.IsDisposed)
                FileForm.Focus();
            else
                (FileForm = new DockableFileTree()).Show(DockPanel);
        }

        private void btnViewPropertyGrid_Click(object sender, EventArgs e)
        {
            if (!PropForm.IsDisposed)
                PropForm.Focus();
            else
                (PropForm = new DockablePropertyGrid()).Show(DockPanel);
        }
    }
}
