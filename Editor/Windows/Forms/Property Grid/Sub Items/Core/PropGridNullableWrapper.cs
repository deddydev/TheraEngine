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
        protected override bool UpdateDisplayInternal(object value)
        {
            bool editable = IsEditable();
            //bool notNull = value is null;
            if (chkNull.Checked = value is null)
            {
                if (pnlEditors.Enabled)
                {
                    pnlEditors.Enabled = false;
                    //foreach (PropGridItem item in pnlEditors.Controls)
                    //    item.SetReferenceHolder(null);
                }
            }
            else
            {
                if (!pnlEditors.Enabled)
                {
                    pnlEditors.Enabled = true;
                    //foreach (PropGridItem item in pnlEditors.Controls)
                    //    item.SetReferenceHolder(new PropGridItemRefNullableInfo(ParentInfo, ValueType));
                }
            }
            chkNull.Enabled = editable;
            return false;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            object o = chkNull.Checked ? null : Editor.UserCreateInstanceOf(ValueType, true, this);
            UpdateValue(o, true);
        }
        protected internal override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            
            pnlEditors.Controls.Clear();

            if (DataType != null)
            {
                ValueType = DataType.GetGenericArguments()[0];
                var types = TheraPropertyGrid.GetControlTypes(ValueType);
                var items = TheraPropertyGrid.InstantiatePropertyEditors(types, null, DataChangeHandler);
                foreach (var item in items)
                {
                    item.Dock = System.Windows.Forms.DockStyle.Top;
                    item.AutoSize = true;
                    pnlEditors.Controls.Add(item);
                    item.SetReferenceHolder(new PropGridItemRefNullableInfo(this, MemberInfo, ValueType));
                }
            }
            else
                ValueType = null;

            pnlEditors.Enabled = false;
        }
    }
}
