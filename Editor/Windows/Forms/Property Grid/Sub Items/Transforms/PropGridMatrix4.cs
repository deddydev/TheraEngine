using System;
using TheraEngine.Animation;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Matrix4))]
    public partial class PropGridMatrix4 : PropGridItem
    {
        public PropGridMatrix4()
        {
            InitializeComponent();
            _boxes = new[,]
            {
                { r0c0, r0c1, r0c2, r0c3 },
                { r1c0, r1c1, r1c2, r1c3 },
                { r2c0, r2c1, r2c2, r2c3 },
                { r3c0, r3c1, r3c2, r3c3 },
            };
            for (int r = 0; r < 4; ++r)
                for (int c = 0; c < 4; ++c)
                {
                    NumericInputBoxSingle box = _boxes[r, c];
                    box.ValueChanged += PropGridMatrix4_ValueChanged;
                    box.GotFocus += Box_GotFocus;
                    box.LostFocus += Box_LostFocus;
                }
        }

        private void Box_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void Box_GotFocus(object sender, EventArgs e) => IsEditing = true;

        private void PropGridMatrix4_ValueChanged(NumericInputBoxBase<float> box, float? previous, float? current)
            => UpdateValue(new Matrix4(
                r0c0.Value.Value, r0c1.Value.Value, r0c2.Value.Value, r0c3.Value.Value,
                r1c0.Value.Value, r1c1.Value.Value, r1c2.Value.Value, r1c3.Value.Value,
                r2c0.Value.Value, r2c1.Value.Value, r2c2.Value.Value, r2c3.Value.Value,
                r3c0.Value.Value, r3c1.Value.Value, r3c2.Value.Value, r3c3.Value.Value), false);

        private readonly NumericInputBoxSingle[,] _boxes;

        protected override bool UpdateDisplayInternal(object value)
        {
            bool editable = IsEditable();
            Matrix4 m = (Matrix4)value;
            for (int r = 0; r < 4; ++r)
                for (int c = 0; c < 4; ++c)
                {
                    NumericInputBoxSingle box = _boxes[r, c];
                    box.Value = m[r, c];
                    box.Enabled = editable;
                }

            return false;
        }

        public override bool CanAnimate => true;
        protected override BasePropAnim CreateAnimation() => new PropAnimMatrix4(0.0f, true, true);
    }
}
