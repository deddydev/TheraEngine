using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.HUD
{
    public class ButtonHudComponent : InteractableHudComponent
    {
        public ButtonHudComponent()
        {
            _highlightable = true;
            _selectable = true;
        }
    }
}