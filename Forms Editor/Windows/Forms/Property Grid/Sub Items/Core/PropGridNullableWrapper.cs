using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Nullable<>))]
    public partial class PropGridNullableWrapper : PropGridItem
    {
        public PropGridNullableWrapper() => InitializeComponent();
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            if (chkNull.Checked = value is null)
            {
                pnlEditor.Visible = false;
            }
            else if (value.GetType() == DataType)
            {
                pnlEditor.Visible = true;
                Type t = DataType.GetGenericArguments()[0];
                if (TheraPropertyGrid.InPlaceEditorTypes.ContainsKey(t))
                {
                    var types = TheraPropertyGrid.GetControlTypes(t);
                    TheraPropertyGrid.CreateControls(types, null, pnlEditor, null, null, null, false, DataChangeHandler);
                }
            }
            chkNull.Enabled = pnlEditor.Enabled = !ParentInfo.IsReadOnly();

            //throw new Exception(DataType.GetFriendlyName() + " is not a Nullable<T> type.");
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
            => UpdateValue(chkNull.Checked, true);
    }
}
