using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
