using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridCategory : UserControl
    {
        public PropGridCategory()
        {
            InitializeComponent();
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
        public void AddProperty(PropGridItem item, object[] attributes)
        {
            var displayName = attributes.FirstOrDefault(x => x is DisplayNameAttribute) as DisplayNameAttribute;
            var description = attributes.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute;
            string name = displayName?.DisplayName ?? item.Property.Name;
            string desc = string.IsNullOrWhiteSpace(description?.Description) ? null : description.Description;
            Label label = new Label()
            {
                Text = name,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 200, 220),
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 2),
                Tag = desc,
            };
            label.MouseHover += Label_MouseHover;
            item.SetLabel(label);
            tblProps.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblProps.RowCount = tblProps.RowStyles.Count;
            tblProps.Controls.Add(label, 0, tblProps.RowCount - 1);
            item.Dock = DockStyle.Fill;
            tblProps.Controls.Add(item, 1, tblProps.RowCount - 1);
        }

        private void Label_MouseHover(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (label.Tag != null)
                toolTip1.Show(label.Tag.ToString(), FindForm());
        }
    }
}
