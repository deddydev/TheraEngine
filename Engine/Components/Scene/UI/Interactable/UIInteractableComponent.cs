﻿using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    public interface IUIInteractableComponent : IUIBoundableComponent
    {
        bool IsMouseOver { get; set; }
        bool IsMouseDirectlyOver { get; set; }
        bool IsFocused { get; set; }

        event Action GotFocus;
        event Action LostFocus;

        internal void MouseMove(Vec2 prevPositionLocal, Vec2 newPositionLocal);
    }
    public delegate void DelMouseMove(float x, float y);
    /// <summary>
    /// UI component that can be interacted with by the player.
    /// </summary>
    public abstract class UIInteractableComponent : UIMaterialComponent, IUIInteractableComponent
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

        private bool _registerInputsOnFocus = true;
        [Category("Interactable")]
        public bool RegisterInputsOnFocus
        {
            get => _registerInputsOnFocus;
            set => Set(ref _registerInputsOnFocus, value);
        }

        private bool _isFocused = false;
        [Category("State")]
        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (Set(ref _isFocused, value))
                {
                    if (_isFocused)
                        OnGotFocus();
                    else
                        OnLostFocus();
                }
            }
        }
        private bool _isMouseOver = false;
        [Category("State")]
        public bool IsMouseOver
        {
            get => _isMouseOver;
            set
            {
                if (Set(ref _isMouseOver, value))
                {
                    if (_isMouseOver)
                        OnMouseEnter();
                    else
                        OnMouseLeave();
                }
            }
        }
        private bool _isMouseDirectlyOver = false;
        [Category("State")]
        public bool IsMouseDirectlyOver
        {
            get => _isMouseDirectlyOver;
            set => Set(ref _isMouseDirectlyOver, value);
        }

        public UIInteractableComponent GamepadUpComponent { get; set; }
        public UIInteractableComponent GamepadDownComponent { get; set; }
        public UIInteractableComponent GamepadLeftComponent { get; set; }
        public UIInteractableComponent GamepadRightComponent { get; set; }

        protected virtual void OnMouseMove(float x, float y) { }
        protected virtual void OnMouseEnter() => MouseEnter?.Invoke();
        protected virtual void OnMouseLeave() => MouseLeave?.Invoke();

        protected virtual void OnGamepadEnter() { }
        protected virtual void OnGamepadLeave() { }

        protected virtual void OnGotFocus()
        {
            if (RegisterInputsOnFocus)
            {
                var input = OwningUserInterface?.LocalPlayerController?.Input;
                if (input != null)
                {
                    input.Unregister = false;
                    RegisterInputs(input);
                }
            }

            GotFocus?.Invoke();
        }
        protected virtual void OnLostFocus()
        {
            if (RegisterInputsOnFocus)
            {
                var input = OwningUserInterface?.LocalPlayerController?.Input;
                if (input != null)
                {
                    input.Unregister = true;
                    RegisterInputs(input);
                    input.Unregister = false;
                }
            }

            LostFocus?.Invoke();
        }
        
        void IUIInteractableComponent.MouseMove(Vec2 prevPositionLocal, Vec2 newPositionLocal)
        {
            
        }
    }
}
