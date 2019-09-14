using System;
using System.Collections;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridIListElementWrapper : PropGridItem
    {
        public PropGridIListElementWrapper() => InitializeComponent();

        public override PropGridCategory ParentCategory
        {
            get => base.ParentCategory;
            set
            {
                base.ParentCategory = value;
                foreach (PropGridItem item in pnlEditors.Controls)
                    item.ParentCategory = value;
            }
        }
        public override Label Label
        {
            get => base.Label;
            set
            {
                base.Label = value;
                foreach (PropGridItem item in pnlEditors.Controls)
                    item.Label = value;
            }
        }

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
                var items = TheraPropertyGrid.InstantiatePropertyEditors(types, parentInfo, ParentCategory, DataChangeHandler);
                foreach (var item in items)
                {
                    item.Dock = DockStyle.Top;
                    item.AutoSize = true;
                    pnlEditors.Controls.Add(item);
                }
            }
            
            btnRemove.Visible = !((parentInfo as PropGridMemberInfoIList)?.List?.IsFixedSize ?? true);
        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            var parentInfo = GetMemberInfoAs<PropGridMemberInfoIList>();
            if (parentInfo is null)
                return;

            int index = parentInfo.Index;
            IList list = parentInfo.List;
            if (list != null && index >= 0 && index < list.Count)
                list.RemoveAt(index);
        }
    }
}
