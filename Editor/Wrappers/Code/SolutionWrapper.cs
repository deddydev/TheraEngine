using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    [NodeWrapper("sln")]
    public class SolutionWrapper : ThirdPartyFileWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static SolutionWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            _menu.Items.Add(new ToolStripMenuItem("Co&mpile", null, CompileAction, Keys.Control | Keys.M));            //2
            _menu.Items.Add(new ToolStripMenuItem("Edit Raw", null, EditRawAction, Keys.F3));                           //3
            _menu.Items.Add(new ToolStripSeparator());                                                                  //4
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //5
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //6
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //7
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //8
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        
        protected static void CompileAction(object sender, EventArgs e)
            => GetInstance<SolutionWrapper>().Compile();
        
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            SolutionWrapper w = GetInstance<SolutionWrapper>();
            w.ContextMenuStrip.Items[2].Visible = false;
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            SolutionWrapper w = GetInstance<SolutionWrapper>();
            w.ContextMenuStrip.Items[2].Visible = 
                string.Equals(w.FilePath, Editor.Instance.Project?.SolutionPath ?? string.Empty,
                StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion
        
        public SolutionWrapper() : base(_menu) { }

        private async void Compile()
        {
            var project = Editor.Instance.Project;
            if (project is null)
                return;

            await Editor.RunOperationAsync(
                "Compiling project...",
                "Finished compiling project.",
                async (p, c) => await project.CompileAsync());
        }
    }
}