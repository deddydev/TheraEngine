using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(EventVec4))]
    public partial class PropGridEventVec4 : PropGridItem
    {
        public PropGridEventVec4()
        {
            InitializeComponent();
        }

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
            if (_eventVec4 != null)
                _eventVec4.X = current.Value;
        }
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null)
                _eventVec4.Y = current.Value;
        }
        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null)
                _eventVec4.Z = current.Value;
        }
        private void numericInputBoxW_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec4 != null)
                _eventVec4.W = current.Value;
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }
    }
}
