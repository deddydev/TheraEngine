using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(bool))]
    public partial class PropGridBool : PropGridItem
    {
        public PropGridBool() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            if (value is bool boolVal)
                checkBox1.Checked = boolVal;
            else
                throw new Exception(DataType.GetFriendlyName() + " is not a boolean type.");
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
            => UpdateValue(checkBox1.Checked);
    }
}
