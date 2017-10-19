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

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(IByteColor))]
    public partial class PropGridByteColor : PropGridItem
    {
        public PropGridByteColor() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetPropertyValue();
            
            if (Property.PropertyType == typeof(ColorF4))
            {
                ColorF4 color = (ColorF4)value;
                colorControl1.Color = color.Color;
            }
            else
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not a ColorF4 type.");
        }

        private void PropGridFileRef_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

        }

        private void colorControl1_Load(object sender, EventArgs e)
        {

        }

        private void btnShowSelector_Click(object sender, EventArgs e)
        {
            if (colorControl1.Visible)
            {
                colorControl1.Visible = false;
                btnShowSelector.Text = "▼";
            }
            else
            {
                colorControl1.Color = ((IByteColor)GetPropertyValue()).Color;
                colorControl1.Visible = true;
                btnShowSelector.Text = "▲";
            }
        }
    }
}
