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
        public HudManager(Viewport v) : base(null)
        {
            _owningViewport = v;
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
