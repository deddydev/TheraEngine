using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheraEditor.Core.Extensions;
using TheraEngine.Core.Maths;
using TheraEngine.Timers;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class MemberLabelInfo
    {
        public string Description { get; set; }
        public Point StartLocation { get; set; }
        public Point EndLocation { get; set; }
        public EventHandler<FrameEventArgs> LerpMethod;
        public PropGridMemberInfo ParentInfo { get; set; }

        public MemberLabelInfo(string description, Point startLocation, Point endLocation)
        {
            Description = description;
            StartLocation = startLocation;
            EndLocation = endLocation;
        }
    }
    public interface IPropGridCategory : ICollapsible
    {
        TheraPropertyGrid PropertyGrid { get; }
        bool Visible { get; set; }
        BetterTableLayoutPanel PropertyTable { get; }
        bool ReadOnly { get; }
        string CategoryName { get; set; }

        void DestroyProperties();
    }
    public partial class PropGridCategory : UserControl, IPropGridCategory
    {
        BetterTableLayoutPanel IPropGridCategory.PropertyTable => PropertyTable;

        public PropGridCategory()
        {
            InitializeComponent();
            DestroyProperties();

            bool invalid = string.IsNullOrWhiteSpace(CategoryName);
            if (lblCategoryName.Visible = !invalid)
                PropertyTable.Visible = !Editor.GetSettings().PropertyGrid.CollapsedCategories.Contains(CategoryName);
            else
                PropertyTable.Visible = true;
        }

        public void Expand() => PropertyTable.Visible = true;
        public void Collapse() => PropertyTable.Visible = false;
        public void Toggle() => PropertyTable.Visible = !PropertyTable.Visible;
        public ControlCollection ChildControls => PropertyTable.Controls;

        public TheraPropertyGrid PropertyGrid { get; internal set; }
        public string CategoryName
        {
            get => lblCategoryName.Text;
            set
            {
                lblCategoryName.Text = value;

                bool invalid = string.IsNullOrWhiteSpace(value);
                if (lblCategoryName.Visible = !invalid)
                    PropertyTable.Visible = !Editor.GetSettings().PropertyGrid.CollapsedCategories.Contains(CategoryName);
                else
                    PropertyTable.Visible = true;
            }
        }

        private bool _readOnly = false;
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                //for (int i = 0; i < tblProps.RowStyles.Count; ++i)
                //{
                //    Panel panel = tblProps.GetControlFromPosition(1, i) as Panel;
                //    foreach (PropGridItem item in panel)
                //    {
                //        item
                //    }
                //}
            }
        }

        public void DestroyProperties()
        {
            if (PropertyTable != null)
            {
                for (int i = 0; i < PropertyTable.ColumnCount; ++i)
                {
                    for (int x = 0; x < PropertyTable.RowCount; ++x)
                    {
                        Control control = PropertyTable.GetControlFromPosition(i, x);
                        if (control is PropGridItem item)
                        {
                            PropertyGrid.RemoveVisibleItem(item);
                        }
                        else if (control is BetterTableLayoutPanel tbl)
                        {
                            for (int r = 0; r < tbl.RowCount; ++r)
                            {
                                Control control2 = tbl.GetControlFromPosition(0, r);
                                if (control2 is PropGridItem item2)
                                {
                                    PropertyGrid.RemoveVisibleItem(item2);
                                }
                            }
                        }
                        control?.Dispose();
                    }
                }
                PropertyTable.Controls.Clear();
                PropertyTable.RowStyles.Clear();
                PropertyTable.RowCount = 0;
            }
        }
        //public Label AddMethod(PropGridMethod methodControl, object[] attributes, string displayName)
        //{
        //    var description = attributes.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute;

        //    //string fName = methodControl.Method.GetFriendlyName();
        //    //int paren = fName.IndexOf('(');
        //    //string methodName = fName.Substring(0, paren);

        //    string desc = string.IsNullOrWhiteSpace(description?.Description) ? null : description.Description;
        //    Label label = new Label()
        //    {
        //        Text = displayName,
        //        TextAlign = ContentAlignment.MiddleRight,
        //        AutoSize = true,
        //        ForeColor = Color.FromArgb(200, 200, 220),
        //        Dock = DockStyle.Fill,
        //        Padding = new Padding(3, 0, 3, 0),
        //        Margin = new Padding(0),
        //        Tag = desc,
        //    };
        //    label.MouseEnter += Label_MouseEnter;
        //    label.MouseLeave += Label_MouseLeave;
        //    tblProps.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //    tblProps.RowCount = tblProps.RowStyles.Count;
        //    tblProps.Controls.Add(label, 0, tblProps.RowCount - 1);

        //    methodControl.SetLabel(label);
        //    methodControl.Dock = DockStyle.Fill;
        //    methodControl.Margin = new Padding(0);
        //    methodControl.Padding = new Padding(0);
        //    tblProps.Controls.Add(methodControl, 1, tblProps.RowCount - 1);
            
        //    return label;
        //}
        //public Label AddEvent(PropGridEvent eventControl, object[] attributes, string displayName)
        //{
        //    var description = attributes.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute;

        //    //string fName = methodControl.Method.GetFriendlyName();
        //    //int paren = fName.IndexOf('(');
        //    //string methodName = fName.Substring(0, paren);

        //    string desc = string.IsNullOrWhiteSpace(description?.Description) ? null : description.Description;
        //    Label label = new Label()
        //    {
        //        Text = displayName,
        //        TextAlign = ContentAlignment.MiddleRight,
        //        AutoSize = true,
        //        ForeColor = Color.FromArgb(200, 200, 220),
        //        Dock = DockStyle.Fill,
        //        Padding = new Padding(3, 0, 3, 0),
        //        Margin = new Padding(0),
        //        Tag = desc,
        //    };
        //    label.MouseEnter += Label_MouseEnter;
        //    label.MouseLeave += Label_MouseLeave;
        //    tblProps.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //    tblProps.RowCount = tblProps.RowStyles.Count;
        //    tblProps.Controls.Add(label, 0, tblProps.RowCount - 1);

        //    eventControl.SetLabel(label);
        //    eventControl.Dock = DockStyle.Fill;
        //    eventControl.Margin = new Padding(0);
        //    eventControl.Padding = new Padding(0);
        //    tblProps.Controls.Add(eventControl, 1, tblProps.RowCount - 1);

        //    return label;
        //}
        public Label AddMember(List<PropGridItem> editors)
        {
            Label label = null;
            if (PropertyTable.InvokeRequired)
            {
                PropertyTable.Invoke((Action)(() =>
                {
                    PropertyTable.BeginUpdate();
                    PropertyTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    PropertyTable.RowCount = PropertyTable.RowStyles.Count;
                    label = AddRowToTable(editors);
                    PropertyTable.EndUpdate();
                }));
            }
            else
            {
                PropertyTable.BeginUpdate();
                PropertyTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                PropertyTable.RowCount = PropertyTable.RowStyles.Count;
                label = AddRowToTable(editors);
                PropertyTable.EndUpdate();
            }

            return label;
        }

        private Label AddRowToTable(List<PropGridItem> editors)
        {
            Label label = new Label()
            {
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                ForeColor = Color.FromArgb(200, 200, 220),
                Dock = DockStyle.Fill,
                Padding = new Padding(3, 2, 3, 4),
                Margin = new Padding(0),
                Text = null,
            };
            label.MouseEnter += Label_MouseEnter;
            label.MouseLeave += Label_MouseLeave;
            PropertyTable.Controls.Add(label, 0, PropertyTable.RowCount - 1);
            label.Tag = new MemberLabelInfo(null, label.Location, new Point(label.Location.X + 10, label.Location.Y));
            if (editors.Count > 1)
            {
                BetterTableLayoutPanel tbl = new BetterTableLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };
                tbl.ColumnStyles.Clear();
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tbl.ColumnCount = 1;
                foreach (PropGridItem item in editors)
                {
                    item.SetLabel(label);
                    item.Dock = DockStyle.Top;
                    item.Margin = new Padding(0);
                    item.Padding = new Padding(0);
                    item.ParentCategory = this;

                    tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    tbl.RowCount = PropertyTable.RowStyles.Count;
                    tbl.Controls.Add(item, 0, tbl.RowCount - 1);
                    PropertyGrid?.AddVisibleItem(item);
                }
                PropertyTable.Controls.Add(tbl, 1, PropertyTable.RowCount - 1);
            }
            else
            {
                PropGridItem item = editors[0];
                item.SetLabel(label);
                item.Dock = DockStyle.Fill;
                item.Margin = new Padding(0);
                item.Padding = new Padding(0);
                item.ParentCategory = this;
                PropertyTable.Controls.Add(item, 1, PropertyTable.RowCount - 1);
                PropertyGrid?.AddVisibleItem(item);
            }
            return label;
        }

        //public TextBox AddTextMember(List<PropGridItem> editors, object[] attributes, bool readOnly)
        //{
        //    //var parentInfo = editors[0].ParentInfo;
        //    var description = attributes.FirstOrDefault(x => x is DescriptionAttribute) as DescriptionAttribute;
        //    string desc = string.IsNullOrWhiteSpace(description?.Description) ? null : description.Description;
        //    string name = ResolveMemberName(editors[0], attributes);
        //    TextBox textBox = new TextBox()
        //    {
        //        Text = name,
        //        TextAlign = HorizontalAlignment.Right,
        //        AutoSize = true,
        //        ForeColor = Color.FromArgb(200, 200, 220),
        //        Dock = DockStyle.Fill,
        //        Padding = new Padding(3, 2, 3, 4),
        //        Margin = new Padding(0),
        //    };
        //    textBox.MouseEnter += Label_MouseEnter;
        //    textBox.MouseLeave += Label_MouseLeave;
        //    textBox.TextChanged += Label_TextChanged;
        //    tblProps.BeginUpdate();
        //    tblProps.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //    tblProps.RowCount = tblProps.RowStyles.Count;
        //    tblProps.Controls.Add(textBox, 0, tblProps.RowCount - 1);
        //    if (editors.Count > 1)
        //    {
        //        Panel p = new Panel()
        //        {
        //            Dock = DockStyle.Fill,
        //            Margin = new Padding(0),
        //            Padding = new Padding(0),
        //            AutoSize = true,
        //            AutoSizeMode = AutoSizeMode.GrowAndShrink,
        //        };
        //        foreach (PropGridItem item in editors)
        //        {
        //            //item.SetLabel(label);
        //            item.Dock = DockStyle.Top;
        //            item.Margin = new Padding(0);
        //            item.Padding = new Padding(0);
        //            item.ReadOnly = readOnly;
        //            item.ParentCategory = this;
        //            p.Controls.Add(item);
        //        }
        //        tblProps.Controls.Add(p, 1, tblProps.RowCount - 1);
        //    }
        //    else
        //    {
        //        PropGridItem item = editors[0];
        //        //item.SetLabel(label);
        //        item.Dock = DockStyle.Fill;
        //        item.Margin = new Padding(0);
        //        item.Padding = new Padding(0);
        //        tblProps.Controls.Add(item, 1, tblProps.RowCount - 1);
        //    }
        //    tblProps.EndUpdate();

        //    textBox.Tag = new MemberLabelInfo(desc, textBox.Location, new Point(textBox.Location.X + 10, textBox.Location.Y));

        //    return textBox;
        //}
        private void Label_TextChanged(object sender, EventArgs e)
        {

        }
        private void Label_MouseLeave(object sender, EventArgs e)
        {
            Label label = sender as Label;

            if (!(label.Tag is MemberLabelInfo info))
                return;

            if (!string.IsNullOrWhiteSpace(info.Description))
                toolTip1.Hide(label);

            label.LerpLocation(info.StartLocation, 2.0f, ref info.LerpMethod, Interp.CosineTimeModifier);
        }
        private void Label_MouseEnter(object sender, EventArgs e)
        {
            Label label = sender as Label;

            if (!(label.Tag is MemberLabelInfo info))
                return;

            if (!string.IsNullOrWhiteSpace(info.Description))
                toolTip1.Show(info.Description, label);

            label.LerpLocation(info.EndLocation, 2.0f, ref info.LerpMethod, Interp.CosineTimeModifier);
        }
        private void lblCategoryName_MouseEnter(object sender, EventArgs e)
        {
            lblCategoryName.BackColor = pnlSide.BackColor = Color.FromArgb(100, 142, 140);
        }
        private void lblCategoryName_MouseLeave(object sender, EventArgs e)
        {
            lblCategoryName.BackColor = pnlSide.BackColor = Color.FromArgb(60, 102, 100);
        }
        private void lblCategoryName_MouseDown(object sender, MouseEventArgs e)
        {
            PropertyTable.Visible = !PropertyTable.Visible;

            List<string> cats = Editor.GetSettings().PropertyGridRef.File.CollapsedCategories;

            if (PropertyTable.Visible)
                cats.Remove(CategoryName);
            else
                cats.Add(CategoryName);

            //Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(this);
        }
        //private void PropGridCategory_Resize(object sender, EventArgs e)
        //{
        //    tblProps.MinimumSize = tblProps.MaximumSize = new Size(Width - 10, 0);
        //}
    }
}
