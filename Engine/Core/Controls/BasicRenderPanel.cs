using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine
{
    public class BasicRenderPanel : RenderPanel<BaseScene>
    {
        public override int MaxViewports => 1;

        public BasicRenderPanel()
        {
            World = new World();
            World.RenderPanel = this;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public World World { get; } = new World();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Camera Camera
        {
            get => _viewports.Count == 0 ? null : _viewports[0].Camera;
            set
            {
                GetOrAddViewport(Actors.ELocalPlayerIndex.One).Camera = value;
                value.Resize(Width, Height);
            }
        }

        protected override BaseScene GetScene(Viewport v) => World.Scene;
    }
}
