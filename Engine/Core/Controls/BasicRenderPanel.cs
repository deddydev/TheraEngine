using System;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine
{
    public class BasicRenderPanel : BaseRenderPanel
    {
        public override int MaxViewports => 1;
        public event Action PreRendered, PostRendered;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Scene3D Scene { get; } = new Scene3D();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Camera Camera
        {
            get => _viewports[0].Camera;
            set => _viewports[0].Camera = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Engine.DesignMode)
                AddViewport();
        }

        protected virtual void PreRender() => PreRendered?.Invoke();
        protected virtual void PostRender() => PostRendered?.Invoke();
        protected override void OnRender()
        {
            Viewport v = _viewports[0];

            PreRender();
            _context.BeginDraw();
            v.Render(Scene, v.Camera, v.Camera.Frustum, null);
            _context.EndDraw();
            PostRender();
        }
    }
}
