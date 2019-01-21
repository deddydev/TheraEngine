using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Vec3))]
    public partial class PropGridVec3 : PropGridItem
    {
        public PropGridVec3()
        {
            InitializeComponent();

            numericInputBoxX.GotFocus += NumericInputBox_GotFocus;
            numericInputBoxY.GotFocus += NumericInputBox_GotFocus;
            numericInputBoxZ.GotFocus += NumericInputBox_GotFocus;

            numericInputBoxX.LostFocus += NumericInputBox_LostFocus;
            numericInputBoxY.LostFocus += NumericInputBox_LostFocus;
            numericInputBoxZ.LostFocus += NumericInputBox_LostFocus;
        }

        private void NumericInputBox_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBox_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override void UpdateDisplayInternal(object value)
        {
            Vec3 v = (Vec3)value;
            numericInputBoxX.Value = v.X;
            numericInputBoxY.Value = v.Y;
            numericInputBoxZ.Value = v.Z;

            numericInputBoxX.Enabled = numericInputBoxY.Enabled = numericInputBoxZ.Enabled = IsEditable();
        }
        
        private void numericInputBox_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec3(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value), false);
    }
}
