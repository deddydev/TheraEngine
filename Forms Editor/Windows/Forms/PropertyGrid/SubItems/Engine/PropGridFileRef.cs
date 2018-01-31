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

        public IFileRef _fileRef;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();

            if (typeof(IFileRef).IsAssignableFrom(DataType))
            {
                _fileRef = value as IFileRef;
                label1.Text = _fileRef?.ReferencedType.GetFriendlyName();
                textBox1.Text = _fileRef?.ReferencePath;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not an IFileRef type.");
        }
        protected override void OnDragDrop(DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            bool copy = (e.KeyState & (int)KeyStateFlags.Ctrl) != 0;
            if (e.Effect != DragDropEffects.None)
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                {
                    if (paths.Length > 0)
                    {
                        string path = paths[0];
                    }
                }
                else
                {
                    BaseWrapper[] nodes = Editor.Instance.ContentTree.DraggedNodes;
                    if (nodes.Length > 0)
                    {
                        BaseWrapper node = nodes[0];
                    }
                }
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
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
