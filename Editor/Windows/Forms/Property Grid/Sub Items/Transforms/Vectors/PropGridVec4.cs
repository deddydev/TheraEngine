using System;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Vec4))]
    public partial class PropGridVec4 : PropGridItem
    {
        public PropGridVec4()
        {
            InitializeComponent();

            numericInputBoxX.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxY.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxZ.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxW.GotFocus += NumericInputBoxX_GotFocus;

            numericInputBoxX.LostFocus += NumericInputBoxX_LostFocus;
            numericInputBoxY.LostFocus += NumericInputBoxX_LostFocus;
            numericInputBoxZ.LostFocus += NumericInputBoxX_LostFocus;
            numericInputBoxW.LostFocus += NumericInputBoxX_LostFocus;
        }

        private void NumericInputBoxX_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBoxX_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override void UpdateDisplayInternal(object value)
        {
            Vec4 v = (Vec4)value;
            numericInputBoxX.Value = v.X;
            numericInputBoxY.Value = v.Y;
            numericInputBoxZ.Value = v.Z;
            numericInputBoxW.Value = v.W;

            numericInputBoxX.Enabled = numericInputBoxY.Enabled = numericInputBoxZ.Enabled = numericInputBoxW.Enabled = IsEditable();
        }
        
        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value), false);
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value), false);
        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value), false);
        private void numericInputBoxW_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value), false);

        public override bool CanAnimate => true;
        protected override BasePropAnim CreateAnimation() => new PropAnimVec4(0.0f, true, true);
    }
}
