using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(nameof(Resources.GenericFile))]
    public class GLSLWrapper : GenericTextFileWrapper<GLSLScript>
    {

    }
}