﻿using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(bool))]
    public partial class PropGridBool : PropGridItem
    {
        public PropGridBool() => InitializeComponent();
        protected override void UpdateDisplayInternal(object value)
        {
            chkValue.Checked = (bool)value;
            chkValue.Enabled = IsEditable();
        }
        private void chkValue_CheckedChanged(object sender, EventArgs e)
            => UpdateValue(chkValue.Checked, true);
    }
}
