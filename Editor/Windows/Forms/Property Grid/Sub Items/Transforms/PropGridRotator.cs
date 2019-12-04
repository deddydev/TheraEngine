using System;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Rotator))]
    public partial class PropGridRotator : PropGridItem
    {
        public PropGridRotator()
        {
            InitializeComponent();
            cboOrder.Items.AddRange(Enum.GetValues(typeof(ERotationOrder)).Cast<object>().ToArray());

            numericInputBoxPitch.Tag = nameof(Rotator.Pitch);
            numericInputBoxYaw.Tag = nameof(Rotator.Yaw);
            numericInputBoxRoll.Tag = nameof(Rotator.Roll);

            numericInputBoxPitch.GotFocus += InputGotFocus;
            numericInputBoxYaw.GotFocus += InputGotFocus;
            numericInputBoxRoll.GotFocus += InputGotFocus;

            numericInputBoxPitch.LostFocus += InputLostFocus;
            numericInputBoxYaw.LostFocus += InputLostFocus;
            numericInputBoxRoll.LostFocus += InputLostFocus;
        }
        protected override object RefObject => _rotator;

        public Rotator _rotator;
        protected override bool UpdateDisplayInternal(object value)
        {
            _rotator = value as Rotator;
            bool notNull = _rotator != null;
            bool editable = IsEditable();

            if (notNull)
            {
                numericInputBoxPitch.Value = _rotator.Pitch;
                numericInputBoxYaw.Value = _rotator.Yaw;
                numericInputBoxRoll.Value = _rotator.Roll;
                cboOrder.SelectedIndex = (int)_rotator.Order;
            }
            else
            {
                numericInputBoxPitch.Value = null;
                numericInputBoxYaw.Value = null;
                numericInputBoxRoll.Value = null;
                cboOrder.SelectedIndex = 0;
            }

            chkNull.Checked = !notNull;
            chkNull.Enabled = editable;
            cboOrder.Enabled = numericInputBoxPitch.Enabled = numericInputBoxYaw.Enabled = numericInputBoxRoll.Enabled = editable && notNull;
            return false;
        }

        private void numericInputBoxPitch_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_rotator is null || _updating)
                return;

            _rotator.Pitch = current.Value;
            OnValueChanged();
        }
        private void numericInputBoxYaw_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_rotator is null || _updating)
                return;

            _rotator.Yaw = current.Value;
            OnValueChanged();
        }
        private void numericInputBoxRoll_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_rotator is null || _updating)
                return;

            _rotator.Roll = current.Value;
            OnValueChanged();
        }
        private void cboOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rotator is null || _updating)
                return;

            SubmitPreManualStateChange(_rotator, nameof(Rotator.Order));
            _rotator.Order = (ERotationOrder)cboOrder.SelectedIndex;
            SubmitPostManualStateChange(_rotator, nameof(Rotator.Order));
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
    }
}
