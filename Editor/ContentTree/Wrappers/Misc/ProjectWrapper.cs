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

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            ProjectWrapper w = GetInstance<ProjectWrapper>();
        }
        #endregion
        
        public ProjectWrapper()
        {
            Menu = new TMenu()
            {
                TMenuOption.Rename,
                TMenuOption.Explorer,
                new TMenuOption("Generate Solution", GenerateSolution, Keys.Control | Keys.B),
                TMenuOption.Edit,
                TMenuOption.EditRaw,
                TMenuDivider.Instance,
                TMenuOption.Cut,
                TMenuOption.Copy,
                TMenuOption.Paste,
                TMenuOption.Delete,
            };
        }
        public override void Edit()
        {
            Editor.Instance.LoadProject(FileRef.Path.Path);
        }
        private async void GenerateSolution()
        {
            var res = await FileRef.GetInstanceAsync();
            if (res is null)
                return;

            res.GenerateSolution();
        }

    }
}
