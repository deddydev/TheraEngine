using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Animation;
using TheraEditor.Properties;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(PropAnimFloat), nameof(Resources.GenericFile))]
    public class PropAnimFloatWrapper : FileWrapper<PropAnimFloat>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static PropAnimFloatWrapper()
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
            PropAnimFloatWrapper w = GetInstance<PropAnimFloatWrapper>();
        }
        #endregion
        
        public PropAnimFloatWrapper() : base() { }

        DockablePropAnimFloatGraph _graph;
        public override void EditResource()
        {
            if (_graph != null)
            {
                _graph.TargetAnimation = ResourceRef.File;
                _graph.Focus();
            }
            else
            {
                _graph = new DockablePropAnimFloatGraph { TargetAnimation = ResourceRef.File };
                _graph.FormClosed += _graph_FormClosed;
                _graph.Show(Editor.Instance.DockPanel);
            }
        }
        private void _graph_FormClosed(object sender, FormClosedEventArgs e)
        {
            _graph = null;
        }
    }
}
