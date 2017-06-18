using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Input.Devices;

namespace TheraEngine.Rendering.HUD
{
    public delegate void DelScrolling(bool up);
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
