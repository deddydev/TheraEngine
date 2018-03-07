using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(EventVec2))]
    public partial class PropGridEventVec2 : PropGridItem
    {
        public PropGridEventVec2()
        {
            InitializeComponent();

            numericInputBoxX.Tag = nameof(EventVec2.X);
            numericInputBoxY.Tag = nameof(EventVec2.Y);

            numericInputBoxX.GotFocus += InputGotFocus;
            numericInputBoxY.GotFocus += InputGotFocus;

            numericInputBoxX.LostFocus += InputLostFocus;
            numericInputBoxY.LostFocus += InputLostFocus;
        }
        protected override object RefObject => _eventVec2;

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

        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(checkBox1.Checked ? null : Editor.UserCreateInstanceOf(DataType, true), true);
        }
    }
}
