using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using CustomEngine;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(ResourceType.World)]
    public class WorldWrapper : GenericWrapper
    {
        private static ContextMenuStrip _menu;
        static WorldWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            WorldWrapper w = GetInstance<WorldWrapper>();
        }
        
        public WorldWrapper(IWin32Window owner) : base(owner) { }
        public WorldWrapper() : base() { }
    }
}
