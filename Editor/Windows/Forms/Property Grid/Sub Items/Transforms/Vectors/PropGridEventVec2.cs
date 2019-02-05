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
        protected override bool UpdateDisplayInternal(object value)
        {
            _eventVec2 = value as EventVec2;
            bool notNull = _eventVec2 != null;
            bool editable = IsEditable();

            if (notNull)
            {
                numericInputBoxX.Value = _eventVec2.X;
                numericInputBoxY.Value = _eventVec2.Y;
            }
            else
            {
                numericInputBoxX.Value = null;
                numericInputBoxY.Value = null;
            }

            chkNull.Checked = !notNull;
            chkNull.Enabled = editable;
            numericInputBoxX.Enabled = numericInputBoxY.Enabled = editable && notNull;
            return false;
        }
        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec2 != null && !_updating)
            {
                _eventVec2.X = current.Value;
                OnValueChanged();
            }
        }
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec2 != null && !_updating)
            {
                _eventVec2.Y = current.Value;
                OnValueChanged();
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
    }
}
