using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheraEditor.Core.Extensions
{
    public static class ControlExtension
    {
        public static Control FindControlAtPoint(this Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    child = c.FindControlAtPoint(new Point(pos.X - c.Left, pos.Y - c.Top));
                    if (child == null) return c;
                    else return child;
                }
            }
            return null;
        }

        public static Control FindControlAtCursor(this Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return form.FindControlAtPoint(form.PointToClient(pos));
            return null;
        }

        public static void InvokeIfNecessary(this Control control, Delegate del, params object[] args)
        {
            if (control.InvokeRequired)
                control.Invoke(del, args);
            else
                del.DynamicInvoke(args);
        }
    }
}
