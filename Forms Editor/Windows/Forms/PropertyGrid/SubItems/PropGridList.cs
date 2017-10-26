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
        private Deque<Type> _controlTypes;
        protected override async void UpdateDisplayInternal()
        {
            object value = GetValue();
            if (value is IList list)
            {
                lblObjectTypeName.Text = ValueType.GetFriendlyName();
                btnAdd.Visible = !list.IsFixedSize;
                _elementType = list.DetermineElementType();
                _list = list;
                _controlTypes = await Task.Run(() => TheraPropertyGrid.GetControlTypes(_elementType));
            }
            else if (value is Exception ex)
            {
                
            }
            else
            {
                throw new Exception(ValueType.GetFriendlyName() + " is not an IList type.");
            }
        }

        private void pnlHeader_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void pnlHeader_MouseLeave(object sender, EventArgs e)
        {
        }

        private void pnlHeader_MouseEnter(object sender, EventArgs e)
        {
        }

        private void pnlElements_VisibleChanged(object sender, EventArgs e)
        {
            if (propGridCategory1.Visible)
            {
                if (propGridCategory1.tblProps.Controls.Count == 0)
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
                propGridCategory1.DestroyProperties();
            else
            {
                ConcurrentDictionary<int, List<PropGridItem>> controls = new ConcurrentDictionary<int, List<PropGridItem>>();
                await Task.Run(() => Parallel.For(0, list.Count, i =>
                {
                    List<PropGridItem> items = TheraPropertyGrid.CreateControls(_controlTypes, list, i);
                    controls.TryAdd(i, items);
                }));
                for (int i = 0; i < list.Count; ++i)
                {
                    propGridCategory1.AddProperty(controls[i], new object[0]);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = _list.Count;
            object value = Editor.UserCreateInstanceOf(_elementType);
            _list.Add(value);
            var items = TheraPropertyGrid.CreateControls(_controlTypes, _list, i);
            propGridCategory1.AddProperty(items, new object[0]);
            Editor.Instance.PropertyGridForm.theraPropertyGrid1.pnlProps.ScrollControlIntoView(items[items.Count - 1]);
        }

        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
            => pnlHeader.BackColor = Color.FromArgb(14, 18, 34);
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
            => pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            propGridCategory1.Visible = !propGridCategory1.Visible;
            Editor.Instance.PropertyGridForm.theraPropertyGrid1.pnlProps.ScrollControlIntoView(this);
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            checkBox1.Enabled = enabled;
        }
    }
}
