using System;
using System.ComponentModel;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public delegate void DelMouseMove(float x, float y);
    /// <summary>
    /// UI component that can be interacted with by the player.
    /// </summary>
    public abstract class UIInteractableComponent : UIMaterialRectangleComponent
    {
        public UIInteractableComponent() 
            : base() { }

        public UIInteractableComponent(TMaterial material, bool flipVerticalUVCoord = false) 
            : base(material, flipVerticalUVCoord) { }

        [Category("Events")]
        public event Action GotFocus;
        [Category("Events")]
        public event Action LostFocus;
        [Category("Events")]
        public event DelMouseMove MouseMove;
        [Category("Events")]
        public event Action MouseEnter;
        [Category("Events")]
        public event Action MouseLeave;
        
        public virtual void OnMouseMove(float x, float y) { }

        public virtual void OnMouseEnter() { }
        public virtual void OnMouseLeave() { }

        public virtual void OnGamepadEnter() { }
        public virtual void OnGamepadLeave() { }

        public virtual void OnGotFocus() { }
        public virtual void OnLostFocus() { }

        public UIInteractableComponent GamepadUpComponent { get; set; }
        public UIInteractableComponent GamepadDownComponent { get; set; }
        public UIInteractableComponent GamepadLeftComponent { get; set; }
        public UIInteractableComponent GamepadRightComponent { get; set; }

        protected internal override void RegisterInputs(InputInterface input)
        {
            base.RegisterInputs(input);
        }
    }
}
