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
            ColorPalette.TabButtonSelectedInactiveHovered.Background = Color.Orange;
            ColorPalette.TabButtonSelectedInactiveHovered.Border = Color.Orange;
            ColorPalette.TabButtonSelectedInactiveHovered.Glyph = Color.Orange;
            ColorPalette.TabButtonSelectedInactivePressed.Background = Color.Orange;
            ColorPalette.TabButtonSelectedInactivePressed.Border = Color.Orange;
            ColorPalette.TabButtonSelectedInactivePressed.Glyph = Color.Orange;

            ColorPalette.TabButtonSelectedActiveHovered.Background = Color.Orange;
            ColorPalette.TabButtonSelectedActiveHovered.Border = Color.Orange;
            ColorPalette.TabButtonSelectedActiveHovered.Glyph = Color.Orange;
            ColorPalette.TabButtonSelectedActivePressed.Background = Color.Orange;
            ColorPalette.TabButtonSelectedActivePressed.Border = Color.Orange;
            ColorPalette.TabButtonSelectedActivePressed.Glyph = Color.Orange;

            ColorPalette.TabButtonUnselectedTabHoveredButtonHovered.Background = Color.Orange;
            ColorPalette.TabButtonUnselectedTabHoveredButtonHovered.Border = Color.Orange;
            ColorPalette.TabButtonUnselectedTabHoveredButtonHovered.Glyph = Color.Orange;
            ColorPalette.TabButtonUnselectedTabHoveredButtonPressed.Background = Color.Orange;
            ColorPalette.TabButtonUnselectedTabHoveredButtonPressed.Border = Color.Orange;
            ColorPalette.TabButtonUnselectedTabHoveredButtonPressed.Glyph = Color.Orange;

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

            ColorPalette.ToolWindowCaptionButtonActiveHovered.Background = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonActiveHovered.Border = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonActiveHovered.Glyph = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonInactiveHovered.Background = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonInactiveHovered.Border = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonInactiveHovered.Glyph = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonPressed.Background = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonPressed.Border = Color.Orange;
            ColorPalette.ToolWindowCaptionButtonPressed.Glyph = Color.Orange;

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
