using System.Windows.Forms;
using System.ComponentModel;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;
using TheraEditor.Properties;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(TMaterial), nameof(Resources.GenericFile))]
    public class MaterialWrapper : FileWrapper<TMaterial> { }
}
