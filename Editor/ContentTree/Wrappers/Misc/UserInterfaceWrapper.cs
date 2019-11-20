using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEngine.Actors.Types.Pawns;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.GenericFile))]
    public class UserInterfaceWrapper : FileWrapper<IUserInterface> { }
}