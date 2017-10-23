using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(IList))]
    public partial class PropGridList : PropGridItem
    {
        public PropGridList() => InitializeComponent();
        private IList _list;
        protected override void UpdateDisplayInternal()
        {
            object value = GetPropertyValue();
            if (value is IList list)
            {
                btnAdd.Visible = !list.IsFixedSize;
                _list = list;
                for (int i = 0; i < _list.Count; ++i)
                {
                    object obj = _list[i];
                    
                }
            }
            else if (value is Exception ex)
            {
                
            }
            else
            {
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not an IList type.");
            }
        }

        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
            pnlElements.Visible = !pnlElements.Visible;
            Editor.Instance.PropertyGridForm.theraPropertyGrid1.pnlProps.ScrollControlIntoView(this);
        }

        private void pnlHeader_MouseLeave(object sender, EventArgs e)
        {
            pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        }

        private void pnlHeader_MouseEnter(object sender, EventArgs e)
        {
            pnlHeader.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void pnlElements_VisibleChanged(object sender, EventArgs e)
        {
            if (pnlElements.Visible)
            {
                //if (pnlElements.Controls.Count == 0)
                //    LoadProperties(_object);
            }
            else
            {
                LoadProperties(null);
            }
        }

        private void LoadProperties(object @object)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }
    }
}
