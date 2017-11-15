using System.Windows.Forms;

namespace System
{
    public static class ToolStripItemExtension
    {
        public static ContextMenuStrip GetContextMenuStrip(this ToolStripItem item)
        {
            ToolStripItem itemCheck = item;
            while (!(itemCheck.GetCurrentParent() is ContextMenuStrip) && itemCheck.GetCurrentParent() is ToolStripDropDown)
                itemCheck = (itemCheck.GetCurrentParent() as ToolStripDropDown).OwnerItem;
            return itemCheck.GetCurrentParent() as ContextMenuStrip;
        }
        public static ToolStripItem GetRootItem(this ToolStripItem item)
        {
            ToolStripItem itemCheck = item;
            while (itemCheck.GetCurrentParent() is ToolStripDropDown)
                itemCheck = (itemCheck.GetCurrentParent() as ToolStripDropDown).OwnerItem;
            return itemCheck;
        }
    }
}
