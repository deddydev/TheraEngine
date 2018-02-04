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
            //DestroyChildControls();
            Collapsible = true;
            //tblControls.Visible = false;
        }

        private Color 
            _dropDownColor = Color.FromArgb(54, 58, 74), 
            _dropDownHighlightColor = Color.FromArgb(14, 18, 34);

        public Color DropDownColor
        {
            get => _dropDownColor;
            set
            {
                _dropDownColor = value;
                if (_highlighted)
                    lblDropDownName.BackColor = pnlSide.BackColor = _dropDownColor;
            }
        }
        public Color DropDownHighlightColor
        {
            get => _dropDownHighlightColor;
            set
            {
                _dropDownHighlightColor = value;
                if (_highlighted)
                    lblDropDownName.BackColor = pnlSide.BackColor = _dropDownHighlightColor;
            }
        }
        private bool _collapsible;
        public bool Collapsible
        {
            get => _collapsible;
            set
            {
                _collapsible = value;
                if (!_collapsible)
                {
                    lblDropDownName.BackColor = pnlSide.BackColor = _dropDownColor;
                    pnlMain.Visible = true;
                }
                else
                {
                    
                }
            }
        }
        private bool _highlighted;
        public string DropDownName
        {
            get => lblDropDownName.Text;
            set => lblDropDownName.Text = value;
        }
        private void lblCategoryName_MouseEnter(object sender, EventArgs e)
        {
            if (Collapsible)
            {
                _highlighted = true;
                lblDropDownName.BackColor = pnlSide.BackColor = _dropDownHighlightColor;
            }
        }
        private void lblCategoryName_MouseLeave(object sender, EventArgs e)
        {
            if (Collapsible)
            {
                _highlighted = false;
                lblDropDownName.BackColor = pnlSide.BackColor = _dropDownColor;
            }
        }

        private void lblDropDownName_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void lblCategoryName_MouseDown(object sender, MouseEventArgs e)
        {
            if (Collapsible)
                pnlMain.Visible = !pnlMain.Visible;
        }
        //public void DestroyChildControls()
        //{
        //    foreach (Control control in tblControls.Controls)
        //        control.Dispose();
        //    tblControls.Controls.Clear();
        //    tblControls.RowStyles.Clear();
        //    tblControls.RowCount = 0;
        //}
        //public void AddControl(Control control)
        //{
        //    //tblControls.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //    //tblControls.RowCount = tblControls.RowStyles.Count;
        //    //tblControls.Controls.Add(control, 0, tblControls.RowCount - 1);
        //}
        //public void RemoveControl(Control control)
        //{
        //    //int row = tblControls.GetRow(control);
        //    //tblControls.Controls.Remove(control);
        //    //tblControls.RowStyles.RemoveAt(row);
        //    //tblControls.RowCount = tblControls.RowStyles.Count;
        //}
        //public void ClearControls()
        //{
        //    //tblControls.Controls.Clear();
        //    //tblControls.RowStyles.Clear();
        //    //tblControls.RowCount = 0;
        //}
    }
}
