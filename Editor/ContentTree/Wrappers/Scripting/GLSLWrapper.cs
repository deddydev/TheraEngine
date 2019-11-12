using TheraEditor.Properties;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.PythonIcon), nameof(Resources.PythonIcon))]
    public class GLSLWrapper : GenericTextFileWrapper<GLSLScript> { }
}