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
    [PropGridItem(
        typeof(sbyte), typeof(byte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong),
        typeof(float), typeof(double),
        typeof(decimal)
    )]
    public partial class PropGridNumeric : PropGridItem
    {
        public PropGridNumeric()
        {
            InitializeComponent();
        }
        protected override void OnPropertySet()
        {
            object value = GetPropertyValue();

            if (value is sbyte sbyteVal)
                numericInputBox1.SetValue(sbyteVal);
            else if (value is byte byteVal)
                numericInputBox1.SetValue(byteVal);
            else if (value is short shortVal)
                numericInputBox1.SetValue(shortVal);
            else if (value is ushort ushortVal)
                numericInputBox1.SetValue(ushortVal);
            else if (value is int intVal)
                numericInputBox1.SetValue(intVal);
            else if (value is uint uintVal)
                numericInputBox1.SetValue(uintVal);
            else if (value is long longVal)
                numericInputBox1.SetValue(longVal);
            else if (value is ulong ulongVal)
                numericInputBox1.SetValue(ulongVal);
            else if (value is float floatVal)
                numericInputBox1.SetValue(floatVal);
            else if (value is double doubleVal)
                numericInputBox1.SetValue(doubleVal);
            else if (value is decimal decimalVal)
                numericInputBox1.SetValue(decimalVal);
            else
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not a numeric type.");
        }

        protected override void OnLabelSet()
        {
            Label.MouseDown += LabelMouseDown;
            Label.MouseUp += LabelMouseUp;
        }

        private int _y = 0;
        private void LabelMouseDown(object sender, MouseEventArgs e)
        {
            if (numericInputBox1.Value != null)
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
            int diff = (e.Location.Y - _y);
            if (diff == 0)
                return;
            if (diff < 0)
                numericInputBox1.Value += numericInputBox1._smallIncrement;
            else
                numericInputBox1.Value -= numericInputBox1._smallIncrement;
            _y = e.Location.Y;
        }

        private void numericInputBox1_ValueChanged(decimal? previous, decimal? current)
        {
            if (Property.PropertyType == typeof(sbyte))
                UpdatePropertyValue(decimal.ToSByte(current.Value));
            else if (Property.PropertyType == typeof(byte))
                UpdatePropertyValue(decimal.ToByte(current.Value));
            else if (Property.PropertyType == typeof(short))
                UpdatePropertyValue(decimal.ToInt16(current.Value));
            else if (Property.PropertyType == typeof(ushort))
                UpdatePropertyValue(decimal.ToUInt16(current.Value));
            else if (Property.PropertyType == typeof(int))
                UpdatePropertyValue(decimal.ToInt32(current.Value));
            else if (Property.PropertyType == typeof(uint))
                UpdatePropertyValue(decimal.ToUInt32(current.Value));
            else if(Property.PropertyType == typeof(long))
                UpdatePropertyValue(decimal.ToInt64(current.Value));
            else if (Property.PropertyType == typeof(ulong))
                UpdatePropertyValue(decimal.ToUInt64(current.Value));
            else if (Property.PropertyType == typeof(float))
                UpdatePropertyValue(decimal.ToSingle(current.Value));
            else if (Property.PropertyType == typeof(double))
                UpdatePropertyValue(decimal.ToDouble(current.Value));
            else
                UpdatePropertyValue(current.Value);
        }
    }
}
