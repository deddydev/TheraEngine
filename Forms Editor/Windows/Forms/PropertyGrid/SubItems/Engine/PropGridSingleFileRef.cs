using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Files;
using TheraEditor.Wrappers;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(IFileRef))]
    public partial class PropGridFileRef : PropGridItem
    {
        public PropGridFileRef() => InitializeComponent();
        
        public IFileRef _fileRef;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            
            if (typeof(IFileRef).IsAssignableFrom(ValueType))
            {
                _fileRef = value as IFileRef;
                label1.Text = _fileRef?.ReferencedType.GetFriendlyName();
                textBox1.Text = _fileRef?.FilePath;
            }
            else
                throw new Exception(ValueType.GetFriendlyName() + " is not an IFileRef type.");
        }

        private void PropGridFileRef_DragDrop(object sender, DragEventArgs e)
        {
            Type treeNodeType = typeof(BaseFileWrapper);
            if (e.Data.GetDataPresent(treeNodeType))
            {
                BaseFileWrapper node = (BaseFileWrapper)e.Data.GetData(treeNodeType);
                
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            btnBrowse.PerformClick();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

        }
    }
}
