using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    [TreeFileType("", "")]
    public class ProjectWrapper : FileWrapper<TProject>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static ProjectWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Generate Solution", null, RegenSolutionAction, Keys.F5));
            _menu.Items.Add(new ToolStripSeparator());
            FillContextMenuDefaults(_menu);
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }

        private static void RegenSolutionAction(object sender, EventArgs e)
            => GetInstance<ProjectWrapper>().GenerateSolution();

        private async void GenerateSolution()
        {
            var res = await ResourceRef.GetInstanceAsync();
            if (res is null)
                return;
            
            res.GenerateSolution();
        }
        
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            ProjectWrapper w = GetInstance<ProjectWrapper>();
        }
        #endregion

        public ProjectWrapper() : base(_menu) { }
        
        public override void EditResource()
        {
            Editor.Instance.LoadProject(ResourceRef.Path.Path);
        }
    }
}
