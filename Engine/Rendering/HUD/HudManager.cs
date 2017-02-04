using CustomEngine.Rendering.Cameras;
using System.Drawing;
using System;

namespace CustomEngine.Rendering.HUD
{
    /// <summary>
    /// Each viewport has a hud manager. 
    /// The main form also has a hud manager to overlay over everything if necessary.
    /// </summary>
    public partial class HudManager : DockableHudComponent
    {
        private Viewport _owningViewport;
        private RenderPanel _owningPanel;
        private OrthographicCamera _camera;
        public HudManager(Viewport v) : base(null)
        {
            _owningViewport = v;
            _owningPanel = _owningViewport.OwningPanel;
            _camera = new OrthographicCamera();
        }
        public HudManager(RenderPanel p) : base(null)
        {
            _owningViewport = null;
            _owningPanel = p;
            _camera = new OrthographicCamera();
        }

        public override RectangleF ParentResized(RectangleF parentRegion)
        {
            RectangleF region = base.ParentResized(parentRegion);
            _camera.Resize(Width, Height);
            return region;
        }
        public void DebugPrint(string message)
        {
            
        }
        public void Render()
        {

        }
    }
}
