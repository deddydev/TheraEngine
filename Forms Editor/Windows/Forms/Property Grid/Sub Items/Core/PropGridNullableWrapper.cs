using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Nullable<>))]
    public partial class PropGridNullableWrapper : PropGridItem
    {
        public PropGridNullableWrapper() => InitializeComponent();
        /// <summary>
        /// The value type referenced by the nullable object.
        /// </summary>
        public Type ValueType { get; private set; }
        protected override void UpdateDisplayInternal(object value)
        {
            bool editable = IsEditable();
            bool notNull = value is null;
            //TODO: support showing specific editors for derived value types?
            if (chkNull.Checked = value is null)
            {
                if (pnlEditors.Enabled)
                {
                    pnlEditors.Enabled = false;
                    foreach (PropGridItem item in pnlEditors.Controls)
                        item.SetReferenceHolder(null);
                }
            }
            else
            {
                if (!pnlEditors.Enabled)
                {
                    pnlEditors.Enabled = true;
                    foreach (PropGridItem item in pnlEditors.Controls)
                        item.SetReferenceHolder(new PropGridItemRefNullableInfo(ParentInfo));
                }
            }
            chkNull.Enabled = editable;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNull.Checked)
            {
                UpdateValue(null, true);
            }
            else
            {
                object o = Editor.UserCreateInstanceOf(ValueType, true);
                UpdateValue(o, true);
            }
        }
        protected internal override void SetReferenceHolder(PropGridItemRefInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            ValueType = DataType?.GetGenericArguments()[0];
            UpdateEditors();
        }
        private void UpdateEditors()
        {
            pnlEditors.Controls.Clear();
            if (DataType != null)
            {
                object value = GetValue();
                ValueType = DataType.GetGenericArguments()[0];
                var types = TheraPropertyGrid.GetControlTypes(ValueType);
                var items = TheraPropertyGrid.InstantiatePropertyEditors(types, null, DataChangeHandler);
                foreach (var item in items)
                {
                    item.Dock = System.Windows.Forms.DockStyle.Top;
                    item.AutoSize = true;
                    pnlEditors.Controls.Add(item);
                }
            }
        }
    }
}
