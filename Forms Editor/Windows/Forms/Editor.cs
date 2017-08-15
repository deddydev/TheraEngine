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
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Input.Devices;
using TheraEngine.Tests;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor : FormBase
    {
        private static Editor _instance;
        public static Editor Instance => _instance ?? (_instance = new Editor());
        
        private Project _project;
        private Assembly _gameProgram;

        public ResourceTree ContentTree => contentTree;

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
                if (_project != null)
                {
                    _project.EditorState = new EditorState();
                    contentTree?.DisplayProject(_project);
                }
            }
        }
        
        public Editor()
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
            menuStrip1.Renderer = new ToolStripRenderer();
            DoubleBuffered = false;
            Engine.Initialize(renderPanel1, true);

            GenerateInitialActorList();
            if (Engine.World != null)
            {
                Engine.World.State.SpawnedActors.PostAdded += SpawnedActors_PostAdded;
                Engine.World.State.SpawnedActors.PostRemoved += SpawnedActors_PostRemoved;
            }
            
            InitBorderless();
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

            if (DesignMode)
                return;

            OnRedrawn = Redraw;
            Engine.RegisterRenderTick(RenderTick);
            actorPropertyGrid.SelectedObject = Engine.World?.Settings;
            //Engine.SetPaused(true, PlayerIndex.One, true);
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
            try { Invoke(OnRedrawn); } catch { }
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
            actorPropertyGrid.SelectedObject = actorTree.SelectedNode == null ? Engine.World.Settings : actorTree.SelectedNode.Tag;
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
            Cursor.Clip = new Rectangle();
            InputInterface.GlobalRegisters.Remove(RegisterInput);
            Engine.World.SpawnActor(_editorCameraPawn);
            Engine.World.EndPlay();
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

        #region Borderless Form

        private FormWindowState previousWindowState;

        private Color hoverTextColor = Color.FromArgb(62, 109, 181);

        public Color HoverTextColor
        {
            get { return hoverTextColor; }
            set { hoverTextColor = value; }
        }

        private Color downTextColor = Color.FromArgb(25, 71, 138);

        public Color DownTextColor
        {
            get { return downTextColor; }
            set { downTextColor = value; }
        }

        private Color hoverBackColor = Color.FromArgb(213, 225, 242);

        public Color HoverBackColor
        {
            get { return hoverBackColor; }
            set { hoverBackColor = value; }
        }

        private Color downBackColor = Color.FromArgb(163, 189, 227);

        public Color DownBackColor
        {
            get { return downBackColor; }
            set { downBackColor = value; }
        }

        private Color normalBackColor = Color.White;

        public Color NormalBackColor
        {
            get { return normalBackColor; }
            set { normalBackColor = value; }
        }

        public enum MouseState
        {
            Normal,
            Hover,
            Down
        }

        protected void SetLabelColors(Control control, MouseState state)
        {
            if (!ContainsFocus) return;

            var textColor = ActiveTextColor;
            var backColor = NormalBackColor;

            switch (state)
            {
                case MouseState.Normal:
                    textColor = Color.FromArgb(224, 224, 224);
                    backColor = Color.Transparent;
                    //Cursor = Cursors.Default;
                    break;
                case MouseState.Hover:
                    textColor = HoverTextColor;
                    backColor = HoverBackColor;
                    //Cursor = Cursors.Hand;
                    break;
                case MouseState.Down:
                    textColor = DownTextColor;
                    backColor = DownBackColor;
                    //Cursor = Cursors.Hand;
                    break;
            }

            control.ForeColor = textColor;
            control.BackColor = backColor;
        }

        public void InitBorderless()
        {
            Activated += MainForm_Activated;
            Deactivate += MainForm_Deactivate;

            foreach (var control in new[] { MinimizeLabel, MaximizeLabel, CloseLabel })
            {
                control.MouseEnter += (s, e) => SetLabelColors((Control)s, MouseState.Hover);
                control.MouseLeave += (s, e) => SetLabelColors((Control)s, MouseState.Normal);
                control.MouseDown += (s, e) => SetLabelColors((Control)s, MouseState.Down);
            }

            TopLeftBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOPLEFT);
            TopRightBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOPRIGHT);
            BottomLeftBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOMLEFT);
            BottomRightBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOMRIGHT);

            TopBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOP);
            LeftBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTLEFT);
            RightBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTRIGHT);
            BottomBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOM);
            
            FormTitle.MouseDown += TitleLabel_MouseDown;
            FormTitle.MouseUp += (s, e) => { if (e.Button == MouseButtons.Right && FormTitle.ClientRectangle.Contains(e.Location)) ShowSystemMenu(MouseButtons); };
            FormTitle.Text = Text;
            TextChanged += (s, e) => FormTitle.Text = Text;

            var marlett = new Font("Marlett", 8.5f);

            MinimizeLabel.Font = marlett;
            MaximizeLabel.Font = marlett;
            CloseLabel.Font = marlett;
            
            MinimizeLabel.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) WindowState = FormWindowState.Minimized; };
            MaximizeLabel.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) ToggleMaximize(); };
            previousWindowState = MinMaxState;
            SizeChanged += MainForm_SizeChanged;
            CloseLabel.MouseClick += (s, e) => Close(e);
        }
        
        void Close(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Close();
        }

        void DecorationMouseDown(MouseEventArgs e, HitTestValues h)
        {
            if (e.Button == MouseButtons.Left) DecorationMouseDown(h);
        }

        private Color activeBorderColor = Color.FromArgb(150, 192, 192);

        public Color ActiveBorderColor
        {
            get { return activeBorderColor; }
            set { activeBorderColor = value; }
        }

        private Color inactiveBorderColor = Color.FromArgb(131, 131, 131);

        public Color InactiveBorderColor
        {
            get { return inactiveBorderColor; }
            set { inactiveBorderColor = value; }
        }

        void MainForm_Deactivate(object sender, EventArgs e)
        {
            SetBorderColor(InactiveBorderColor);
            SetTextColor(InactiveTextColor);
            Engine.TargetRenderFreq = 3.0f;
            Engine.TargetUpdateFreq = 3.0f;
            //MainPanel.Enabled = false;
        }

        void MainForm_Activated(object sender, EventArgs e)
        {
            SetBorderColor(ActiveBorderColor);
            SetTextColor(ActiveTextColor);
            Engine.TargetRenderFreq = Project.EngineSettings.File.CapFPS ? Project.EngineSettings.File.TargetFPS : 0.0f;
            Engine.TargetUpdateFreq = Project.EngineSettings.File.CapUPS ? Project.EngineSettings.File.TargetUPS : 0.0f;
            //MainPanel.Enabled = true;
        }

        private Color activeTextColor = Color.FromArgb(224, 224, 224);

        public Color ActiveTextColor
        {
            get { return activeTextColor; }
            set { activeTextColor = value; }
        }

        private Color inactiveTextColor = Color.FromArgb(177, 177, 177);

        public Color InactiveTextColor
        {
            get { return inactiveTextColor; }
            set { inactiveTextColor = value; }
        }

        protected void SetBorderColor(Color color)
        {
            TopLeftBorderPanel.BackColor = color;
            TopBorderPanel.BackColor = color;
            TopRightBorderPanel.BackColor = color;
            LeftBorderPanel.BackColor = color;
            RightBorderPanel.BackColor = color;
            BottomLeftBorderPanel.BackColor = color;
            BottomBorderPanel.BackColor = color;
            BottomRightBorderPanel.BackColor = color;
            MinimizeLabel.BackColor = color;
            MaximizeLabel.BackColor = color;
            CloseLabel.BackColor = color;
        }

        protected void SetTextColor(Color color)
        {
            //SystemLabel.ForeColor = color;
            FormTitle.ForeColor = color;
            MinimizeLabel.ForeColor = color;
            MaximizeLabel.ForeColor = color;
            CloseLabel.ForeColor = color;
        }

        void MainForm_SizeChanged(object sender, EventArgs e)
        {
            var maximized = MinMaxState == FormWindowState.Maximized;
            MaximizeLabel.Text = maximized ? "2" : "1";

            var panels = new[] { TopLeftBorderPanel, TopRightBorderPanel, BottomLeftBorderPanel, BottomRightBorderPanel,
                TopBorderPanel, LeftBorderPanel, RightBorderPanel, BottomBorderPanel };

            foreach (var panel in panels)
            {
                panel.Visible = !maximized;
            }

            if (previousWindowState != MinMaxState)
            {
                //if (maximized)
                //{
                //    CloseLabel.Left += RightBorderPanel.Width;
                //    CloseLabel.Top = 0;
                //    MaximizeLabel.Left += RightBorderPanel.Width;
                //    MaximizeLabel.Top = 0;
                //    MinimizeLabel.Left += RightBorderPanel.Width;
                //    MinimizeLabel.Top = 0;
                //    //FormTitle.Left -= LeftBorderPanel.Width;
                //    //FormTitle.Width += LeftBorderPanel.Width + RightBorderPanel.Width;
                //    //FormTitle.Top = 0;
                //}
                //else if (previousWindowState == FormWindowState.Maximized)
                //{
                //    //SystemLabel.Left = LeftBorderPanel.Width;
                //    //SystemLabel.Top = TopBorderPanel.Height;
                //    CloseLabel.Left -= RightBorderPanel.Width;
                //    CloseLabel.Top = TopBorderPanel.Height;
                //    MaximizeLabel.Left -= RightBorderPanel.Width;
                //    MaximizeLabel.Top = TopBorderPanel.Height;
                //    MinimizeLabel.Left -= RightBorderPanel.Width;
                //    MinimizeLabel.Top = TopBorderPanel.Height;
                //    //FormTitle.Left += LeftBorderPanel.Width;
                //    //FormTitle.Width -= LeftBorderPanel.Width + RightBorderPanel.Width;
                //    //FormTitle.Top = TopBorderPanel.Height;
                //}

                previousWindowState = MinMaxState;
            }
        }

        private FormWindowState ToggleMaximize()
        {
            return WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private DateTime titleClickTime = DateTime.MinValue;
        private Point titleClickPosition = Point.Empty;

        void TitleLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var clickTime = (DateTime.Now - titleClickTime).TotalMilliseconds;
                if (clickTime < SystemInformation.DoubleClickTime && e.Location == titleClickPosition)
                    ToggleMaximize();
                else
                {
                    titleClickTime = DateTime.Now;
                    titleClickPosition = e.Location;
                    DecorationMouseDown(HitTestValues.HTCAPTION);
                }
            }
        }

        #endregion

        class ToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ToolStripRenderer() : base(new ColorTable())
            {
                RoundedEdges = false;
            }
        }
        class ColorTable : ProfessionalColorTable
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

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            //if (float.TryParse(toolStripTextBox1.Text, out float fps))
            //    Engine.TargetRenderFreq = fps;
        }
    }
}
