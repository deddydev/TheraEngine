﻿using System;
using System.Windows.Forms;
using TheraEngine.Animation;
using TheraEngine.Core.Attributes;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(float))]
    public partial class PropGridSingle : PropGridItem
    {
        public override bool CanAnimate => true;

        public PropGridSingle()
        {
            InitializeComponent();
            numericInputBox1.GotFocus += NumericInputBox1_GotFocus;
            numericInputBox1.LostFocus += NumericInputBox1_LostFocus;
        }
        private void NumericInputBox1_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBox1_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override bool UpdateDisplayInternal(object value)
        {
            numericInputBox1.Value = (float)value;
            numericInputBox1.Enabled = IsEditable();
            return false;
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
            int diff = e.Location.Y - _y;
            if (diff == 0)
                return;
            if (diff < 0)
                numericInputBox1.Value += numericInputBox1.SmallIncrement;
            else
                numericInputBox1.Value -= numericInputBox1.SmallIncrement;
            _y = e.Location.Y;
        }

        private void numericInputBox1_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            UpdateValue(current.Value, false);
        }
        protected internal override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            if (parentInfo is PropGridMemberInfoProperty propInfo)
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
        protected override void OnLabelSet()
        {
            Label.MouseDown += LabelMouseDown;
            Label.MouseUp += LabelMouseUp;
            base.OnLabelSet();
        }
        protected override BasePropAnim CreateAnimation() => new PropAnimFloat(0.0f, true, true);
    }
}
