using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(string))]
    public partial class PropGridText : PropGridItem
    {
        public PropGridText() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            Enabled = DataType == typeof(string);
            textBox1.Text = value?.ToString();
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
    }
}
