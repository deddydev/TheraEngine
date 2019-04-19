using System;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Nullable<>))]
    public partial class PropGridNullableWrapper : PropGridItem
    {
        public PropGridNullableWrapper() => InitializeComponent();
        /// <summary>
        /// The value type referenced by the nullable object.
        /// </summary>
        public TypeProxy ValueType { get; private set; }
        protected override bool UpdateDisplayInternal(object value)
        {
            bool editable = IsEditable();
            bool isNull = value is null;
            if (isNull)
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
            if (chkNull.Checked != isNull)
                chkNull.Checked = isNull;
            chkNull.Enabled = editable;
            return false;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
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

                PropGridItemRefNullableInfo parentInfoNullable = new PropGridItemRefNullableInfo(this, MemberInfo, ValueType);
                
                var types = TheraPropertyGrid.GetControlTypes(ValueType);
                var items = TheraPropertyGrid.InstantiatePropertyEditors(types, parentInfoNullable, ParentCategory, DataChangeHandler);

                foreach (var item in items)
                {
                    item.Dock = System.Windows.Forms.DockStyle.Top;
                    item.AutoSize = true;
                    pnlEditors.Controls.Add(item);
                }
            }
            else
                ValueType = null;

            pnlEditors.Enabled = false;
        }
    }
}
