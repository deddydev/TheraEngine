using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using CustomEngine;
using CustomEngine.Worlds;
using TheraEditor.Properties;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(World), SystemImages.GenericFile)]
    public class WorldWrapper : GenericWrapper
    {
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
        
        public WorldWrapper(string path) : base(path) { }
    }
}
