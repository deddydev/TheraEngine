using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
                        CapFPS = false,
                        TargetFPS = 60.0f,
                    }
                };
            }

            Engine.EditorState.InGameMode = false;
            Engine.SetGame(Project);
            InitializeComponent();
            DoubleBuffered = false;
            Engine.Initialize(renderPanel1, true);

            GenerateInitialActorList();
            if (Engine.World != null)
            {
                Engine.World.State.SpawnedActors.PostAdded += SpawnedActors_PostAdded;
                Engine.World.State.SpawnedActors.PostRemoved += SpawnedActors_PostRemoved;
            }

            var fileObjectTypes = 
                from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetExportedTypes()
                where assemblyType.IsSubclassOf(typeof(FileObject)) && !assemblyType.IsAbstract
                select assemblyType;
            Dictionary<string, NamespaceNode> nodes = new Dictionary<string, NamespaceNode>();
            foreach (Type t in fileObjectTypes)
            {
                string path = t.Namespace;
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                if (nodes.ContainsKey(name))
                    nodes[name].Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t);
                else
                {
                    NamespaceNode node = new NamespaceNode(name, this);
                    nodes.Add(name, node);
                    testToolStripMenuItem.DropDownItems.Add(node.Button);
                }
            }
        }

        private class NamespaceNode
        {
            public NamespaceNode(string name, Editor editor)
            {
                _editor = editor;
                _name = name;
                _children = new Dictionary<string, NamespaceNode>();
                Button = new ToolStripDropDownButton(_name) { ShowDropDownArrow = true };
            }

            Editor _editor;
            string _name;
            Dictionary<string, NamespaceNode> _children;
            ToolStripDropDownButton _button;

            public string Name { get => _name; set => _name = value; }
            private Dictionary<string, NamespaceNode> Children { get => _children; set => _children = value; }
            public ToolStripDropDownButton Button { get => _button; set => _button = value; }

            public void Add(string path, Type t)
            {
                if (string.IsNullOrEmpty(path))
                {
                    ToolStripDropDownButton btn = new ToolStripDropDownButton(t.Name)
                    {
                        ShowDropDownArrow = false,
                        Tag = t,
                    };
                    btn.Click += _editor.OnNewClick;
                    _button.DropDownItems.Add(btn);
                    return;
                }
                int dotIndex = path.IndexOf(".");
                string name = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
                if (_children.ContainsKey(name))
                    _children[name].Add(dotIndex > 0 ? path.Substring(dotIndex + 1) : null, t);
                else
                {
                    NamespaceNode node = new NamespaceNode(name, _editor);
                    _children.Add(name, node);
                    Button.DropDownItems.Add(node.Button);
                }
            }
        }

        private void OnNewClick(object sender, EventArgs e)
        {
            if (sender is ToolStripDropDownButton button)
            {
                Type fileType = button.Tag as Type;
                FileObject file = (FileObject)Activator.CreateInstance(fileType);
                string dir = contentTree.SelectedNode.Tag as string;
                if (dir.Contains("."))
                    dir = Path.GetDirectoryName(dir);
                file.Export(dir, file.Name, FileFormat.XML);
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

            if (DesignMode)
                return;

            OnRedrawn = Redraw;
            Engine.RegisterRenderTick(RenderTick);
            actorPropertyGrid.SelectedObject = Engine.World?.Settings;
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
            if (string.IsNullOrEmpty(_project.ReferencePath))
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

        private void cboContentViewTypes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btPlay_Click(object sender, EventArgs e)
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
    }
}
