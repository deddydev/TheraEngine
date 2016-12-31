using System;
using System.ComponentModel;

namespace CustomEngine.Rendering.HUD
{
    public class ButtonHudComponent : DockableHudComponent
    {
        public ButtonHudComponent(HudComponent owner) : base(owner)
        {
            _highlightable = true;
            _selectable = true;
        }
    }
}
