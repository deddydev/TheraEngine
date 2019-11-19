using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine.Worlds;

namespace TheraEditor.Wrappers
{
    [TreeFileType(nameof(Resources.GenericFile))]
    public class WorldWrapper : FileWrapper<World>
    {
        public override void Edit() => Editor.Instance.TrySetWorld(FileRef);
    }
}
