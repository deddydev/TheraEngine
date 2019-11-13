using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files.XML;
using TheraEngine.ThirdParty;

namespace TheraEditor.Wrappers
{
    [TreeFileType("csproj")]
    public class CSProjWrapper : ThirdPartyFileWrapper
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static CSProjWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.F2));                              //0
            _menu.Items.Add(new ToolStripMenuItem("&Open In Explorer", null, ExplorerAction, Keys.Control | Keys.O));   //1
            _menu.Items.Add(new ToolStripMenuItem("Co&mpile", null, CompileAction, Keys.Control | Keys.M));             //2
            _menu.Items.Add(new ToolStripMenuItem("Edit", null, EditAction, Keys.F4));                                  //3
            _menu.Items.Add(new ToolStripMenuItem("Edit Raw", null, EditRawAction, Keys.F3));                           //4
            _menu.Items.Add(new ToolStripSeparator());                                                                  //5
            _menu.Items.Add(new ToolStripMenuItem("&Cut", null, CutAction, Keys.Control | Keys.X));                     //6
            _menu.Items.Add(new ToolStripMenuItem("&Copy", null, CopyAction, Keys.Control | Keys.C));                   //7
            _menu.Items.Add(new ToolStripMenuItem("&Paste", null, PasteAction, Keys.Control | Keys.V));                 //8
            _menu.Items.Add(new ToolStripMenuItem("&Delete", null, DeleteAction, Keys.Control | Keys.Delete));          //9
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }

        protected static void CompileAction(object sender, EventArgs e)
            => GetInstance<CSProjWrapper>().Compile();

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            CSProjWrapper w = GetInstance<CSProjWrapper>();
        }
        #endregion

        public CSProjWrapper() : base(_menu) { }

        public MSBuild.Project Project { get; set; }

        public async void Compile()
        {
            var project = Editor.Instance.Project;
            if (project is null)
                return;

            await Editor.RunOperationAsync(
                "Compiling project...",
                "Finished compiling project.",
                async (p, c) => await project.CompileAsync());
        }
        public override async void Edit()
        {
            if (Project is null)
                Project = await XMLSchemaDefinition<MSBuild.Project>.ImportAsync(FilePath);
            
            Editor.Instance.MSBuildTreeForm.SetProject(Project);
        }
    }
}