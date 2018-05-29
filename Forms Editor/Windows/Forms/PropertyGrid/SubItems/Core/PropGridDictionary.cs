using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IDictionary))]
    public partial class PropGridDictionary : PropGridItem
    {
        public PropGridDictionary() => InitializeComponent();
        private IDictionary _dictionary = null;
        private Type _valueType, _keyType;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            lblObjectTypeName.Text = DataType.GetFriendlyName();
            chkNull.Visible = DataType.IsValueType;
            if (typeof(IDictionary).IsAssignableFrom(DataType))
            {
                _dictionary = value as IDictionary;
                chkNull.Visible = DataType.IsClass;
                if (!(chkNull.Checked = _dictionary == null))
                {
                    lblObjectTypeName.Enabled = _dictionary.Count > 0;
                    btnAdd.Visible = !_dictionary.IsFixedSize;
                    _valueType = _dictionary.DetermineValueType();
                    _keyType = _dictionary.DetermineKeyType();
                }
                else
                {
                    lblObjectTypeName.Enabled = false;
                    btnAdd.Visible = false;
                }
            }
            else if (value is Exception ex)
            {
                
            }
            else
            {
                throw new Exception(DataType.GetFriendlyName() + " is not an IDictionary type.");
            }
        }
        
        private void pnlElements_VisibleChanged(object sender, EventArgs e)
        {
            if (propGridDicItems.Visible)
            {
                if (propGridDicItems.tblProps.Controls.Count == 0)
                    LoadDictionary(_dictionary);
            }
            else
            {
                LoadDictionary(null);
            }
        }

        protected override void DestroyHandle()
        {
            LoadDictionary(null);
            base.DestroyHandle();
        }

        private async void LoadDictionary(IDictionary dic)
        {
            if (dic == null)
                propGridDicItems.DestroyProperties();
            else
            {
                ConcurrentDictionary<int, List<PropGridItem>> controls = new ConcurrentDictionary<int, List<PropGridItem>>();
                await Task.Run(() => Parallel.For(0, dic.Count, i =>
                {
                    Deque<Type> controlTypes = TheraPropertyGrid.GetControlTypes(dic[i]?.GetType());
                    List<PropGridItem> items = TheraPropertyGrid.InstantiatePropertyEditors(controlTypes, dic, i, DataChangeHandler);
                    controls.TryAdd(i, items);
                }));
                propGridDicItems.tblProps.SuspendLayout();
                for (int i = 0; i < dic.Count; ++i)
                {
                    Label label = propGridDicItems.AddProperty(controls[i], new object[0], false);
                    label.MouseEnter += Label_MouseEnter;
                    label.MouseLeave += Label_MouseLeave;
                    label.MouseDown += Label_MouseDown;
                    label.MouseUp += Label_MouseUp;
                }
                propGridDicItems.tblProps.ResumeLayout(true);
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
            if (_dictionary == null || _dictionary.Count == 0)
                return;
            Label label = (Label)sender;
            label.BackColor = Color.FromArgb(82, 83, 90);
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            if (_dictionary == null || _dictionary.Count == 0)
                return;
            Label label = (Label)sender;
            label.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = _dictionary.Count;
            object value = Editor.UserCreateInstanceOf(_valueType, true);
            if (value == null)
                return;

            _dictionary.Add(value);
            var items = TheraPropertyGrid.InstantiatePropertyEditors(
                TheraPropertyGrid.GetControlTypes(value?.GetType()), _dictionary, i, DataChangeHandler);
            propGridDicItems.AddProperty(items, new object[0], false);
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(items[items.Count - 1]);
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (_dictionary != null)
                pnlHeader.BackColor = Color.FromArgb(14, 18, 34);
        }
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            if (_dictionary != null)
                pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        }
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            if (_dictionary != null)
            {
                propGridDicItems.Visible = !propGridDicItems.Visible;
                Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(this);
            }
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            chkNull.Enabled = enabled;
        }

        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true), true);
        }
    }
}
