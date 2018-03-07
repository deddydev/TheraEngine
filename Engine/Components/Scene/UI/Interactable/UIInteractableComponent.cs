using System;
using System.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// UI component that can be interacted with by the player.
    /// </summary>
    public class UIInteractableComponent : UIDockableComponent
    {
        [Category("Events")]
        public event Action LeftClickDown;
        [Category("Events")]
        public event Action LeftClickUp;
        [Category("Events")]
        public event Action RightClickDown;
        [Category("Events")]
        public event Action RightClickUp;
        [Category("Events")]
        public event Action Scrolled;
        [Category("Events")]
        public event Action MouseMove;
        [Category("Events")]
        public event Action MouseEnter;
        [Category("Events")]
        public event Action MouseExit;

        //Used to select a new component when the user moves the gamepad stick.
        protected UIComponent _left, _right, _down, _up;

        public virtual void OnButtonInput(EKey key, ButtonInputType inputType)
        {

        }
        internal protected virtual void OnLeftClickDown()
        {

        }
    }
}
