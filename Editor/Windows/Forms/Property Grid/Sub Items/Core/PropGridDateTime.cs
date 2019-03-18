using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(DateTime))]
    public partial class PropGridDateTime : PropGridItem
    {
        public PropGridDateTime() => InitializeComponent();
        
        protected DateTime _dateTime;

        protected override bool UpdateDisplayInternal(object value)
        {
            //Value is boxed as object, so this doesn't work
            //if (pnlProps.Visible && !ReferenceEquals(value, _object))
            //    LoadProperties(value);
            
            _dateTime = (DateTime)value;
            lblObjectTypeName.Text = value.ToString();
            
            return false;
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            lblObjectTypeName.BackColor = chkNull.BackColor = Color.FromArgb(105, 140, 170);
        }

        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            lblObjectTypeName.BackColor = chkNull.BackColor = Color.Transparent;
        }
                
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            ChooseDateTime();
        }

        private DateTimePicker _dialog = null;
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;

            if (chkNull.Checked)
            {
                UpdateValue(null, true);
                if ((GetValue() is null))
                    return;

                Engine.PrintLine("Unable to set this property to null.");
            }

            ChooseDateTime();
        }

        private void ChooseDateTime()
        {
            _dialog = new DateTimePicker();
            _dialog.Value = _dateTime;
            _dialog.ValueChanged += _dialog_ValueChanged;
            _dialog.Show();
        }

        private void _dialog_ValueChanged(object sender, EventArgs e) => UpdateValue(_dialog.Value, false);
    }
}
