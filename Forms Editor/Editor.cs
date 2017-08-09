using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Editors;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Input.Devices;
using TheraEngine.Tests;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;

namespace TheraEditor
{
    public partial class Editor : Form
    {
        private static Editor _instance;
        public static Editor Instance => _instance ?? (_instance = new Editor());

        private SingleFileRef<Project> _project;
        private Assembly _gameProgram;

        public ResourceTree ContentTree => contentTree;

        public Project Project
        {
            get => _project;
            set => _project = value;
        }

        public Editor()
        {
            _instance = this;
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
                        CapUPS = false,
                        TargetUPS = 30.0f,
                    }
                };
            }

            Engine.EditorState.InGameMode = false;
            Engine.SetGame(Project);
            InitializeComponent();
            menuStrip1.Renderer = new ToolStripRenderer();
            DoubleBuffered = false;
            Engine.Initialize(renderPanel1, true);

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
        class ToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ToolStripRenderer() : base(new clr())
            {
                RoundedEdges = true;
            }
        }
        class MenuColorTable : ProfessionalColorTable
        {
            public MenuColorTable() => UseSystemColors = false;
            public override Color MenuBorder => Color.FromArgb(255, 62, 63, 70);
            public override Color MenuItemBorder => Color.DarkViolet;
            public override Color MenuItemSelected => Color.Cornsilk;
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(255, 102, 103, 110);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(255, 95, 96, 103);
            public override Color MenuStripGradientBegin => Color.AliceBlue;
            public override Color MenuStripGradientEnd => Color.DodgerBlue;
        }
        class clr : ProfessionalColorTable
        {
            public override Color ButtonSelectedHighlight
            {
                get { return ButtonSelectedGradientMiddle; }
            }
            public override Color ButtonSelectedHighlightBorder
            {
                get { return ButtonSelectedBorder; }
            }
            public override Color ButtonPressedHighlight
            {
                get { return ButtonPressedGradientMiddle; }
            }
            public override Color ButtonPressedHighlightBorder
            {
                get { return ButtonPressedBorder; }
            }
            public override Color ButtonCheckedHighlight
            {
                get { return ButtonCheckedGradientMiddle; }
            }
            public override Color ButtonCheckedHighlightBorder
            {
                get { return ButtonSelectedBorder; }
            }
            public override Color ButtonPressedBorder
            {
                get { return ButtonSelectedBorder; }
            }
            public override Color ButtonSelectedBorder
            {
                get { return Color.FromArgb(255, 98, 98, 98); }
            }
            public override Color ButtonCheckedGradientBegin
            {
                get { return Color.FromArgb(255, 144, 144, 144); }
            }
            public override Color ButtonCheckedGradientMiddle
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonCheckedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonSelectedGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonSelectedGradientMiddle
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonSelectedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonPressedGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonPressedGradientMiddle
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonPressedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color CheckBackground
            {
                get { return Color.FromArgb(255, 173, 173, 173); }
            }
            public override Color CheckSelectedBackground
            {
                get { return Color.FromArgb(255, 173, 173, 173); }
            }
            public override Color CheckPressedBackground
            {
                get { return Color.FromArgb(255, 140, 140, 140); }
            }
            public override Color GripDark
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color GripLight
            {
                get { return Color.FromArgb(255, 83, 83, 83); }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return Color.FromArgb(255, 85, 85, 85); }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginRevealedGradientBegin
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginRevealedGradientMiddle
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginRevealedGradientEnd
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color MenuStripGradientBegin
            {
                get { return Color.FromArgb(255, 138, 138, 138); }
            }
            public override Color MenuStripGradientEnd
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuItemBorder
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuBorder
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color MenuItemPressedGradientMiddle
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color RaftingContainerGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color RaftingContainerGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color SeparatorDark
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color SeparatorLight
            {
                get { return Color.FromArgb(255, 62, 62, 62); }
            }
            public override Color StatusStripGradientBegin
            {
                get { return Color.FromArgb(255, 112, 112, 112); }
            }
            public override Color StatusStripGradientEnd
            {
                get { return Color.FromArgb(255, 97, 97, 97); }
            }
            public override Color ToolStripBorder
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color ToolStripGradientBegin
            {
                get { return Color.FromName("DimGray"); }
            }
            public override Color ToolStripGradientMiddle
            {
                get { return Color.FromArgb(255, 89, 89, 89); }
            }
            public override Color ToolStripGradientEnd
            {
                get { return Color.FromArgb(255, 88, 88, 88); }
            }
            public override Color ToolStripContentPanelGradientBegin
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ToolStripContentPanelGradientEnd
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ToolStripPanelGradientBegin
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color ToolStripPanelGradientEnd
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color OverflowButtonGradientBegin
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color OverflowButtonGradientMiddle
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color OverflowButtonGradientEnd
            {
                get { return Color.FromArgb(255, 79, 79, 79); }
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            OnRedrawn = Redraw;
            Engine.RegisterRenderTick(RenderTick);
            actorPropertyGrid.SelectedObject = Engine.World?.Settings;
            Engine.SetPaused(true, PlayerIndex.One, true);
            Engine.Run();

            _editorCameraPawn = new FlyingCameraPawn(PlayerIndex.One)
            {
                Hud = new EditorHud(renderPanel1.ClientSize, this)
            };
            Engine.World.SpawnActor(_editorCameraPawn);
        }

        FlyingCameraPawn _editorCameraPawn;

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
                Filter = FileObject.GetFileHeader(typeof(World)).GetFilter(),
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
                    OpenProject(Project.Create(fbd.SelectedPath, "NewProject"));
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
                OpenProject(FileObject.Import<Project>(ofd.FileName));

            bool projectOpened = Project != null;
            btnEngineSettings.Enabled = projectOpened;
            btnProjectSettings.Enabled = projectOpened;
            btnUserSettings.Enabled = projectOpened;
        }

        private void BtnSaveProject_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_project.ReferencePath))
            {
                BtnSaveProjectAs_Click(sender, e);
                return;
            }
            _project.ExportReference();
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
                _project.ExportReference(dir, name, fmt, true);
            }
        }

        private void BtnProjectSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Project;
        }

        private void BtnEngineSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Project?.EngineSettings;
        }

        private void BtnEditorSettings_Click(object sender, EventArgs e)
        {

        }

        private void BtnUserSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Project?.UserSettings;
        }

        private void BtnWorldSettings_Click(object sender, EventArgs e)
        {
            actorPropertyGrid.SelectedObject = Engine.World?.Settings;
        }

        private void CboContentViewTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BtPlay_Click(object sender, EventArgs e)
        {
            renderPanel1.Focus();
            renderPanel1.Capture = true;
            Cursor.Clip = renderPanel1.RectangleToScreen(renderPanel1.ClientRectangle);
            Cursor.Hide();
            InputInterface.GlobalRegisters.Add(RegisterInput);
            Engine.World.DespawnActor(_editorCameraPawn);
            Engine.World.BeginPlay();
        }

        private void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, EndGameplay, InputPauseType.TickAlways);
        }

        private void EndGameplay()
        {
            Cursor.Show();
            Cursor.Clip = new System.Drawing.Rectangle();
            InputInterface.GlobalRegisters.Remove(RegisterInput);
            Engine.World.SpawnActor(_editorCameraPawn);
            Engine.World.EndPlay();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Engine.CloseApplication();
        }
    }
}
