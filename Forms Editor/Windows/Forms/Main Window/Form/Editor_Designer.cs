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
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Editor;
using TheraEngine.Files;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
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

            bool sameGameMode = ReferenceEquals(ActiveRenderForm?.GameMode, control?.GameMode);

            if (ActiveRenderForm != null)
            {
                int index = (int)ActiveRenderForm.PlayerIndex;
                if (index < Engine.LocalPlayers.Count)
                {
                    LocalPlayerController c = Engine.LocalPlayers[index];
                    ActiveRenderForm.RenderPanel.UnregisterController(c);
                    c.ControlledPawn = null;
                }

                if (!sameGameMode)
                    ActiveRenderForm.GameMode?.EndGameplay();
            }
            ActiveRenderForm = control;
            if (ActiveRenderForm != null)
            {
                if (!sameGameMode)
                    ActiveRenderForm.GameMode?.BeginGameplay();

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

        #endregion

        public Editor() : base()
        {
            Instance = this;

            _editorGameMode = new EditorGameMode();
            InitializeComponent();

            FormTitle2.MouseDown += new MouseEventHandler(TitleLabel_MouseDown);
            TheraEngineText.MouseDown += new MouseEventHandler(TitleLabel_MouseDown);
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
        }

        public UndoManager UndoManager { get; } = new UndoManager();

        private Project _project;
        private Assembly _gameProgram;
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
        public static EditorSettings GetSettings() => GetSettingsRef()?.File;

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

        private DockableAnalytics _analyticsForm;
        public DockableAnalytics Analytics => GetForm(ref _analyticsForm);

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
            set
            {
                if (InvokeRequired)
                {
                    Invoke((Action)(() => CurrentWorld = value));
                    return;
                }

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

                bool isNull = Engine.World == null;

                btnWorldSettings.Enabled = btnSaveWorld.Enabled = btnSaveWorldAs.Enabled = !isNull;

                GenerateInitialActorList();
                if (!isNull)
                {
                    Engine.World.State.SpawnedActors.PostAnythingAdded += SpawnedActors_PostAdded;
                    Engine.World.State.SpawnedActors.PostAnythingRemoved += SpawnedActors_PostRemoved;
                }
                PropertyGridForm.PropertyGrid.TargetFileObject = Engine.World?.Settings;
            }
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
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CheckUpdates();

            //xcopy /Y "$(SolutionDir)Libraries\$(Platform)\FreeImage.dll" "$(TargetDir)"

            //TODO: read editor state file instead
            string lastOpened = Properties.Settings.Default.LastOpened;
            //"C:\\Users\\David\\Desktop\\test project\\NewProject.xtproj";
            if (!string.IsNullOrEmpty(lastOpened))
                Project = TFileObject.Load<Project>(lastOpened);
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
            if (Engine.Scene != null)
            {
                Engine.Scene.UpdateShadowMaps();
            }
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.UpdateTick(sender, e);
        }
        private void SwapBuffers()
        {
            if (Engine.Scene != null)
            {
                Engine.Scene.Lights.SwapBuffers();
            }
            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.SwapBuffers();
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            try { Invoke((Action)Redraw); } catch { }
            //Redraw();
        }

        private void Redraw()
        {
            if (BaseRenderPanel.WorldPanel == null)
                return;

            BaseRenderPanel.WorldPanel.CaptureContext();

            if (Engine.Scene != null)
            {
                Engine.Scene.Voxelize();
                Engine.Scene.RenderShadowMaps();
            }

            for (int i = 0; i < 4; ++i)
                if (RenderFormActive(i))
                    GetRenderForm(i).RenderPanel.Invalidate();

            //Application.DoEvents();
        }
        //protected override void OnResizeBegin(EventArgs e)
        //{\
        //    RenderForm.RenderPanel.BeginResize();
        //    base.OnResizeBegin(e);
        //}
        //protected override void OnResizeEnd(EventArgs e)
        //{
        //    RenderForm.RenderPanel.EndResize();
        //    base.OnResizeEnd(e);
        //}
        public static GlobalFileRef<EditorSettings> DefaultSettingsRef { get; }
            = new GlobalFileRef<EditorSettings>(Path.GetFullPath(string.Format(Application.StartupPath + "{0}..{0}..{0}..{0}Editor{0}Config.xset", Path.DirectorySeparatorChar)), () => new EditorSettings());

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

                string configFile = _project.EditorSettings.GetFullDockConfigPath();
                //Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
                //DockPanel.SaveAsXml(configFile);

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
        public void OpenProject()
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<Project>(),
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK && CloseProject())
                    Project = TFileObject.Load<Project>(ofd.FileName);
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
                            BaseRenderPanel renderPanel = (ActiveRenderForm as DockableWorldRenderForm)?.RenderPanel ?? FocusViewport(0).RenderPanel;
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
                            BaseRenderPanel renderPanel = (ActiveRenderForm as DockableWorldRenderForm)?.RenderPanel ?? FocusViewport(0).RenderPanel;
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
        public void OpenWorld()
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<World>(),
                Multiselect = false
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK && CloseWorld())
                    CurrentWorld = TFileObject.Load<World>(ofd.FileName);
            }
        }
    }
}
