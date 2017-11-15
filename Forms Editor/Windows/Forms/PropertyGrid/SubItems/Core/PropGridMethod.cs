﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridMethod : PropGridItem
    {
        public PropGridMethod() => InitializeComponent();

        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            lblObjectTypeName.Text = DataType.GetFriendlyName();
            checkBox1.Visible = DataType.IsValueType;
            if (typeof(IList).IsAssignableFrom(DataType))
            {

            }
            else if (value is Exception ex)
            {
                
            }
            else
            {
                throw new Exception(DataType.GetFriendlyName() + " is not an IList type.");
            }
        }

        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void pnlHeader_MouseLeave(object sender, EventArgs e)
        {
        }

        private void pnlHeader_MouseEnter(object sender, EventArgs e)
        {
        }

        private void pnlElements_VisibleChanged(object sender, EventArgs e)
        {
            
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
            => pnlHeader.BackColor = Color.FromArgb(14, 18, 34);
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
            => pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            propGridCategory1.Visible = !propGridCategory1.Visible;
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(this);
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }
    }
}
