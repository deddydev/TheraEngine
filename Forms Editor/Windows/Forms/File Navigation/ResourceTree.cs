using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using TheraEngine.Files;
using TheraEditor.Properties;
using TheraEditor.Wrappers;
using System.IO;
using System.Collections.Generic;
using TheraEngine;
using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Linq;
using Core.Win32.Native;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using TheraEditor.Core;

namespace TheraEditor.Windows.Forms
{
    public interface IMappableShortcutControl
    {
        Dictionary<Keys, Func<bool>> MappableActions { get; }
    }
    [Flags]
    public enum KeyStateFlags : int
    {
        LeftMouse = 0b000001,
        RightMouse = 0b000010,
        Shift = 0b000100,
        Ctrl = 0b001000,
        MiddleMouse = 0b010000,
        Alt = 0b100000,
    }
    /// <summary>
    /// Extended TreeView made specifically for synchronization with file directories.
    /// </summary>
    public class ResourceTree : TreeViewEx<BaseWrapper>, IMappableShortcutControl
    {
        private ToolTip _labelToolTip;
        private FileSystemWatcher _contentWatcher;
        private bool _allowIcons = true;
        private Dictionary<string, BaseFileWrapper> _externallyModifiedNodes = new Dictionary<string, BaseFileWrapper>();

        public event EventHandler SelectionChanged;

        [DefaultValue(true)]
        public bool AllowContextMenus { get; set; } = true;

        [DefaultValue(true)]
        public bool ShowIcons
        {
            get => _allowIcons;
            set => ImageList = (_allowIcons = value) ? Images : null;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new BaseWrapper SelectedNode
        {
            get => base.SelectedNode as BaseWrapper;
            set
            {
                if (base.SelectedNode == value)
                    return;

                base.SelectedNode = value;
                SelectedNodes = new List<BaseWrapper>() { value };
            }
        }
        private static ImageList _imgList;
        public static ImageList Images
        {
            get
            {
                if (_imgList == null)
                {
                    _imgList = new ImageList()
                    {
                        ImageSize = new Size(24, 24),
                        ColorDepth = ColorDepth.Depth32Bit,
                    };

                    _imgList.Images.Add(nameof(Resources.GenericFile), Resources.GenericFile);
                    _imgList.Images.Add(nameof(Resources.ClosedFolder), Resources.ClosedFolder);
                    _imgList.Images.Add(nameof(Resources.OpenFolder), Resources.OpenFolder);
                    _imgList.Images.Add(nameof(Resources.LockedFolder), Resources.GenericFile);

                    Type fileWrapper = typeof(BaseFileWrapper);
                    var types = Engine.FindTypes(t => fileWrapper.IsAssignableFrom(t));
                    foreach (Type t in types)
                    {
                        var wrapper = t.GetCustomAttribute<NodeWrapperAttribute>();
                        if (wrapper != null && !string.IsNullOrWhiteSpace(wrapper.ImageName))
                        {
                            object o = Resources.ResourceManager.GetObject(wrapper.ImageName, Resources.Culture);
                            if (o is Bitmap bmp && !_imgList.Images.ContainsKey(wrapper.ImageName))
                                _imgList.Images.Add(wrapper.ImageName, bmp);
                        }
                    }
                }
                return _imgList;
            }
        }

        public ResourceTree()
        {
            SetStyle(ControlStyles.UserMouse, true);

            _dragTimer.Interval = 200;
            _dragTimer.Tick += new EventHandler(Timer_Tick);

            LabelEdit = false;
            AllowDrop = true;
            Sorted = true;
            TreeViewNodeSorter = new NodeComparer();
            DrawMode = TreeViewDrawMode.OwnerDrawText;

            _labelToolTip = new ToolTip()
            {
                BackColor = Color.FromArgb(92, 93, 100),
                ForeColor = Color.FromArgb(224, 224, 224)
            };

            ItemDrag += new ItemDragEventHandler(TreeView_ItemDrag);
            DragOver += new DragEventHandler(TreeView1_DragOver);
            DragDrop += new DragEventHandler(TreeView1_DragDrop);
            DragEnter += new DragEventHandler(TreeView1_DragEnter);
            DragLeave += new EventHandler(TreeView1_DragLeave);
            GiveFeedback += new GiveFeedbackEventHandler(TreeView1_GiveFeedback);

            MappableActions = new Dictionary<Keys, Func<bool>>()
            {
                { Keys.Delete,           DeleteSelectedNodes    },
                { Keys.Control | Keys.C, CopySelectedNodes      },
                { Keys.Control | Keys.X, CutSelectedNodes       },
                { Keys.Control | Keys.V, Paste                  },
                { Keys.Control | Keys.A, SelectAllVisibleNodes  },
            };
        }

        private class NodeComparer : IComparer<TreeNode>, IComparer
        {
            public int Compare(TreeNode x, TreeNode y)
            {
                bool xNull = ReferenceEquals(x, null);
                bool yNull = ReferenceEquals(y, null);
                if (xNull)
                {
                    if (yNull)
                        return 0;
                    else
                        return 1;
                }
                else if (yNull)
                    return -1;
                bool xFile = x is BaseFileWrapper;
                bool yFile = y is BaseFileWrapper;
                if (xFile == yFile)
                    return x.Text.CompareTo(y.Text);
                else if (xFile)
                    return 1;
                else
                    return -1;
            }
            public int Compare(object x, object y)
                => Compare(x as TreeNode, y as TreeNode);
        }
        
        public void OpenPath(string path)
        {
            BeginUpdate();

            _contentWatcher = null;
            Nodes.Clear();
            ShowIcons = true;

            bool? isDir = path?.IsDirectoryPath();
            if (isDir != null)
            {
                string dir = isDir.Value ? path : Path.GetDirectoryName(path);

                BaseWrapper b = BaseWrapper.Wrap(dir);
                if (b != null)
                {
                    Nodes.Add(b);
                    b.Expand();
                }

                _contentWatcher = new FileSystemWatcher(dir, "*.*")
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true,
                    NotifyFilter =
                    NotifyFilters.FileName |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.Size |
                    NotifyFilters.CreationTime |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite
                };
                _contentWatcher.Changed += ContentWatcherUpdate;
                _contentWatcher.Created += ContentWatcherUpdate;
                _contentWatcher.Deleted += ContentWatcherUpdate;
                _contentWatcher.Renamed += ContentWatcherRename;
            }

            EndUpdate();
        }

        #region Keys
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Dictionary<Keys, Func<bool>> MappableActions { get; private set; }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == NativeConstants.WM_KEYDOWN || 
                msg.Msg == NativeConstants.WM_SYSKEYDOWN)
            {
                if (EditingLabelNode != null)
                {
                    switch (keyData)
                    {
                        case Keys.OemQuestion:              // /
                        case Keys.OemQuestion | Keys.Shift: // ?
                        case Keys.D8 | Keys.Shift:          // *
                        case Keys.Oem5:                     // \
                        case Keys.Oem5 | Keys.Shift:        // |
                        case Keys.Oemcomma | Keys.Shift:    // <
                        case Keys.OemPeriod | Keys.Shift:   // >
                        case Keys.Oem7 | Keys.Shift:        // "
                        case Keys.Oem1 | Keys.Shift:        // :
                            IntPtr editHandle = NativeMethods.SendMessage(Handle, NativeConstants.TVM_GETEDITCONTROL, IntPtr.Zero, IntPtr.Zero);
                            string message = "A file name can't contain any of the following characters:\n\\ / : * ? \" < > |";
                            NativeMethods.GetWindowRect(editHandle, out RECT r);
                            Point clientPoint = PointToClient(new Point(r.left, r.bottom));
                            _labelToolTip.Show(message, this, clientPoint.X, clientPoint.Y);
                            return true;
                        default:
                            if (_labelToolTip.Active)
                                _labelToolTip.Hide(this);
                            break;
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (MappableActions.ContainsKey(e.KeyData))
            {
                e.SuppressKeyPress = false;
                var func = MappableActions[e.KeyData];
                e.Handled = func();
                if (e.Handled)
                {
                    Engine.PrintLine(e.KeyData.ToString() + ": " + func.Method.Name);
                }
                return;
            }
            base.OnKeyDown(e);
        }

        #region Shortcuts
        [Description("Deletes all nodes that are currently selected.")]
        public bool DeleteSelectedNodes()
        {
            if (SelectedNodes.Count > 0)
            {
                BeginUpdate();
                foreach (BaseWrapper b in new List<BaseWrapper>(SelectedNodes))
                    b.Delete();
                EndUpdate();
                return true;
            }
            return false;
        }
        [Description("Selects all nodes that are currently visible.")]
        public bool SelectAllVisibleNodes()
        {
            List<BaseWrapper> nodes = new List<BaseWrapper>();
            TreeNode node = Nodes[0];
            while (node != null)
            {
                nodes.Add(node as BaseWrapper);
                node = node.NextVisibleNode;
            }
            SelectedNodes = nodes;
            return true;
        }
        [Description("Copies the selected nodes to the clipboard.")]
        public bool CopySelectedNodes()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
                return false;
            SetClipboard(SelectedNodes.Select(x => x.FilePath).ToArray(), false);
            return true;
        }
        [Description("Removes the selected nodes and moves them to the clipboard.")]
        public bool CutSelectedNodes()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
                return false;
            SetClipboard(SelectedNodes.Select(x => x.FilePath).ToArray(), true);
            return true;
        }
        [Description("Pastes files/folders from the clipboard to the selected node.")]
        public bool Paste()
        {
            if (SelectedNode != null)
            {
                Paste(SelectedNode.FilePath);
                return true;
            }
            return false;
        }
        #endregion

        #endregion

        #region Copy Cut Paste
        public static void SetClipboard(string[] paths, bool cut)
        {
            if (paths == null || paths.Length == 0)
                return;

            DataObject data = new DataObject(DataFormats.FileDrop, paths);
            MemoryStream stream = new MemoryStream(4);
            byte[] bytes = new byte[] { (byte)(cut ? 2 : 5), 0, 0, 0 };
            stream.Write(bytes, 0, bytes.Length);
            data.SetData("Preferred DropEffect", stream);
            Clipboard.SetDataObject(data);
        }
        public void Paste(string destPath)
        {
            bool cut = false;
            string[] pastedPaths = null;

            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                pastedPaths = data.GetData(DataFormats.FileDrop) as string[];
                MemoryStream stream = data.GetData("Preferred DropEffect", true) as MemoryStream;
                int flag = stream.ReadByte();
                cut = flag == 2;
                if (!cut && flag != 5)
                    return;
            }
            if (pastedPaths == null || pastedPaths.Length == 0)
                return;
            
            bool? isDestDir = destPath.IsDirectoryPath();
            if (isDestDir.HasValue && !isDestDir.Value)
                destPath = Path.GetDirectoryName(destPath);
            if (!destPath.EndsWith("\\"))
                destPath += "\\";

            //WatchProjectDirectory = false;
            foreach (string pastedPath in pastedPaths)
            {
                bool? isDir = pastedPath.IsDirectoryPath();
                if (isDir == null)
                    continue;

                string name = Path.GetFileName(pastedPath);

                bool sameLocation = string.Equals(pastedPath, destPath + name, StringComparison.InvariantCulture);
                if (sameLocation)
                {
                    if (cut)
                        continue;

                    if (isDir.Value)
                    {
                        name += " - Copy";
                        var names = Directory.GetDirectories(destPath).Select(x => Path.GetFileNameWithoutExtension(x));
                        if (names.Contains(name))
                        {
                            int i = 2;
                            string name2;
                            while (names.Contains(name2 = string.Format("{0} ({1})", name, i.ToString()))) ++i;
                            name = name2;
                        }
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(pastedPath);
                        string ext = Path.GetExtension(pastedPath);
                        name = fileName + " - Copy" + ext;
                        var names = Directory.GetFiles(destPath).Select(x => Path.GetFileName(x));
                        if (names.Contains(name))
                        {
                            int i = 2;
                            string name2;
                            while (names.Contains(name2 = string.Format("{0} - Copy ({1}){2}", fileName, i.ToString(), ext))) ++i;
                            name = name2;
                        }
                    }
                }

                string tempDestPath = destPath + name;

                //bool caseChange = string.Equals(pastedPath, destPath, StringComparison.InvariantCultureIgnoreCase) && !sameLocation;

                if (isDir.Value)
                {
                    if (!Directory.Exists(tempDestPath))
                        Directory.CreateDirectory(tempDestPath);
                    if (cut)
                    {
                        FileSystem.MoveDirectory(pastedPath, tempDestPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                    }
                    else
                    {
                        FileSystem.CopyDirectory(pastedPath, tempDestPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                    }
                }
                else
                {
                    if (cut)
                    {
                        FileSystem.MoveFile(pastedPath, tempDestPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                    }
                    else
                    {
                        FileSystem.CopyFile(pastedPath, tempDestPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                    }
                    //FindOrCreatePath(destPath);
                }
            }
            //WatchProjectDirectory = true;

            //BaseWrapper node = FindOrCreatePath(destPath);
            //if (node != null && node.IsPopulated)
            //{

            //}
        }
        #endregion

        #region Content Watcher
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WatchProjectDirectory
        {
            get => _contentWatcher.EnableRaisingEvents;
            set => _contentWatcher.EnableRaisingEvents = value;
        }
        public BaseWrapper GetNode(string path)
        {
            TreeNode[] changedNodes = Nodes.Find(path, true);
            if (changedNodes.Length == 0)
                return null;
            if (changedNodes.Length > 1)
                Engine.PrintLine("More than one node with the path " + path);
            return changedNodes[0] as BaseWrapper;
        }
        private BaseWrapper FindOrCreatePath(string path)
        {
            BaseWrapper current = Nodes[0] as BaseWrapper;
            string projectFilePath = current.FilePath;
            string currentPath = projectFilePath;
            string relativePath = path.MakePathRelativeTo(projectFilePath);
            string[] pathHierarchy = relativePath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in pathHierarchy)
            {
                currentPath += Path.DirectorySeparatorChar + name;
                if (name == "..")
                {
                    if (current != null)
                        current = current.Parent;
                    else
                        throw new Exception("Not a valid path.");
                }
                else
                {
                    bool found = false;
                    if (!current.IsPopulated)
                    {
                        //The rest of the path will be populated when the user expands this node.
                        //Just end now instead of expanding and continuing
                        //current.Expand();
                        //bool? isDir = currentPath.IsDirectory();
                        if (current is FolderWrapper && current.Nodes.Count == 0 && Directory.GetFileSystemEntries(current.FilePath).Length > 0)
                            current.Nodes.Add("...");
                        break;
                    }

                    foreach (BaseWrapper searchNode in current.Nodes)
                        if (searchNode.Text == name)
                        {
                            current = searchNode;
                            found = true;
                            break;
                        }
                    if (found)
                        continue;

                    //Folder or file not found. Add it.
                    BaseWrapper n = BaseWrapper.Wrap(currentPath);
                    current.Nodes.Add(n);
                    current = n;
                }
            }
            return current;
        }
        private void ContentWatcherRename(object sender, RenamedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, RenamedEventArgs>(ContentWatcherRename), sender, e);
                return;
            }

            Engine.PrintLine("Renamed '{0}' to '{1}'", e.OldFullPath, e.FullPath);

            BaseWrapper node = GetNode(e.OldFullPath);
            if (node != null)
            {
                node.Text = Path.GetFileName(e.FullPath);
                node.FilePath = e.FullPath;
                if (node is FolderWrapper f && f.IsPopulated)
                    foreach (BaseWrapper b in f.Nodes)
                        b.FixPath(f.FilePath);
            }
        }
        private void ContentWatcherUpdate(object sender, FileSystemEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, FileSystemEventArgs>(ContentWatcherUpdate), sender, e);
                return;
            }

            Engine.PrintLine("{0} '{1}'", e.ChangeType.ToString(), e.FullPath);

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Renamed:
                    throw new Exception();
                case WatcherChangeTypes.Deleted:
                    GetNode(e.FullPath)?.Remove();
                    break;
                case WatcherChangeTypes.Created:
                    BaseWrapper bw = FindOrCreatePath(e.FullPath);
                    if (bw != null)
                    {
                        bw.EnsureVisible();
                        //SelectedNode = bw;
                    }
                    break;
                case WatcherChangeTypes.Changed:
                    
                    //The change of a file or folder. The types of changes include: 
                    //changes to size, attributes, security settings, last write, and last access time.
                    BaseWrapper changedNode = GetNode(e.FullPath);
                    if (changedNode is BaseFileWrapper b && b.IsLoaded)
                    {
                        if (b.AlwaysReload)
                            b.Reload();
                        else
                        {
                            string message = "The file " + e.FullPath + " has been externally modified.\nDo you want to reload it?";
                            if (b.SingleInstance.EditorState.HasChanges)
                                message += "\nYou will have the option to save your in-editor changes elsewhere if so.";
                            Form f = Parent.FindForm();
                            if (MessageBox.Show(f, message, "Externally modified file", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                            {
                                message = "Save your changes elsewhere before reloading?";
                                DialogResult d = MessageBox.Show(f, message, "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                if (d == DialogResult.Cancel)
                                    AddExternallyModifiedNode(b);
                                else
                                {
                                    if (d == DialogResult.Yes)
                                    {
                                        SaveFileDialog sfd = new SaveFileDialog()
                                        {
                                            Filter = b.SingleInstance.GetFilter()
                                        };
                                        if (sfd.ShowDialog(f) == DialogResult.OK)
                                        {
                                            b.SingleInstance.Export(sfd.FileName);
                                        }
                                    }
                                    b.Reload();
                                }
                            }
                            else
                                AddExternallyModifiedNode(b);
                        }
                    }
                    break;
            }
        }
        private void AddExternallyModifiedNode(BaseFileWrapper n)
        {
            n.ExternallyModified = true;
            _externallyModifiedNodes.Add(n.FullPath, n);
        }
        #endregion

        #region Label Edit
        protected override void OnRequestDisplayText(NodeRequestTextEventArgs e)
        {
            if (e.Node is BaseFileWrapper b)
                e.Label = e.Label + Path.GetExtension(b.FilePath);
        }
        protected override void OnRequestEditText(NodeRequestTextEventArgs e)
        {
            if (e.Node is BaseFileWrapper)
            {
                if (!string.IsNullOrEmpty(e.Node.Text))
                {
                    int i = e.Node.Text.LastIndexOf('.');
                    if (i >= 0)
                        e.Label = e.Node.Text.Substring(0, i);
                }
            }
        }
        protected override void OnValidateLabelEdit(NodeRequestTextEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                //e.CancelEdit = false;
                e.Label = e.Node.Text;

                //e.CancelEdit = true;
                //MessageBox.Show("Name cannot be empty.");
            }
            e.Label.Trim();
            //else if (e.Label.IndexOfAny(new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) >= 0)
            //{
            //    e.CancelEdit = true;
            //    MessageBox.Show("A file name can't contain any of the following characters:\n\\ / : * ? \" < > |");
            //}
        }
        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnAfterLabelEdit(e);

            if (e.Node is BaseWrapper b)
            {
                bool isFile = b is BaseFileWrapper;
                string dir = Path.GetDirectoryName(b.FilePath);
                string newName = b.Text;
                string newPath = dir + "\\" + b.Text;
                if (!string.Equals(b.FilePath, newPath, StringComparison.InvariantCulture))
                {
                    LabelEdit = false;
                    WatchProjectDirectory = false;
                    Sort();

                    try
                    {
                        //Windows is case-sensitive, but file/directory move isn't. Pretty dumb.
                        bool caseChange = string.Equals(b.FilePath, newPath, StringComparison.InvariantCultureIgnoreCase);
                        if (isFile)
                        {
                            if (caseChange)
                            {
                                string[] names = Directory.GetFiles(dir);
                                string name = "temp";
                                int i = 0;
                                while (names.Contains(name + i.ToString())) ++i;
                                string tempPath = dir + "\\" + name + i.ToString();
                                File.Move(b.FilePath, tempPath);
                                File.Move(tempPath, newPath);
                            }
                            else
                                File.Move(b.FilePath, newPath);
                        }
                        else
                        {
                            if (caseChange)
                            {
                                string[] names = Directory.GetDirectories(dir);
                                string name = "temp";
                                int i = 0;
                                while (names.Contains(name + i.ToString())) ++i;
                                string tempPath = dir + "\\" + name + i.ToString();
                                Directory.Move(b.FilePath, tempPath);
                                Directory.Move(tempPath, newPath);
                            }
                            else
                                Directory.Move(b.FilePath, newPath);
                        }
                        b.FilePath = newPath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    WatchProjectDirectory = true;
                }
            }

            if (EditingLabelNode == null && _labelToolTip.Active)
                _labelToolTip.Hide(this);
        }
        #endregion

        #region Dragging

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BaseWrapper[] DraggedNodes => _draggedNodes;

        private BaseWrapper _dropNode = null;
        private BaseWrapper[] _draggedNodes;
        private BaseWrapper _previousDropNode = null;
        private System.Windows.Forms.Timer _dragTimer = new System.Windows.Forms.Timer();
        private ImageList _draggingImageList = new ImageList();
        private DragDropFilter _dragFilter = null;
        
        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (SelectedNodes.Count == 0)
                return;
            
            _draggedNodes = new BaseWrapper[SelectedNodes.Count];
            SelectedNodes.CopyTo(_draggedNodes);
            SelectedNodes.Clear();

            string text;
            Bitmap bmp;
            if (_draggedNodes.Length == 1)
            {
                int w = (_draggedNodes[0].Bounds.Size.Width + Indent).Clamp(1, 256);
                int h = _draggedNodes[0].Bounds.Height.Clamp(1, 256);
                text = _draggedNodes[0].Text;
                _draggingImageList.ImageSize = new Size(w, h);
                bmp = new Bitmap(w, h);
            }
            else
            {
                text = _draggedNodes.Length.ToString() + " items";
                Size size = TextRenderer.MeasureText(text, Font);
                _draggingImageList.ImageSize = size;
                bmp = new Bitmap(size.Width, size.Height);
            }

            Graphics gfx = Graphics.FromImage(bmp);
            
            gfx.DrawString(text, Font, new SolidBrush(ForeColor), Indent, 0.0f);

            _draggingImageList.Images.Add(bmp);
            
            Point p = PointToClient(MousePosition);
            int dx = p.X + Indent - _draggedNodes[0].Bounds.Left;
            int dy = p.Y - _draggedNodes[0].Bounds.Top;

            if (DragHelper.ImageList_BeginDrag(_draggingImageList.Handle, 0, dx, dy))
            {
                //This is a synchronous operation, which freezes the whole simulation. Not good.
                //DoDragDrop(bmp, DragDropEffects.Move | DragDropEffects.Copy);

                //This filters events in the windows message loop, which is not a blocking operation.
                _dragFilter = new DragDropFilter(bmp, DragDropEffects.Move | DragDropEffects.Copy);
                _dragFilter.Done += _dragFilter_Done;
                _dragFilter.BeginFiltering();
            }
        }

        private void _dragFilter_Done(object sender, EventArgs e)
        {
            DragHelper.ImageList_EndDrag();
            _dragTimer.Enabled = false;
            _dropNode = null;
            _draggedNodes = null;
            _previousDropNode = null;
            _draggingImageList.Images.Clear();
        }

        private void TreeView1_DragOver(object sender, DragEventArgs e)
        {
            Point screenPoint = new Point(e.X, e.Y);
            Point clientPoint = PointToClient(screenPoint);
            DragHelper.ImageList_DragMove(clientPoint.X - Left, clientPoint.Y - Top);
            
            _dropNode = GetNodeAt(PointToClient(screenPoint)) as BaseWrapper;

            string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (_dropNode == null && (paths == null || paths.Length == 0))
            {
                _dropNode = null;
                SelectedNode = null;
                e.Effect = DragDropEffects.None;
                return;
            }

            //Can't drag a node into itself or a child node!
            TreeNode tmpNode = _dropNode;
            while (tmpNode != null)
            {
                if (_draggedNodes.Any(x => x == tmpNode))
                {
                    _dropNode = null;
                    SelectedNode = null;
                    e.Effect = DragDropEffects.None;
                    return;
                }
                tmpNode = tmpNode.Parent;
            }

            bool ctrl = (e.KeyState & (int)KeyStateFlags.Ctrl) != 0;
            e.Effect = ctrl ? DragDropEffects.Copy : DragDropEffects.Move;

            if (_previousDropNode != _dropNode)
            {
                DragHelper.ImageList_DragShowNolock(false);
                SelectedNode = _dropNode;
                DragHelper.ImageList_DragShowNolock(true);
                _previousDropNode = _dropNode;
            }
        }
        //private static bool CompareToType(Type compared, Type to)
        //{
        //    Type bType;
        //    if (compared == to)
        //        return true;
        //    else
        //    {
        //        bType = compared.BaseType;
        //        while (bType != null && bType != typeof(FileObject))
        //        {
        //            if (to == bType)
        //                return true;

        //            bType = bType.BaseType;
        //        }
        //    }
        //    return false;
        //}
        //private static bool CompareTypes(FileObject r1, FileObject r2)
        //{
        //    return CompareTypes(r1.GetType(), r2.GetType());
        //}
        //private static bool CompareTypes(Type type1, Type type2)
        //{
        //    Type bType1, bType2;
        //    if (type1 == type2)
        //        return true;
        //    else
        //    {
        //        bType2 = type2.BaseType;
        //        while (bType2 != null && bType2 != typeof(FileObject))
        //        {
        //            bType1 = type1.BaseType;
        //            while (bType1 != null && bType1 != typeof(FileObject))
        //            {
        //                if (bType1 == bType2)
        //                    return true;
        //                bType1 = bType1.BaseType;
        //            }
        //            bType2 = bType2.BaseType;
        //        }
        //    }
        //    return false;
        //}

        //private static bool TryDrop(FileObject dragging, FileObject dropping)
        //{
        //    //if (dropping.Parent == null)
        //    //    return false;

        //    bool good = false;
        //    //int destIndex = dropping.Index;

        //    //good = CompareTypes(dragging, dropping);
            
        //    //foreach (Type t in dropping.Parent.AllowedChildTypes)
        //    //    if (good = CompareToType(dragging.GetType(), t))
        //    //        break;

        //    //if (good)
        //    //{
        //    //    if (dragging.Parent != null)
        //    //        dragging.Parent.RemoveChild(dragging);
        //    //    if (destIndex < dropping.Parent.Children.Count)
        //    //        dropping.Parent.InsertChild(dragging, true, destIndex);
        //    //    else
        //    //        dropping.Parent.AddChild(dragging, true);

        //    //    dragging.OnMoved();
        //    //}

        //    return good;
        //}

        //private static bool TryAddChild(ResourceNode dragging, ResourceNode dropping)
        //{
        //    bool good = false;

        //    Type dt = dragging.GetType();
        //    if (dropping.Children.Count != 0)
        //        good = CompareTypes(dropping.Children[0].GetType(), dt);
        //    else
        //        foreach (Type t in dropping.AllowedChildTypes)
        //            if (good = CompareToType(dt, t))
        //                break;

        //    if (good)
        //    {
        //        if (dragging.Parent != null)
        //            dragging.Parent.RemoveChild(dragging);
        //        dropping.AddChild(dragging);

        //        dragging.OnMoved();
        //    }

        //    return good;
        //}

        private void TreeView1_DragDrop(object sender, DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            bool copy = (e.KeyState & (int)KeyStateFlags.Ctrl) != 0;
            if (_dropNode != null && e.Effect != DragDropEffects.None)
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                {
                    foreach (string path in paths)
                        _dropNode.HandlePathDrop(path, copy);
                }
                else
                {
                    BeginUpdate();
                    foreach (BaseWrapper draggedNode in _draggedNodes)
                        if (draggedNode != _dropNode)
                            _dropNode.HandleNodeDrop(draggedNode, copy);
                    EndUpdate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        private void TreeView1_DragEnter(object sender, DragEventArgs e)
        {
            DragHelper.ImageList_DragEnter(Handle, e.X - Left, e.Y - Top);
            _dragTimer.Enabled = true;
        }

        private void TreeView1_DragLeave(object sender, EventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            _dragTimer.Enabled = false;
        }

        private void TreeView1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            //if (e.Effect == DragDropEffects.Move || 
            //    e.Effect == DragDropEffects.Copy)
            //{
            //    e.UseDefaultCursors = false;
            //    Cursor = Cursors.Default;
            //}
            //else
            //    e.UseDefaultCursors = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Point pt = PointToClient(MousePosition);
            TreeNode node = GetNodeAt(pt);

            if (node == null) return;
            if (pt.Y < 30)
            {
                if (node.PrevVisibleNode != null)
                {
                    node = node.PrevVisibleNode;

                    DragHelper.ImageList_DragShowNolock(false);
                    node.EnsureVisible();
                    Refresh();
                    DragHelper.ImageList_DragShowNolock(true);
                }
            }
            else if (pt.Y > Size.Height - 30)
            {
                if (node.NextVisibleNode != null)
                {
                    node = node.NextVisibleNode;

                    DragHelper.ImageList_DragShowNolock(false);
                    node.EnsureVisible();
                    Refresh();
                    DragHelper.ImageList_DragShowNolock(true);
                }
            }
        }
        #endregion

        #region Multiselect
        protected List<BaseWrapper> _selectedNodes = new List<BaseWrapper>();
        protected BaseWrapper _lastNode, _firstNode;
        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public List<BaseWrapper> SelectedNodes
        {
            get => _selectedNodes;
            set
            {
                UnhighlightSelectedNodes();
                _selectedNodes.Clear();
                _selectedNodes = value;
                HighlightSelectedNodes();
                Refresh();
                SelectionChanged?.Invoke(this, null);
            }
        }
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);
            
            bool bControl = (ModifierKeys == Keys.Control && DraggedNodes == null);
            bool bShift = (ModifierKeys == Keys.Shift && DraggedNodes == null);
            BaseWrapper node = e.Node as BaseWrapper;

            // selecting twice the node while pressing CTRL ?
            if (bControl && _selectedNodes.Contains(node))
            {
                // unselect it (let framework know we don't want selection this time)
                e.Cancel = true;

                // update nodes
                UnhighlightNode(node);
                _selectedNodes.Remove(node);
                return;
            }

            _lastNode = node;

            if (!bShift)
                _firstNode = node; // store begin of shift sequence
        }
        private void OnAfterSelectMultiselect(TreeViewEventArgs e)
        {
            bool bControl = (ModifierKeys == Keys.Control && DraggedNodes == null);
            bool bShift = (ModifierKeys == Keys.Shift && DraggedNodes == null);
            BaseWrapper node = (BaseWrapper)e.Node;

            BeginUpdate();
            if (bControl)
            {
                if (!_selectedNodes.Contains(node)) // new node ?
                {
                    _selectedNodes.Add(node);
                }
                else  // not new, remove it from the collection
                {
                    UnhighlightSelectedNodes();
                    _selectedNodes.Remove(node);
                }
                HighlightSelectedNodes();
            }
            else
            {
                // SHIFT is pressed
                if (bShift)
                {
                    Queue<BaseWrapper> myQueue = new Queue<BaseWrapper>();

                    BaseWrapper uppernode = _firstNode;
                    BaseWrapper bottomnode = node;
                    // case 1 : begin and end nodes are parent
                    bool bParent = IsAncestor(_firstNode, e.Node); // is m_firstNode parent (direct or not) of e.Node
                    if (!bParent)
                    {
                        bParent = IsAncestor(bottomnode, uppernode);
                        if (bParent) // swap nodes
                        {
                            BaseWrapper t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }

                    //bParent may have been modifed under !bParent
                    if (bParent)
                    {
                        BaseWrapper n = bottomnode;
                        while (n != uppernode.Parent)
                        {
                            if (!_selectedNodes.Contains(n)) // new node ?
                                myQueue.Enqueue(n);

                            n = n.Parent;
                        }
                    }
                    // case 2 : nor the begin nor the end node are descendant one another
                    else
                    {
                        if ((uppernode.Parent == null && bottomnode.Parent == null) || (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                        {
                            int nIndexUpper = uppernode.Index;
                            int nIndexBottom = bottomnode.Index;
                            if (nIndexBottom < nIndexUpper) // reversed?
                            {
                                BaseWrapper t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            BaseWrapper n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!_selectedNodes.Contains(n)) // new node ?
                                    myQueue.Enqueue(n);

                                n = (BaseWrapper)n.NextNode;

                                nIndexUpper++;
                            } // end while

                        }
                        else
                        {
                            if (!_selectedNodes.Contains(uppernode)) myQueue.Enqueue(uppernode);
                            if (!_selectedNodes.Contains(bottomnode)) myQueue.Enqueue(bottomnode);
                        }
                    }

                    _selectedNodes.AddRange(myQueue);

                    HighlightSelectedNodes();
                    _firstNode = node; // let us chain several SHIFTs if we like it
                } // end if m_bShift
                else
                {
                    // in the case of a simple click, just add this item
                    if (_selectedNodes != null && _selectedNodes.Count > 0)
                    {
                        UnhighlightSelectedNodes();
                        _selectedNodes.Clear();
                    }
                    _selectedNodes.Add(node);
                    HighlightSelectedNodes();
                }
            }
            EndUpdate();
        }
        protected bool IsAncestor(TreeNode ancestorNode, TreeNode descendantNode)
        {
            if (ancestorNode == descendantNode)
                return true;

            TreeNode n = descendantNode;
            bool bFound = false;
            while (!bFound && n != null)
            {
                n = n.Parent;
                bFound = (n == ancestorNode);
            }
            return bFound;
        }
        public Color HighlightBackColor { get; set; } = SystemColors.Highlight;
        public Color HighlightTextColor { get; set; } = SystemColors.HighlightText;
        private void HighlightNode(TreeNode node)
        {
            //if (node != null)
            //{
            //    node.BackColor = HighlightBackColor;
            //    node.ForeColor = HighlightTextColor;
            //}
        }
        private void UnhighlightNode(TreeNode node)
        {
            //if (node != null)
            //{
            //    node.BackColor = BackColor;
            //    node.ForeColor = ForeColor;
            //}
        }
        protected void HighlightSelectedNodes()
            => _selectedNodes.ForEach(x => HighlightNode(x));
        protected void UnhighlightSelectedNodes()
            => _selectedNodes.ForEach(x => UnhighlightNode(x));
        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            TreeNodeStates state = e.State;
            Font font = e.Node.NodeFont ?? e.Node.TreeView.Font;
            Color fore = e.Node.ForeColor;
            if (fore == Color.Empty)
                fore = ForeColor;
            Color back = e.Node.BackColor;
            if (back == Color.Empty)
                back = BackColor;
            if (SelectedNodes.Contains(e.Node as BaseWrapper))
            {
                e.Graphics.FillRectangle(new SolidBrush(HighlightBackColor), e.Bounds);
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, HighlightTextColor, HighlightBackColor);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, HighlightTextColor, HighlightBackColor, TextFormatFlags.GlyphOverhangPadding);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(back), e.Bounds);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, TextFormatFlags.GlyphOverhangPadding);
            }
        }
        //private ContextMenuStrip GetMultiSelectMenuStrip()
        //{
        //    var nodes = SelectedNodes;
        //    var singleNode = SelectedNode as IMultiSelectableWrapper;
        //    if (singleNode == null) return null;

        //    foreach (var node in nodes)
        //    {
        //        var type = node.GetType();
        //        if (!type.IsAssignableFrom(singleNode.GetType()))
        //        {
        //            More than one type of node is selected
        //            return null;
        //        }
        //    }

        //    return singleNode.MultiSelectMenuStrip;
        //}
        #endregion
        
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            ((BaseWrapper)e.Node).OnExpand();
            base.OnBeforeExpand(e);
        }
        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            ((BaseWrapper)e.Node).OnCollapse();
            base.OnBeforeCollapse(e);
        }
        protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is BaseFileWrapper f)
                f.EditResource();
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x204)
            {
                int x = (int)m.LParam & 0xFFFF, y = (int)m.LParam >> 16;

                if (GetNodeAt(x, y) is BaseWrapper n)
                {
                    Rectangle r = n.Bounds;
                    r.X -= 25; r.Width += 25;
                    SelectedNode = r.Contains(x, y) ? n : null;
                }
                else
                    SelectedNode = null;

                m.Result = IntPtr.Zero;
                return;
            }
            else if (m.Msg == 0x205)
            {
                int x = (int)m.LParam & 0xFFFF, y = (int)m.LParam >> 16;

                BaseWrapper selected = SelectedNode;
                if (AllowContextMenus && selected != null && selected.ContextMenuStrip != null)
                {
                    Rectangle r = selected.Bounds;
                    r.X -= 25; r.Width += 25;
                    if (r.Contains(x, y))
                        selected.ContextMenuStrip.Show(this, x, y);
                }
            }
            base.WndProc(ref m);
        }

        public void Clear()
        {
            BeginUpdate();
            //foreach (BaseWrapper n in Nodes)
            //    n.Unlink();
            Nodes.Clear();
            EndUpdate();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            OnAfterSelectMultiselect(e);
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            base.Dispose(disposing);
        }
    }

    public class DragHelper
    {
        static DragHelper() => InitCommonControls();

        [DllImport("comctl32.dll")]
        public static extern bool InitCommonControls();

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_BeginDrag(IntPtr himlTrack, int iTrack, int dxHotspot, int dyHotspot);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragMove(int x, int y);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern void ImageList_EndDrag();

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragEnter(IntPtr hwndLock, int x, int y);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragLeave(IntPtr hwndLock);

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragShowNolock(bool fShow);
    }
}
