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
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
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
                        item.SetReferenceHolder(new PropGridItemRefDirectInfo(value, ValueType));
                }
                else
                {
                    foreach (PropGridItem item in pnlEditors.Controls)
                        ((PropGridItemRefDirectInfo)item.ParentInfo).Target = value;
                }
            }
            chkNull.Enabled = editable;

            //throw new Exception(DataType.GetFriendlyName() + " is not a Nullable<T> type.");
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNull.Checked)
            {
                UpdateValue(null, true);
            }
            else
            {
                ValueType = DataType.GetGenericArguments()[0];
                object o = Editor.UserCreateInstanceOf(ValueType, true);
                UpdateValue(o, true);
            }
        }
        protected internal override void SetReferenceHolder(PropGridItemRefInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
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
