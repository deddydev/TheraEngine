using System;
using System.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Hud component that can be interacted with by the player.
    /// </summary>
    public class InteractableHudComponent : UIDockableComponent
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
