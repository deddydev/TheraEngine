using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(EventVec4))]
    public partial class PropGridEventVec4 : PropGridItem
    {
        public PropGridEventVec4()
        {
            InitializeComponent();

            numericInputBoxX.Tag = nameof(EventVec4.X);
            numericInputBoxY.Tag = nameof(EventVec4.Y);
            numericInputBoxZ.Tag = nameof(EventVec4.Z);
            numericInputBoxW.Tag = nameof(EventVec4.W);

            numericInputBoxX.GotFocus += InputGotFocus;
            numericInputBoxY.GotFocus += InputGotFocus;
            numericInputBoxZ.GotFocus += InputGotFocus;
            numericInputBoxW.GotFocus += InputGotFocus;

            numericInputBoxX.LostFocus += InputLostFocus;
            numericInputBoxY.LostFocus += InputLostFocus;
            numericInputBoxZ.LostFocus += InputLostFocus;
            numericInputBoxW.LostFocus += InputLostFocus;
        }
        protected override object RefObject => _eventVec4;

        public EventVec4 _eventVec4;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();

            _eventVec4 = value as EventVec4;
            bool notNull = _eventVec4 != null;
            bool editable = IsEditable();

            if (notNull)
            {
                numericInputBoxX.Value = _eventVec4.X;
                numericInputBoxY.Value = _eventVec4.Y;
                numericInputBoxZ.Value = _eventVec4.Z;
                numericInputBoxW.Value = _eventVec4.W;
            }
            else
            {
                numericInputBoxX.Value = null;
                numericInputBoxY.Value = null;
                numericInputBoxZ.Value = null;
                numericInputBoxW.Value = null;
            }

            chkNull.Checked = !notNull;
            chkNull.Enabled = editable;
            numericInputBoxX.Enabled = numericInputBoxY.Enabled = numericInputBoxZ.Enabled = numericInputBoxW.Enabled = editable && notNull;
        }

        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
            {
                _eventVec4.X = current.Value;
                OnValueChanged();
            }
        }
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
            {
                _eventVec4.Y = current.Value;
                OnValueChanged();
            }
        }
        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
            {
                _eventVec4.Z = current.Value;
                OnValueChanged();
            }
        }
        private void numericInputBoxW_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null && !_updating)
            {
                _eventVec4.W = current.Value;
                OnValueChanged();
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true), true);
        }
    }
}
