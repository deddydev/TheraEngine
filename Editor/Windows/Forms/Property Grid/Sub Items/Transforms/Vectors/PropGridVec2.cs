using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Vec2))]
    public partial class PropGridVec2 : PropGridItem
    {
        public PropGridVec2()
        {
            InitializeComponent();

            numericInputBoxX.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxY.GotFocus += NumericInputBoxX_GotFocus;

            numericInputBoxX.LostFocus += NumericInputBoxX_LostFocus;
            numericInputBoxY.LostFocus += NumericInputBoxX_LostFocus;
        }

        private void NumericInputBoxX_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBoxX_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override void UpdateDisplayInternal(object value)
        {
            Vec2 v = (Vec2)value;
            numericInputBoxX.Value = v.X;
            numericInputBoxY.Value = v.Y;

            numericInputBoxX.Enabled = numericInputBoxY.Enabled = IsEditable();
        }
        
        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec2(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value), false);
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec2(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value), false);

        public override bool CanAnimate => true;
        protected override BasePropAnim CreateAnimation() => new PropAnimVec2(0.0f, true, true);
    }
}
