using System;
using System.Collections;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridIListElementWrapper : PropGridItem
    {
        public PropGridIListElementWrapper() => InitializeComponent();

        protected override bool UpdateDisplayInternal(object value)
        {
            foreach (PropGridItem item in pnlEditors.Controls)
                item.UpdateDisplay();
            return true;
        }
        protected internal override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            
            pnlEditors.Controls.Clear();

            if (DataType != null)
            {
                var types = TheraPropertyGrid.GetControlTypes(DataType);
                var items = TheraPropertyGrid.InstantiatePropertyEditors(types, parentInfo, DataChangeHandler);
                foreach (var item in items)
                {
                    item.Dock = System.Windows.Forms.DockStyle.Top;
                    item.AutoSize = true;
                    pnlEditors.Controls.Add(item);
                }
            }

            pnlEditors.Enabled = false;

            btnRemove.Visible = !((parentInfo as PropGridMemberInfoIList)?.List?.IsFixedSize ?? true);
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            var parentInfo = GetParentInfo<PropGridMemberInfoIList>();
            if (parentInfo == null)
                return;

            int index = parentInfo.Index;
            IList list = parentInfo.List;
            if (list != null && index >= 0 && index < list.Count)
                list.RemoveAt(index);
        }
    }
}
