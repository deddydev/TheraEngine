using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IList))]
    public partial class PropGridList : PropGridItem, ICollapsible
    {
        public PropGridList() => InitializeComponent();

        private IList _list = null;
        private Type _elementType;
        private int _displayedCount = 0;

        protected override void UpdateDisplayInternal(object value)
        {
            _list = value as IList;

            if (Editor.GetSettings().PropertyGrid.ShowTypeNames)
            {
                string typeName = (value?.GetType() ?? DataType).GetFriendlyName();
                lblObjectTypeName.Text = "[" + typeName + "] ";
            }
            else
                lblObjectTypeName.Text = string.Empty;

            lblObjectTypeName.Text += _list == null ? "null" : _list.Count.ToString() + (_list.Count == 1 ? " item" : " items");
            
            chkNull.Visible = !DataType.IsValueType;
            
            if (!(chkNull.Checked = _list == null))
            {
                lblObjectTypeName.Enabled = _list.Count > 0;
                btnAdd.Visible = !_list.IsFixedSize;
                _elementType = _list.DetermineElementType();

                if (propGridListItems.Visible && _list.Count != _displayedCount)
                    LoadList(_list);
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
                    LoadList(_list);
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
                    List<PropGridItem> items = TheraPropertyGrid.InstantiatePropertyEditors(controlTypes, new PropGridItemRefIListInfo(() => _list, i), DataChangeHandler);
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
            if (_list == null || _list.Count == 0)
                return;
            Label label = (Label)sender;
            label.BackColor = _prevLabelColor;
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            if (_list == null || _list.Count == 0)
                return;
            Label label = (Label)sender;
            _prevLabelColor = label.BackColor;
            label.BackColor = Color.FromArgb(44, 48, 64);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = _list.Count;
            object value = Editor.UserCreateInstanceOf(_elementType, true, this);
            if (value == null)
                return;

            _list.Add(value);
            var items = TheraPropertyGrid.InstantiatePropertyEditors(
                TheraPropertyGrid.GetControlTypes(value?.GetType()), new PropGridItemRefIListInfo(() => _list, i), DataChangeHandler);
            propGridListItems.AddMember(items, new object[0], false);
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(items[items.Count - 1]);
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (_list != null)
                pnlHeader.BackColor = Color.FromArgb(105, 140, 170);
        }
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            if (_list != null)
                pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        }
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            if (_list != null)
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
