using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Actors.Types.Pawns;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.GenericFile))]
    public class UserInterfaceWrapper : FileEditorWrapperBase<UserInterface, DockableUserInterfaceEditor> { }
}