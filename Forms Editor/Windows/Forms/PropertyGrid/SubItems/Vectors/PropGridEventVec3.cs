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
    [PropGridItem(typeof(EventVec3))]
    public partial class PropGridEventVec3 : PropGridItem
    {
        public PropGridEventVec3()
        {
            InitializeComponent();
        }

        public EventVec3 _eventVec3;
        protected override void UpdateDisplayInternal()
        {
            object value = GetPropertyValue();
            
            if (Property.PropertyType == typeof(EventVec3))
            {
                _eventVec3 = value as EventVec3;
                numericInputBoxX.Value = _eventVec3?.X;
                numericInputBoxY.Value = _eventVec3?.Y;
                numericInputBoxZ.Value = _eventVec3?.Z;
            }
            else
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not an EventVec3 type.");
        }

        private void numericInputBoxX_ValueChanged(Single? previous, Single? current)
        {
            if (_eventVec3 != null)
                _eventVec3.X = current.Value;
        }

        private void numericInputBoxY_ValueChanged(Single? previous, Single? current)
        {
            if (_eventVec3 != null)
                _eventVec3.Y = current.Value;
        }

        private void numericInputBoxZ_ValueChanged(Single? previous, Single? current)
        {
            if (_eventVec3 != null)
                _eventVec3.Z = current.Value;
        }
    }
}
