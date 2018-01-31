using System;
using System.Windows.Forms;
using TheraEngine.Files;
using TheraEditor.Wrappers;
using System.Drawing;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IFileRef))]
    public partial class PropGridFileRef : PropGridObject
    {
        public PropGridFileRef() : base()
        {
            //InitializeComponent();
            AllowDrop = true;
        }
        
        protected override void UpdateDisplayInternal()
        {
            base.UpdateDisplayInternal();
            if (typeof(IFileRef).IsAssignableFrom(DataType))
            {
                //_fileRef = _object as IFileRef;
                //label1.Text = _fileRef?.ReferencedType?.GetFriendlyName();
                //textBox1.Text = _fileRef?.ReferencePath;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not an IFileRef type.");
        }
        protected override void OnDragDrop(DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            if (_object == null)
            {
                return;
            }
            bool copy = (e.KeyState & (int)KeyStateFlags.Ctrl) != 0;
            if (e.Effect != DragDropEffects.None)
            {
                IFileRef r = _object as IFileRef;
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                {
                    if (paths.Length > 0)
                    {
                        string path = paths[0];
                        if (path.IsDirectoryPath() == false && !string.IsNullOrWhiteSpace(path))
                        {
                            r.ReferencePath = path;
                            r.IsLoaded = false;
                        }
                    }
                }
                else
                {
                    BaseWrapper[] nodes = Editor.Instance.ContentTree.DraggedNodes;
                    if (nodes.Length > 0)
                    {
                        BaseWrapper node = nodes[0];
                        if (node != null &&
                            node is BaseFileWrapper file && 
                            !string.IsNullOrWhiteSpace(node.FilePath) && 
                            file.FileType.IsAssignableFrom(r.ReferencedType))
                        {
                            r.ReferencePath = file.FilePath;
                            if (r is IGlobalFileRef)
                                r.IsLoaded = file.IsLoaded;
                            else
                                r.IsLoaded = false;
                        }
                    }
                }
            }
        }

        bool _wasVisible;
        protected override void OnDragEnter(DragEventArgs e)
        {
            _wasVisible = pnlProps.Visible;
            if (_object != null && !pnlProps.Visible)
                pnlProps.Visible = true;
        }

        protected override void OnDragLeave(EventArgs e)
        {
            pnlProps.Visible = _wasVisible;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            Point screenPoint = new Point(e.X, e.Y);
            Point clientPoint = PointToClient(screenPoint);
            DragHelper.ImageList_DragMove(clientPoint.X - Left, clientPoint.Y - Top);
        }

        //private void PropGridFileRef_DragDrop(object sender, DragEventArgs e)
        //{
        //    Type treeNodeType = typeof(BaseFileWrapper);
        //    if (e.Data.GetDataPresent(treeNodeType))
        //    {
        //        BaseFileWrapper node = (BaseFileWrapper)e.Data.GetData(treeNodeType);
        //        if (_fileRef.ReferencedType.IsAssignableFrom(node.FileType))
        //        {

        //        }
        //    }
        //}

        //private void textBox1_Enter(object sender, EventArgs e)
        //{
        //    btnBrowse.PerformClick();
        //}

        //private void btnBrowse_Click(object sender, EventArgs e)
        //{

        //}
    }
}
