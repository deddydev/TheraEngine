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

        }
        protected virtual void Unhighlight()
        {

        }
    }
}