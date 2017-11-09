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
        public static Color BackgroundColor => Color.FromArgb(92, 93, 100);
        public static Color TitleBarColor => Color.FromArgb(92, 93, 100);
        public static Color TurquoiseColor => Color.FromArgb(150, 192, 192);
        public static Color TextColor => Color.FromArgb(224, 224, 224);

        private static Editor _instance;
        public static Editor Instance => _instance ?? (_instance = new Editor());
        
        private Project _project;
        private Assembly _gameProgram;
        private DeserializeDockContent _deserializeDockContent;

        public DockableRenderForm RenderForm1 = new DockableRenderForm(LocalPlayerIndex.One, 0);
        public DockableRenderForm RenderForm2;
        public DockableRenderForm RenderForm3;
        public DockableRenderForm RenderForm4;
        public DockableOutputWindow OutputForm = new DockableOutputWindow();
        public DockableActorTree ActorTreeForm = new DockableActorTree();
        public DockableFileTree FileTreeForm = new DockableFileTree();
        public DockablePropertyGrid PropertyGridForm = new DockablePropertyGrid();
        
        public ResourceTree ContentTree => FileTreeForm.ContentTree;
        public TreeView ActorTree => ActorTreeForm.ActorTree;
        
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

                ContentTree?.OpenPath(_project?.FilePath);

                bool projectOpened = _project != null;
                btnEngineSettings.Enabled = projectOpened;
                btnProjectSettings.Enabled = projectOpened;
                btnUserSettings.Enabled = projectOpened;

                if (projectOpened)
                {
                    _project.EditorState = new EditorState();
                    if (string.IsNullOrEmpty(_project.FilePath))
                        Text = "";
                    else
                        Text = _project.FilePath;
                }
                else
                    Text = "";
            }
        }

        EditorGameMode _editorGameMode;
        public Editor() : base()
        {
            _instance = this;

            Project p;

            //TODO: read editor state file instead
            string lastOpened = Properties.Settings.Default.LastOpened;//"C:\\Users\\David\\Desktop\\test project\\NewProject.xtproj";
            if (!string.IsNullOrEmpty(lastOpened))
                p = FileObject.FromFile<Project>(lastOpened);
            else
            {
                p = new Project()
                {
                    OpeningWorld = typeof(TestWorld),
                    UserSettings = new UserSettings(),
                    EngineSettings = new EngineSettings(),
                };
            }

            Engine.EditorState.InGameMode = false;
            Engine.SetGame(p);
            InitializeComponent();

            Project = p;

            menuStrip1.Renderer = new TheraToolstripRenderer();
            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            DockPanel.Theme = new TheraEditorTheme();

            DockPanel.SuspendLayout(true);
            OutputForm.Show(DockPanel, DockState.DockBottom);
            ActorTreeForm.Show(DockPanel, DockState.DockRight);
            FileTreeForm.Show(DockPanel, DockState.DockLeft);
            PropertyGridForm.Show(ActorTreeForm.Pane, DockAlignment.Bottom, 0.5);
            RenderForm1.Show(DockPanel, DockState.Document);
            DockPanel.ResumeLayout(true, true);

            DoubleBuffered = false;
            Engine.SetGamePanel(RenderForm1.RenderPanel, false);
            Engine.Game.State.GameMode = _editorGameMode = new EditorGameMode();
            Engine.Initialize(true, false);

            GenerateInitialActorList();
            if (Engine.World != null)
            {
                Engine.World.State.SpawnedActors.PostAdded += SpawnedActors_PostAdded;
                Engine.World.State.SpawnedActors.PostRemoved += SpawnedActors_PostRemoved;
            }
        }

        /// <summary>
        /// Creates an instance of T using user-chosen derived type, constructor and parameters.
        /// </summary>
        /// <typeparam name="T">The object type to create.</typeparam>
        /// <returns>A newly created instance of T.</returns>
        public static T UserCreateInstanceOf<T>(bool allowDerivedTypes)
            => (T)UserCreateInstanceOf(typeof(T), allowDerivedTypes);
        /// <summary>
        /// Creates an instance of elementType using user-chosen derived type, constructor and parameters.
        /// </summary>
        /// <param name="type">The object type to create.</param>
        /// <returns>A newly created instance of elementType.</returns>
        public static object UserCreateInstanceOf(Type type, bool allowDerivedTypes)
        {
            ObjectCreator creator = new ObjectCreator();
            if (creator.Initialize(type, allowDerivedTypes))
                creator.ShowDialog();
            return creator.ConstructedObject;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            OnRedrawn = Redraw;
            Engine.RegisterRenderTick(RenderTick);
            PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
            Engine.SetPaused(true, LocalPlayerIndex.One, true);
            Engine.Run();
        }

        public static void SetPropertyGridObject(object obj)
        {
            Instance.PropertyGridForm.PropertyGrid.TargetObject = obj;
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DockableActorTree).ToString())
                return ActorTreeForm;
            else if (persistString == typeof(DockableFileTree).ToString())
                return FileTreeForm;
            else if (persistString == typeof(DockableOutputWindow).ToString())
                return OutputForm;
            else if (persistString == typeof(DockablePropertyGrid).ToString())
                return PropertyGridForm;
            else
            {
                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length == 0)
                    return null;
                string type = parsedStrings[0];
                if (type == typeof(RenderForm).ToString())
                {
                    if (parsedStrings.Length < 2)
                        return null;
                    switch (parsedStrings[1])
                    {
                        case "0": return RenderForm1;
                        case "1": return RenderForm2;
                        case "2": return RenderForm3;
                        case "3": return RenderForm4;
                        default: return null;
                    }
                }
                return null;
            }
        }
        private void GenerateInitialActorList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(GenerateInitialActorList));
                return;
            }
            ActorTreeForm.ActorTree.Nodes.Clear();
            if (Engine.World != null)
                ActorTreeForm.ActorTree.Nodes.AddRange(Engine.World.State.SpawnedActors.Select(x => x.EditorState.TreeNode = new TreeNode(x.ToString()) { Tag = x }).ToArray());
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
                ActorTreeForm.ActorTree.Nodes.Add(t);
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
                ActorTreeForm.ActorTree.Nodes.Remove(item.EditorState.TreeNode);
            }
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            Engine.UnregisterRenderTick(RenderTick);

            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            DockPanel.SaveAsXml(configFile);

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

        /// <summary>
        /// The world that the editor is currently editing.
        /// </summary>
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
                Engine.SetCurrentWorld(value, true, true, false);
            }
        }

        public static SingleFileRef<EditorSettings> Settings { get; }
            = new SingleFileRef<EditorSettings>(Path.GetFullPath(Path.Combine(Application.StartupPath, "..\\..\\..\\Editor", "Config.xset")), () => new EditorSettings());

        private void ActorTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = ActorTreeForm.ActorTree.SelectedNode == null ? Engine.World.Settings : ActorTreeForm.ActorTree.SelectedNode.Tag;
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
            PropertyGridForm.PropertyGrid.TargetObject = Project;
        }

        private void BtnEngineSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Project?.EngineSettings;
        }

        private void BtnEditorSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Editor.Settings;
        }

        private void BtnUserSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Project?.UserSettings;
        }

        private void BtnWorldSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
        }

        private void CboContentViewTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BtPlay_Click(object sender, EventArgs e)
        {
            RenderPanel p = DockableRenderForm.ActiveRenderForm.RenderPanel;
            p.Focus();
            p.Capture = true;
            Cursor.Clip = p.RectangleToScreen(p.ClientRectangle);
            //Cursor.Hide();
            InputInterface.GlobalRegisters.Add(RegisterInput);
            Engine.SetGameMode(Engine.GetGameMode());
            Engine.SetPaused(false, LocalPlayerIndex.One, true);
            Engine.World.BeginPlay();
        }

        private void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, EndGameplay, InputPauseType.TickAlways);
        }

        private void EndGameplay()
        {
            //Cursor.Show();
            Cursor.Clip = new Rectangle();
            InputInterface.GlobalRegisters.Remove(RegisterInput);
            Engine.World.EndPlay();
            Engine.SetGameMode(_editorGameMode);
            Engine.SetPaused(true, LocalPlayerIndex.One, true);
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (CloseProject())
                Engine.ShutDown();
            else
                e.Cancel = true;
            base.OnFormClosing(e);
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
            if (!ActorTreeForm.IsDisposed)
                ActorTreeForm.Focus();
            else
                (ActorTreeForm = new DockableActorTree()).Show(DockPanel);
        }

        private void Viewport1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm1 != null && !RenderForm1.IsDisposed)
                RenderForm1.Focus();
            else
                (RenderForm1 = new DockableRenderForm(LocalPlayerIndex.One, 0)).Show(DockPanel);
        }

        private void viewport2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm2 != null && !RenderForm2.IsDisposed)
                RenderForm2.Focus();
            else
                (RenderForm2 = new DockableRenderForm(LocalPlayerIndex.One, 1)).Show(DockPanel);
        }

        private void viewport3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm3 != null && !RenderForm3.IsDisposed)
                RenderForm3.Focus();
            else
                (RenderForm3 = new DockableRenderForm(LocalPlayerIndex.One, 2)).Show(DockPanel);
        }

        private void viewport4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderForm4 != null && !RenderForm4.IsDisposed)
                RenderForm4.Focus();
            else
                (RenderForm4 = new DockableRenderForm(LocalPlayerIndex.One, 3)).Show(DockPanel);
        }

        private void btnViewFileTree_Click(object sender, EventArgs e)
        {
            if (!FileTreeForm.IsDisposed)
                FileTreeForm.Focus();
            else
                (FileTreeForm = new DockableFileTree()).Show(DockPanel);
        }

        private void btnViewPropertyGrid_Click(object sender, EventArgs e)
        {
            if (!PropertyGridForm.IsDisposed)
                PropertyGridForm.Focus();
            else
                (PropertyGridForm = new DockablePropertyGrid()).Show(DockPanel);
        }

        private void btnViewOutput_Click(object sender, EventArgs e)
        {
            if (OutputForm != null && !OutputForm.IsDisposed)
                OutputForm.Focus();
            else
                (OutputForm = new DockableOutputWindow()).Show(DockPanel);
        }
    }
}
