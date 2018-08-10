using TheraEditor.Windows.Forms;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine.Windows.Forms
{
    public class ModelEditorRenderPanel : RenderPanel<BaseScene>
    {
        public DockableModelEditorRenderForm Owner { get; set; }
        
        protected override BaseScene GetScene(Viewport v) => Owner.Form.World?.Scene;
    }
}
