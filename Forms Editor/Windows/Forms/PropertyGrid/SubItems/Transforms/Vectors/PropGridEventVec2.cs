using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(EventVec2))]
    public partial class PropGridEventVec2 : PropGridItem
    {
        public PropGridEventVec2()
        {
            InitializeComponent();
            numericInputBoxX.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxY.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxX.LostFocus += NumericInputBoxX_LostFocus;
            numericInputBoxY.LostFocus += NumericInputBoxX_LostFocus;
        }
        private void NumericInputBoxX_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBoxX_GotFocus(object sender, EventArgs e) => IsEditing = true;

        public EventVec2 _eventVec2;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            if (DataType == typeof(EventVec2))
            {
                _eventVec2 = value as EventVec2;
                numericInputBoxX.Value = _eventVec2?.X;
                numericInputBoxY.Value = _eventVec2?.Y;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not an EventVec2 type.");
        }
        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec2 != null && !_updating)
                _eventVec2.X = current.Value;
        }
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec2 != null && !_updating)
                _eventVec2.Y = current.Value;
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(checkBox1.Checked ? null : Editor.UserCreateInstanceOf(DataType, true));
        }
    }
}
