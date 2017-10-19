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
    [PropGridItem(typeof(ColorF4), typeof(ColorF3))]
    public partial class PropGridFloatColor : PropGridItem
    {
        public PropGridFloatColor() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetPropertyValue();
            
            if (Property.PropertyType == typeof(ColorF3))
            {
                ColorF3 color = (ColorF3)value;
                panel1.BackColor = color.Color;
                colorControl1.Color = color.Color;
            }
            else if (Property.PropertyType == typeof(ColorF4))
            {
                ColorF4 color = (ColorF4)value;
                panel1.BackColor = color.Color;
                colorControl1.Color = color.Color;
            }
            else
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not ColorF3 or ColorF4.");
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

        private void colorControl1_Closed(object sender, EventArgs e)
        {
            if (colorControl1.DialogResult == DialogResult.OK)
            {
                if (Property.PropertyType == typeof(ColorF3))
                {
                    UpdatePropertyValue((ColorF3)colorControl1.Color);
                }
                else if (Property.PropertyType == typeof(ColorF4))
                {
                    UpdatePropertyValue((ColorF4)colorControl1.Color);
                }
            }
            colorControl1.Visible = false;
            btnShowSelector.Text = "▼";
        }

        private void colorControl1_ColorChanged(Color c)
        {

        }

        private void colorControl1_OnColorChanged(Color selection)
        {

        }
    }
}
