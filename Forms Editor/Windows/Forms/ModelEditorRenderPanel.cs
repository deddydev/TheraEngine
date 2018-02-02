using TheraEngine.Rendering;

namespace TheraEngine.Windows.Forms
{
    public class ModelEditorRenderPanel : RenderPanel<Scene3D>
    {
        public Scene3D Scene { get; set; }

        protected override Scene3D GetScene(Viewport v)
        {
            return Scene;
        }

        protected override void PreRender()
        {

        }
    }
}
