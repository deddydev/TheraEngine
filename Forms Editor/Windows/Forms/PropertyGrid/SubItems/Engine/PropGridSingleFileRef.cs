using System;
using System.Windows.Forms;
using TheraEngine.Files;
using TheraEditor.Wrappers;

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

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
        }

        //public IFileRef _fileRef;
        //protected override void UpdateDisplayInternal()
        //{
        //    object value = GetValue();

        //    if (typeof(IFileRef).IsAssignableFrom(DataType))
        //    {
        //        _fileRef = value as IFileRef;
        //        label1.Text = _fileRef?.ReferencedType.GetFriendlyName();
        //        textBox1.Text = _fileRef?.ReferencePath;
        //    }
        //    else
        //        throw new Exception(DataType.GetFriendlyName() + " is not an IFileRef type.");
        //}

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
