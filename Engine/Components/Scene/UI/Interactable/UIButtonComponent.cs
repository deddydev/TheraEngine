﻿namespace TheraEngine.Rendering.UI
{
    public class ButtonHudComponent : UIInteractableComponent
    {
        public ButtonHudComponent()
        {

        }

        public override void OnMouseEnter() => Highlight();
        public override void OnMouseLeave() => Unhighlight();
        public override void OnGamepadEnter() => Highlight();
        public override void OnGamepadLeave() => Unhighlight();
        
        protected virtual void Highlight()
        {

        }
        protected virtual void Unhighlight()
        {

        }
    }
}