using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    [TreeFileType("sln")]
    public class SolutionWrapper : FileWrapper
    {
        public SolutionWrapper()
        {
            var compOp = new TMenuOption("Compile", Compile, Keys.Control | Keys.B);
            compOp.Opening += CompOp_Opening;
            compOp.Closing += CompOp_Closing;

            Menu = new TMenu()
            {
                TMenuOption.Rename,
                TMenuOption.Explorer,
                compOp,
                TMenuOption.Edit,
                TMenuOption.EditRaw,
                TMenuDivider.Instance,
                TMenuOption.Cut,
                TMenuOption.Copy,
                TMenuOption.Paste,
                TMenuOption.Delete,
            };
        }

        private void CompOp_Opening(TheraMenuItem obj)
            => obj.Visible =
                string.Equals(FilePath, Editor.Instance.Project?.SolutionPath ?? string.Empty,
                StringComparison.InvariantCultureIgnoreCase);

        private void CompOp_Closing(TheraMenuItem obj)
            => obj.Visible = false;

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