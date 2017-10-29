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
    [PropGridItem(typeof(EventVec2))]
    public partial class PropGridEventVec2 : PropGridItem
    {
        public PropGridEventVec2() => InitializeComponent();
        
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
            if (_eventVec2 != null)
                _eventVec2.X = current.Value;
        }
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec2 != null)
                _eventVec2.Y = current.Value;
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }
    }
}
