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
    [PropGridItem(typeof(Vec3), typeof(EventVec3))]
    public partial class PropGridVec3 : PropGridItem
    {
        public PropGridVec3()
        {
            InitializeComponent();
        }

        public EventVec3 _eventVec3;
        protected override void OnPropertySet()
        {
            object value = GetPropertyValue();
            
            if (value is Vec3 vec3Val)
            {
                numericInputBoxX.SetValue(vec3Val.X);
                numericInputBoxY.SetValue(vec3Val.Y);
                numericInputBoxZ.SetValue(vec3Val.Z);
            }
            else if (value is EventVec3 evec3Val)
            {
                _eventVec3 = evec3Val;
                numericInputBoxX.SetValue(evec3Val.X);
                numericInputBoxY.SetValue(evec3Val.Y);
                numericInputBoxZ.SetValue(evec3Val.Z);
            }
            else
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not a Vec3 type.");
        }

        protected override void OnLabelSet()
        {
            Label.MouseDown += LabelMouseDown;
            Label.MouseUp += LabelMouseUp;
        }

        private int _y = 0;
        private void LabelMouseDown(object sender, MouseEventArgs e)
        {
            if (numericInputBoxX.Value != null)
            {
                _y = e.Location.Y;
                Label.MouseMove += LabelMouseMove;
            }
        }
        private void LabelMouseUp(object sender, MouseEventArgs e)
        {
            Label.MouseMove -= LabelMouseMove;
        }
        private void LabelMouseMove(object sender, MouseEventArgs e)
        {
            int diff = (e.Location.Y - _y) / 8;
            if (diff == 0)
                return;
            if (diff < 0)
                numericInputBoxX.Value += numericInputBoxX._smallIncrement;
            else
                numericInputBoxX.Value -= numericInputBoxX._smallIncrement;
            _y = e.Location.Y;
        }

        private void numericInputBoxX_ValueChanged(decimal? previous, decimal? current)
        {
            if (_eventVec3 != null)
                _eventVec3.X = decimal.ToSingle(current.Value);
            else
                UpdatePropertyValue(new Vec3(
                    decimal.ToSingle(numericInputBoxX.Value.Value),
                    decimal.ToSingle(numericInputBoxY.Value.Value), 
                    decimal.ToSingle(numericInputBoxZ.Value.Value)));
        }

        private void numericInputBoxY_ValueChanged(decimal? previous, decimal? current)
        {
            if (_eventVec3 != null)
                _eventVec3.Y = decimal.ToSingle(current.Value);
            else
                UpdatePropertyValue(new Vec3(
                    decimal.ToSingle(numericInputBoxX.Value.Value),
                    decimal.ToSingle(numericInputBoxY.Value.Value),
                    decimal.ToSingle(numericInputBoxZ.Value.Value)));
        }

        private void numericInputBoxZ_ValueChanged(decimal? previous, decimal? current)
        {
            if (_eventVec3 != null)
                _eventVec3.Z = decimal.ToSingle(current.Value);
            else
                UpdatePropertyValue(new Vec3(
                    decimal.ToSingle(numericInputBoxX.Value.Value),
                    decimal.ToSingle(numericInputBoxY.Value.Value),
                    decimal.ToSingle(numericInputBoxZ.Value.Value)));
        }
    }
}
