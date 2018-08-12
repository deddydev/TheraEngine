using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Concurrent;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IList))]
    public partial class PropGridList : PropGridItem, ICollapsible
    {
        public PropGridList() => InitializeComponent();
        private IList _list = null;
        private Type _elementType;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            lblObjectTypeName.Text = DataType.GetFriendlyName();
            chkNull.Visible = !DataType.IsValueType;
            
            _list = value as IList;
            if (!(chkNull.Checked = _list == null))
            {
                lblObjectTypeName.Enabled = _list.Count > 0;
                btnAdd.Visible = !_list.IsFixedSize;
                _elementType = _list.DetermineElementType();
            }
            else
            {
                lblObjectTypeName.Enabled = false;
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
            if (list == null)
                propGridListItems.DestroyProperties();
            else
            {
                propGridListItems.tblProps.SuspendLayout();
                for (int i = 0; i < list.Count; ++i)
                {
                    Deque<Type> controlTypes = TheraPropertyGrid.GetControlTypes(list[i]?.GetType());
                    List<PropGridItem> items = TheraPropertyGrid.InstantiatePropertyEditors(controlTypes, new PropGridItemRefIListInfo(list, i), DataChangeHandler);
                    Label label = propGridListItems.AddProperty(items, new object[0], false);
                    label.MouseEnter += Label_MouseEnter;
                    label.MouseLeave += Label_MouseLeave;
                    label.MouseDown += Label_MouseDown;
                    label.MouseUp += Label_MouseUp;
                }
                propGridListItems.tblProps.ResumeLayout(true);
            }
        }

        private void Label_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void Label_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            if (_list == null || _list.Count == 0)
                return;
            Label label = (Label)sender;
            label.BackColor = Color.FromArgb(82, 83, 90);
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            if (_list == null || _list.Count == 0)
                return;
            Label label = (Label)sender;
            label.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = _list.Count;
            object value = Editor.UserCreateInstanceOf(_elementType, true);
            if (value == null)
                return;

            _list.Add(value);
            var items = TheraPropertyGrid.InstantiatePropertyEditors(
                TheraPropertyGrid.GetControlTypes(value?.GetType()), new PropGridItemRefIListInfo(_list, i), DataChangeHandler);
            propGridListItems.AddProperty(items, new object[0], false);
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(items[items.Count - 1]);
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (_list != null)
                pnlHeader.BackColor = Color.FromArgb(14, 18, 34);
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
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true), true);
        }
    }
}
