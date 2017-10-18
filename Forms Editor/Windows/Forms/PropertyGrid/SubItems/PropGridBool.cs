using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(bool))]
    public partial class PropGridBool : PropGridItem
    {
        public PropGridBool() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetPropertyValue();
            if (value is bool boolVal)
                checkBox1.Checked = boolVal;
            else
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not a boolean type.");
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
            => UpdatePropertyValue(checkBox1.Checked);
    }
}
