using TheraEditor.Properties;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.GenericFile), nameof(Resources.GenericFile))]
    public class MaterialWrapper : FileWrapper<TMaterial> { }
}
