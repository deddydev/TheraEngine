using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(EventVec4))]
    public partial class PropGridEventVec4 : PropGridItem
    {
        public PropGridEventVec4()
        {
            InitializeComponent();

            numericInputBoxX.Tag = "X";
            numericInputBoxY.Tag = "Y";
            numericInputBoxZ.Tag = "Z";
            numericInputBoxW.Tag = "W";

            numericInputBoxX.GotFocus += InputGotFocus;
            numericInputBoxY.GotFocus += InputGotFocus;
            numericInputBoxZ.GotFocus += InputGotFocus;
            numericInputBoxW.GotFocus += InputGotFocus;

            numericInputBoxX.LostFocus += InputLostFocus;
            numericInputBoxY.LostFocus += InputLostFocus;
            numericInputBoxZ.LostFocus += InputLostFocus;
            numericInputBoxW.LostFocus += InputLostFocus;
        }
        protected override object ClassObject => _eventVec4;

        public EventVec4 _eventVec4;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            
            if (DataType == typeof(EventVec4))
            {
                _eventVec4 = value as EventVec4;
                numericInputBoxX.Value = _eventVec4?.X;
                numericInputBoxY.Value = _eventVec4?.Y;
                numericInputBoxZ.Value = _eventVec4?.Z;
                numericInputBoxW.Value = _eventVec4?.W;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not an EventVec4 type.");
        }

        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
                _eventVec4.X = current.Value;
        }
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
                _eventVec4.Y = current.Value;
        }
        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
                _eventVec4.Z = current.Value;
        }
        private void numericInputBoxW_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
                _eventVec4.W = current.Value;
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
