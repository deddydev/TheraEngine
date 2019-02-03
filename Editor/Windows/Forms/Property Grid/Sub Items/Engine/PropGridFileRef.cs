using System;
using System.Windows.Forms;
using TheraEngine.Core.Files;
using TheraEditor.Wrappers;
using System.Drawing;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(true, typeof(IFileRef))]
    public partial class PropGridFileRef : PropGridObject
    {
        public PropGridFileRef() : base()
        {
            //InitializeComponent();
            AllowDrop = true;
        }
        
        protected override void UpdateDisplayInternal(object value)
        {
            base.UpdateDisplayInternal(value);
            //if (typeof(IFileRef).IsAssignableFrom(DataType))
            //{
            //    //_fileRef = _object as IFileRef;
            //    //label1.Text = _fileRef?.ReferencedType?.GetFriendlyName();
            //    //textBox1.Text = _fileRef?.ReferencePath;
            //}
            //else
            //    throw new Exception(DataType.GetFriendlyName() + " is not an IFileRef type.");
        }
        protected override void OnDragDrop(DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            bool copy = (e.KeyState & (int)KeyStateFlags.Ctrl) != 0;
            if (e.Effect != DragDropEffects.None)
            {
                IFileRef r = _object as IFileRef;
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                {
                    if (paths.Length > 0)
                    {
                        string path = paths[0];
                        if (path.IsExistingDirectoryPath() == false && !string.IsNullOrWhiteSpace(path))
                        {
                            r.Path.Path = path;
                            r.IsLoaded = false;
                            if (_wasNull)
                                UpdateValue(_object, false);
                            UpdateDisplay();
                            return;
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
                            !string.IsNullOrWhiteSpace(node.FilePath))
                        {
                            if (file.FileType == null || (file.FileType != null && file.FileType.IsAssignableFrom(r.ReferencedType)))
                            {
                                r.Path.Path = file.FilePath;
                                if (r is IGlobalFileRef)
                                    r.IsLoaded = file.IsLoaded;
                                else
                                    r.IsLoaded = false;
                                if (_wasNull)
                                    UpdateValue(_object, false);
                                UpdateDisplay();
                                return;
                            }
                        }
                    }
                }
            }
            if (_wasNull)
                _object = null;
            UpdateDisplay();
        }

        bool _wasVisible, _wasNull;
        protected override void OnDragEnter(DragEventArgs e)
        {
            _wasVisible = pnlProps.Visible;
            if (_wasNull = _object == null)
                _object = Activator.CreateInstance(DataType);
            if (!pnlProps.Visible)
                pnlProps.Visible = true;
            UpdateDisplay();
            DragHelper.ImageList_DragEnter(Handle, e.X - Left, e.Y - Top);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            pnlProps.Visible = _wasVisible;
            if (_wasNull)
                _object = null;
            UpdateDisplay();
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
