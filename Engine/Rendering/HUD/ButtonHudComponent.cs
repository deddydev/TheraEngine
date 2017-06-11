using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.HUD
{
    public class ButtonHudComponent : DockableHudComponent
    {
        public ButtonHudComponent()
        {
            _highlightable = true;
            _selectable = true;
        }
    }
}
