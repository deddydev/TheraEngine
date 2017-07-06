using System;
using System.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Rendering.HUD
{
    /// <summary>
    /// Hud component that can be interacted with by the player.
    /// </summary>
    public class InteractableHudComponent : DockableHudComponent
    {
        [Category("Events")]
        public event Action Highlighted;
        [Category("Events")]
        public event Action Clicked;

        public virtual void OnHighlighted()
        {
            if (_highlightable)
                Highlighted?.Invoke();
        }
        public virtual void OnButtonInput(EKey key, ButtonInputType inputType)
        {
            if (_selectable)
                Clicked?.Invoke();
        }
    }
}
