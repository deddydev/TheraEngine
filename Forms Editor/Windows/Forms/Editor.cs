using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using WeifenLuo.WinFormsUI.Docking;
using EnvDTE;
using TheraEngine.Tests;

namespace TheraEditor.Windows.Forms
{
    public interface IEditorControl
    {
        /// <summary>
        /// The player index this control allows input from.
        /// </summary>
        LocalPlayerIndex PlayerIndex { get; }
        /// <summary>
        /// The render panel with the viewport that will be possessed by the desired player.
        /// </summary>
        BaseRenderPanel RenderPanel { get; }
        /// <summary>
        /// The pawn the player will possess for editing purposes.
        /// </summary>
        IPawn EditorPawn { get; }
    }
    public partial class Editor : TheraForm
    {
        public static IEditorControl ActiveRenderForm { get; private set; } = null;
        /// <summary>
        /// This will possess and unpossess the necessary viewports and pawns corresponding to the given editor control.
        /// </summary>
        /// <param name="control">The editor control that the user is focused on.</param>
        public static void SetActiveEditorControl(IEditorControl control)
        {
            //TODO: change game mode back to editor game mode if editing detached from active gameplay

            if (ActiveRenderForm == control)
                return;

            if (ActiveRenderForm != null)
            {
                int index = (int)ActiveRenderForm.PlayerIndex;
                if (index < Engine.LocalPlayers.Count)
                {
                    LocalPlayerController c = Engine.LocalPlayers[index];
                    ActiveRenderForm.RenderPanel.UnregisterController(c);
                    c.ControlledPawn = null;
                }
            }
            ActiveRenderForm = control;
            if (ActiveRenderForm != null)
            {
                int index = (int)control.PlayerIndex;
                if (index < Engine.LocalPlayers.Count)
                {
                    LocalPlayerController c = Engine.LocalPlayers[index];
                    ActiveRenderForm.RenderPanel.GetOrAddViewport(0).RegisterController(c);
                    c.ControlledPawn = ActiveRenderForm.EditorPawn;
                }
            }
        }

        public static Color BackgroundColor => Color.FromArgb(92, 93, 100);
        public static Color TitleBarColor => Color.FromArgb(92, 93, 100);
        public static Color TurquoiseColor => Color.FromArgb(150, 192, 192);
        public static Color TextColor => Color.FromArgb(224, 224, 224);

        private static Editor _instance;
        public static Editor Instance => _instance ?? (_instance = new Editor());
        
        private Project _project;
        private Assembly _gameProgram;
        private DeserializeDockContent _deserializeDockContent;

        #region Instanced Dock Forms
        //Dockable forms with a limited amount of instances
        private DockableRenderForm[] _renderForms = new DockableRenderForm[4];
        public bool RenderFormActive(int i)
        {
            DockableRenderForm form = _renderForms[i];
            return form != null && !form.IsDisposed;
        }
        public DockableRenderForm GetRenderForm(int i)
        {
            DockableRenderForm form = _renderForms[i];
            if (form == null || form.IsDisposed)
            {
                Engine.PrintLine("Created viewport " + (i + 1).ToString());
                form = _renderForms[i] = new DockableRenderForm(LocalPlayerIndex.One, i);
                form.Show(DockPanel);
            }
            return form;
        }
        public DockableRenderForm RenderForm1 => GetRenderForm(0);
        public DockableRenderForm RenderForm2 => GetRenderForm(1);
        public DockableRenderForm RenderForm3 => GetRenderForm(2);
        public DockableRenderForm RenderForm4 => GetRenderForm(3);

        public T GetForm<T>(ref T value) where T : DockContent, new()
        {
            if (value == null || value.IsDisposed)
            {
                value = new T();
                Engine.PrintLine("Created " + value.GetType().GetFriendlyName());
                value.Show(DockPanel);
            }
            return value;
        }

        private DockableOutputWindow _outputForm;
        public DockableOutputWindow OutputForm => GetForm(ref _outputForm);

        private DockableActorTree _actorTreeForm;
        public DockableActorTree ActorTreeForm => GetForm(ref _actorTreeForm);

        private DockableFileTree _fileTreeForm;
        public DockableFileTree FileTreeForm => GetForm(ref _fileTreeForm);

        private DockablePropertyGrid _propertyGridForm;
        public DockablePropertyGrid PropertyGridForm => GetForm(ref _propertyGridForm);

        private DockableWelcomeWindow _welcomeForm;
        public DockableWelcomeWindow WelcomeForm => GetForm(ref _welcomeForm);

        #endregion

        public ResourceTree ContentTree => FileTreeForm.ContentTree;
        public TreeView ActorTree => ActorTreeForm.ActorTree;
        
        public Project Project
        {
            get => _project;
            set
            {
                if (value != null && _project == value)
                    return;

                GameState = EEditorGameplayState.Editing;

                if (!CloseProject())
                    return;

                ClearDockPanel();

                _project = value;

                Engine.ShutDown();
                Engine.SetGame(_project);

                bool projectOpened = _project != null;
                btnEngineSettings.Enabled = 
                btnProjectSettings.Enabled = 
                btnUserSettings.Enabled = 
                btnPlay.Enabled =
                btnPlayDetached.Enabled = 
                btnCompile.Enabled = 
                projectOpened;

                if (projectOpened)
                {
                    if (string.IsNullOrEmpty(_project.FilePath))
                        Text = "";
                    else
                    {
                        Text = _project.FilePath;
                        ContentTree.OpenPath(_project.FilePath);
                    }

                    string configFile = _project.EditorSettings.GetFullDockConfigPath();
                    //Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

                    if (!string.IsNullOrWhiteSpace(configFile) && File.Exists(configFile))
                        DockPanel.LoadFromXml(configFile, _deserializeDockContent);
                    else
                    {
                        DockPanel.SuspendLayout(true);
                        OutputForm.Show(DockPanel, DockState.DockBottom);
                        ActorTreeForm.Show(DockPanel, DockState.DockRight);
                        FileTreeForm.Show(DockPanel, DockState.DockLeft);
                        PropertyGridForm.Show(ActorTreeForm.Pane, DockAlignment.Bottom, 0.5);
                        RenderForm1.Show(DockPanel, DockState.Document);
                        DockPanel.ResumeLayout(true, true);
                    }

                    Engine.Initialize(false);
                    Engine.RegisterRenderTick(RenderTick);
                    Engine.SetPaused(true, LocalPlayerIndex.One, true);
                    Engine.Run();

                    CurrentWorld = _project.OpeningWorldRef?.File;
                    Engine.SetActiveGameMode(_editorGameMode);

                    //Type visualStudioType = Type.GetTypeFromProgID("VisualStudio.DTE");
                    //DTE dte = Activator.CreateInstance(visualStudioType) as DTE;
                    //dte.MainWindow.Visible = true;
                }
                else
                {
                    Text = "";

                    DockPanel.SuspendLayout(true);
                    OutputForm.Show(DockPanel, DockState.DockBottom);
                    WelcomeForm.Show(DockPanel, DockState.Document);
                    DockPanel.ResumeLayout(true, true);
                }
            }
        }

        /// <summary>
        /// The world that the editor is currently editing.
        /// </summary>
        public World CurrentWorld
        {
            get => Engine.World;
            set
            {
                if (Engine.World != null &&
                    Engine.World.EditorState != null &&
                    Engine.World.EditorState.HasChanges)
                {
                    DialogResult r = MessageBox.Show(this, "Save changes to current world?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (r == DialogResult.Cancel)
                        return;
                    else if (r == DialogResult.Yes)
                        Engine.World.Export();
                    Engine.World.EditorState = null;
                }

                Engine.SetCurrentWorld(value, true, false);

                GenerateInitialActorList();
                if (Engine.World != null)
                {
                    Engine.World.State.SpawnedActors.PostAnythingAdded += SpawnedActors_PostAdded;
                    Engine.World.State.SpawnedActors.PostAnythingRemoved += SpawnedActors_PostRemoved;
                }
                PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
            }
        }

        EditorGameMode _editorGameMode;
        public Editor() : base()
        {
            _instance = this;
            
            _editorGameMode = new EditorGameMode();
            InitializeComponent();

            menuStrip1.Renderer = new TheraToolstripRenderer();
            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            DockPanel.Theme = new TheraEditorTheme();

            AutoScaleMode = AutoScaleMode.Font;
            DoubleBuffered = false;
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

            OnRedrawn = Redraw;

            //TODO: read editor state file instead
            string lastOpened = Properties.Settings.Default.LastOpened;//"C:\\Users\\David\\Desktop\\test project\\NewProject.xtproj";
            if (!string.IsNullOrEmpty(lastOpened))
            {
                Project = FileObject.Load<Project>(lastOpened);
            }
            else
            {
                Project = new Project() //null
                {
                    OpeningWorldRef = typeof(UnitTestingWorld),
                    UserSettingsRef = new UserSettings(),
                    EngineSettingsRef = new EngineSettings(),
                    EditorSettingsRef = new EditorSettings(),
                    ProjectStateRef = new ProjectState(),
                };
            }
            
            //if (DesignMode)
            //    return;

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (CloseProject())
            {
                Engine.UnregisterRenderTick(RenderTick);
                Engine.ShutDown();
            }
            else
                e.Cancel = true;

            base.OnClosing(e);
        }

        public static void SetPropertyGridObject(object obj)
        {
            Instance.PropertyGridForm.PropertyGrid.TargetObject = obj;
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
            if (Engine.World != null && !Engine.ShuttingDown)
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
            if (Engine.World != null && !Engine.ShuttingDown)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<IActor>(SpawnedActors_PostRemoved), item);
                    return;
                }
                ActorTreeForm.ActorTree.Nodes.Remove(item.EditorState.TreeNode);
                item.EditorState.TreeNode = null;
            }
        }
        
        public Action OnRedrawn;
        private void Redraw()
        {
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.Invalidate();
            
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
                Filter = FileObject.GetFilter<World>(),
                Multiselect = false
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                CurrentWorld = FileObject.Load<World>(ofd.FileName);
        }

        private void BtnNewMaterial_Click(object sender, EventArgs e)
        {
            new MaterialEditorForm().Show();
        }

        private void BtnNewWorld_Click(object sender, EventArgs e)
        {
            CurrentWorld = new World();
        }

        public static GlobalFileRef<EditorSettings> Settings { get; }
            = new GlobalFileRef<EditorSettings>(Path.GetFullPath(string.Format(Application.StartupPath + "{0}..{0}..{0}..{0}Editor{0}Config.xset", Path.DirectorySeparatorChar)), () => new EditorSettings());
        
        private bool CloseProject()
        {
            if (_project != null)
            {
                if (_project.EditorState != null && _project.EditorState.HasChanges)
                {
                    DialogResult r = MessageBox.Show(this, "Save changes to current project?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (r == DialogResult.Cancel)
                        return false;
                    else if (r == DialogResult.Yes)
                        _project.Export();
                }

                string configFile = _project.EditorSettings.GetFullDockConfigPath();
                //Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
                DockPanel.SaveAsXml(configFile);

                ClearDockPanel();

                _project.EditorState = null;
                _project = null;
            }
            return true;
        }
        public void ClearDockPanel()
        {
            foreach (IDockContent document in DockPanel.DocumentsToArray())
            {
                document.DockHandler.DockPanel = null;
                document.DockHandler.Close();
            }
        }
        private void BtnNewProject_Click(object sender, EventArgs e) => CreateNewProject();
        /// <summary>
        /// Asks the user to select a folder to create a new project in, creates it there, and then loads it for editing.
        /// Closes the currently opened project if there is one.
        /// </summary>
        public void CreateNewProject()
        {
            if (!CloseProject())
                return;
            
            FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                Description = "",
            };

            if (fbd.ShowDialog() == DialogResult.OK)
                Project = Project.Create(fbd.SelectedPath, "NewProject");
        }
        public void OpenProject()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileObject.GetFilter<Project>(),
            };

            if (ofd.ShowDialog() == DialogResult.OK && CloseProject())
                Project = FileObject.Load<Project>(ofd.FileName);
        }
        private void BtnOpenProject_Click(object sender, EventArgs e) => OpenProject();
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
                Filter = FileObject.GetFilter<Project>(),
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _project.Export(sfd.FileName);
            }
        }
        private void BtnProjectSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Project;
        }
        private void BtnEngineSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Project?.EngineSettingsRef;
        }
        private void BtnEditorSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Settings;
        }
        private void BtnUserSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Project?.UserSettingsRef;
        }
        private void BtnWorldSettings_Click(object sender, EventArgs e)
        {
            PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
        }
        private void CboContentViewTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private DockableRenderForm FocusViewport(int index)
        {
            DockableRenderForm form = GetRenderForm(index);
            if (form.IsHidden)
                form.Show(DockPanel, DockState.Document);
            form.Focus();
            return form;
        }

        private void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, EndGameplay, EInputPauseType.TickAlways);
        }
        
        public enum EEditorGameplayState
        {
            /// <summary>
            /// Gameplay is not simulating. Purely in edit mode.
            /// </summary>
            Editing,
            /// <summary>
            /// Gameplay is simulating, but the user is viewing from a third person flying camera.
            /// Editing is allowed.
            /// </summary>
            Detached,
            /// <summary>
            /// Gameplay is simulating and the user is playing it as it should be experienced.
            /// Editing is not allowed.
            /// </summary>
            Attached,
        }

        private EEditorGameplayState _gameState = EEditorGameplayState.Editing;
        public EEditorGameplayState GameState
        {
            get => _gameState;
            set
            {
                if (_gameState == value)
                    return;

                switch (value)
                {
                    case EEditorGameplayState.Attached:
                        if (_gameState == EEditorGameplayState.Editing)
                        {
                            //Transition from editor mode to attached gameplay mode
                            Engine.EditorState.InGameMode = true;
                            BaseRenderPanel renderPanel = (ActiveRenderForm as DockableRenderForm)?.RenderPanel ?? FocusViewport(0).RenderPanel;
                            renderPanel.Focus();
                            renderPanel.Capture = true;
                            //Cursor.Hide();
                            Cursor.Clip = renderPanel.RectangleToScreen(renderPanel.ClientRectangle);

                            InputInterface.GlobalRegisters.Add(RegisterInput);
                            Engine.SetActiveGameMode(Engine.GetGameMode(), true);

                            Engine.SetPaused(false, LocalPlayerIndex.One, true);
                        }
                        else //Detached
                        {
                            //Transition from detached to attached gameplay mode
                        }
                        break;
                    case EEditorGameplayState.Detached:
                        if (_gameState == EEditorGameplayState.Attached)
                        {
                            //Transition from attached to detached gameplay mode
                        }
                        else //Editing
                        {
                            //Transition from editor mode to detached gameplay mode
                            Engine.EditorState.InGameMode = true;
                            BaseRenderPanel renderPanel = (ActiveRenderForm as DockableRenderForm)?.RenderPanel ?? FocusViewport(0).RenderPanel;
                            renderPanel.Focus();
                            renderPanel.Capture = true;
                            //Cursor.Hide();
                            Cursor.Clip = renderPanel.RectangleToScreen(renderPanel.ClientRectangle);

                            InputInterface.GlobalRegisters.Add(RegisterInput);
                            Engine.SetActiveGameMode(_editorGameMode, true);

                            Engine.SetPaused(false, LocalPlayerIndex.One, true);
                        }
                        break;
                    case EEditorGameplayState.Editing:
                        if (_gameState == EEditorGameplayState.Attached)
                        {
                            //Transition from attached gameplay mode to editor mode
                            Engine.EditorState.InGameMode = false;
                            Cursor.Clip = new Rectangle();
                            //Cursor.Show();

                            Engine.SetActiveGameMode(_editorGameMode, false);
                            InputInterface.GlobalRegisters.Remove(RegisterInput);
                            _editorGameMode.BeginGameplay();

                            Engine.SetPaused(true, LocalPlayerIndex.One, true);
                        }
                        else //Detached
                        {
                            //Transition from detached gameplay mode to editor mode
                            Engine.EditorState.InGameMode = false;
                            Cursor.Clip = new Rectangle();
                            //Cursor.Show();

                            _editorGameMode.EndGameplay();
                            InputInterface.GlobalRegisters.Remove(RegisterInput);
                            _editorGameMode.BeginGameplay();

                            Engine.SetPaused(true, LocalPlayerIndex.One, true);
                        }
                        break;
                }

                _gameState = value;
            }
        }
        private void BtPlay_Click(object sender, EventArgs e)
        {
            GameState = EEditorGameplayState.Attached;
        }
        private void btnPlayDetached_Click(object sender, EventArgs e)
        {
            GameState = EEditorGameplayState.Detached;
        }
        private void EndGameplay()
        {
            GameState = EEditorGameplayState.Editing;
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
            ActorTreeForm.Focus();
        }

        private void Viewport1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm1.Focus();
        }

        private void viewport2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm2.Focus();
        }

        private void viewport3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm3.Focus();
        }

        private void viewport4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderForm4.Focus();
        }

        private void btnViewFileTree_Click(object sender, EventArgs e)
        {
            FileTreeForm.Focus();
        }

        private void btnViewPropertyGrid_Click(object sender, EventArgs e)
        {
            PropertyGridForm.Focus();
        }

        private void btnViewOutput_Click(object sender, EventArgs e)
        {
            OutputForm.Focus();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
                if (type == typeof(DockableRenderForm).ToString())
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
    }
}
