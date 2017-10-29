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
    [PropGridItem(typeof(UInt16))]
    public partial class PropGridUInt16 : PropGridItem
    {
        public PropGridUInt16()
        {
            InitializeComponent();
        }
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();

            if (value is UInt16 UInt16Val)
                numericInputBox1.Value = UInt16Val;
            else
                throw new Exception(DataType.GetFriendlyName() + " is not a UInt16 type.");
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
                numericInputBox1.Value += numericInputBox1.SmallIncrement;
            else
                numericInputBox1.Value -= numericInputBox1.SmallIncrement;
            _y = e.Location.Y;
        }

        private void numericInputBox1_ValueChanged(NumericInputBoxBase<UInt16> box, UInt16? previous, UInt16? current)
        {
            UpdateValue(current.Value);
        }
    }
}
