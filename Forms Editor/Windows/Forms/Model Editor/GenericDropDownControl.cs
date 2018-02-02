using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms
{
    public partial class GenericDropDownControl : UserControl
    {
        public GenericDropDownControl()
        {
            InitializeComponent();
            DestroyChildControls();
            tblControls.Visible = false;
        }
        public string DropDownName
        {
            get => lblDropDownName.Text;
            set => lblDropDownName.Text = value;
        }
        public void DestroyChildControls()
        {
            foreach (Control control in tblControls.Controls)
                control.Dispose();
            tblControls.Controls.Clear();
            tblControls.RowStyles.Clear();
            tblControls.RowCount = 0;
        }
        private void lblCategoryName_MouseEnter(object sender, EventArgs e)
        {
            lblDropDownName.BackColor = pnlSide.BackColor = Color.FromArgb(14, 18, 34);
        }
        private void lblCategoryName_MouseLeave(object sender, EventArgs e)
        {
            lblDropDownName.BackColor = pnlSide.BackColor = Color.FromArgb(54, 58, 74);
        }
        private void lblCategoryName_MouseDown(object sender, MouseEventArgs e)
        {
            tblControls.Visible = !tblControls.Visible;
        }
        public void AddControl(Control control)
        {
            tblControls.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblControls.RowCount = tblControls.RowStyles.Count;
            tblControls.Controls.Add(control, 0, tblControls.RowCount - 1);
        }
        public void RemoveControl(Control control)
        {
            int row = tblControls.GetRow(control);
            tblControls.Controls.Remove(control);
            tblControls.RowStyles.RemoveAt(row);
            tblControls.RowCount = tblControls.RowStyles.Count;
        }
    }
}
