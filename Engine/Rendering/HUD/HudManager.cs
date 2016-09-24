using System.Collections.Generic;

namespace CustomEngine.Rendering.HUD
{
    public class HudManager : HudComponent
    {
        private Viewport _owningViewport;
        public HudManager(Viewport v)
        {
            _owningViewport = v;
        }
    }
}
