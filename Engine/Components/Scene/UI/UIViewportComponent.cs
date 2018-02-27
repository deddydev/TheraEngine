using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public class UIViewportComponent : UIDockableComponent, I2DRenderable
    {
        public UIViewportComponent() : base() { }
        
        private Viewport _viewport = new Viewport(1.0f, 1.0f);

        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(ERenderPass2D.OnTop, 20, 0);
        public Camera Camera { get; set; }
        
        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 r = base.Resize(parentBounds);
            _viewport.Resize(Width, Height);
            return r;
        }
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            _viewport.Position = ScreenTranslation;
        }
        public void Render()
        {
            Scene scene = Camera?.OwningComponent?.OwningScene;
            _viewport.Render(scene, Camera, Camera?.Frustum);
        }
    }
}
