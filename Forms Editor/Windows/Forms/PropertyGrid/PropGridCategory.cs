using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridCategory : UserControl
    {
        public PropGridCategory()
        {
            InitializeComponent();
            DestroyProperties();

            bool invalid = string.IsNullOrWhiteSpace(CategoryName);
            if (lblCategoryName.Visible = !invalid)
                tblProps.Visible = !Editor.GetSettings().PropertyGridRef.File.CollapsedCategories.Contains(CategoryName);
            else
                tblProps.Visible = true;
        }

        public string CategoryName
        {
            get => lblCategoryName.Text;
            set
            {
                lblCategoryName.Text = value;

                bool invalid = string.IsNullOrWhiteSpace(value);
                if (lblCategoryName.Visible = !invalid)
                    tblProps.Visible = !Editor.GetSettings().PropertyGridRef.File.CollapsedCategories.Contains(CategoryName);
                else
                    tblProps.Visible = true;
            }
        }

        public void DestroyProperties()
        {
            foreach (Control control in tblProps.Controls)
                control.Dispose();
            tblProps.Controls.Clear();
            tblProps.RowStyles.Clear();
            tblProps.RowCount = 0;
        }
        public Label AddMethod(PropGridMethod methodControl, object[] attributes, string displayName)
        {
            var description = attributes.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute;

            //string fName = methodControl.Method.GetFriendlyName();
            //int paren = fName.IndexOf('(');
            //string methodName = fName.Substring(0, paren);

            string desc = string.IsNullOrWhiteSpace(description?.Description) ? null : description.Description;
            Label label = new Label()
            {
                Text = displayName,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 200, 220),
                Dock = DockStyle.Fill,
                Padding = new Padding(3, 0, 3, 0),
                Margin = new Padding(0),
                Tag = desc,
            };
            label.MouseEnter += Label_MouseEnter;
            label.MouseLeave += Label_MouseLeave;
            tblProps.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblProps.RowCount = tblProps.RowStyles.Count;
            tblProps.Controls.Add(label, 0, tblProps.RowCount - 1);

            methodControl.SetLabel(label);
            methodControl.Dock = DockStyle.Fill;
            methodControl.Margin = new Padding(0);
            methodControl.Padding = new Padding(0);
            tblProps.Controls.Add(methodControl, 1, tblProps.RowCount - 1);
            
            return label;
        }
        public Label AddProperty(List<PropGridItem> editors, object[] attributes, bool readOnly)
        {
            var displayNameAttrib = attributes.FirstOrDefault(x => x is DisplayNameAttribute) as DisplayNameAttribute;
            var description = attributes.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute;

            string displayName = displayNameAttrib?.DisplayName;
            string propName = editors[0].Property?.Name;
            string name;

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                name = displayName;
            }
            else if (!string.IsNullOrWhiteSpace(propName))
            {
                name = Editor.GetSettings().PropertyGridRef.File.SplitCamelCase ? propName.SplitCamelCase() : propName;
            }
            else
            {
                name = string.Format("[{0}]", editors[0].IListIndex);
            }

            string desc = string.IsNullOrWhiteSpace(description?.Description) ? null : description.Description;
            Label label = new Label()
            {
                Text = name,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 200, 220),
                Dock = DockStyle.Fill,
                Padding = new Padding(3, 0, 3, 0),
                Margin = new Padding(0),
                Tag = desc,
            };
            label.MouseEnter += Label_MouseEnter;
            label.MouseLeave += Label_MouseLeave;
            tblProps.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblProps.RowCount = tblProps.RowStyles.Count;
            tblProps.Controls.Add(label, 0, tblProps.RowCount - 1);
            if (editors.Count > 1)
            {
                Panel p = new Panel()
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };
                foreach (PropGridItem item in editors)
                {
                    item.SetLabel(label);
                    item.Dock = DockStyle.Top;
                    item.Margin = new Padding(0);
                    item.Padding = new Padding(0);
                    item.ReadOnly = readOnly;
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

            return label;
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label.Tag != null)
                toolTip1.Hide(label);
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label.Tag != null)
                toolTip1.Show(label.Tag.ToString(), label);
        }

        private void lblCategoryName_MouseEnter(object sender, EventArgs e)
        {
            lblCategoryName.BackColor = pnlSide.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void lblCategoryName_MouseLeave(object sender, EventArgs e)
        {
            lblCategoryName.BackColor = pnlSide.BackColor = Color.FromArgb(54, 58, 74);
        }

        private void lblCategoryName_MouseDown(object sender, MouseEventArgs e)
        {
            tblProps.Visible = !tblProps.Visible;

            if (tblProps.Visible)
                Editor.GetSettings().PropertyGridRef.File.CollapsedCategories.Remove(CategoryName);
            else
                Editor.GetSettings().PropertyGridRef.File.CollapsedCategories.Add(CategoryName);

            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(this);
        }

        //private void PropGridCategory_Resize(object sender, EventArgs e)
        //{
        //    tblProps.MinimumSize = tblProps.MaximumSize = new Size(Width - 10, 0);
        //}
    }
}
