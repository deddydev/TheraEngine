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
    [PropGridItem(typeof(string), typeof(object))]
    public partial class PropGridText : PropGridItem
    {
        public PropGridText() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetPropertyValue();
            Enabled = Property.PropertyType == typeof(string);
            textBox1.Text = value?.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            UpdatePropertyValue(textBox1.Text);
        }
    }
}
