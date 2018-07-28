using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Concurrent;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IDictionary))]
    public partial class PropGridDictionary : PropGridItem, ICollapsible
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

        private void LoadDictionary(IDictionary dic)
        {
            if (dic == null)
                propGridDicItems.DestroyProperties();
            else
            {
                ConcurrentDictionary<int, List<PropGridItem>> controls = new ConcurrentDictionary<int, List<PropGridItem>>();
               
                object[] dicKeys = new object[dic.Keys.Count];
                dic.Keys.CopyTo(dicKeys, 0);
                Array.Sort(dicKeys);

                Deque<Type> valueTypes = TheraPropertyGrid.GetControlTypes(_valueType);
                Deque<Type> keyTypes = TheraPropertyGrid.GetControlTypes(_keyType);

                //await Task.Run(() => Parallel.For(0, dic.Count, i =>
                int r = 0;
                foreach (var key in dicKeys)
                {
                    Type vt = dic[key]?.GetType();
                    Deque<Type> valueTypes2 = valueTypes;
                    if (vt != null && vt.IsSubclassOf(_valueType))
                        valueTypes2 = TheraPropertyGrid.GetControlTypes(vt);

                    Type kt = key?.GetType();
                    Deque<Type> keyTypes2 = keyTypes;
                    if (kt != null && kt.IsSubclassOf(_keyType))
                        valueTypes2 = TheraPropertyGrid.GetControlTypes(kt);

                    List<PropGridItem> keys = TheraPropertyGrid.InstantiateKeyPropertyEditors(keyTypes2, dic, key, DataChangeHandler);
                    List<PropGridItem> values = TheraPropertyGrid.InstantiateValuePropertyEditors(valueTypes2, dic, key, DataChangeHandler);

                    int count = keys.Count + values.Count;
                    List<PropGridItem> interlaced = new List<PropGridItem>(count);
                    int valueIndex = -1;
                    int keyIndex = -1;
                    for (int x = 0; x < count; ++x)
                        interlaced.Add(((x & 1) == 0) ? keys[++keyIndex] : values[++valueIndex]);

                    controls.TryAdd(r++, interlaced);
                }//));
                propGridDicItems.tblProps.SuspendLayout();
                for (int i = 0; i < controls.Count; ++i)
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
            object key = Editor.UserCreateInstanceOf(_keyType, true);
            if (key == null)
                return;
            object value = Editor.UserCreateInstanceOf(_valueType, true);
            if (value == null)
                return;
            
            _dictionary.Add(key, value);

            var keys = TheraPropertyGrid.InstantiateKeyPropertyEditors(
                TheraPropertyGrid.GetControlTypes(key?.GetType()), _dictionary, key, DataChangeHandler);
            var values = TheraPropertyGrid.InstantiateValuePropertyEditors(
                TheraPropertyGrid.GetControlTypes(value?.GetType()), _dictionary, key, DataChangeHandler);

            int count = keys.Count + values.Count;
            List<PropGridItem> interlaced = new List<PropGridItem>(count);
            int valueIndex = -1;
            int keyIndex = -1;
            for (int x = 0; x < count; ++x)
                interlaced.Add(((x & 1) == 0) ? keys[++keyIndex] : values[++valueIndex]);
            
            propGridDicItems.AddProperty(interlaced, new object[0], false);
            Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(interlaced[interlaced.Count - 1]);
        }

        public void Expand() => propGridDicItems.Visible = true;
        public void Collapse() => propGridDicItems.Visible = false;
        public void Toggle() => propGridDicItems.Visible = !propGridDicItems.Visible;
        public ControlCollection ChildControls => propGridDicItems.ChildControls;

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
