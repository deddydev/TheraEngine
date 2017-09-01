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

namespace TheraEditor.Windows.Forms
{
    public enum SystemImages
    {
        GenericFile,
        ClosedFolder,
        OpenFolder,
        LockedFolder,
        Project,
        World,
        Map,
        Actor,
        SceneComponent,
        LogicComponent,
        Settings,
    }
    public class ResourceTree : TreeView
    {
        const int LEFT_MOUSE_BIT = 1;
        const int RIGHT_MOUSE_BIT = 2;
        const int SHIFT_BIT = 4;
        const int CTRL_BIT = 8;
        const int MIDDLE_MOUSE_BIT = 16;
        const int ALT_BIT = 32;

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

        public new BaseWrapper SelectedNode
        {
            get => base.SelectedNode as BaseWrapper;
            set
            {
                if (base.SelectedNode == value)
                    return;

                base.SelectedNode = value;
                SelectionChanged?.Invoke(this, null);
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
                    _imgList.Images.AddRange(new Image[]
                    {
                        Resources.GenericFile,      //0
                        Resources.ClosedFolder,     //1
                        Resources.OpenFolder,       //2
                        Resources.LockedFolder,     //3
                        Resources.ProjectFile,      //4
                    });
                }
                return _imgList;
            }
        }

        public ResourceTree()
        {
            SetStyle(ControlStyles.UserMouse, true);

            _dragTimer.Interval = 200;
            _dragTimer.Tick += new EventHandler(Timer_Tick);

            LabelEdit = true;
            AllowDrop = true;
            Sorted = true;
            TreeViewNodeSorter = new NodeComparer();

            ItemDrag += new ItemDragEventHandler(TreeView_ItemDrag);
            DragOver += new DragEventHandler(TreeView1_DragOver);
            DragDrop += new DragEventHandler(TreeView1_DragDrop);
            DragEnter += new DragEventHandler(TreeView1_DragEnter);
            DragLeave += new EventHandler(TreeView1_DragLeave);
            GiveFeedback += new GiveFeedbackEventHandler(TreeView1_GiveFeedback);
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

        public BaseWrapper CreateNode(string path)
        {
            return BaseWrapper.Wrap(path);
        }
        
        public void DisplayProject(Project p)
        {
            SuspendLayout();

            _contentWatcher = null;
            Nodes.Clear();
            ShowIcons = true;

            if (p == null || string.IsNullOrEmpty(p.FilePath))
            {
                ResumeLayout(true);
                return;
            }

            string dir = Path.GetDirectoryName(p.FilePath);

            BaseWrapper b = CreateNode(dir);
            if (b != null)
            {
                Nodes.Add(b);
                b.Expand();
            }

            ResumeLayout(true);

            _contentWatcher = new FileSystemWatcher(Path.GetDirectoryName(p.FilePath), "*.*")
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                    {
                        e.SuppressKeyPress = true;
                        e.Handled = CopySelectedNodes();
                        return;
                    }
                    break;
                case Keys.X:
                    if (e.Modifiers == Keys.Control)
                    {
                        e.SuppressKeyPress = true;
                        e.Handled = CutSelectedNodes();
                        return;
                    }
                    break;
                case Keys.V:
                    if (e.Modifiers == Keys.Control)
                    {
                        e.SuppressKeyPress = true;
                        e.Handled = Paste();
                        return;
                    }
                    break;
                case Keys.A:
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    if (e.Modifiers == Keys.Control)
                    {

                        return;
                    }
                    else
                    {

                        return;
                    }
            }

            base.OnKeyDown(e);
        }

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
        public bool CopySelectedNodes()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
                return false;
            SetClipboard(SelectedNodes.Select(x => x.FilePath).ToArray(), false);
            return true;
        }
        public bool CutSelectedNodes()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
                return false;
            SetClipboard(SelectedNodes.Select(x => x.FilePath).ToArray(), true);
            return true;
        }
        public bool Paste()
        {
            if (SelectedNode != null)
            {
                Paste(SelectedNode.FilePath);
                return true;
            }
            return false;
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
            
            bool? isDestDir = destPath.IsDirectory();
            if (isDestDir.HasValue && !isDestDir.Value)
                destPath = Path.GetDirectoryName(destPath);
            if (!destPath.EndsWith("\\"))
                destPath += "\\";

            WatchProjectDirectory = false;
            foreach (string pastedPath in pastedPaths)
            {
                bool? isDir = pastedPath.IsDirectory();
                if (isDir == null)
                    continue;

                string name = Path.GetFileName(pastedPath);
                destPath += name;
                if (isDir.Value)
                {
                    if (!Directory.Exists(destPath))
                        Directory.CreateDirectory(destPath);
                    if (cut)
                        FileSystem.MoveDirectory(pastedPath, destPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                    else
                        FileSystem.CopyDirectory(pastedPath, destPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                }
                else
                {
                    if (cut)
                        FileSystem.MoveFile(pastedPath, destPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                    else
                        FileSystem.CopyFile(pastedPath, destPath, UIOption.AllDialogs, UICancelOption.DoNothing);
                    FindOrCreatePath(destPath);
                }
            }
            WatchProjectDirectory = true;

            BaseWrapper node = FindOrCreatePath(destPath);
            if (node != null && node.IsPopulated)
            {

            }
        }
        #endregion

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

        #region Content Watcher
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WatchProjectDirectory
        {
            get => _contentWatcher.EnableRaisingEvents;
            set => _contentWatcher.EnableRaisingEvents = value;
        }
        private void ContentWatcherRename(object sender, RenamedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, RenamedEventArgs>(ContentWatcherRename), sender, e);
                return;
            }

            Engine.DebugPrint("Externally renamed '{0}' to '{1}'", -1, e.OldFullPath, e.FullPath);

            BaseWrapper node = GetNode(e.OldFullPath);
            node.Text = e.Name;
            node.FilePath = e.FullPath;
        }
        public BaseWrapper GetNode(string path)
        {
            TreeNode[] changedNodes = Nodes.Find(path, true);
            if (changedNodes.Length == 0)
                return null;
            if (changedNodes.Length > 1)
                Engine.DebugPrint("More than one node with the path " + path);
            return changedNodes[0] as BaseWrapper;
        }
        private BaseWrapper FindOrCreatePath(string path)
        {
            BaseWrapper current = Nodes[0] as BaseWrapper;
            string projectFilePath = current.FilePath;
            string currentPath = projectFilePath;
            string relativePath = path.MakePathRelativeTo(projectFilePath);
            string[] pathHierarchy = relativePath.Split('\\');
            foreach (string name in pathHierarchy)
            {
                currentPath += "\\" + name;
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
                    BaseWrapper n = CreateNode(currentPath);
                    current.Nodes.Add(n);
                    n.EnsureVisible();
                    current = n;
                }
            }
            return current;
        }
        private void ContentWatcherUpdate(object sender, FileSystemEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, FileSystemEventArgs>(ContentWatcherUpdate), sender, e);
                return;
            }

            Engine.DebugPrint("Externally {0} '{1}'", -1, e.ChangeType.ToString().ToLower(), e.FullPath);

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Renamed:
                    throw new Exception();
                case WatcherChangeTypes.Deleted:
                    GetNode(e.FullPath)?.Remove();
                    break;
                case WatcherChangeTypes.Changed:

                    //The change of a file or folder. The types of changes include: 
                    //changes to size, attributes, security settings, last write, and last access time.
                    BaseWrapper changedNode = GetNode(e.FullPath);
                    if (changedNode is BaseFileWrapper b)
                    {
                        if (b.IsLoaded)
                        {
                            if (b.AlwaysReload)
                                b.Reload();
                            else
                            {
                                string message = "The file " + e.FullPath + " has been externally modified.\nDo you want to reload it?";
                                if (b.FileObject.EditorState.HasChanges)
                                    message += "\nYou will have the option to save your in-editor changes elsewhere if so.";
                                Form f = Parent.FindForm();
                                if (MessageBox.Show(f, message, "Externally modified file", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
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
                                                Filter = b.FileObject.FileHeader.GetFilter()
                                            };
                                            if (sfd.ShowDialog(f) == DialogResult.OK)
                                            {
                                                b.FileObject.Export(sfd.FileName);
                                            }
                                        }
                                        b.Reload();
                                    }
                                }
                                else
                                    AddExternallyModifiedNode(b);
                            }
                        }
                    }
                    break;
                case WatcherChangeTypes.Created:
                    FindOrCreatePath(e.FullPath);
                    break;
            }
        }
        private void AddExternallyModifiedNode(BaseFileWrapper n)
        {
            n.ExternallyModified = true;
            _externallyModifiedNodes.Add(n.FullPath, n);
        }
        #endregion

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Node is BaseFileWrapper)
            {
                if (!string.IsNullOrEmpty(e.Node.Text))
                {
                    int i = e.Node.Text.LastIndexOf('.');
                    if (i >= 0)
                        e.Node.Text = e.Node.Text.Substring(0, i);
                }
            }
            base.OnBeforeLabelEdit(e);
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            string text = e.Label ?? e.Node?.Text;
            if (text != null)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    e.CancelEdit = true;
                    MessageBox.Show("Name cannot be empty.");
                    e.Node.BeginEdit();
                }
                else
                {
                    if (!e.Node.IsEditing)
                        return;
                    e.Node.EndEdit(false);
                    if (e.Node is BaseWrapper b)
                    {
                        bool isFile = b is BaseFileWrapper;

                        if (isFile)
                            b.Text = text + Path.GetExtension(b.FilePath);
                        else
                            b.Text = text;

                        Sort();

                        //Rename actual file/folder
                        //TODO: correct file name extension
                        WatchProjectDirectory = false;

                        string dir = Path.GetDirectoryName(b.FilePath);
                        string newName = b.Text;
                        string newPath = dir + "\\" + b.Text;

                        if (!string.Equals(b.FilePath, newPath, StringComparison.InvariantCulture))
                        {
                            if (isFile)
                                File.Move(b.FilePath, newPath);
                            else
                                Directory.Move(b.FilePath, newPath);
                            b.FilePath = newPath;
                        }

                        WatchProjectDirectory = true;
                    }
                }
            }
            base.OnAfterLabelEdit(e);
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            ((BaseWrapper)e.Node).OnExpand();
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
                    if (r.Contains(x, y))
                        SelectedNode = n;
                }

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
            //BeginUpdate();
            //foreach (BaseWrapper n in Nodes)
            //    n.Unlink();
            //Nodes.Clear();
            //EndUpdate();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            OnAfterSelectMultiselect(e);
        }

        //protected override void OnMouseDoubleClick(MouseEventArgs e)
        //{
        //    //if ((e.Button == MouseButtons.Left) && (SelectedNode is BaseWrapper))
        //    //    ((BaseWrapper)SelectedNode).OnDoubleClick();
        //    //else
        //        base.OnMouseDoubleClick(e);
        //}

        protected override void Dispose(bool disposing)
        {
            Clear();
            base.Dispose(disposing);
        }

        #region Dragging
        public BaseWrapper DragNode => _dragNode;
        private BaseWrapper _dragNode = null, _dropNode = null;
        private BaseWrapper[] _draggedNodes;
        private BaseWrapper _tempDropNode = null;
        private Timer _dragTimer = new Timer();
        private ImageList _draggingImageList = new ImageList();
        
        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _draggingImageList.Images.Clear();
            Bitmap bmp;
            if (SelectedNodes.Count > 1)
            {
                _draggedNodes = new BaseWrapper[SelectedNodes.Count];
                SelectedNodes.CopyTo(_draggedNodes);

                string count = _draggedNodes.Length.ToString() + " items";
                Size size = TextRenderer.MeasureText(count, Font);
                
                _draggingImageList.ImageSize = size;
                bmp = new Bitmap(size.Width, size.Height);
                Graphics gfx = Graphics.FromImage(bmp);
                
                gfx.DrawString(count, Font, new SolidBrush(ForeColor), 0.0f, 0.0f);

                _draggingImageList.Images.Add(bmp);
            }
            else
            {
                SelectedNode = _dragNode = e.Item as BaseWrapper;

                int w = (_dragNode.Bounds.Size.Width + Indent).ClampMin(1);
                int h = _dragNode.Bounds.Height.ClampMin(1);

                _draggingImageList.ImageSize = new Size(w, h);
                bmp = new Bitmap(w, h);

                Graphics gfx = Graphics.FromImage(bmp);

                gfx.DrawImage(Images.Images[SelectedNode.ImageIndex < 0 ? 0 : SelectedNode.ImageIndex], 0, 0);
                gfx.DrawString(_dragNode.Text, Font, new SolidBrush(ForeColor), Indent, 0.0f);

                _draggingImageList.Images.Add(bmp);
            }

            Point p = PointToClient(MousePosition);

            int dx = p.X + Indent - _dragNode.Bounds.Left;
            int dy = p.Y - _dragNode.Bounds.Top - 50;

            if (DragHelper.ImageList_BeginDrag(_draggingImageList.Handle, 0, dx, dy))
            {
                DoDragDrop(bmp, DragDropEffects.Move);
                DragHelper.ImageList_EndDrag();
            }
        }
        private void TreeView1_DragOver(object sender, DragEventArgs e)
        {
            Point formP = PointToClient(new Point(e.X, e.Y));
            DragHelper.ImageList_DragMove(formP.X - Left, formP.Y - Top);

            _dropNode = GetNodeAt(PointToClient(new Point(e.X, e.Y))) as BaseWrapper;

            string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (_dropNode == null && (paths == null || paths.Length == 0))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            //Can't drag parent node into child node!!!
            TreeNode tmpNode = _dropNode;
            if (tmpNode != null)
                while (tmpNode.Parent != null)
                {
                    if (tmpNode.Parent == _dragNode)
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }

                    tmpNode = tmpNode.Parent;
                }

            e.Effect = (e.KeyState & CTRL_BIT) == 0 ? DragDropEffects.Move : DragDropEffects.Copy;

            if (_tempDropNode != _dropNode)
            {
                DragHelper.ImageList_DragShowNolock(false);
                SelectedNode = _dropNode;
                DragHelper.ImageList_DragShowNolock(true);
                _tempDropNode = _dropNode;
            }
        }
        private static bool CompareToType(Type compared, Type to)
        {
            Type bType;
            if (compared == to)
                return true;
            else
            {
                bType = compared.BaseType;
                while (bType != null && bType != typeof(FileObject))
                {
                    if (to == bType)
                        return true;

                    bType = bType.BaseType;
                }
            }
            return false;
        }
        private static bool CompareTypes(FileObject r1, FileObject r2)
        {
            return CompareTypes(r1.GetType(), r2.GetType());
        }
        private static bool CompareTypes(Type type1, Type type2)
        {
            Type bType1, bType2;
            if (type1 == type2)
                return true;
            else
            {
                bType2 = type2.BaseType;
                while (bType2 != null && bType2 != typeof(FileObject))
                {
                    bType1 = type1.BaseType;
                    while (bType1 != null && bType1 != typeof(FileObject))
                    {
                        if (bType1 == bType2)
                            return true;
                        bType1 = bType1.BaseType;
                    }
                    bType2 = bType2.BaseType;
                }
            }
            return false;
        }

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
            /*
                1 (bit 0)
                The left mouse button.

                2 (bit 1)
                The right mouse button.

                4 (bit 2)
                The SHIFT key.

                8 (bit 3)
                The CTRL key.

                16 (bit 4)
                The middle mouse button.

                32 (bit 5)
                The ALT key.
            */

            bool copy = (e.KeyState & CTRL_BIT) != 0;
            if (_dropNode != null && e.Effect != DragDropEffects.None)
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                {
                    foreach (string path in paths)
                        _dropNode.HandlePathDrop(path, copy);
                }
                else
                {
                    if (_dragNode != _dropNode)
                        _dropNode.HandleNodeDrop(_dragNode, copy);
                }
            }

            _dragTimer.Enabled = false;
            _dropNode = null;
            _dragNode = null;
            _tempDropNode = null;
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
            if (e.Effect == DragDropEffects.Move)
            {
                e.UseDefaultCursors = false;
                Cursor = Cursors.Default;
            }
            else
                e.UseDefaultCursors = true;
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
        public List<BaseWrapper> SelectedNodes
        {
            get => _selectedNodes;
            set
            {
                UnhighlightSelectedNodes();
                _selectedNodes.Clear();
                _selectedNodes = value;
                HighlightSelectedNodes();
            }
        }
        
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);
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
            if (!bShift) _firstNode = node; // store begin of shift sequence
        }
        private void OnAfterSelectMultiselect(TreeViewEventArgs e)
        {
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);
            BaseWrapper node = (BaseWrapper)e.Node;

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
                }
            }
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
        private void HighlightNode(TreeNode node)
        {
            node.BackColor = SystemColors.Highlight;
            node.ForeColor = SystemColors.HighlightText;
        }
        private void UnhighlightNode(TreeNode node)
        {
            node.BackColor = BackColor;
            node.ForeColor = ForeColor;
        }
        protected void HighlightSelectedNodes()
            => _selectedNodes.ForEach(x => HighlightNode(x));
        protected void UnhighlightSelectedNodes()
            => _selectedNodes.ForEach(x => UnhighlightNode(x));
        #endregion
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
