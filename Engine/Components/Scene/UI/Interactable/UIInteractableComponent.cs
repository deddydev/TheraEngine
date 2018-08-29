using System;
using System.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Rendering.UI
{
    public delegate void DelMouseMove(float x, float y);
    /// <summary>
    /// UI component that can be interacted with by the player.
    /// </summary>
    public abstract class UIInteractableComponent : UIMaterialRectangleComponent
    {
        [Category("Events")]
        public event DelMouseMove MouseMove;
        [Category("Events")]
        public event Action MouseEnter;
        [Category("Events")]
        public event Action MouseLeave;

        //Used to select a new component when the user moves the gamepad stick.
        protected UIInteractableComponent _left, _right, _down, _up;
        
        public virtual void OnMouseMove(float x, float y) { }
        public virtual void OnMouseEnter() { }
        public virtual void OnMouseLeave() { }
        public virtual void OnGamepadEnter() { }
        public virtual void OnGamepadLeave() { }
    }
}
