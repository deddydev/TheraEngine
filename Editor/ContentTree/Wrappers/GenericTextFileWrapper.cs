using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    public class GenericTextFileWrapper<T> : FileWrapper<T> where T : TextFile
    {
        public GenericTextFileWrapper() : base() { }

        public override void Edit()
            => Editor.Instance.UserEditText(File, DockState.Document);
    }
}
