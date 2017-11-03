using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Concurrent;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(IList))]
    public partial class PropGridList : PropGridItem
    {
        public PropGridList() => InitializeComponent();
        private IList _list;
        private Type _elementType;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            lblObjectTypeName.Text = DataType.GetFriendlyName();
            checkBox1.Visible = DataType.IsValueType;
            if (typeof(IList).IsAssignableFrom(DataType))
            {
                IList list = value as IList;
                if (!(checkBox1.Checked = list == null))
                {
                    btnAdd.Visible = !list.IsFixedSize;
                    _elementType = list.DetermineElementType();
                    _list = list;
                }
            }
            else if (value is Exception ex)
            {
                
            }
            else
            {
                throw new Exception(DataType.GetFriendlyName() + " is not an IList type.");
            }
        }
        
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

        private async void LoadList(IList list)
        {
            if (list == null)
                propGridListItems.DestroyProperties();
            else
            {
                ConcurrentDictionary<int, List<PropGridItem>> controls = new ConcurrentDictionary<int, List<PropGridItem>>();
                await Task.Run(() => Parallel.For(0, list.Count, i =>
                {
                    Deque<Type> controlTypes = TheraPropertyGrid.GetControlTypes(list[i]?.GetType());
                    List<PropGridItem> items = TheraPropertyGrid.CreateControls(controlTypes, list, i);
                    controls.TryAdd(i, items);
                }));
                propGridListItems.tblProps.SuspendLayout();
                for (int i = 0; i < list.Count; ++i)
                {
                    propGridListItems.AddProperty(controls[i], new object[0]);
                }
                propGridListItems.tblProps.ResumeLayout(true);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = _list.Count;
            object value = Editor.UserCreateInstanceOf(_elementType, true);
            if (value == null)
                return;

            _list.Add(value);
            var items = TheraPropertyGrid.CreateControls(TheraPropertyGrid.GetControlTypes(value?.GetType()), _list, i);
            propGridListItems.AddProperty(items, new object[0]);
            Editor.Instance.PropertyGridForm.theraPropertyGrid1.pnlProps.ScrollControlIntoView(items[items.Count - 1]);
        }

        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
            => pnlHeader.BackColor = Color.FromArgb(14, 18, 34);
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
            => pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            propGridListItems.Visible = !propGridListItems.Visible;
            Editor.Instance.PropertyGridForm.theraPropertyGrid1.pnlProps.ScrollControlIntoView(this);
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }
    }
}
