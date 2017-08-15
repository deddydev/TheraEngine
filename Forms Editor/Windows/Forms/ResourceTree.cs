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
        private delegate void DelegateOpenFile(string s, TreeNode t);
        private DelegateOpenFile _openFileDelegate;
        private FileSystemWatcher _contentWatcher;
        private bool _allowIcons = false;
        private TreeNode _selected;
        private Dictionary<string, BaseFileWrapper> _externallyModifiedNodes = new Dictionary<string, BaseFileWrapper>();

        public event EventHandler SelectionChanged;

        [DefaultValue(true)]
        public bool AllowContextMenus { get; set; } = true;

        [DefaultValue(false)]
        public bool ShowIcons
        {
            get => _allowIcons;
            set => ImageList = (_allowIcons = value) ? Images : null;
        }

        public new TreeNode SelectedNode
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
        
        public BaseWrapper CreateNode(string path)
        {
            return BaseWrapper.Wrap(path);
        }
        
        public void DisplayProject(Project p)
        {
            ShowIcons = true;
            AllowContextMenus = true;

            string dir = Path.GetDirectoryName(p.FilePath);

            BaseWrapper b = CreateNode(dir);
            Nodes.Add(b);
            b.Expand();

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

        #region Copy Cut Paste
        public static void CopyMultiple()
        {

        }
        public static void CutMultiple()
        {

        }
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
        public static void Paste(string filePath)
        {
            string copyPath = null;
            bool cut = false;
            string[] paths = null;

            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                paths = data.GetData(DataFormats.FileDrop) as string[];
                MemoryStream stream = data.GetData("Preferred DropEffect", true) as MemoryStream;
                int flag = stream.ReadByte();
                cut = flag == 2;
                if ((!cut && flag != 5) || paths == null || paths.Length == 0)
                    return;
            }

            string destFolder = filePath;
            bool? isDestDir = destFolder.IsDirectory();
            if (isDestDir.HasValue && !isDestDir.Value)
                destFolder = Path.GetDirectoryName(destFolder);

            foreach (string path in paths)
            {
                bool? isDir = copyPath.IsDirectory();
                if (isDir == null)
                    continue;

                if (isDir.Value)
                {
                    if (cut)
                        FileSystem.MoveDirectory(copyPath, destFolder, UIOption.AllDialogs, UICancelOption.DoNothing);
                    else
                        FileSystem.CopyDirectory(copyPath, destFolder, UIOption.AllDialogs, UICancelOption.DoNothing);
                }
                else
                {
                    if (cut)
                        FileSystem.MoveFile(copyPath, destFolder, UIOption.AllDialogs, UICancelOption.DoNothing);
                    else
                        FileSystem.CopyFile(copyPath, destFolder, UIOption.AllDialogs, UICancelOption.DoNothing);
                }
            }
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
        private void ContentWatcherRename(object sender, RenamedEventArgs e)
        {
            Engine.DebugPrint("File renamed: " + e.FullPath);

            TreeNode[] nodes = Nodes.Find(e.OldFullPath, true);
            if (nodes.Length == 0)
                return;
            if (nodes.Length > 1)
                Engine.DebugPrint("More than one node with the path " + e.OldFullPath);
            TreeNode node = nodes[0];
            node.Text = e.Name;
            node.Name = e.FullPath;
        }
        private void ContentWatcherUpdate(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    Engine.DebugPrint("File changed: " + e.FullPath);

                    //The change of a file or folder. The types of changes include: 
                    //changes to size, attributes, security settings, last write, and last access time.
                    TreeNode[] nodes = Nodes.Find(e.FullPath, true);
                    if (nodes.Length == 0)
                        return;
                    if (nodes.Length > 1)
                        Engine.DebugPrint("More than one node with the path " + e.FullPath);
                    if (nodes[0] is BaseFileWrapper b)
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
                case WatcherChangeTypes.Deleted:
                    Engine.DebugPrint("File deleted: " + e.FullPath);

                    TreeNode[] nodes2 = Nodes.Find(e.FullPath, true);
                    if (nodes2.Length == 0)
                        return;
                    if (nodes2.Length > 1)
                        Engine.DebugPrint("More than one node with the path " + e.FullPath);
                    TreeNode node = nodes2[0];

                    if (InvokeRequired)
                        Invoke(new Action(() => node.Remove()));
                    else
                        node.Remove();
                    break;
                case WatcherChangeTypes.Created:
                    Engine.DebugPrint("File created: " + e.FullPath);

                    string relativePath = e.FullPath.MakePathRelativeTo(((BaseWrapper)Nodes[0]).FilePath);
                    string[] nodeNames = relativePath.Split('\\');
                    BaseWrapper current = null;
                    foreach (string name in nodeNames)
                    {
                        if (name == "..")
                        {
                            if (current != null)
                                current = current.Parent;
                            else
                                return; //Not a valid path.
                        }
                        else
                        {
                            foreach (BaseWrapper b2 in Nodes)
                                if (b2.Text == name)
                                {
                                    current = b2;
                                    continue;
                                }
                        }
                    }

                    BaseWrapper newNode = CreateNode(e.FullPath);
                    if (InvokeRequired)
                        Invoke(new Action(() => Nodes.Add(newNode)));
                    else
                        Nodes.Add(newNode);
                    break;
                case WatcherChangeTypes.Renamed:
                    throw new Exception();
            }
            
        }
        private void AddExternallyModifiedNode(BaseFileWrapper n)
        {
            n.ExternallyModified = true;
            _externallyModifiedNodes.Add(n.FullPath, n);
        }
        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
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
                    WatchProjectDirectory = false;
                    string dir = Path.GetDirectoryName(e.Node.Name);
                    string newName = e.Label;
                    string newPath = dir + "\\" + e.Label;
                    if (e.Node is FolderWrapper)
                        Directory.Move(e.Node.Name, newPath);
                    else
                        File.Move(e.Node.Name, newPath);
                    e.Node.Name = newPath;
                    WatchProjectDirectory = true;
                }
            }
            base.OnAfterLabelEdit(e);
        }
        #endregion

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

                if (AllowContextMenus && _selected != null && _selected.ContextMenuStrip != null)
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
            OnAfterSelectMultiselect(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            //if ((e.Button == MouseButtons.Left) && (SelectedNode is BaseWrapper))
            //    ((BaseWrapper)SelectedNode).OnDoubleClick();
            //else
                base.OnMouseDoubleClick(e);
        }

        protected override void Dispose(bool disposing)
        {
            Clear();
            base.Dispose(disposing);
        }

        #region Dragging
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

            gfx.DrawImage(Images.Images[SelectedNode.ImageIndex < 0 ? 0 : SelectedNode.ImageIndex], 0, 0);
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
        #endregion

        #region Multiselect
        protected List<TreeNode> m_coll = new List<TreeNode>();
        protected TreeNode m_lastNode, m_firstNode;
        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
        public List<TreeNode> SelectedNodes
        {
            get => m_coll;
            set
            {
                UnhighlightSelectedNodes();
                m_coll.Clear();
                m_coll = value;
                HighlightSelectedNodes();
            }
        }
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            // selecting twice the node while pressing CTRL ?
            if (bControl && m_coll.Contains(e.Node))
            {
                // unselect it (let framework know we don't want selection this time)
                e.Cancel = true;

                // update nodes
                UnhighlightSelectedNodes();
                m_coll.Remove(e.Node);
                HighlightSelectedNodes();
                return;
            }

            m_lastNode = e.Node;
            if (!bShift) m_firstNode = e.Node; // store begin of shift sequence
        }
        private void OnAfterSelectMultiselect(TreeViewEventArgs e)
        {
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!m_coll.Contains(e.Node)) // new node ?
                {
                    m_coll.Add(e.Node);
                }
                else  // not new, remove it from the collection
                {
                    UnhighlightSelectedNodes();
                    m_coll.Remove(e.Node);
                }
                HighlightSelectedNodes();
            }
            else
            {
                // SHIFT is pressed
                if (bShift)
                {
                    Queue<TreeNode> myQueue = new Queue<TreeNode>();

                    TreeNode uppernode = m_firstNode;
                    TreeNode bottomnode = e.Node;
                    // case 1 : begin and end nodes are parent
                    bool bParent = IsAncestor(m_firstNode, e.Node); // is m_firstNode parent (direct or not) of e.Node
                    if (!bParent)
                    {
                        bParent = IsAncestor(bottomnode, uppernode);
                        if (bParent) // swap nodes
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }

                    //bParent may have been modifed under !bParent
                    if (bParent)
                    {
                        TreeNode n = bottomnode;
                        while (n != uppernode.Parent)
                        {
                            if (!m_coll.Contains(n)) // new node ?
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
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            TreeNode n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!m_coll.Contains(n)) // new node ?
                                    myQueue.Enqueue(n);

                                n = n.NextNode;

                                nIndexUpper++;
                            } // end while

                        }
                        else
                        {
                            if (!m_coll.Contains(uppernode)) myQueue.Enqueue(uppernode);
                            if (!m_coll.Contains(bottomnode)) myQueue.Enqueue(bottomnode);
                        }
                    }

                    m_coll.AddRange(myQueue);

                    HighlightSelectedNodes();
                    m_firstNode = e.Node; // let us chain several SHIFTs if we like it
                } // end if m_bShift
                else
                {
                    // in the case of a simple click, just add this item
                    if (m_coll != null && m_coll.Count > 0)
                    {
                        UnhighlightSelectedNodes();
                        m_coll.Clear();
                    }
                    m_coll.Add(e.Node);
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
            => m_coll.ForEach(x => HighlightNode(x));
        protected void UnhighlightSelectedNodes()
            => m_coll.ForEach(x => UnhighlightNode(x));
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
