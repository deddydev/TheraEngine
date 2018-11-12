﻿using System;
using System.Windows.Forms;
using TheraEngine.Core.Attributes;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Double))]
    public partial class PropGridDouble : PropGridItem
    {
        public PropGridDouble()
        {
            InitializeComponent();
            numericInputBox1.GotFocus += NumericInputBox1_GotFocus;
            numericInputBox1.LostFocus += NumericInputBox1_LostFocus;
        }
        private void NumericInputBox1_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBox1_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override void UpdateDisplayInternal(object value)
        {
            numericInputBox1.Value = (Double)value;
            numericInputBox1.Enabled = IsEditable();
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

        private void numericInputBox1_ValueChanged(NumericInputBoxBase<Double> box, Double? previous, Double? current)
        {
            UpdateValue(current.Value, false);
        }
        protected internal override void SetReferenceHolder(PropGridItemRefInfo parentInfo)
        {
            if (parentInfo is PropGridItemRefPropertyInfo propInfo)
            {
                object[] attribs = propInfo.Property.GetCustomAttributes(true);
                foreach (object attrib in attribs)
                {
                    if (attrib is TNumericPrefixSuffixAttribute prefixSuffix)
                    {
                        numericInputBox1.NumberPrefix = prefixSuffix.Prefix;
                        numericInputBox1.NumberSuffix = prefixSuffix.Suffix;
                        break;
                    }
                }
            }
            base.SetReferenceHolder(parentInfo);
        }
    }
}
