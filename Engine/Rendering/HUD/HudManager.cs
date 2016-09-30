using CustomEngine.Rendering.Cameras;
using System;

namespace CustomEngine.Rendering.HUD
{
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public class HudManager : HudComponent
    {
        private Viewport _owningViewport;
        private OrthographicCamera _camera;
        public HudManager(Viewport v) : base(null)
        {
            _owningViewport = v;
            _camera = new OrthographicCamera();
        }

        public void Resize(float width, float height)
        {
            _camera.Resize(width, height);
            OnResized();
        }

        public override void Render()
        {
            _camera.LoadProjection();
            _camera.LoadModelView();
            base.Render();
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        public void ShowMessage(string message)
        {
            
        }
    }
}
