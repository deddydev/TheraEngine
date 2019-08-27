using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    [NodeWrapper("", "")]
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

        private static async void RegenSolutionAction(object sender, EventArgs e)
            => await GetInstance<ProjectWrapper>().GenerateSolution();

        private async Task GenerateSolution()
        {
            var res = await ResourceRef.GetInstanceAsync();
            if (res == null)
                return;

            await res.GenerateSolutionAsync();
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
        
        public override async void EditResource()
        {
            var res = await ResourceRef.GetInstanceAsync();

            Editor.Instance.Project = res;
        }
    }
}
