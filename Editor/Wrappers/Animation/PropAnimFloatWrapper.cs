using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Animation;
using TheraEditor.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile))]
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
        public override async void EditResource()
        {
            if (_graph == null)
            {
                _graph = new DockablePropAnimFloatGraph();
                _graph.FormClosed += _graph_FormClosed;
                _graph.Show(Editor.Instance.DockPanel, DockState.Document);
            }
            PropAnimFloat anim = await ResourceRef.GetInstanceAsync();
            _graph.TargetAnimation = anim;
            _graph.Focus();
        }
        private void _graph_FormClosed(object sender, FormClosedEventArgs e)
        {
            _graph.FormClosed -= _graph_FormClosed;
            _graph = null;
        }
    }
}
