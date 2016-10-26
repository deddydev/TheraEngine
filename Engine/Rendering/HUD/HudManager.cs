using CustomEngine.Rendering.Cameras;

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

        public override void Render(float delta)
        {
            _camera.SetCurrent();
            base.Render(delta);
        }

        protected override void OnRender(float delta)
        {
            base.OnRender(delta);
        }

        public void ShowMessage(string message)
        {
            
        }
    }
}
