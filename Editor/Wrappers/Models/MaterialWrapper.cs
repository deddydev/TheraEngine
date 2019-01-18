using TheraEditor.Properties;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile))]
    public class MaterialWrapper : FileWrapper<TMaterial> { }
}
