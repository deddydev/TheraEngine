using System.Windows.Forms;
using System.ComponentModel;
using TheraEngine.Worlds;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(Project), SystemImages.GenericFile)]
    public class ProjectWrapper : FileWrapper<Project>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static ProjectWrapper()
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
            ProjectWrapper w = GetInstance<ProjectWrapper>();
        }
        #endregion

        public ProjectWrapper() : base() { }
        
        public override void EditResource()
        {
            Editor.Instance.Project = Resource;
        }
    }
}
