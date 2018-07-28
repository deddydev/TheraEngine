using TheraEditor.Windows.Forms;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine.Windows.Forms
{
    public class ModelEditorRenderPanel : RenderPanel<Scene3D>
    {
        public DockableModelEditorRenderForm Owner { get; set; }
        
        protected override Scene3D GetScene(Viewport v) => Owner.Form.World?.Scene;
    }
}
