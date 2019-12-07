using System;
using System.Windows.Forms;
using TheraEditor.ContentTree.Core;
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

            Menu.Insert(3, compOp);
        }

        private void CompOp_Opening(ITMenuItem obj)
            => obj.Visible =
                string.Equals(FilePath, Editor.Instance.Project?.SolutionPath ?? string.Empty,
                StringComparison.InvariantCultureIgnoreCase);

        private void CompOp_Closing(ITMenuItem obj)
            => obj.Visible = false;

        private void Compile()
        {
            Editor.Instance.Compile();
        }
    }
}