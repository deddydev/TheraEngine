﻿using System;
using System.Windows.Forms;
using TheraEngine.Animation;

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
                numericInputBox1.SetValue(numericInputBox1.Value + numericInputBox1.SmallIncrement, true);
            else
                numericInputBox1.SetValue(numericInputBox1.Value - numericInputBox1.SmallIncrement, true);
            _y = e.Location.Y;
        }
        private void LabelMouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.SizeNS;
        }
        private void LabelMouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }
        private void numericInputBox1_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            UpdateValue(current.Value, false);
        }
        protected internal override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            if (parentInfo is PropGridMemberInfoProperty propInfo)
            {
                propInfo.Property.GetNumericPrefixSuffixAttribute(out string prefix, out string suffix);
                if (!string.IsNullOrEmpty(prefix))
                    numericInputBox1.NumberPrefix = prefix;
                if (!string.IsNullOrEmpty(suffix))
                    numericInputBox1.NumberSuffix = suffix;
            }
            base.SetReferenceHolder(parentInfo);
        }
        protected override void OnLabelSet()
        {
            Label.MouseDown += LabelMouseDown;
            Label.MouseUp += LabelMouseUp;
            Label.MouseEnter += LabelMouseEnter;
            Label.MouseLeave += LabelMouseLeave;
            base.OnLabelSet();
        }

        protected override BasePropAnim CreateAnimation() => new PropAnimFloat(0.0f, true, true);
    }
}
