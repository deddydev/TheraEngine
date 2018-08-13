using System;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(UInt32))]
    public partial class PropGridUInt32 : PropGridItem
    {
        public PropGridUInt32()
        {
            InitializeComponent();
            numericInputBox1.GotFocus += NumericInputBox1_GotFocus;
            numericInputBox1.LostFocus += NumericInputBox1_LostFocus;
        }
        private void NumericInputBox1_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBox1_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            numericInputBox1.Value = (UInt32)value;
        }

        protected override void OnLabelSet()
        {
            Label.MouseDown += LabelMouseDown;
            Label.MouseUp += LabelMouseUp;
        }

        private int _y = 0;
        private void LabelMouseDown(object sender, MouseEventArgs e)
        {
            if (numericInputBox1.Value != null)
            {
                _y = e.Location.Y;
                Label.MouseMove += LabelMouseMove;
            }
        }
        private void LabelMouseUp(object sender, MouseEventArgs e)
        {
            Label.MouseMove -= LabelMouseMove;
        }
        private void LabelMouseMove(object sender, MouseEventArgs e)
        {
            int diff = (e.Location.Y - _y);
            if (diff == 0)
                return;
            if (diff < 0)
                numericInputBox1.Value += numericInputBox1.SmallIncrement;
            else
                numericInputBox1.Value -= numericInputBox1.SmallIncrement;
            _y = e.Location.Y;
        }

        private void numericInputBox1_ValueChanged(NumericInputBoxBase<UInt32> box, UInt32? previous, UInt32? current)
        {
            UpdateValue(current.Value, false);
        }
    }
}
