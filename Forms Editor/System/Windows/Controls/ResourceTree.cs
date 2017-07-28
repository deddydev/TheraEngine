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

namespace TheraEditor
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
        private delegate void DelegateOpenFile(string s, TreeNode t);
        private DelegateOpenFile _openFileDelegate;
        private FileSystemWatcher _contentWatcher;
        private Dictionary<string, BaseWrapper> _externallyModifiedNodes = new Dictionary<string, BaseWrapper>();

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
        public BaseWrapper CreateNode(string path)
        {
            return BaseWrapper.Wrap(path);
        }
        public void DisplayProject(Project p)
        {
            string dir = Path.GetDirectoryName(p.FilePath);
            
            Nodes.Add(CreateNode(dir));
            
            _contentWatcher = new FileSystemWatcher(Path.GetDirectoryName(p.FilePath), "*.*")
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite,
            };
            _contentWatcher.Changed += _contentWatcher_Changed;
            _contentWatcher.Created += _contentWatcher_Created;
            _contentWatcher.Deleted += _contentWatcher_Deleted;
            _contentWatcher.Renamed += _contentWatcher_Renamed;
        }
        private void _contentWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            TreeNode[] nodes = Nodes.Find(e.OldFullPath, true);
            if (nodes.Length == 0)
                return;
            if (nodes.Length > 1)
                Engine.DebugPrint("More than one node with the path " + e.OldFullPath);
            TreeNode node = nodes[0];
            node.Text = e.Name;
            node.Name = e.FullPath;
        }
        private void _contentWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            TreeNode[] nodes = Nodes.Find(e.FullPath, true);
            if (nodes.Length == 0)
                return;
            if (nodes.Length > 1)
                Engine.DebugPrint("More than one node with the path " + e.FullPath);
            TreeNode node = nodes[0];
            node.Remove();
        }
        private void _contentWatcher_Created(object sender, FileSystemEventArgs e)
        {
            string relativePath = e.FullPath.MakePathRelativeTo(((BaseWrapper)Nodes[0]).FilePath);
            string[] nodeNames = relativePath.Split('\\');
            BaseWrapper current = null;
            foreach (string name in nodeNames)
            {
                if (name == "..")
                {
                    if (current != null)
                        current = (BaseWrapper)current.Parent;
                    else
                        return; //Not a valid path.
                }
                else
                {
                    foreach (BaseWrapper b in Nodes)
                        if (b.Text == name)
                        {
                            current = b;
                            continue;
                        }
                }
            }
            Nodes.Add(CreateNode(e.FullPath));
        }
        private void _contentWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //The change of a file or folder. The types of changes include: 
            //changes to size, attributes, security settings, last write, and last access time.
            TreeNode[] nodes = Nodes.Find(e.FullPath, true);
            if (nodes.Length == 0)
                return;
            if (nodes.Length > 1)
                Engine.DebugPrint("More than one node with the path " + e.FullPath);
            BaseWrapper b = (BaseWrapper)nodes[0];
            if (b.IsLoaded)
            {
                if (b.AlwaysReload)
                    b.Reload();
                else
                {
                    string message = "The file " + e.FullPath + " has been externally modified.\nDo you want to reload it?";
                    if (b.Resource.EditorState.HasChanges)
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
                                    Filter = b.Resource.FileHeader.GetFilter()
                                };
                                if (sfd.ShowDialog(f) == DialogResult.OK)
                                {
                                    b.Resource.Export(sfd.FileName);
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
        private void AddExternallyModifiedNode(BaseWrapper n)
        {
            n.ExternallyModified = true;
            _externallyModifiedNodes.Add(n.FullPath, n);
        }
        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                MessageBox.Show("Name cannot be empty.");
                e.Node.BeginEdit();
            }
            else
            {
                e.Node.EndEdit(false);
            }
            
            base.OnAfterLabelEdit(e);
        }
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            ((BaseWrapper)e.Node).OnExpand();
        }

        public event EventHandler SelectionChanged;

        private bool _allowContextMenus = true;
        [DefaultValue(true)]
        public bool AllowContextMenus
        {
            get => _allowContextMenus;
            set => _allowContextMenus = value;
        }

        private bool _allowIcons = true;
        [DefaultValue(true)]
        public bool ShowIcons
        {
            get => _allowIcons;
            set => ImageList = (_allowIcons = value) ? Images : null;
        }

        private TreeNode _selected;
        new public TreeNode SelectedNode
        {
            get => base.SelectedNode;
            set
            {
                if (_selected == value)
                    return;

                _selected = base.SelectedNode = value;
                SelectionChanged?.Invoke(this, null);
            }
        }
        public ResourceTree()
        {
            SetStyle(ControlStyles.UserMouse, true);

            _timer.Interval = 200;
            _timer.Tick += new EventHandler(Timer_Tick);

            AllowDrop = true;

            ItemDrag += new ItemDragEventHandler(TreeView_ItemDrag);
            DragOver += new DragEventHandler(TreeView1_DragOver);
            DragDrop += new DragEventHandler(TreeView1_DragDrop);
            DragEnter += new DragEventHandler(TreeView1_DragEnter);
            DragLeave += new EventHandler(TreeView1_DragLeave);
            GiveFeedback += new GiveFeedbackEventHandler(TreeView1_GiveFeedback);

            _openFileDelegate = new DelegateOpenFile(ImportFile);
        }
        
        //private TreeNode CreateFolder(string path)
        //{
        //    TreeNode t = new TreeNode(Path.GetFileNameWithoutExtension(path));
        //    string parentFolder = Path.GetDirectoryName(path);
        //    if (_nodes.ContainsKey(parentFolder))
        //}
        //private TreeNode CreateFile(string path)
        //{
        //    TreeNode t = new TreeNode(Path.GetFileNameWithoutExtension(path));
        //}

        private void Populate()
        {
            //TreeNode rootNode;

            //DirectoryInfo info = new DirectoryInfo(_contentPath);
            //if (info.Exists)
            //{
            //    rootNode = BaseWrapper.Wrap(_contentPath);
            //    rootNode.Tag = info;
            //    GetDirectories(info.GetDirectories(), rootNode);
            //    Nodes.Add(rootNode);
            //}
        }
        
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x204)
            {
                int x = (int)m.LParam & 0xFFFF, y = (int)m.LParam >> 16;

                TreeNode n = GetNodeAt(x, y);
                if (n != null)
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

                if ((_allowContextMenus) && (_selected != null) && (_selected.ContextMenuStrip != null))
                {
                    Rectangle r = _selected.Bounds;
                    r.X -= 25; r.Width += 25;
                    if (r.Contains(x, y))
                        _selected.ContextMenuStrip.Show(this, x, y);
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
            SelectedNode = e.Node;
            base.OnAfterSelect(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            //if ((e.Button == MouseButtons.Left) && (SelectedNode is BaseWrapper))
            //    ((BaseWrapper)SelectedNode).OnDoubleClick();
            //else
                base.OnMouseDoubleClick(e);
        }

        protected override void Dispose(bool disposing) { Clear(); base.Dispose(disposing); }

        private TreeNode _dragNode = null;
        private TreeNode _tempDropNode = null;
        private Timer _timer = new Timer();

        private ImageList imageListDrag = new ImageList();

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _dragNode = (TreeNode)e.Item;
            SelectedNode = _dragNode;

            imageListDrag.Images.Clear();
            imageListDrag.ImageSize = new Size(
                (_dragNode.Bounds.Size.Width + Indent + 7).Clamp(1, 256),
                _dragNode.Bounds.Height.Clamp(1, 256));

            Bitmap bmp = new Bitmap(_dragNode.Bounds.Width + Indent + 7, _dragNode.Bounds.Height);

            Graphics gfx = Graphics.FromImage(bmp);

            gfx.DrawImage(Images.Images[SelectedNode.ImageIndex], 0, 0);
            gfx.DrawString(_dragNode.Text, Font, new SolidBrush(ForeColor), Indent + 7.0f, 4.0f);

            imageListDrag.Images.Add(bmp);

            Point p = PointToClient(MousePosition);

            int dx = p.X + Indent - _dragNode.Bounds.Left;
            int dy = p.Y - _dragNode.Bounds.Top - 25;

            if (DragHelper.ImageList_BeginDrag(imageListDrag.Handle, 0, dx, dy))
            {
                DoDragDrop(bmp, DragDropEffects.Move);
                DragHelper.ImageList_EndDrag();
            }
        }

        /// <summary>
        /// Imports an external file to be a sibling or child of the given tree node.
        /// </summary>
        /// <param name="path">The absolute path to the file to import</param>
        /// <param name="dropNode">The node this file was dropped on</param>
        private void ImportFile(string path, TreeNode dropNode)
        {
            if (dropNode != null)
            {
                if (!(dropNode is FolderWrapper))
                    dropNode = dropNode.Parent;
                //dropNode.Nodes.Add(BaseWrapper.Wrap(path));
            }

            _timer.Enabled = false;
            _dragNode = null;
        }
        private void TreeView1_DragOver(object sender, DragEventArgs e)
        {
            Array a = (Array)e.Data.GetData(DataFormats.FileDrop);

            Point formP = PointToClient(new Point(e.X, e.Y));
            DragHelper.ImageList_DragMove(formP.X - Left, formP.Y - Top);

            TreeNode dropNode = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
            if (dropNode == null && a == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Move;

            if (_tempDropNode != dropNode)
            {
                DragHelper.ImageList_DragShowNolock(false);
                SelectedNode = dropNode;
                DragHelper.ImageList_DragShowNolock(true);
                _tempDropNode = dropNode;
            }

            TreeNode tmpNode = dropNode;
            if (tmpNode != null)
                while (tmpNode.Parent != null)
                {
                    if (tmpNode.Parent == _dragNode)
                        e.Effect = DragDropEffects.None;

                    tmpNode = tmpNode.Parent;
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

        private static bool TryDrop(FileObject dragging, FileObject dropping)
        {
            //if (dropping.Parent == null)
            //    return false;

            bool good = false;
            //int destIndex = dropping.Index;

            //good = CompareTypes(dragging, dropping);
            
            //foreach (Type t in dropping.Parent.AllowedChildTypes)
            //    if (good = CompareToType(dragging.GetType(), t))
            //        break;

            //if (good)
            //{
            //    if (dragging.Parent != null)
            //        dragging.Parent.RemoveChild(dragging);
            //    if (destIndex < dropping.Parent.Children.Count)
            //        dropping.Parent.InsertChild(dragging, true, destIndex);
            //    else
            //        dropping.Parent.AddChild(dragging, true);

            //    dragging.OnMoved();
            //}

            return good;
        }

        //private static bool TryAddChild(ResourceNode dragging, ResourceNode dropping)
        //{
        //    bool good = false;

        //    //Type dt = dragging.GetType();
        //    //if (dropping.Children.Count != 0)
        //    //    good = CompareTypes(dropping.Children[0].GetType(), dt);
        //    //else
        //    //    foreach (Type t in dropping.AllowedChildTypes)
        //    //        if (good = CompareToType(dt, t))
        //    //            break;

        //    //if (good)
        //    //{
        //    //    if (dragging.Parent != null)
        //    //        dragging.Parent.RemoveChild(dragging);
        //    //    dropping.AddChild(dragging);

        //    //    dragging.OnMoved();
        //    //}

        //    return good;
        //}

        private void TreeView1_DragDrop(object sender, DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            TreeNode dropNode = GetNodeAt(PointToClient(new Point(e.X, e.Y)));

            Array externalData = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (externalData != null)
            {
                string path = null;
                for (int i = 0; i < externalData.Length; i++)
                {
                    path = externalData.GetValue(i).ToString();
                    BeginInvoke(_openFileDelegate, path, dropNode);
                }
            }
            else
            {
                //if (_dragNode != dropNode)
                //{
                //    BaseWrapper drag = ((BaseWrapper)_dragNode);
                //    BaseWrapper drop = ((BaseWrapper)dropNode);
                //    FileObject dragging = drag.Resource;
                //    FileObject dropping = drop.Resource;

                //    if (dropping.Parent == null)
                //        goto End;

                //    bool ok = false;
                //    if (ModifierKeys == Keys.Shift)
                //        ok = TryAddChild(dragging, dropping);
                //    else
                //        ok = TryDrop(dragging, dropping);

                //    if (ok)
                //    {
                //        BaseWrapper b = FindResource(dragging);
                //        if (b != null)
                //        {
                //            b.EnsureVisible();
                //            SelectedNode = b;
                //        }
                //    }

                //    End:
                //    _dragNode = null;
                //    _timer.Enabled = false;
                //}
            }
        }

        private void TreeView1_DragEnter(object sender, DragEventArgs e)
        {
            DragHelper.ImageList_DragEnter(Handle, e.X - Left, e.Y - Top);
            _timer.Enabled = true;
        }

        private void TreeView1_DragLeave(object sender, EventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            _timer.Enabled = false;
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
    }

    public class DragHelper
    {
        [DllImport("comctl32.dll")]
        public static extern bool InitCommonControls();

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_BeginDrag(IntPtr himlTrack, int
            iTrack, int dxHotspot, int dyHotspot);

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

        static DragHelper()
        {
            InitCommonControls();
        }
    }
}
