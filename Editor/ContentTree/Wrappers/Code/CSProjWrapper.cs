using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files.XML;
using TheraEngine.ThirdParty;

namespace TheraEditor.Wrappers
{
    [TreeFileType("csproj")]
    public class CSProjWrapper : FileWrapper
    {
        #region Menu
        private static TheraMenu _menu;
        static CSProjWrapper()
        {
            _menu = new TheraMenu();
            _menu.Add(RenameOption()); //0
            _menu.Add(ExplorerOption()); //1
            _menu.Add(new TheraMenuOption("Co&mpile", nameof(Compile), Keys.Control | Keys.M)); //2
            _menu.Add(EditOption()); //3
            _menu.Add(EditRawOption()); //4
            _menu.Add(new TheraMenuDivider()); //5
            _menu.Add(CutOption()); //6
            _menu.Add(CopyOption()); //7
            _menu.Add(PasteOption()); //8
            _menu.Add(DeleteOption()); //9
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