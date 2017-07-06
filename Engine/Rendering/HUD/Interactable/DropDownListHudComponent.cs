using System.ComponentModel;

namespace TheraEngine.Rendering.HUD
{
    public delegate void DelScrolling(bool up);
    public class DropDownListHudComponent : InteractableHudComponent
    {
        [Category("Events")]
        public event DelScrolling Scrolled;
        public virtual void OnScrolled(bool up)
        {
            if (_scrollable)
                Scrolled?.Invoke(up);
        }
        public DropDownListHudComponent()
        {
            _highlightable = true;
            _selectable = true;
        }
    }
}
