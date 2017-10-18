﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridCategory : UserControl
    {
        public PropGridCategory()
        {
            InitializeComponent();
            tblProps.RowStyles.Clear();
        }

        public string CategoryName
        {
            get => lblCategoryName.Text;
            set => lblCategoryName.Text = value;
        }

        public void DestroyProperties()
        {
            tblProps.Controls.Clear();
        }
        public void AddProperty(List<PropGridItem> editors, object[] attributes)
        {
            var displayName = attributes.FirstOrDefault(x => x is DisplayNameAttribute) as DisplayNameAttribute;
            var description = attributes.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute;
            string name = displayName?.DisplayName ?? editors[0].Property.Name;
            string desc = string.IsNullOrWhiteSpace(description?.Description) ? null : description.Description;
            Label label = new Label()
            {
                Text = name,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 200, 220),
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(2),
                Tag = desc,
            };
            label.MouseHover += Label_MouseHover;
            tblProps.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblProps.RowCount = tblProps.RowStyles.Count;
            tblProps.Controls.Add(label, 0, tblProps.RowCount - 1);
            if (editors.Count > 1)
            {
                Panel p = new Panel()
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0),
                    Padding = new Padding(0)
                };
                foreach (PropGridItem item in editors)
                {
                    item.SetLabel(label);
                    item.Dock = DockStyle.Top;
                    item.Margin = new Padding(0);
                    item.Padding = new Padding(0, 1, 0, 1);
                    p.Controls.Add(item);
                }
                tblProps.Controls.Add(p, 1, tblProps.RowCount - 1);
            }
            else
            {
                PropGridItem item = editors[0];
                item.SetLabel(label);
                item.Dock = DockStyle.Fill;
                item.Margin = new Padding(0);
                item.Padding = new Padding(0);
                tblProps.Controls.Add(item, 1, tblProps.RowCount - 1);
            }
        }

        private void Label_MouseHover(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label.Tag != null)
                toolTip1.Show(label.Tag.ToString(), FindForm());
        }

        private void lblCategoryName_MouseEnter(object sender, EventArgs e)
        {
            lblCategoryName.BackColor = panel1.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void lblCategoryName_MouseLeave(object sender, EventArgs e)
        {
            lblCategoryName.BackColor = panel1.BackColor = Color.FromArgb(54, 58, 74);
        }

        private void lblCategoryName_MouseDown(object sender, MouseEventArgs e)
        {
            tblProps.Visible = !tblProps.Visible;
        }

        //private void PropGridCategory_Resize(object sender, EventArgs e)
        //{
        //    tblProps.MinimumSize = tblProps.MaximumSize = new Size(Width - 10, 0);
        //}
    }
}
