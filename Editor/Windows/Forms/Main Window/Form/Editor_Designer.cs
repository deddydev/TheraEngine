﻿using Extensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Input.Devices;
using TheraEngine.Networking;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;
using WindowsNativeInterop;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor : TheraForm, IMappableShortcutControl, IDockPanelOwner
    {
        DockPanel IDockPanelOwner.DockPanelRef => DockPanel;

        public Editor() : base()
        {
            InitializeComponent();

#if !DEBUG
            btnPackageNewRelease.Visible = false;
#endif

            DockPanel.Theme = new TheraEditorTheme();

            FormTitle2.MouseDown += TitleBar_MouseDown;
            TheraEngineText.MouseDown += TitleBar_MouseDown;
            PaddingPanel.MouseDown += TitleBar_MouseDown;
            lblVersion.MouseDown += TitleBar_MouseDown;

            FormTitle2.Text = Text;

            menuStrip1.Renderer = new TheraToolStripRenderer();
            _deserializeDockContent = GetContentFromPersistString;

            AutoScaleMode = AutoScaleMode.None;
            DoubleBuffered = false;

            KeyPreview = true;
            MappableActions = new Dictionary<Keys, Func<bool>>()
            {
                { Keys.Control | Keys.Z, Undo },
                { Keys.Control | Keys.Y, Redo },
            };

            TheraEngineText.Font = Engine.MakeFont("origicide", 10.0f, FontStyle.Regular);

            lblYourIpPort.Text = $"Your IP: {NetworkConnection.GetLocalIPAddressV4()}";
            CursorManager.GlobalWrapCursorWithinClip = false;

            if (lblVersion is null)
                return;

#if DEBUG
            lblVersion.Visible = true;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            lblVersion.Text = $"Editor Debug Ver {version} --- Built {buildDate}";
#else
            lblVersion.Visible = false;
#endif
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

        private DeserializeDockContent _deserializeDockContent;

        #region Instanced Dock Forms
        //Dockable forms with a limited amount of instances
        private DockableWorldRenderPanel[] _renderForms = new DockableWorldRenderPanel[4];
        public bool RenderFormActive(int i)
        {
            DockableWorldRenderPanel form = _renderForms[i];
            return form != null && !form.IsDisposed;
        }
        public DockableWorldRenderPanel GetRenderForm(int i)
        {
            DockableWorldRenderPanel form = _renderForms[i];
            if (form is null || form.IsDisposed)
            {
                Engine.Out("Created viewport " + (i + 1).ToString());
                form = _renderForms[i] = new DockableWorldRenderPanel(ELocalPlayerIndex.One, i);
                form.Show(DockPanel);
            }
            return form;
        }
        public DockableWorldRenderPanel RenderForm1 => GetRenderForm(0);
        public DockableWorldRenderPanel RenderForm2 => GetRenderForm(1);
        public DockableWorldRenderPanel RenderForm3 => GetRenderForm(2);
        public DockableWorldRenderPanel RenderForm4 => GetRenderForm(3);
        
        private static bool GetFormActive<T>(T value) where T : DockContent
            => value != null && !value.IsDisposed;
        
        public T GetForm<T>(ref T value, DockState defaultDockState = DockState.Document) where T : DockContent, new()
        {
            if (value != null && !value.IsDisposed)
                return value;

            value = new T();
            Engine.Out("Created " + value.GetType().GetFriendlyName());
            if (InvokeRequired)
            {
                T value2 = value;
                Invoke((Action)(() => value2.Show(DockPanel, defaultDockState)));
            }
            else
                value.Show(DockPanel);
            return value;
        }

        private DockableErrorList _errorListForm;
        public bool DockableErrorListFormActive => GetFormActive(_errorListForm);
        public DockableErrorList ErrorListForm => GetForm(ref _errorListForm, DockState.DockBottom);
        
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

        public int WorldManagerId { get; private set; }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Engine.Instance.DomainProxyCreated += Instance_ProxySet;
            Engine.Instance.DomainProxyDestroying += Instance_ProxyUnset;
            Instance_ProxySet(Engine.DomainProxy);
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            Engine.Instance.DomainProxyCreated -= Instance_ProxySet;
            Engine.Instance.DomainProxyDestroying -= Instance_ProxyUnset;
            Instance_ProxyUnset(Engine.DomainProxy);
        }
        private void Instance_ProxyUnset(EngineDomainProxy proxy)
        {
            try
            {
                proxy.UnregisterWorldManager(WorldManagerId);

                if (proxy is EngineDomainProxyEditor editorProxy)
                {
                    editorProxy.PreWorldChanged -= WorldPreChanged;
                    editorProxy.PostWorldChanged -= WorldPostChanged;
                }
            }
            catch
            {

            }
        }
        private void Instance_ProxySet(EngineDomainProxy proxy)
        {
            WorldManagerId = proxy.RegisterWorldManager<EditorWorldManager>();

            proxy.SponsorObject(this);
            proxy.SponsorObject(ActorTreeForm);

            if (proxy is EngineDomainProxyEditor editorProxy)
            {
                editorProxy.PreWorldChanged += WorldPreChanged;
                editorProxy.PostWorldChanged += WorldPostChanged;
            }
        }
        //private void WorldSettingsUnloaded(WorldSettings settings)
        //{
        //    if (settings != null)
        //        settings.Maps.Changed -= Maps_Changed;
        //}
        //private void WorldSettingsLoaded(WorldSettings settings)
        //{
        //    if (settings != null)
        //        settings.Maps.Changed += Maps_Changed;
        //}
        //private void Maps_Changed()
        //{
        //    ActorTreeForm.Clear();
        //}

#region World Management
        internal void TrySetWorld(FileRef<World> worldRef)
        {
            if (DomainProxy.TryCloseWorld())
                DomainProxy.LoadWorld(worldRef.Path.Path);
        }
        private void WorldPreChanged()
        {
            ActorTreeForm.UnlinkWorld();
        }
        private void WorldPostChanged()
        {
            IWorld world = DomainProxy.World;
            bool worldExists = world != null;

            //var sref = world?.SettingsRef;
            //if (sref != null)
            //{
            //    sref.Loaded += (WorldSettingsLoaded);
            //    sref.Unloaded += (WorldSettingsUnloaded);
            //}

            if (InvokeRequired)
                Invoke((Action)(() => { btnWorldSettings.Enabled = btnSaveWorld.Enabled = btnSaveWorldAs.Enabled = worldExists; }));
            else
                btnWorldSettings.Enabled = btnSaveWorld.Enabled = btnSaveWorldAs.Enabled = worldExists;

            PropertyGridForm.PropertyGrid.TargetObject = world?.Settings;

            ActorTreeForm.LinkWorld(world);
        }
        public bool TryCloseWorld()
        {
            return DomainProxy.TryCloseWorld();
        }
        /// <summary>
        /// Creates and loads a new world for editing.
        /// Closes the currently opened world if there is one.
        /// </summary>
        public void CreateNewWorld()
        {
            if (!TryCloseWorld())
                return;

            DomainProxy.CreateNewWorld();
        }
        public void OpenWorld()
        {
            if (!TryCloseWorld())
                return;

            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.CreateFilter<World>(),
                Multiselect = false
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                    DomainProxy.LoadWorld(ofd.FileName);
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

            EditorSettings defaultSettings = await DefaultSettingsRef.GetInstanceAsync();
            List<string> recentFiles = defaultSettings?.RecentlyOpenedProjectPaths;

            string lastOpened = null;
            if (recentFiles != null && recentFiles.Count > 0)
                lastOpened = recentFiles[recentFiles.Count - 1];
            
            LoadProject(lastOpened);

            //Run on UI side
            Engine.Run();

            CheckUpdates();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UpdateRecentProjectPaths();

            if (TryCloseProject())
            {
                //SetRenderTicking(false);
                Engine.Stop();
                Engine.ShutDown();
            }
            else
                e.Cancel = true;

            base.OnFormClosing(e);
        }
        public void UpdateRecentProjectPaths()
        {
            try
            {
                string projectPath = Project?.FilePath;
                if (string.IsNullOrWhiteSpace(projectPath))
                    return;

                var defaultSettings = DefaultSettingsRef.File;
                var list = defaultSettings.RecentlyOpenedProjectPaths;
                if (list != null)
                {
                    if (list.Contains(projectPath))
                        list.Remove(projectPath);
                    list.Add(projectPath);
                }
                else
                    defaultSettings.RecentlyOpenedProjectPaths = new List<string>() { projectPath };

                defaultSettings.Export();
            }
            catch
            {

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
                    Engine.Out(e.KeyData.ToString().Replace(", ", "+") + ": " + func.Method.Name);
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

        #region Project Management

        public TProject Project
        {
            get => _project;
            set => _project = value;
        }
        private TProject _project;

        public static List<IFileEditorControl> OpenEditors { get; } = new List<IFileEditorControl>();

        /// <summary>
        /// Asks the user to select a folder to create a new project in, creates it there, and then loads it for editing.
        /// Closes the currently opened project if there is one.
        /// </summary>
        public void CreateNewProject()
        {
            if (!TryCloseProject())
                return;
            
            ProjectCreatorForm.Focus();
            ProjectCreatorForm.ProjectCreated += ProjectCreatorForm_ProjectCreated;
        }
        private void ProjectCreatorForm_ProjectCreated()
        {
            Project = ProjectCreatorForm.Project;
            PropertyGridForm.PropertyGrid.TargetObject = Project;
        }
        public void OpenProject()
        {
            if (!TryCloseProject())
                return;

            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.CreateFilter<TProject>(),
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                    LoadProject(ofd.FileName);
            }
        }
        public void SaveProject() => DomainProxy.SaveFile(Project);
        public void SaveProjectAs() => DomainProxy.SaveFileAs(Project);
        public bool TryCloseProject()
        {
            var editors = OpenEditors.ToArray();
            foreach (var form in editors)
                if (form.AllowFileClose())
                    ((Form)form).Close();
                else
                    return false;

            if (_project?.EditorState is null || !_project.EditorState.HasChanges)
            {
                SaveDockState();
                ClearDockPanel();
                _project = null;
                return true;
            }

            DialogResult result = MessageBox.Show(
                this,
                "Save changes to current project?",
                "Save changes?",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation);

            if (result == DialogResult.Cancel)
                return false;

            DomainProxy.SaveFile(_project);

            SaveDockState();
            ClearDockPanel();

            _project = null;

            return true;
        }
        public void SaveDockState()
        {
            string configFile = GetDockingConfigPath();
            if (configFile?.IsExistingDirectoryPath() == false)
                DockPanel.SaveAsXml(configFile);
        }
        public string GetDockingConfigPath() => Project?.EditorSettings?.GetFullDockConfigPath();
        public async Task SetProjectAsync(TProject project)
        {
            //Engine.ShutDown();
            //Engine.SetGame(_project);

            _project = project;

            bool projectOpened = _project != null;
            btnProjectEngineSettings.Enabled =
            btnProjectSettings.Enabled =
            btnUserSettings.Enabled =
            btnPlay.Enabled =
            btnPlayDetached.Enabled =
            btnCompile.Enabled =
            projectOpened;

            ClearDockPanel();

            if (projectOpened)
            {
                if (string.IsNullOrEmpty(_project.FilePath))
                {
                    var result = ProjectCreatorForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        //ProjectCreatorForm.Project;
                    }
                }

                //Problem: project needs to exist in its own game domain
                //Create domain using project, load project AGAIN in domain, 
                await _project.CreateGameDomainAsync(false); 
                var errors = _project?.LastBuildLog?.Errors;
                if (errors != null && errors.Count > 0)
                    _project.LastBuildLog.Display();

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
                
                Text = _project.FilePath;
                ContentTree.OpenPath(_project.FilePath);
                
                //Engine.SetWorldPanel(RenderForm1.RenderPanel, false);
                //Engine.Initialize();
                //SetRenderTicking(true);
                //Engine.SetPaused(true, ELocalPlayerIndex.One, true);

                DomainProxy.LoadWorld(_project?.Game?.OpeningWorldRef?.Path?.Path);

                UpdateRecentProjectPaths();
            }
            else
            {
                //CreateGameDomain(null, null, null);

                Text = string.Empty;

                DockPanel.SuspendLayout(true);
                OutputForm.Show(DockPanel, DockState.DockBottom);
                WelcomeForm.Show(DockPanel, DockState.Document);
                DockPanel.ResumeLayout(true, true);
            }

            Engine.Out($"Set project to {_project?.FilePath?.ToString() ?? "null"}.");
        }
        public async void LoadProject(string projectPath)
        {
            if (!TryCloseProject())
                return;

            DomainProxy.GameState = EEditorGameplayState.Editing;

            TProject project = null;

            if (projectPath != null && File.Exists(projectPath))
                project = await TFileObject.LoadAsync<TProject>(projectPath);

            await SetProjectAsync(project);
        }

#endregion

        private bool Undo()
        {
            bool canUndo = DomainProxy.CanUndo;
            DomainProxy.Undo();
            return canUndo;
        }
        private bool Redo()
        {
            bool canRedo = DomainProxy.CanRedo;
            DomainProxy.Redo();
            return canRedo;
        }

        public void ClearDockPanel()
        {
            foreach (IDockContent document in DockPanel.Contents.ToArray())
            {
                document.DockHandler.DockPanel = null;
                document.DockHandler.Close();
            }
        }
        
        private DockableWorldRenderPanel FocusViewport(int index)
        {
            DockableWorldRenderPanel form = GetRenderForm(index);
            if (form.IsHidden)
                form.Show(DockPanel, DockState.Document);
            form.Focus();
            return form;
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(DockableActorTree).ToString())
                return ActorTreeForm;
            if (persistString == typeof(DockableFileTree).ToString())
                return FileTreeForm;
            if (persistString == typeof(DockableOutputWindow).ToString())
                return OutputForm;
            if (persistString == typeof(DockablePropertyGrid).ToString())
                return PropertyGridForm;

            string[] parsedStrings = persistString.Split(',');
            if (parsedStrings.Length == 0)
                return null;
            string type = parsedStrings[0];
            if (type != typeof(DockableWorldRenderPanel).ToString())
                return null;
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
        
        private void connectAsServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Server s = Engine.ServerConnection;
            if (s is null)
            {
                s = new Server();
                Engine.Network = s;
            }

            s.InitializeConnection(NetworkConnection.ServerPort, null);

            Engine.Out($"Started hosting server at {NetworkConnection.GetLocalIPAddressV4()}:{NetworkConnection.ServerPort}.");
        }

        private async void connectAsClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Client c = Engine.ClientConnection;
            if (c is null)
            {
                c = new Client();
                Engine.Network = c;
            }

            IPAddress addr = CreateAddress(txtTargetIPPort.Text);
            if (addr != null)
            {
                IPEndPoint point = new IPEndPoint(addr, NetworkConnection.ServerPort);
                c.InitializeConnection(NetworkConnection.ClientPort, point);

                Engine.Out($"Requesting connection to server at {point.Address.ToString()}:{NetworkConnection.ServerPort}.");
            }
            else
            {
                Engine.Out($"'{txtTargetIPPort.Text}' is not a valid IP configuration.");
                return;
            }

            bool success = await c.RequestConnectionAsync();
            Engine.Out(success
                ? "Successfully connected to the target server."
                : "Failed to connect to the target server.");
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

        /// <summary>
        /// Associates an extension with the editor.
        /// </summary>
        /// <param name="ext">The extension of the file to associate.</param>
        /// <param name="fileName">The name of the file type.</param>
        /// <param name="keyName">The key to use for the association.</param>
        public static void AssociateExtension(string ext, string fileName, string keyName)
        {
            string openWithPath = Assembly.GetExecutingAssembly().Location;

            RegistryKey baseKey = Registry.ClassesRoot.CreateSubKey(ext);
            baseKey.SetValue("", keyName);
            baseKey.Close();

            RegistryKey openMethod = Registry.ClassesRoot.CreateSubKey(keyName);
            openMethod.SetValue("", fileName);
            openMethod.CreateSubKey("DefaultIcon").SetValue("", "\"" + openWithPath + "\",0");
            openMethod.Close();

            RegistryKey shell = openMethod.CreateSubKey("Shell");
            shell.CreateSubKey("edit").CreateSubKey("command").SetValue("", "\"" + openWithPath + "\"" + " \"%1\"");
            shell.CreateSubKey("open").CreateSubKey("command").SetValue("", "\"" + openWithPath + "\"" + " \"%1\"");
            shell.Close();

            //Delete the key instead of trying to change it
            string keyLocation = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + ext;
            RegistryKey currentUser = Registry.CurrentUser.OpenSubKey(keyLocation, true);
            currentUser.DeleteSubKey("UserChoice", false);
            currentUser.Close();

            //Tell explorer the file association has been changed
            NativeMethods.SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
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
                if (_value != null && !_value.IsDisposed)
                    return _value;

                _value = new T();
                Engine.Out("Created " + _value.GetType().GetFriendlyName());
                _onCreated?.Invoke(_value);
                return _value;
            }
        }
    }
}
