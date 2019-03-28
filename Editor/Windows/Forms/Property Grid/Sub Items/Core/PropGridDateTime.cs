using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(DateTime))]
    public partial class PropGridDateTime :
#if DEBUG
        DesignerBugFixUserPropGridItem
#else
        PropGridItem
#endif
    {
        public PropGridDateTime()
        {
            InitializeComponent();
        }
        
        protected override bool UpdateDisplayInternal(object value)
        {
            //Value is boxed as object, so this doesn't work
            //if (pnlProps.Visible && !ReferenceEquals(value, _object))
            //    LoadProperties(value);

            if (!IsEditing)
            {
                DateTime time = (DateTime)value;
                time = time < dateTimePicker1.MinDate ? dateTimePicker1.MinDate : (time > dateTimePicker1.MaxDate ? dateTimePicker1.MaxDate : time);
                dateTimePicker1.Value = time;
            }
            
            return false;
        }

        private void _dialog_ValueChanged(object sender, EventArgs e)
        {
            if (IsEditing)
                UpdateValue(dateTimePicker1.Value, false);
        }

        private void dateTimePicker1_DropDown(object sender, EventArgs e)
        {
            _dropDownOpen = true;
            IsEditing = true;
        }

        private void dateTimePicker1_MouseCaptureChanged(object sender, EventArgs e)
        {
            //IsEditing = _mouseOver || _dropDownOpen;
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            _dropDownOpen = false;
            IsEditing = _mouseOver;
            UpdateValue(dateTimePicker1.Value, true);
        }

        private bool _mouseOver = false;
        private bool _dropDownOpen = false;
        private void dateTimePicker1_MouseEnter(object sender, EventArgs e)
        {
            _mouseOver = true;
        }
        private void dateTimePicker1_MouseLeave(object sender, EventArgs e)
        {
            _mouseOver = false;
        }
    }
}
