using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using TheraEngine.Files;
using TheraEditor.Properties;
using TheraEditor.Wrappers;
using System.IO;
using TheraEngine;
using System.Collections.Generic;
using Microsoft.Build.Construction;

namespace TheraEditor
{
    public enum SystemImages
    {
        GenericFile,
        ClosedFolder,
        OpenFolder,
    }
    public class ResourceTree : TreeView
    {
        private delegate void DelegateOpenFile(string s, TreeNode t);
        private DelegateOpenFile _openFileDelegate;
        private Dictionary<string, TreeNode> _nodes;
        private FileSystemWatcher _contentWatcher;
        
        public const string ConfigFolder = "Config";
        public const string SourceFolder = "Source";
        public const string AssetsFolder = "Assets";
        public const string IntermediateFolder = "Intermediate";

        public static readonly string[] ReservedRootFolders =
        {
            ConfigFolder,
            SourceFolder,
            AssetsFolder,
            IntermediateFolder,
        };

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
                        Resources.GenericFile,
                        Resources.ClosedFolder,
                        Resources.OpenFolder,
                    });
                }
                return _imgList;
            }
        }

        public void AddNode(TreeNode node)
        {
            Nodes.Add(node);
        }
        public void RemoveNode(TreeNode node)
        {
            Nodes.Remove(node);
        }
        public void RemoveNodeAt(int index)
        {
            Nodes.RemoveAt(index);
        }
        public void AddNode(TreeNode node, TreeNode parent)
        {

        }
        public void RemoveNode(TreeNode node, TreeNode parent)
        {

        }

        public void DisplayProject(Project p)
        {
            _contentWatcher = null;

            _nodes = new Dictionary<string, TreeNode>();
            string path = Path.GetDirectoryName(p.FilePath);

            Nodes.Clear();
            var stack = new Stack<TreeNode>();
            var rootDirectory = new DirectoryInfo(p.FilePath);
            var node = new TreeNode(p.Name) { Tag = p };
            stack.Push(node);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                var directoryInfo = (DirectoryInfo)currentNode.Tag;
                foreach (var dirInfo in directoryInfo.GetDirectories())
                {
                    var childDirectoryNode = new TreeNode(dirInfo.Name) { Tag = dirInfo };
                    _nodes[dirInfo.FullName] = childDirectoryNode;
                    currentNode.Nodes.Add(childDirectoryNode);
                    stack.Push(childDirectoryNode);
                }
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    var treeNode = new TreeNode(fileInfo.Name) { Tag = fileInfo };
                    _nodes[fileInfo.FullName] = treeNode;
                    currentNode.Nodes.Add(treeNode);
                }
            }

            Nodes.Add(node);

            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                _contentWatcher = new FileSystemWatcher(path, "*.*")
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
        }
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);
            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Nodes[0].Text == "..." && e.Node.Nodes[0].Tag == null)
                {
                    e.Node.Nodes.Clear();

                    //get the list of sub direcotires
                    string[] dirs = Directory.GetDirectories(e.Node.Tag.ToString());

                    foreach (string dir in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode node = new TreeNode(di.Name, 0, 1);
                        try
                        {
                            //keep the directory's full path in the tag for use later
                            node.Tag = dir;

                            //if the directory has sub directories add the place holder
                            if (di.GetDirectories().Length > 0)
                                node.Nodes.Add(null, "...", 0, 0);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            //display a locked folder icon
                            node.ImageIndex = 12;
                            node.SelectedImageIndex = 12;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "DirectoryLister",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            e.Node.Nodes.Add(node);
                        }
                    }
                }
            }
        }

        public event EventHandler SelectionChanged;

        private bool _allowContextMenus = true;
        [DefaultValue(true)]
        public bool AllowContextMenus
        {
            get => _allowContextMenus;
            set => _allowContextMenus = value;
        }

        private bool _allowIcons = false;
        [DefaultValue(false)]
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
        private void _contentWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!_nodes.ContainsKey(e.OldFullPath))
                return;
            TreeNode node = _nodes[e.OldFullPath];
            node.Name = e.Name;
            _nodes.Remove(e.OldFullPath);
            _nodes.Add(e.FullPath, node);
        }
        private void _contentWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!_nodes.ContainsKey(e.FullPath))
                return;
            _nodes[e.FullPath].Remove();
            _nodes.Remove(e.FullPath);
        }
        private void _contentWatcher_Created(object sender, FileSystemEventArgs e)
        {
            TreeNode node = new TreeNode();
            _nodes.Add(e.FullPath, node);

            string dir, name;
            FileAttributes attr = File.GetAttributes(e.FullPath);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                dir = e.FullPath;
            }
            else
            {
                dir = Path.GetDirectoryName(e.FullPath);
            }
            name = Path.GetFileNameWithoutExtension(e.FullPath);
            
            //TreeNode folder;
            //if (!_nodes.ContainsKey(dir))
            //    folder = CreateFolder(dir);
            //else
            //    folder = _nodes[dir];

            //folder.Nodes.Add(new TreeNode());
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
        private void _contentWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!_nodes.ContainsKey(e.FullPath))
            {
                return;
            }
        }
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

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0)
                {
                    Tag = subDir,
                    ImageKey = "folder"
                };
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
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
            BeginUpdate();
            foreach (BaseWrapper n in Nodes)
                n.Unlink();
            Nodes.Clear();
            EndUpdate();
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
