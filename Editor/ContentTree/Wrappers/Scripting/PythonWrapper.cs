using TheraEditor.Properties;
using TheraEngine.Scripting;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.PythonIcon), nameof(Resources.PythonIcon))]
    public class PythonWrapper : GenericTextFileWrapper<PythonScript> { }
}