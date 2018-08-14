﻿using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(bool))]
    public partial class PropGridBool : PropGridItem
    {
        public PropGridBool() => InitializeComponent();
        protected override void UpdateDisplayInternal(object value)
        {
            if (value is bool boolVal)
            {
                checkBox1.Checked = boolVal;
                checkBox1.Enabled = !ParentInfo.IsReadOnly();
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not a boolean type.");
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
            => UpdateValue(checkBox1.Checked, true);
    }
}
