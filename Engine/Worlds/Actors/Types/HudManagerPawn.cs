using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors.Components;
using System;
using CustomEngine.Worlds.Actors.Types;
using CustomEngine.Worlds;

namespace CustomEngine.Rendering.HUD
{
    public partial class HudManager : DockableHudComponent
    {
        float _linearRight = 0.0f, _linearForward = 0.0f, _linearUp = 0.0f;
        float 
            _scrollSpeed = 2.0f, 
            _mouseRotateSpeed = 0.2f,
            _mouseTranslateSpeed = 0.0f,
            _gamepadRotateSpeed = 150.0f,
            _gamepadTranslateSpeed = 30.0f,
            _keyboardTranslateSpeed = 20.0f;
        
        bool _shift = false;
        Vec2 _cursorPos = Vec2.Zero;

        CameraComponent CameraComponent { get { return RootComponent as CameraComponent; } }
        protected override void SetDefaults()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic);
            base.SetDefaults();
        }
        protected override SceneComponent SetupComponents()
        {
            return new PositionComponent();
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolled);
            input.RegisterMouseMove(MouseMove, false);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnSelect);
            input.RegisterButtonPressed(EKey.ShiftLeft, OnShift);
            input.RegisterButtonPressed(EKey.ShiftRight, OnShift);

            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickX, OnRightStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickY, OnRightStickY, false);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnSelect);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnCancel);

            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, OnTogglePause);
            input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, OnTogglePause);
        }
        private void OnTogglePause()
        {
            Engine.TogglePause();
            if (!Engine.IsPaused)
                LocalPlayerController.ControlledPawn = null;
        }
        private void OnLeftStickX(float value) { }
        private void OnLeftStickY(float value) { }
        private void OnRightStickX(float value) { }
        private void OnRightStickY(float value) { }
        
        private void OnShift(bool pressed) { _shift = pressed; }
        private void OnScrolled(bool up) { }

        private HudComponent _focusedComponent;
        private void OnSelect()
        {
            _focusedComponent?.OnSelected();
        }
        private void OnCancel()
        {

        }
        
        private void OnLeftClick(bool pressed)
        {

        }
        private void OnGamepadSelect()
        {

        }
        public void MouseMove(float x, float y)
        {
            _cursorPos.X = x;
            _cursorPos.Y = y;
            HighlightScene(false);
        }
        public void ShowContextMenu()
        {

        }
        private void HighlightScene(bool gamepad)
        {

        }
        
        internal override void Tick(float delta)
        {
            
        }
    }
}
