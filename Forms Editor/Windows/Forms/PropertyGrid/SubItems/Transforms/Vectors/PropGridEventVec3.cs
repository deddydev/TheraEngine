using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(EventVec3))]
    public partial class PropGridEventVec3 : PropGridItem
    {
        public PropGridEventVec3()
        {
            InitializeComponent();
        }

        public EventVec3 _eventVec3;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            
            if (DataType == typeof(EventVec3))
            {
                _eventVec3 = value as EventVec3;
                numericInputBoxX.Value = _eventVec3?.X;
                numericInputBoxY.Value = _eventVec3?.Y;
                numericInputBoxZ.Value = _eventVec3?.Z;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not an EventVec3 type.");
        }

        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
                _eventVec3.X = current.Value;
        }

        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
                _eventVec3.Y = current.Value;
        }

        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
                _eventVec3.Z = current.Value;
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }
    }
}
