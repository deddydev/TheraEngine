using System.ComponentModel;

namespace TheraEngine.Rendering.UI
{
    public delegate void DelScrolling(bool up);
    public class UIComboBoxComponent : UIInteractableComponent
    {
        [Category("Events")]
        public event DelScrolling Scrolled;

        public UIComboBoxComponent()
        {

        }
    }
}
