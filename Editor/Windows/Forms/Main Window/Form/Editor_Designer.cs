using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.GameModes;
using TheraEngine.Input.Devices;
using TheraEngine.Networking;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public interface IEditorControl
    {
        /// <summary>
        /// The player index this control allows input from.
        /// </summary>
        ELocalPlayerIndex PlayerIndex { get; }
        /// <summary>
        /// The render panel with the viewport that will be possessed by the desired player.
        /// </summary>
        BaseRenderPanel RenderPanel { get; }
        /// <summary>
        /// The pawn the player will possess for editing purposes.
        /// </summary>
        IPawn EditorPawn { get; }
        /// <summary>
        /// The game mode used for this render form.
        /// </summary>
        BaseGameMode GameMode { get; }
    }
    public partial class Editor : TheraForm, IMappableShortcutControl, IDockPanelOwner
    {
        DockPanel IDockPanelOwner.DockPanelRef => DockPanel;

        public Editor() : base()
        {
            Instance = this;
            Engine.BeginOperation += BeginOperation;
            Engine.EndOperation += EndOperation;

            _editorGameMode = new EditorGameMode();
            InitializeComponent();
            DockPanel.Theme = new TheraEditorTheme();

            FormTitle2.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            TheraEngineText.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            PaddingPanel.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            lblVersion.MouseDown += new MouseEventHandler(TitleBar_MouseDown);

            FormTitle2.Text = Text;

            menuStrip1.Renderer = new TheraToolstripRenderer();
            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            AutoScaleMode = AutoScaleMode.None;
            DoubleBuffered = false;

            KeyPreview = true;
            MappableActions = new Dictionary<Keys, Func<bool>>()
            {
                { Keys.Control | Keys.Z, Undo },
                { Keys.Control | Keys.Y, Redo },
            };

            TheraEngineText.Font = Engine.MakeFont("origicide", 10.0f, FontStyle.Regular);

            lblYourIpPort.Text = "Your IP: " + NetworkConnection.GetLocalIPAddressV4();
            CursorManager.GlobalWrapCursorWithinClip = false;

            if (lblVersion != null)
            {
#if DEBUG
                lblVersion.Visible = true;
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
                lblVersion.Text = $"Editor Debug Ver {version} --- Built {buildDate}";
#else
                lblVersion.Visible = false;
#endif
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Dictionary<Keys, Func<bool>> MappableActions { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ResourceTree ContentTree => FileTreeForm.ContentTree;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TreeView ActorTree => ActorTreeForm.ActorTree;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public UndoManager UndoManager { get; } = new UndoManager();

        private TProject _project;
        private EditorGameMode _editorGameMode;
        private DeserializeDockContent _deserializeDockContent;

        #region Instanced Dock Forms
        //Dockable forms with a limited amount of instances
        private DockableWorldRenderForm[] _renderForms = new DockableWorldRenderForm[4];
        public bool RenderFormActive(int i)
        {
            DockableWorldRenderForm form = _renderForms[i];
            return form != null && !form.IsDisposed;
        }
        public DockableWorldRenderForm GetRenderForm(int i)
        {
            DockableWorldRenderForm form = _renderForms[i];
            if (form == null || form.IsDisposed)
            {
                Engine.PrintLine("Created viewport " + (i + 1).ToString());
                form = _renderForms[i] = new DockableWorldRenderForm(ELocalPlayerIndex.One, i);
                form.Show(DockPanel);
            }
            return form;
        }
        public DockableWorldRenderForm RenderForm1 => GetRenderForm(0);
        public DockableWorldRenderForm RenderForm2 => GetRenderForm(1);
        public DockableWorldRenderForm RenderForm3 => GetRenderForm(2);
        public DockableWorldRenderForm RenderForm4 => GetRenderForm(3);
        
        private bool GetFormActive<T>(T value) where T : DockContent
            => value != null && !value.IsDisposed;
        
        public T GetForm<T>(ref T value, DockState defaultDockState = DockState.Document) where T : DockContent, new()
        {
            if (value == null || value.IsDisposed)
            {
                value = new T();
                Engine.PrintLine("Created " + value.GetType().GetFriendlyName());
                if (InvokeRequired)
                {
                    T value2 = value;
                    Invoke((Action)(() => value2.Show(DockPanel, defaultDockState)));
                }
                else
                    value.Show(DockPanel);
            }
            return value;
        }

        private DockableErrorList _errorListForm;
        public bool DockableErrorListFormActive => GetFormActive(_errorListForm);
        public DockableErrorList ErrorListForm => GetForm(ref _errorListForm, DockState.DockBottomAutoHide);
        
        private DockableProjectCreator _projectCreatorForm;
        public bool ProjectCreatorFormActive => GetFormActive(_projectCreatorForm);
        public DockableProjectCreator ProjectCreatorForm => GetForm(ref _projectCreatorForm);
        
        private DockableGPUAnalytics _analyticsForm;
        public bool AnalyticsFormActive => GetFormActive(_analyticsForm);
        public DockableGPUAnalytics GPUAnalyticsForm => GetForm(ref _analyticsForm);

        private DockableOutputWindow _outputForm;
        public bool OutputFormActive => GetFormActive(_outputForm);
        public DockableOutputWindow OutputForm => GetForm(ref _outputForm, DockState.DockBottom);

        private DockableActorTree _actorTreeForm;
        public bool ActorTreeFormActive => GetFormActive(_actorTreeForm);
        public DockableActorTree ActorTreeForm => GetForm(ref _actorTreeForm, DockState.DockRight);

        private DockableFileTree _fileTreeForm;
        public bool FileTreeFormActive => GetFormActive(_fileTreeForm);
        public DockableFileTree FileTreeForm => GetForm(ref _fileTreeForm, DockState.DockLeft);
        
        private DockableMSBuildTree _msBuildTreeForm;
        public bool MSBuildTreeFormActive => GetFormActive(_msBuildTreeForm);
        public DockableMSBuildTree MSBuildTreeForm => GetForm(ref _msBuildTreeForm, DockState.DockLeft);

        private DockablePropertyGrid _propertyGridForm;
        public bool PropertyGridFormActive => GetFormActive(_propertyGridForm);
        public DockablePropertyGrid PropertyGridForm => GetForm(ref _propertyGridForm);

        private DockableWelcomeWindow _welcomeForm;
        public bool WelcomeFormActive => GetFormActive(_welcomeForm);
        public DockableWelcomeWindow WelcomeForm => GetForm(ref _welcomeForm);

        #endregion

        #region World Management
        /// <summary>
        /// The world that the editor is currently editing.
        /// </summary>
        public TWorld CurrentWorld
        {
            get => Engine.World;
            set => SetWorld(value);
        }
        private async void SetWorld(TWorld world)
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action<TWorld>)SetWorld, BaseRenderPanel.PanelType.Rendering, world))
                return;

            if (Engine.World?.EditorState?.HasChanges ?? false)
            {
                DialogResult r = MessageBox.Show(this, "Save changes to current world?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (r == DialogResult.Cancel)
                    return;
                else if (r == DialogResult.Yes)
                    await Engine.World.ExportAsync();
                Engine.World.EditorState = null;
            }

            Engine.SetCurrentWorld(world, true, false);

            bool worldExists = Engine.World != null;

            btnWorldSettings.Enabled = btnSaveWorld.Enabled = btnSaveWorldAs.Enabled = worldExists;

            GenerateInitialActorList();
            if (worldExists)
            {
                Engine.World.State.SpawnedActors.PostAnythingAdded += SpawnedActors_PostAdded;
                Engine.World.State.SpawnedActors.PostAnythingRemoved += SpawnedActors_PostRemoved;
            }
            PropertyGridForm.PropertyGrid.TargetObject = Engine.World?.Settings;
        }
        public bool CloseWorld()
        {
            if (CurrentWorld != null)
            {
                if (CurrentWorld.EditorState != null && CurrentWorld.EditorState.HasChanges)
                {
                    DialogResult r = MessageBox.Show(this, "Save changes to current world?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (r == DialogResult.Cancel)
                        return false;
                    else if (r == DialogResult.Yes)
                        SaveFile(CurrentWorld);
                }

                CurrentWorld.EditorState = null;
                CurrentWorld = null;
            }
            return true;
        }
        /// <summary>
        /// Creates and loads a new world for editing.
        /// Closes the currently opened world if there is one.
        /// </summary>
        public void CreateNewWorld()
        {
            if (!CloseWorld())
                return;

            CurrentWorld = new TWorld();
        }
        public async void OpenWorld()
        {
            if (!CloseWorld())
                return;

            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<TWorld>(),
                Multiselect = false
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                    CurrentWorld = await TFileObject.LoadAsync<TWorld>(ofd.FileName);
            }
        }
        #endregion

        #region Overrides
        protected override void OnTextChanged(EventArgs e)
        {
            if (FormTitle2 != null)
                FormTitle2.Text = Text;

            base.OnTextChanged(e);
        }
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            EditorSettings settings = GetSettings();
            var recentFiles = settings?.RecentlyOpenedProjectPaths; //Properties.Settings.Default.RecentFiles;
            if (recentFiles != null && recentFiles.Count > 0)
            {
                string lastOpened = recentFiles[recentFiles.Count - 1];
                if (!string.IsNullOrEmpty(lastOpened))
                    Project = await TFileObject.LoadAsync<TProject>(lastOpened);
                else
                    Project = null;
            }
            else
                Project = null;

            Engine.Run();

            CheckUpdates();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UpdateRecentProjectPaths();

            if (CloseProject())
            {
                SetRenderTicking(false);
                Engine.Stop();
                Engine.ShutDown();
            }
            else
                e.Cancel = true;

            base.OnFormClosing(e);
        }
        public async void UpdateRecentProjectPaths()
        {
            string projectPath = Project?.FilePath;
            if (!string.IsNullOrWhiteSpace(projectPath))
            {
                EditorSettings settings = GetSettings();
                var list = settings.RecentlyOpenedProjectPaths; //Properties.Settings.Default.RecentFiles;
                if (list != null)
                {
                    if (list.Contains(projectPath))
                        list.Remove(projectPath);
                    list.Add(projectPath);
                }
                else
                {
                    //Properties.Settings.Default.RecentFiles = new StringCollection() { projectPath };
                    settings.RecentlyOpenedProjectPaths = new List<string>() { projectPath };
                }

                await settings.ExportAsync();
                //Properties.Settings.Default.Save();
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (MappableActions.ContainsKey(e.KeyData))
            {
                e.SuppressKeyPress = false;
                var func = MappableActions[e.KeyData];
                e.Handled = func();
                if (e.Handled)
                    Engine.PrintLine(e.KeyData.ToString().Replace(", ", "+") + ": " + func.Method.Name);
                return;
            }
            base.OnKeyDown(e);
        }
        protected override void OnDeactivate(EventArgs e)
        {
            TheraEngineText.ForeColor = InactiveTextColor;
            base.OnDeactivate(e);
        }
        protected override void OnActivated(EventArgs e)
        {
            TheraEngineText.ForeColor = TurquoiseColorLight;
            base.OnActivated(e);
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
        #endregion

        #region Ticking
        public bool IsRenderTicking { get; private set; }
        public void SetRenderTicking(bool isRendering)
        {
            if (isRendering && !IsRenderTicking)
            {
                IsRenderTicking = true;
                Engine.RegisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
            else if (!isRendering && IsRenderTicking)
            {
                IsRenderTicking = false;
                Engine.UnregisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
        }
        private void UpdateTick(object sender, FrameEventArgs e)
        {
            Engine.Scene?.GlobalUpdate();
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.UpdateTick(sender, e);
        }
        private void SwapBuffers()
        {
            Engine.Scene?.GlobalSwap();
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.SwapBuffers();
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            if (BaseRenderPanel.WorldPanel == null)
                return;

            BaseRenderPanel.WorldPanel.CaptureContext();
            Engine.Scene?.GlobalPreRender();
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.Invalidate();
        }
        #endregion

        #region Project Management
        public TProject Project
        {
            get => _project;
            set => SetProject(value);
        }
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
                        SaveFile(_project);
                }

                string configFile = _project.EditorSettings?.GetFullDockConfigPath();
                if (configFile != null && configFile.IsExistingDirectoryPath() == false)
                    DockPanel.SaveAsXml(configFile);

                ClearDockPanel();

                _project.EditorState = null;
                _project = null;
            }
            return true;
        }
        /// <summary>
        /// Asks the user to select a folder to create a new project in, creates it there, and then loads it for editing.
        /// Closes the currently opened project if there is one.
        /// </summary>
        public void CreateNewProject()
        {
            if (!CloseProject())
                return;
            
            ProjectCreatorForm.Focus();
            ProjectCreatorForm.ProjectCreated += ProjectCreatorForm_ProjectCreated;
        }
        private void ProjectCreatorForm_ProjectCreated()
        {
            Project = ProjectCreatorForm.Project;
            PropertyGridForm.PropertyGrid.TargetObject = Project;
        }
        public async void OpenProject()
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<TProject>(),
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK && CloseProject())
                    Project = await TFileObject.LoadAsync<TProject>(ofd.FileName);
            }
        }
        private void SetProject(TProject value)
        {
            if (value != null && _project == value)
                return;

            if (!CloseProject())
                return;

            GameState = EEditorGameplayState.Editing;
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

            ClearDockPanel();

            if (projectOpened)
            {
                string configFile = _project.EditorSettings?.GetFullDockConfigPath();
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

                if (string.IsNullOrEmpty(_project.FilePath))
                    Text = string.Empty;
                else
                {
                    Text = _project.FilePath;
                    ContentTree.OpenPath(_project.FilePath);
                }

                Engine.SetWorldPanel(RenderForm1.RenderPanel, false);
                Engine.SetActiveGameMode(_editorGameMode);
                Engine.Initialize(false);
                SetRenderTicking(true);
                Engine.SetPaused(true, ELocalPlayerIndex.One, true);

                CurrentWorld = _project.OpeningWorldRef?.File;

                UpdateRecentProjectPaths();
            }
            else
            {
                Text = string.Empty;

                DockPanel.SuspendLayout(true);
                OutputForm.Show(DockPanel, DockState.DockBottom);
                WelcomeForm.Show(DockPanel, DockState.Document);
                DockPanel.ResumeLayout(true, true);
            }
        }
        #endregion

        #region Game Mode
        private void RegisterInput(InputInterface input)
        {
            input.RegisterKeyEvent(EKey.F1, EButtonInputType.Pressed, EndGameplay, EInputPauseType.TickAlways);
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
        private Rectangle _prevClip;
        private void CaptureMouse(BaseRenderPanel panel)
        {
            CursorManager.GlobalWrapCursorWithinClip = true;
            Engine.EditorState.InEditMode = false;
            panel.Focus();
            //panel.Capture = true;
            _prevClip = Cursor.Clip;
            Cursor.Clip = panel.RectangleToScreen(panel.ClientRectangle);
            //HideCursor();
        }
        private void ReleaseMouse()
        {
            CursorManager.GlobalWrapCursorWithinClip = false;
            Engine.EditorState.InEditMode = true;
            //ShowCursor();
            Cursor.Clip = _prevClip;
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
                    case EEditorGameplayState.Attached: SetAttachedGameState(); break;
                    case EEditorGameplayState.Detached: SetDetachedGameState(); break;
                    case EEditorGameplayState.Editing: SetEditingGameState(); break;
                }

                _gameState = value;
            }
        }
        private void SetEditingGameState()
        {
            btnPlay.Text = "Play";
            btnPlayDetached.Text = "Play Detached";

            if (_gameState == EEditorGameplayState.Attached)
            {
                ReleaseMouse();
            }
            else //Detached
            {
                //Mouse is already released
            }

            SetEditorGameMode();

            Engine.Pause(ELocalPlayerIndex.One, true);
        }
        private void SetDetachedGameState()
        {
            btnPlay.Text = "Stop";
            btnPlayDetached.Text = "Play Attached";

            if (_gameState == EEditorGameplayState.Attached)
            {
                ReleaseMouse();
            }
            else //Editing
            {
                SetProjectGameMode();
            }

            Engine.Unpause(ELocalPlayerIndex.One, true);
        }
        private void SetAttachedGameState()
        {
            btnPlay.Text = "Stop";
            btnPlayDetached.Text = "Play Detached";

            BaseRenderPanel renderPanel = (ActiveRenderForm as DockableWorldRenderForm)?.RenderPanel ?? FocusViewport(0).RenderPanel;

            CaptureMouse(renderPanel);

            if (_gameState == EEditorGameplayState.Editing)
            {
                SetProjectGameMode();
            }
            else //Detached
            {
                //TODO: reattach the user to the pawn it is supposed to be controlling
            }

            Engine.Unpause(ELocalPlayerIndex.One, true);
        }
        private void SetProjectGameMode()
        {
            BaseGameMode gameMode = Engine.GetGameMode();
            Engine.SetActiveGameMode(gameMode, false);
            InputInterface.GlobalRegisters.Add(RegisterInput);
            gameMode?.BeginGameplay();
        }
        private void SetEditorGameMode()
        {
            Engine.SetActiveGameMode(_editorGameMode, false);
            InputInterface.GlobalRegisters.Remove(RegisterInput);
            _editorGameMode.BeginGameplay();
        }
        #endregion

        private bool Undo()
        {
            bool canUndo = UndoManager.CanUndo;
            UndoManager.Undo();
            return canUndo;
        }
        private bool Redo()
        {
            bool canRedo = UndoManager.CanRedo;
            UndoManager.Redo();
            return canRedo;
        }

        private void GenerateInitialActorList() => ActorTreeForm.GenerateInitialActorList();
        private void SpawnedActors_PostAdded(IActor item) => ActorTreeForm.ActorSpawned(item);
        private void SpawnedActors_PostRemoved(IActor item) => ActorTreeForm.ActorDespawned(item);

        public void ClearDockPanel()
        {
            foreach (IDockContent document in DockPanel.Contents.ToArray())
            {
                document.DockHandler.DockPanel = null;
                document.DockHandler.Close();
            }
        }
        
        private DockableWorldRenderForm FocusViewport(int index)
        {
            DockableWorldRenderForm form = GetRenderForm(index);
            if (form.IsHidden)
                form.Show(DockPanel, DockState.Document);
            form.Focus();
            return form;
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
                if (type == typeof(DockableWorldRenderForm).ToString())
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
        
        private void connectAsServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Server s = Engine.ServerConnection;
            if (s == null)
            {
                s = new Server();
                Engine.Network = s;
            }

            s.InitializeConnection(NetworkConnection.ServerPort, null);

            Engine.PrintLine($"Started hosting server at {NetworkConnection.GetLocalIPAddressV4()}:{NetworkConnection.ServerPort}.");
        }

        private async void connectAsClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client c = Engine.ClientConnection;
            if (c == null)
            {
                c = new Client();
                Engine.Network = c;
            }

            IPAddress addr = CreateAddress(txtTargetIPPort.Text);
            if (addr != null)
            {
                IPEndPoint point = new IPEndPoint(addr, NetworkConnection.ServerPort);
                c.InitializeConnection(NetworkConnection.ClientPort, point);

                Engine.PrintLine($"Requesting connection to server at {point.Address.ToString()}:{NetworkConnection.ServerPort}.");
            }
            else
            {
                Engine.PrintLine($"'{txtTargetIPPort.Text}' is not a valid IP configuration.");
                return;
            }

            bool success = await c.RequestConnectionAsync();
            if (success)
                Engine.PrintLine("Successfully connected to the target server.");
            else
                Engine.PrintLine("Failed to connect to the target server.");
        }

        private IPEndPoint CreateEndPoint(string ipPort)
        {
            string[] target = ipPort.Trim().Split(':');
            if (target.Length != 2)
                return null;

            if (!int.TryParse(target[1], out int port))
                return null;

            string[] ipParts = target[0].Split('.');
            if (ipParts.Length != 4)
                return null;

            long ip = 0L;
            for (int i = ipParts.Length - 1; i >= 0; --i)
            {
                if (!uint.TryParse(ipParts[i], out uint partValue))
                    return null;

                ip |= partValue << (8 * i);
            }

            return new IPEndPoint(ip, port);
        }
        private IPAddress CreateAddress(string ipAddr)
        {
            string[] ipParts = ipAddr.Split('.');
            if (ipParts.Length != 4)
                return null;

            long ip = 0L;
            for (int i = ipParts.Length - 1; i >= 0; --i)
            {
                if (!uint.TryParse(ipParts[i], out uint partValue))
                    return null;

                ip |= partValue << (8 * i);
            }

            return new IPAddress(ip);
        }

        public void SaveFile(TFileObject file)
        {
            if (file == null)
                return;

            if (string.IsNullOrEmpty(file.FilePath))
            {
                SaveFileAs(file);
                return;
            }

            SaveFile(file, file.FilePath);
        }
        public void SaveFileAs(TFileObject file)
        {
            if (file == null)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = TFileObject.GetFilter(file.GetType(), true, true, false, true),
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    SaveFile(file, sfd.FileName);
            }
        }
        private async void SaveFile(TFileObject file, string filePath, ESerializeFlags flags = ESerializeFlags.Default)
        {
            ContentTree.BeginFileSaveWithProgress(filePath, "Saving file...", out Progress<float> progress, out CancellationTokenSource cancel);
            await file.ExportAsync(filePath, flags, progress, cancel.Token);
            ContentTree.EndFileSave(filePath);
        }
    }
    public class DockableFormInstance<T> where T : DockContent, new()
    {
        public DockableFormInstance(Action<T> onCreated)
            => _onCreated = onCreated;

        private event Action<T> _onCreated;
        private T _value;

        public bool Active
            => _value != null && !_value.IsDisposed;

        public T Form
        {
            get
            {
                if (_value == null || _value.IsDisposed)
                {
                    _value = new T();
                    Engine.PrintLine("Created " + _value.GetType().GetFriendlyName());
                    _onCreated?.Invoke(_value);
                }
                return _value;
            }
        }
    }
}
