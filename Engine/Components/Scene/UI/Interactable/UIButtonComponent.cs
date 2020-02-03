using System.Drawing;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public class UIButtonComponent : UIInteractableComponent
    {
        public UIButtonComponent()
        {

        }

        protected override void OnMouseEnter() => Highlight();
        protected override void OnMouseLeave() => Unhighlight();
        protected override void OnGamepadEnter() => Highlight();
        protected override void OnGamepadLeave() => Unhighlight();

        public virtual void Click()
        {

        }
        
        protected virtual void Highlight()
        {
            var param = Parameter<ShaderVec4>("MatColor");
            if (param != null)
                param.Value = Color.Orange;
        }
        protected virtual void Unhighlight()
        {
            var param = Parameter<ShaderVec4>("MatColor");
            if (param != null)
                param.Value = Color.Magenta;
        }
    }
}