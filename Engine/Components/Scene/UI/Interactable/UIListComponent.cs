using System.ComponentModel;

namespace TheraEngine.Rendering.UI
{
    public class ListHudComponent : InteractableHudComponent
    {
        [Category("Events")]
        public event DelScrolling Scrolled;

        public virtual void OnScrolled(bool up)
        {
            if (_scrollable)
                Scrolled?.Invoke(up);
        }
        public ListHudComponent()
        {
            _highlightable = true;
            _selectable = true;
        }
    }
}
