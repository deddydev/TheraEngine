using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Animation;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.GenericFile), nameof(Resources.GenericFile))]
    public class PropAnimVec2Wrapper : FileEditorWrapperBase<PropAnimVec2, DockablePropAnimVec2Graph> { }
}
