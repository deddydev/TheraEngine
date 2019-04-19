using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;

namespace TheraEngine
{
    public class BasicRenderPanel : RenderPanel<IScene>
    {
        public override int MaxViewports => 1;
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public World World { get; } = new World();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICamera Camera
        {
            get => Viewports.Count == 0 ? null : Viewports[0].Camera;
            set
            {
                GetOrAddViewport(ELocalPlayerIndex.One).Camera = value;
                value.Resize(Width, Height);
            }
        }

        protected override IScene GetScene(Viewport v) => World.Scene;
    }
}
