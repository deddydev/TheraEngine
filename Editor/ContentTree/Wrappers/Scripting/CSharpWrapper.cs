using TheraEditor.Properties;
using TheraEngine.Scripting;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.CSharpScript), nameof(Resources.CSharpScript))]
    public class CSharpWrapper : GenericTextFileWrapper<ScriptCSharp> { }
}