using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IList))]
    public partial class PropGridIList : PropGridItem, ICollapsible
    {
        public PropGridIList() => InitializeComponent();

        public IList List { get; private set; } = null;

        private Type _elementType;
        private int _displayedCount = 0;

        public override PropGridCategory ParentCategory
        {
            get => base.ParentCategory;
            set
            {
                base.ParentCategory = value;
                propGridListItems.PropertyGrid = ParentCategory?.PropertyGrid;
            }
        }
        protected override void UpdateDisplayInternal(object value)
        {
            List = value as IList;
            
            if (Editor.GetSettings().PropertyGrid.ShowTypeNames)
            {
                string typeName = (value?.GetType() ?? DataType).GetFriendlyName();
                lblObjectTypeName.Text = "[" + typeName + "] ";
            }
            else
                lblObjectTypeName.Text = string.Empty;

            lblObjectTypeName.Text += List == null ? "null" : List.Count.ToString() + (List.Count == 1 ? " item" : " items");
            
            chkNull.Visible = !DataType.IsValueType;
            
            if (!(chkNull.Checked = List == null))
            {
                lblObjectTypeName.Enabled = List.Count > 0;
                btnAdd.Visible = !List.IsFixedSize;
                _elementType = List.DetermineElementType();

                if (propGridListItems.Visible && List.Count != _displayedCount)
                    LoadList(List);
            }
            else
            {
                lblObjectTypeName.Enabled = false;
                if (propGridListItems.Visible)
                {
                    propGridListItems.Visible = false;
                    LoadList(null);
                }
                btnAdd.Visible = false;
            }
        }

        public void Expand() => propGridListItems.Visible = true;
        public void Collapse() => propGridListItems.Visible = false;
        public void Toggle() => propGridListItems.Visible = !propGridListItems.Visible;
        public ControlCollection ChildControls => propGridListItems.ChildControls;

        private void pnlElements_VisibleChanged(object sender, EventArgs e)
        {
            if (propGridListItems.Visible)
            {
                if (propGridListItems.tblProps.Controls.Count == 0)
                    LoadList(List);
            }
            else
            {
                LoadList(null);
            }
        }

        protected override void DestroyHandle()
        {
            LoadList(null);
            base.DestroyHandle();
        }

        private void LoadList(IList list)
        {
            propGridListItems.DestroyProperties();
            if (list != null)
            {
                _displayedCount = list.Count;
                propGridListItems.tblProps.SuspendLayout();
                for (int i = 0; i < list.Count; ++i)
                {
                    Deque<Type> controlTypes = TheraPropertyGrid.GetControlTypes(list[i]?.GetType() ?? _elementType);
                    List<PropGridItem> items = TheraPropertyGrid.InstantiatePropertyEditors(controlTypes, new PropGridMemberInfoIList(this, i), DataChangeHandler);
                    Label label = propGridListItems.AddMember(items, new object[0], false);
                    label.MouseEnter += Label_MouseEnter;
                    label.MouseLeave += Label_MouseLeave;
                    label.MouseDown += Label_MouseDown;
                    label.MouseUp += Label_MouseUp;
                    label.HandleDestroyed += Label_HandleDestroyed;
                }
                propGridListItems.tblProps.ResumeLayout(true);
            }
            else
                _displayedCount = 0;
        }

        private void Label_HandleDestroyed(object sender, EventArgs e)
        {
            if (sender is Label label)
            {
                label.MouseEnter -= Label_MouseEnter;
                label.MouseLeave -= Label_MouseLeave;
                label.MouseDown -= Label_MouseDown;
                label.MouseUp -= Label_MouseUp;
            }
        }

        private void Label_MouseUp(object sender, MouseEventArgs e)
        {

        }
        private void Label_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private Color _prevLabelColor;
        private void Label_MouseLeave(object sender, EventArgs e)
        {
            if (List == null || List.Count == 0)
                return;
            Label label = (Label)sender;
            label.BackColor = _prevLabelColor;
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            if (List == null || List.Count == 0)
                return;
            Label label = (Label)sender;
            _prevLabelColor = label.BackColor;
            label.BackColor = Color.FromArgb(44, 48, 64);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = List.Count;
            object value = Editor.UserCreateInstanceOf(_elementType, true, this);
            if (value == null)
                return;

            List.Add(value);
            var items = TheraPropertyGrid.InstantiatePropertyEditors(
                TheraPropertyGrid.GetControlTypes(value?.GetType()), new PropGridMemberInfoIList(this, i), DataChangeHandler);
            propGridListItems.AddMember(items, new object[0], false);
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(items[items.Count - 1]);
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (List != null)
                pnlHeader.BackColor = Color.FromArgb(105, 140, 170);
        }
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            if (List != null)
                pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        }
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            if (List != null)
            {
                propGridListItems.Visible = !propGridListItems.Visible;
                Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(this);
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
    }
}
