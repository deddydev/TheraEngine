using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Rotator))]
    public partial class PropGridRotator : PropGridItem
    {
        public PropGridRotator()
        {
            InitializeComponent();
            cboOrder.DataSource = Enum.GetValues(typeof(RotationOrder));

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
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            
            if (DataType == typeof(Rotator))
            {
                _rotator = value as Rotator;
                numericInputBoxPitch.Value = _rotator?.Pitch;
                numericInputBoxYaw.Value = _rotator?.Yaw;
                numericInputBoxRoll.Value = _rotator?.Roll;
                cboOrder.SelectedIndex = (int)_rotator.Order;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not a Rotator type.");
        }

        private void numericInputBoxPitch_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_rotator != null && !_updating)
            {
                _rotator.Pitch = current.Value;
                OnValueChanged();
            }
        }
        private void numericInputBoxYaw_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_rotator != null && !_updating)
            {
                _rotator.Yaw = current.Value;
                OnValueChanged();
            }
        }
        private void numericInputBoxRoll_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_rotator != null && !_updating)
            {
                _rotator.Roll = current.Value;
                OnValueChanged();
            }
        }
        private void cboOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_rotator != null && !_updating)
            {
                SubmitPreManualStateChange(_rotator, "Order");
                _rotator.Order = (RotationOrder)cboOrder.SelectedIndex;
                SubmitPostManualStateChange(_rotator, "Order");
            }
        }

        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(checkBox1.Checked ? null : Editor.UserCreateInstanceOf(DataType, true), true);
        }
    }
}
