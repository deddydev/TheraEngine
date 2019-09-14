using Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IDictionary))]
    public partial class PropGridIDictionary : PropGridItem, ICollapsible
    {
        public PropGridIDictionary() => InitializeComponent();

        public IDictionary Dictionary { get; private set; }

        public override PropGridCategory ParentCategory
        {
            get => base.ParentCategory;
            set
            {
                base.ParentCategory = value;
                propGridDicItems.PropertyGrid = ParentCategory?.PropertyGrid;
            }
        }

        private Type _valueType, _keyType;
        protected override bool UpdateDisplayInternal(object value)
        {
            Dictionary = value as IDictionary;
            TypeProxy type = value?.GetTypeProxy() ?? DataType;

            lblObjectTypeName.Text = type.GetFriendlyName();
            chkNull.Visible = !type.IsValueType;

            if (!(chkNull.Checked = Dictionary is null))
            {
                lblObjectTypeName.Enabled = Dictionary.Count > 0;
                btnAdd.Visible = !Dictionary.IsFixedSize;
                _valueType = Dictionary.DetermineValueType();
                _keyType = Dictionary.DetermineKeyType();
            }
            else
            {
                lblObjectTypeName.Enabled = false;
                btnAdd.Visible = false;
            }
            return false;
        }
        
        private void pnlElements_VisibleChanged(object sender, EventArgs e)
        {
            if (propGridDicItems.Visible)
            {
                if (propGridDicItems.PropertyTable.Controls.Count == 0)
                    LoadDictionary(Dictionary);
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
            propGridDicItems.PropertyTable.SuspendLayout();

            if (dic is null)
                propGridDicItems.DestroyProperties();
            else
                await Task.Run(() =>
                {
                    ConcurrentDictionary<int, List<PropGridItem>> controls = new ConcurrentDictionary<int, List<PropGridItem>>();
                    ConcurrentDictionary<TypeProxy, Deque<TypeProxy>> editorTypeCaches = new ConcurrentDictionary<TypeProxy, Deque<TypeProxy>>();

                    object[] dicKeys = new object[dic.Keys.Count];
                    dic.Keys.CopyTo(dicKeys, 0);
                    Array.Sort(dicKeys);

                    Deque<TypeProxy> valueTypes = TheraPropertyGrid.GetControlTypes(_valueType);
                    Deque<TypeProxy> keyTypes = TheraPropertyGrid.GetControlTypes(_keyType);

                    Parallel.For(0, dic.Count, i =>
                    {
                        var key = dicKeys[i];
                        TypeProxy keyType = key?.GetTypeProxy();
                        TypeProxy valType = dic[key]?.GetTypeProxy() ?? _valueType;

                        Deque<TypeProxy> keyControlTypes;
                        if (editorTypeCaches.ContainsKey(keyType))
                            keyControlTypes = editorTypeCaches[keyType];
                        else
                        {
                            keyControlTypes = TheraPropertyGrid.GetControlTypes(keyType);
                            editorTypeCaches.TryAdd(keyType, keyControlTypes);
                        }
                        Deque<TypeProxy> valControlTypes;
                        if (editorTypeCaches.ContainsKey(valType))
                            valControlTypes = editorTypeCaches[valType];
                        else
                        {
                            valControlTypes = TheraPropertyGrid.GetControlTypes(valType);
                            editorTypeCaches.TryAdd(valType, valControlTypes);
                        }
                        
                        List<PropGridItem> keys = TheraPropertyGrid.InstantiatePropertyEditors(keyControlTypes, 
                            new PropGridItemRefIDictionaryInfo(this, key, true), ParentCategory, DataChangeHandler);
                        List<PropGridItem> values = TheraPropertyGrid.InstantiatePropertyEditors(valControlTypes,
                            new PropGridItemRefIDictionaryInfo(this, key, false), ParentCategory, DataChangeHandler);

                        //TODO: don't interlace, put key editor in place of label
                        int count = keys.Count + values.Count;
                        List<PropGridItem> interlaced = new List<PropGridItem>(count);
                        int valueIndex = -1;
                        int keyIndex = -1;
                        for (int x = 0; x < count; ++x)
                            interlaced.Add(((x & 1) == 0) ? keys[++keyIndex] : values[++valueIndex]);

                        controls.TryAdd(i, interlaced);
                    });
                    for (int i = 0; i < controls.Count; ++i)
                    {
                        Label label = propGridDicItems.AddMember(controls[i]);
                        label.MouseEnter += Label_MouseEnter;
                        label.MouseLeave += Label_MouseLeave;
                        label.MouseDown += Label_MouseDown;
                        label.MouseUp += Label_MouseUp;
                    }
                });

            propGridDicItems.PropertyTable.ResumeLayout(true);
        }

        private void Label_MouseUp(object sender, MouseEventArgs e) { }
        private void Label_MouseDown(object sender, MouseEventArgs e) { }

        private Color _prevLabelColor;
        private void Label_MouseLeave(object sender, EventArgs e)
        {
            if (Dictionary is null || Dictionary.Count == 0)
                return;
            Label label = (Label)sender;
            label.BackColor = _prevLabelColor;
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            if (Dictionary is null || Dictionary.Count == 0)
                return;
            Label label = (Label)sender;
            _prevLabelColor = label.BackColor;
            label.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = Dictionary.Count;
            object key = Editor.UserCreateInstanceOf(_keyType, true, this);
            if (key is null)
                return;
            object value = Editor.UserCreateInstanceOf(_valueType, true, this);
            if (value is null)
                return;
            
            Dictionary.Add(key, value);

            var keys = TheraPropertyGrid.InstantiatePropertyEditors(
                TheraPropertyGrid.GetControlTypes(key?.GetType()),
                new PropGridItemRefIDictionaryInfo(this,
                key, true), ParentCategory, DataChangeHandler);

            var values = TheraPropertyGrid.InstantiatePropertyEditors(
                TheraPropertyGrid.GetControlTypes(value?.GetType()),
                new PropGridItemRefIDictionaryInfo(this,
                key, false), ParentCategory, DataChangeHandler);

            int count = keys.Count + values.Count;
            List<PropGridItem> interlaced = new List<PropGridItem>(count);
            int valueIndex = -1;
            int keyIndex = -1;
            for (int x = 0; x < count; ++x)
                interlaced.Add(((x & 1) == 0) ? keys[++keyIndex] : values[++valueIndex]);
            
            propGridDicItems.AddMember(interlaced);
            //Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(interlaced[interlaced.Count - 1]);
        }

        public void Expand() => propGridDicItems.Visible = true;
        public void Collapse() => propGridDicItems.Visible = false;
        public void Toggle() => propGridDicItems.Visible = !propGridDicItems.Visible;
        public ControlCollection ChildControls => propGridDicItems.ChildControls;

        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (Dictionary != null)
                pnlHeader.BackColor = Color.FromArgb(14, 18, 34);
        }
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            if (Dictionary != null)
                pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        }
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            if (Dictionary != null)
            {
                propGridDicItems.Visible = !propGridDicItems.Visible;
                //Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(this);
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
    }
}
