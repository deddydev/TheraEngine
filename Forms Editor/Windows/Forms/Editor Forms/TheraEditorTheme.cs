using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public class TheraEditorTheme : VS2015DarkTheme
    {
        public TheraEditorTheme() : base()
        {
            //Extender.FloatWindowFactory = new TheraEditorFloatWindowFactory();
        }
    }

    //internal class TheraEditorFloatWindowFactory : DockPanelExtender.IFloatWindowFactory
    //{
    //    public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane)
    //    {
    //        return new TheraFloatWindow(dockPanel, pane);
    //    }

    //    public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
    //    {
    //        return new TheraFloatWindow(dockPanel, pane, bounds);
    //    }
    //}

    public class TheraFloatWindow : FloatWindow
    {
        internal protected TheraFloatWindow(DockPanel dockPanel, DockPane pane)
              : base(dockPanel, pane)
        {
            InternalConstruct(dockPanel, pane, false, Rectangle.Empty);
        }

        internal protected TheraFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
            : base(dockPanel, pane, bounds)
        {
            InternalConstruct(dockPanel, pane, true, bounds);
        }

        private void InternalConstruct(DockPanel dockPanel, DockPane pane, bool boundsSpecified, Rectangle bounds)
        {
            FormBorderStyle = FormBorderStyle.None;
        }
    }
}
