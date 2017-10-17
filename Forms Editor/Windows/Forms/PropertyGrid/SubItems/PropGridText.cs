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
        public PropGridText()
        {
            InitializeComponent();
        }
        protected override void OnPropertySet()
        {
            object value = GetPropertyValue();
            
            if (Property.PropertyType == typeof(string))
                textBox.Text = value as string;
            else
                textBox.Text = value?.ToString();
        }

        int _x = 0, _y = 0;
        private void verticalSplitter_SplitterMoving(object sender, SplitterEventArgs e)
        {
            Height += e.Y - _y;
        }

        private void verticalSplitter_MouseDown(object sender, MouseEventArgs e)
        {
            _y = e.Y;
        }

        private void horizontalSplitter_MouseDown(object sender, MouseEventArgs e)
        {
            _x = e.X;
        }

        private void horizontalSplitter_SplitterMoving(object sender, SplitterEventArgs e)
        {
            Width += e.X - _x;
        }
    }
}
