using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(string))]
    public partial class PropGridText : PropGridItem
    {
        public PropGridText()
        {
            InitializeComponent();
            textBox1.GotFocus += TextBox1_GotFocus;
            textBox1.LostFocus += TextBox1_LostFocus;
        }
        
        private void TextBox1_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void TextBox1_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            chkNull.Checked = value == null;
            if (Property != null)
            {
                object[] attribs = Property.GetCustomAttributes(true);
                StringAttribute s = attribs.FirstOrDefault(x => x is StringAttribute) as StringAttribute;
                if (s != null)
                {
                    if (s.MultiLine)
                    {

                    }
                    if (s.Path)
                    {

                    }
                    if (s.Unicode)
                    {

                    }
                }
            }
            SetControlsEnabled(DataType == typeof(string) && !chkNull.Checked);
            textBox1.Text = value?.ToString() ?? "";
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            UpdateValue(textBox1.Text);
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            if (textBox1.ReadOnly = !enabled)
            {
                textBox1.BackColor = Color.FromArgb(94, 94, 114);
                textBox1.ForeColor = Color.FromArgb(180, 180, 200);
            }
            else
            {
                textBox1.BackColor = Color.FromArgb(94, 94, 114);
                textBox1.ForeColor = Color.FromArgb(200, 200, 220);
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            if (chkNull.Checked)
            {
                UpdateValue(null);
                SetControlsEnabled(false);
            }
            else
            {
                UpdateValue("");
                SetControlsEnabled(true);
            }
        }
    }
}
