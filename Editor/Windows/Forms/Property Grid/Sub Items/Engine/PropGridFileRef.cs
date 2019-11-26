using Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEditor.Wrappers;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(true, typeof(IFileRef))]
    public partial class PropGridFileRef : PropGridObject
    {
        public PropGridFileRef() : base()
        {
            AllowDrop = true;
        }
        
        protected override void OnDragDrop(DragEventArgs e)
        {
            DragHelper.ImageList_DragLeave(Handle);
            if (e.Effect != DragDropEffects.None)
            {
                bool copy = (e.KeyState & (int)EKeyStateFlags.Ctrl) != 0;
                IFileRef gridRef = _object as IFileRef;
                ContentTreeNode[] nodes = Editor.Instance.ContentTree.DraggedNodes;
                if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
                {
                    if (paths.Length > 0)
                    {
                        string path = paths[0];
                        if (path.IsExistingDirectoryPath() == false && !string.IsNullOrWhiteSpace(path))
                        {
                            gridRef.Path.Path = path;
                            gridRef.IsLoaded = false;
                            if (_wasNull)
                                UpdateValue(_object, false);
                            UpdateDisplay();
                            return;
                        }
                    }
                }
                else
                {
                    if (nodes.Length > 0)
                    {
                        ContentTreeNode node = nodes[0];
                        if (node != null &&
                            node is FileTreeNode file && 
                            file.Wrapper is IBaseFileWrapper pw &&
                            !string.IsNullOrWhiteSpace(node.FilePath))
                        {
                            if (pw.FileType is null || pw.FileType.IsAssignableFrom(gridRef.ReferencedType))
                            {
                                gridRef.Path.Path = file.FilePath;

                                //if (gridRef is IGlobalFileRef)
                                //    gridRef.IsLoaded = file.IsLoaded;
                                //else
                                //    gridRef.IsLoaded = false;

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
            if (_wasNull = _object is null)
            {
                _object = DataType.CreateInstance();
                AppDomainHelper.Sponsor(_object);
            }
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
