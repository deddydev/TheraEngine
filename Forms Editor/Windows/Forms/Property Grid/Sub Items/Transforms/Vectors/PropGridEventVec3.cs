using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(EventVec3))]
    public partial class PropGridEventVec3 : PropGridItem
    {
        public PropGridEventVec3()
        {
            InitializeComponent();

            numericInputBoxX.Tag = nameof(EventVec3.X);
            numericInputBoxY.Tag = nameof(EventVec3.Y);
            numericInputBoxZ.Tag = nameof(EventVec3.Z);

            numericInputBoxX.GotFocus += InputGotFocus;
            numericInputBoxY.GotFocus += InputGotFocus;
            numericInputBoxZ.GotFocus += InputGotFocus;

            numericInputBoxX.LostFocus += InputLostFocus;
            numericInputBoxY.LostFocus += InputLostFocus;
            numericInputBoxZ.LostFocus += InputLostFocus;
        }
        protected override object RefObject => _eventVec3;

        public EventVec3 _eventVec3;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();

            _eventVec3 = value as EventVec3;
            bool notNull = _eventVec3 != null;
            bool editable = IsEditable();

            if (notNull)
            {
                numericInputBoxX.Value = _eventVec3.X;
                numericInputBoxY.Value = _eventVec3.Y;
                numericInputBoxZ.Value = _eventVec3.Z;
            }
            else
            {
                numericInputBoxX.Value = null;
                numericInputBoxY.Value = null;
                numericInputBoxZ.Value = null;
            }

            chkNull.Checked = !notNull;
            chkNull.Enabled = editable;
            numericInputBoxX.Enabled = numericInputBoxY.Enabled = numericInputBoxZ.Enabled = editable && notNull;
        }

        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
            {
                _eventVec3.X = current.Value;
                OnValueChanged();
            }
        }

        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
            {
                _eventVec3.Y = current.Value;
                OnValueChanged();
            }
        }

        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
            {
                _eventVec3.Z = current.Value;
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
