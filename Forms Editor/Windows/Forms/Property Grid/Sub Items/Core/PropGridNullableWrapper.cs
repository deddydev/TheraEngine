using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Nullable<>))]
    public partial class PropGridNullableWrapper : PropGridItem
    {
        public PropGridNullableWrapper() => InitializeComponent();
        public Type ValueType { get; private set; }
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            //Type tt = value.GetType();
            if (chkNull.Checked = value is null)
            {
                pnlEditors.Controls.Clear();
            }
            else
            {
                if (pnlEditors.Controls.Count == 0 && DataType != null)
                {
                    ValueType = DataType.GetGenericArguments()[0];
                    var types = TheraPropertyGrid.GetControlTypes(ValueType);
                    var items = TheraPropertyGrid.InstantiatePropertyEditors(types, new PropGridItemRefDirectInfo(value, ValueType), DataChangeHandler);
                    foreach (var item in items)
                    {
                        item.Dock = System.Windows.Forms.DockStyle.Top;
                        item.AutoSize = true;
                        pnlEditors.Controls.Add(item);
                    }
                }
            }
            chkNull.Enabled = !ParentInfo.IsReadOnly();

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
    }
}
