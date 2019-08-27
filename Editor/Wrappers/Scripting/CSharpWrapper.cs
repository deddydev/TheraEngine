using TheraEditor.Properties;
using TheraEngine.Scripting;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.CSharpScript), nameof(Resources.CSharpScript))]
    public class CSharpWrapper : GenericTextFileWrapper<CSharpScript> { }
}