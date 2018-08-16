using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Extensions;
using TheraEngine.Editor;
using TheraEngine.Files;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Networking;
using TheraEngine.Rendering;
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
        LocalPlayerIndex PlayerIndex { get; }
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
    public partial class Editor : TheraForm, IMappableShortcutControl
    {
        #region Statics
        
        public static Editor Instance { get; private set; }

        public static IEditorControl ActiveRenderForm { get; private set; } = null;
        /// <summary>
        /// This will possess and unpossess the necessary viewports and pawns corresponding to the given editor control.
        /// </summary>
        /// <param name="control">The editor control that the user is focused on.</param>
        public static void SetActiveEditorControl(IEditorControl control)
        {
            if (ActiveRenderForm == control)
                return;

            //bool sameGameMode = ReferenceEquals(ActiveRenderForm?.GameMode, control?.GameMode);

            if (ActiveRenderForm != null)
            {
                int index = (int)ActiveRenderForm.PlayerIndex;
                if (index < Engine.LocalPlayers.Count)
                {
                    LocalPlayerController c = Engine.LocalPlayers[index];
                    ActiveRenderForm.RenderPanel.UnregisterController(c);
                    c.ControlledPawn = null;
                }

                //if (!sameGameMode)
                ActiveRenderForm.GameMode?.EndGameplay();
            }
            ActiveRenderForm = control;
            if (ActiveRenderForm != null)
            {
                //if (!sameGameMode)
                    ActiveRenderForm.GameMode?.BeginGameplay();

                int index = (int)control.PlayerIndex;
                if (index < Engine.LocalPlayers.Count)
                {
                    LocalPlayerController c = Engine.LocalPlayers[index];
                    ActiveRenderForm.RenderPanel.GetOrAddViewport(0).RegisterController(c);
                    c.ControlledPawn = ActiveRenderForm.EditorPawn;
                }

                Engine.PrintLine("Set active render form: " + ActiveRenderForm.ToString());
            }
        }

        public string BuildConfiguration { get; internal set; } = "Debug";
        public string BuildPlatform { get; internal set; } = "Any CPU";
        public void Compile(string filePath)
        {
            ProjectCollection pc = new ProjectCollection();
            Dictionary<string, string> globalProperties = new Dictionary<string, string>
            {
                { "Configuration", BuildConfiguration },
                { "Platform", BuildPlatform },
            };
            BuildRequestData request = new BuildRequestData(filePath, globalProperties, null, new string[] { "Build" }, null);
            BuildResult result = BuildManager.DefaultBuildManager.Build(new BuildParameters(pc), request);
            if (result.OverallResult == BuildResultCode.Success)
            {
                Engine.PrintLine(filePath + " : Build succeeded.");
            }
            else
            {
                Engine.PrintLine(filePath + " : Build failed.");
                foreach (var target in result.ResultsByTarget)
                {
                    if (target.Value.ResultCode == TargetResultCode.Failure)
                    {
                        Engine.PrintLine(target.Key + " : Build failed.");
                        if (target.Value.Exception != null)
                            Engine.PrintLine("Exception:\n" + target.Value.Exception.ToString());
                    }
                }
            }
        }

        private class OperationInfo
        {
            private readonly Action _updated;
            private CancellationTokenSource _token;

            public DateTime StartTime { get; }
            public TimeSpan OperationDuration { get; private set; }
            public Progress<float> Progress { get; }
            public float ProgressValue { get; private set; } = 0.0f;
            public bool IsComplete => ProgressValue >= 0.99f;
            public bool CanCancel => _token != null && _token.Token.CanBeCanceled;

            public OperationInfo(Progress<float> progress, CancellationTokenSource cancel, Action updated)
            {
                _updated = updated;
                Progress = progress;
                if (Progress != null)
                    Progress.ProgressChanged += Progress_ProgressChanged;
                _token = cancel;
                StartTime = DateTime.Now;
            }
            
            private void Progress_ProgressChanged(object sender, float progressValue)
            {
                ProgressValue = progressValue;
                OperationDuration = DateTime.Now - StartTime;
                _updated();
            }

            public void Cancel()
            {
                _token?.Cancel();
                if (Progress != null)
                    Progress.ProgressChanged -= Progress_ProgressChanged;
            }
        }
        private List<OperationInfo> _operations = new List<OperationInfo>();

        public void ReportOperation(string statusBarMessage, Progress<float> progress, CancellationTokenSource token)
        {
            if (_operations.Count == 0)
            {
                toolStripProgressBar1.Value = 0;
            }
            
            _operations.Add(new OperationInfo(progress, token, Info_Updated));
            
            btnCancelOp.Visible = _operations.Any(x => x.CanCancel);
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = statusBarMessage;
        }
        
        private void Info_Updated()
        {
            int resolution = toolStripProgressBar1.Maximum;

            float avgProgress = 0.0f;
            int opCount = _operations.Count;
            for (int i = 0; i < _operations.Count; ++i)
            {
                OperationInfo info = _operations[i];
                avgProgress += info.ProgressValue;
                if (info.IsComplete)
                {
                    toolStripStatusLabel1.Text = $"Operation completed successfully in {Math.Round(info.OperationDuration.TotalSeconds, 2, MidpointRounding.AwayFromZero)} seconds.";
                    _operations.RemoveAt(i--);
                }
            }

            if (opCount == 0)
                return;

            avgProgress /= opCount;

            int value = (int)(avgProgress * (resolution + 0.5f));
            toolStripProgressBar1.ProgressBar.Value = value;
            if (value >= resolution)
                EndOperation();
        }
        private void EndOperation()
        {
            _operations.Clear();
            btnCancelOp.Visible = false;
            toolStripProgressBar1.Visible = false;
        }
        private void btnCancelOp_ButtonClick(object sender, EventArgs e)
        {
            foreach (OperationInfo info in _operations)
            {
                info.Cancel();
            }
            EndOperation();
            toolStripStatusLabel1.Text = _operations.Count == 1 ?
                "Operation was canceled." :
                "Operations were canceled.";
        }

        public static Color BackgroundColor => Color.FromArgb(92, 93, 100);
        public static Color TitleBarColor => Color.FromArgb(92, 93, 100);
        public static Color TurquoiseColor => Color.FromArgb(150, 192, 192);
        public static Color TextColor => Color.FromArgb(224, 224, 224);

        #endregion

        public Editor() : base()
        {
            Instance = this;

            _editorGameMode = new EditorGameMode();
            InitializeComponent();

            FormTitle2.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            TheraEngineText.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            PaddingPanel.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            FormTitle2.Text = Text;

            menuStrip1.Renderer = new TheraToolstripRenderer();
            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            DockPanel.Theme = new TheraEditorTheme();
            AutoScaleMode = AutoScaleMode.None;
            DoubleBuffered = false;
            //menuStrip1.Padding = new Padding(0, (TitlePanel.Height - menuStrip1.Height) / 2, 0, 0);

            KeyPreview = true;
            MappableActions = new Dictionary<Keys, Func<bool>>()
            {
                { Keys.Control | Keys.Z, Undo },
                { Keys.Control | Keys.Y, Redo },
            };

            TheraEngineText.Font = Engine.MakeFont("origicide", 10.0f, FontStyle.Regular);

            lblYourIpPort.Text = "Your IP: " + NetworkConnection.GetLocalIPAddressV4();
            CursorManager.WrapCursorWithinClip = false;
        }

        public UndoManager UndoManager { get; } = new UndoManager();

        private Project _project;
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
                form = _renderForms[i] = new DockableWorldRenderForm(LocalPlayerIndex.One, i);
                form.Show(DockPanel);
            }
            return form;
        }
        public DockableWorldRenderForm RenderForm1 => GetRenderForm(0);
        public DockableWorldRenderForm RenderForm2 => GetRenderForm(1);
        public DockableWorldRenderForm RenderForm3 => GetRenderForm(2);
        public DockableWorldRenderForm RenderForm4 => GetRenderForm(3);
        
        public static GlobalFileRef<EditorSettings> GetSettingsRef() => Instance.Project?.EditorSettingsRef ?? DefaultSettingsRef;
        public static EditorSettings GetSettings() => Instance.Project?.EditorSettingsRef?.File ?? DefaultSettingsRef?.File;

        private bool GetFormActive<T>(T value) where T : DockContent
        {
            return value != null && !value.IsDisposed;
        }
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

        private DockableGPUAnalytics _analyticsForm;
        public bool AnalyticsFormActive => GetFormActive(_analyticsForm);
        public DockableGPUAnalytics GPUAnalyticsForm => GetForm(ref _analyticsForm);

        private DockableOutputWindow _outputForm;
        public bool OutputFormActive => GetFormActive(_outputForm);
        public DockableOutputWindow OutputForm => GetForm(ref _outputForm);

        private DockableActorTree _actorTreeForm;
        public bool ActorTreeFormActive => GetFormActive(_actorTreeForm);
        public DockableActorTree ActorTreeForm => GetForm(ref _actorTreeForm);

        private DockableFileTree _fileTreeForm;
        public bool FileTreeFormActive => GetFormActive(_fileTreeForm);
        public DockableFileTree FileTreeForm => GetForm(ref _fileTreeForm);
        
        private DockableMSBuildTree _msBuildTreeForm;
        public bool MSBuildTreeFormActive => GetFormActive(_msBuildTreeForm);
        public DockableMSBuildTree MSBuildTreeForm => GetForm(ref _msBuildTreeForm);

        private DockablePropertyGrid _propertyGridForm;
        public bool PropertyGridFormActive => GetFormActive(_propertyGridForm);
        public DockablePropertyGrid PropertyGridForm => GetForm(ref _propertyGridForm);

        private DockableWelcomeWindow _welcomeForm;
        public bool WelcomeFormActive => GetFormActive(_welcomeForm);
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
                    Engine.SetPaused(true, LocalPlayerIndex.One, true);
                    Engine.Run();

                    CurrentWorld = _project.OpeningWorldRef?.File;
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
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (FormTitle2 != null)
                FormTitle2.Text = Text;
            base.OnTextChanged(e);
        }

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

        /// <summary>
        /// The world that the editor is currently editing.
        /// </summary>
        public World CurrentWorld
        {
            get => Engine.World;
            set => SetWorld(value);
        }
        private void SetWorld(World world)
        {
            if (Engine.World?.EditorState?.HasChanges ?? false)
            {
                DialogResult r = MessageBox.Show(this, "Save changes to current world?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (r == DialogResult.Cancel)
                    return;
                else if (r == DialogResult.Yes)
                    Engine.World.Export();
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
            PropertyGridForm.PropertyGrid.TargetFileObject = Engine.World?.Settings;
        }
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
            if (type.IsPrimitive)
                return type.GetDefaultValue();

            if (type == typeof(string))
                return string.Empty;

            if (type.IsGenericTypeDefinition)
            {
                using (GenericsSelector gs = new GenericsSelector(type))
                {
                    if (gs.ShowDialog() == DialogResult.OK)
                        type = gs.FinalClassType;
                    else
                        return null;
                }
            }

            using (ObjectCreator creator = new ObjectCreator())
            {
                if (creator.Initialize(type, allowDerivedTypes))
                    creator.ShowDialog();

                return creator.ConstructedObject;
            }
        }
        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CheckUpdates();
            
            //TODO: read editor state file instead
            string lastOpened = Properties.Settings.Default.LastOpened;
            //"C:\\Users\\David\\Desktop\\test project\\NewProject.xtproj";
            if (!string.IsNullOrEmpty(lastOpened))
                Project = await TFileObject.LoadAsync<Project>(lastOpened);
            else
                Project = null;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Properties.Settings.Default.LastOpened = Project?.FilePath;
            Properties.Settings.Default.Save();
            
            if (CloseProject())
            {
                SetRenderTicking(false);
                Engine.ShutDown();
            }
            else
                e.Cancel = true;

            base.OnClosing(e);
        }

        public static void SetPropertyGridObject(IFileObject obj)
        {
            Instance.PropertyGridForm.PropertyGrid.TargetFileObject = obj;
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
                    BeginInvoke(new Action<IActor>(SpawnedActors_PostAdded), item);
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
                    BeginInvoke(new Action<IActor>(SpawnedActors_PostRemoved), item);
                    return;
                }
                if (item?.EditorState?.TreeNode != null)
                {
                    if (_actorTreeForm != null &&
                        !_actorTreeForm.IsDisposed &&
                        _actorTreeForm.ActorTree.Nodes.Count > 0)
                        ActorTreeForm.ActorTree.Nodes.Remove(item.EditorState.TreeNode);
                    item.EditorState.TreeNode = null;
                }
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
        public static GlobalFileRef<EditorSettings> DefaultSettingsRef { get; }
            = new GlobalFileRef<EditorSettings>(Path.Combine(Application.StartupPath, "..", "..", "..", "Editor", "Config.xset"), () => new EditorSettings());

        public void ClearDockPanel()
        {
            foreach (IDockContent document in DockPanel.Contents.ToArray())
            {
                document.DockHandler.DockPanel = null;
                document.DockHandler.Close();
            }
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
                        _project.Export();
                }

                string configFile = _project.EditorSettings?.GetFullDockConfigPath();
                if (configFile != null && configFile.IsDirectoryPath() == false)
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

            using (FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                Description = "",
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                    Project = Project.Create(fbd.SelectedPath, "NewProject");
            }
        }
        public async void OpenProject()
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<Project>(),
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK && CloseProject())
                    Project = await TFileObject.LoadAsync<Project>(ofd.FileName);
            }
        }
        private void CboContentViewTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private DockableWorldRenderForm FocusViewport(int index)
        {
            DockableWorldRenderForm form = GetRenderForm(index);
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

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);
        internal static void ShowCursor()
        {
            Cursor.Show();
            //while (ShowCursor(true) < 0) ;
        }
        internal static void HideCursor()
        {
            Cursor.Hide();
            //while (ShowCursor(false) >= 0) ;
        }

        private Rectangle _prevClip;
        private void CaptureMouse(BaseRenderPanel panel)
        {
            CursorManager.WrapCursorWithinClip = true;
            Engine.EditorState.InEditMode = false;
            panel.Focus();
            //panel.Capture = true;
            _prevClip = Cursor.Clip;
            Cursor.Clip = panel.RectangleToScreen(panel.ClientRectangle);
            //HideCursor();
        }
        private void ReleaseMouse()
        {
            CursorManager.WrapCursorWithinClip = false;
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
            
            Engine.Pause(LocalPlayerIndex.One, true);
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

            Engine.Unpause(LocalPlayerIndex.One, true);
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

            Engine.Unpause(LocalPlayerIndex.One, true);
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Dictionary<Keys, Func<bool>> MappableActions { get; private set; }

        //internal bool DoEvents { get; set; } = true;

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

        private async void CheckUpdates(bool manual = true)
        {
            try
            {
                //string path = Path.Combine(Application.StartupPath, "Updater.exe");
                //if (!File.Exists(path))
                //{
                //    if (manual)
                //        MessageBox.Show("Could not find " + path);
                //    return;
                //}

                string editorVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string engineVer = typeof(Engine).Assembly.GetName().Version.ToString();

                await Github.Updater.CheckUpdates(editorVer, engineVer);

                //Process updater = new Process();
                //ProcessStartInfo info = new ProcessStartInfo()
                //{
                //    FileName = path,
                //    Arguments = String.Format("{0} {1}", editorVer, engineVer),
                //    CreateNoWindow = true,
                //    UseShellExecute = false,
                //    RedirectStandardOutput = true,
                //    RedirectStandardError = true,
                //    WindowStyle = ProcessWindowStyle.Hidden,
                //};
                //updater.StartInfo = info;
                //if (manual)
                //{
                //    updater.OutputDataReceived += Updater_OutputDataReceived;
                //    updater.ErrorDataReceived += Updater_ErrorDataReceived;
                //    updater.Start();
                //    updater.BeginOutputReadLine();
                //    updater.BeginErrorReadLine();
                //}
                //else
                //    updater.Start();
            }
            catch (Exception e)
            {
                if (manual)
                    MessageBox.Show(e.Message);
            }
        }

        private void Updater_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                Engine.PrintLine(e.Data);
        }

        private void Updater_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                Engine.PrintLine(e.Data);
        }

        public List<EditorState> GetDirtyFiles()
        {
            //throw new NotImplementedException();
            return new List<EditorState>();
        }

        private bool CloseWorld()
        {
            if (CurrentWorld != null)
            {
                if (CurrentWorld.EditorState != null && CurrentWorld.EditorState.HasChanges)
                {
                    DialogResult r = MessageBox.Show(this, "Save changes to current world?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (r == DialogResult.Cancel)
                        return false;
                    else if (r == DialogResult.Yes)
                        CurrentWorld.Export();
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

            CurrentWorld = new World();
        }
        public async void OpenWorld()
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<World>(),
                Multiselect = false
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK && CloseWorld())
                    CurrentWorld = await TFileObject.LoadAsync<World>(ofd.FileName);
            }
        }

        //private void btnContact_Click(object sender, EventArgs e)
        //{

        //}

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

        private void extensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cubeMapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textureGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
