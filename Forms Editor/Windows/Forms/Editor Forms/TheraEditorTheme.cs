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
            ColorPalette.TabButtonSelectedInactiveHovered.Background = Color.Red;
            ColorPalette.TabButtonSelectedInactiveHovered.Border = Color.Red;
            ColorPalette.TabButtonSelectedInactiveHovered.Glyph = Color.Red;
            ColorPalette.TabButtonSelectedInactivePressed.Background = Color.Red;
            ColorPalette.TabButtonSelectedInactivePressed.Border = Color.Red;
            ColorPalette.TabButtonSelectedInactivePressed.Glyph = Color.Red;

            ColorPalette.TabButtonSelectedActiveHovered.Background = Color.Red;
            ColorPalette.TabButtonSelectedActiveHovered.Border = Color.Red;
            ColorPalette.TabButtonSelectedActiveHovered.Glyph = Color.Red;
            ColorPalette.TabButtonSelectedActivePressed.Background = Color.Red;
            ColorPalette.TabButtonSelectedActivePressed.Border = Color.Red;
            ColorPalette.TabButtonSelectedActivePressed.Glyph = Color.Red;

            ColorPalette.TabButtonUnselectedTabHoveredButtonHovered.Background = Color.Red;
            ColorPalette.TabButtonUnselectedTabHoveredButtonHovered.Border = Color.Red;
            ColorPalette.TabButtonUnselectedTabHoveredButtonHovered.Glyph = Color.Red;
            ColorPalette.TabButtonUnselectedTabHoveredButtonPressed.Background = Color.Red;
            ColorPalette.TabButtonUnselectedTabHoveredButtonPressed.Border = Color.Red;
            ColorPalette.TabButtonUnselectedTabHoveredButtonPressed.Glyph = Color.Red;

            ColorPalette.TabSelectedActive.Background = Editor.TurquoiseColor;
            ColorPalette.TabSelectedActive.Button = Editor.TurquoiseColor;
            ColorPalette.TabSelectedActive.Text = SystemColors.HighlightText;

            //ColorPalette.TabSelectedInactive.Background = Color.LightGray;
            //ColorPalette.TabSelectedInactive.Button = Color.LightGray;
            //ColorPalette.TabSelectedInactive.Text = Color.LightGray;

            ColorPalette.TabUnselectedHovered.Background = Editor.TitleBarColor;
            ColorPalette.TabUnselectedHovered.Button = Editor.TitleBarColor;
            ColorPalette.TabUnselectedHovered.Text = Editor.TextColor;

            //ColorPalette.MainWindowActive.Background = Editor.TurquoiseColor;

            ColorPalette.ToolWindowCaptionButtonActiveHovered.Background = Color.Red;
            ColorPalette.ToolWindowCaptionButtonActiveHovered.Border = Color.Red;
            ColorPalette.ToolWindowCaptionButtonActiveHovered.Glyph = Color.Red;
            ColorPalette.ToolWindowCaptionButtonInactiveHovered.Background = Color.Red;
            ColorPalette.ToolWindowCaptionButtonInactiveHovered.Border = Color.Red;
            ColorPalette.ToolWindowCaptionButtonInactiveHovered.Glyph = Color.Red;
            ColorPalette.ToolWindowCaptionButtonPressed.Background = Color.Red;
            ColorPalette.ToolWindowCaptionButtonPressed.Border = Color.Red;
            ColorPalette.ToolWindowCaptionButtonPressed.Glyph = Color.Red;

            ColorPalette.ToolWindowCaptionActive.Background = Editor.TurquoiseColor;
            ColorPalette.ToolWindowCaptionActive.Button = Color.Orange;
            ColorPalette.ToolWindowCaptionActive.Grip = Color.FromArgb(200, 242, 242);
            ColorPalette.ToolWindowCaptionActive.Text = SystemColors.HighlightText;

            Measures.DockPadding = 10;

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
