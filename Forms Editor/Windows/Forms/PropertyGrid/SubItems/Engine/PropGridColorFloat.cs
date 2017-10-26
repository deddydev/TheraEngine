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
        
        object _previousColor;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            
            if (ValueType == typeof(ColorF3))
            {
                ColorF3 color = (ColorF3)value;
                panel1.BackColor = color.Color;
                colorControl1.EditAlpha = false;
                colorControl1.Color = color.Color;
            }
            else if (ValueType == typeof(ColorF4))
            {
                ColorF4 color = (ColorF4)value;
                panel1.BackColor = color.Color;
                colorControl1.EditAlpha = true;
                colorControl1.Color = color.Color;
            }
            else
                throw new Exception(ValueType.GetFriendlyName() + " is not ColorF3 or ColorF4.");
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
                _previousColor = GetValue();
                colorControl1.Color = ((IByteColor)_previousColor).Color;
                colorControl1.Visible = true;
                btnShowSelector.Text = "▲";
            }
        }

        private void colorControl1_Closed(object sender, EventArgs e)
        {
            if (colorControl1.DialogResult == DialogResult.OK)
            {
                if (ValueType == typeof(ColorF3))
                {
                    ColorF3 color = (ColorF3)colorControl1.Color;
                    panel1.BackColor = color.Color;
                    UpdateValue(color);
                }
                else if (ValueType == typeof(ColorF4))
                {
                    ColorF4 color = colorControl1.Color;
                    panel1.BackColor = colorControl1.Color;
                    UpdateValue(color);
                }
            }
            else
            {
                if (ValueType == typeof(ColorF3))
                {
                    ColorF3 color = (ColorF3)_previousColor;
                    panel1.BackColor = color.Color;
                    UpdateValue(color);
                }
                else if (ValueType == typeof(ColorF4))
                {
                    ColorF4 color = (ColorF4)_previousColor;
                    panel1.BackColor = colorControl1.Color;
                    UpdateValue(color);
                }
            }
            colorControl1.Visible = false;
            btnShowSelector.Text = "▼";
        }
        
        private void colorControl1_OnColorChanged(Color newColor)
        {
            if (ValueType == typeof(ColorF3))
            {
                ColorF3 color = (ColorF3)newColor;
                panel1.BackColor = color.Color;
                UpdateValue(color);
            }
            else if (ValueType == typeof(ColorF4))
            {
                ColorF4 color = newColor;
                panel1.BackColor = newColor;
                UpdateValue(color);
            }
        }
    }
}
