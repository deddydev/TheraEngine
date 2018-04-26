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
            get => _viewports.Count == 0 ? null : _viewports [0].Camera;
            set
            {
                if (_viewports.Count == 0)
                    AddViewport();
                _viewports[0].Camera = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddViewport();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
        }
        protected override void OnParentVisibleChanged(EventArgs e)
        {
            base.OnParentVisibleChanged(e);
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Engine.DesignMode && Visible && _viewports.Count == 0)
                AddViewport();
        }

        protected virtual void PreRender() => PreRendered?.Invoke();
        protected virtual void PostRender() => PostRendered?.Invoke();
        protected override void OnUpdate()
        {
            if (_viewports.Count == 0)
                return;

            Viewport v = _viewports[0];
            v.Update(Scene, v.Camera, v.Camera.Frustum);
        }
        public override void SwapBuffers()
        {
            if (_viewports.Count > 0)
                _viewports[0].SwapBuffers();
        }
        protected override void OnRender()
        {
            if (_viewports.Count == 0)
                return;

            Viewport v = _viewports[0];

            PreRender();
            _context.BeginDraw();
            v.Render(Scene, v.Camera, null);
            _context.EndDraw();
            PostRender();
        }
    }
}
