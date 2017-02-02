using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors.Components;
using System;
using CustomEngine.Rendering.Cameras;
using System.Linq;
using CustomEngine.Rendering;
using System.Collections.Generic;
using CustomEngine.Rendering.HUD;
using CustomEngine.Worlds.Actors.Types;

namespace CustomEngine.Worlds.Actors
{
    public enum CameraInputType
    {
        TranslateXY,
        RotateYP,
        Select,
        ContextMenu,
    }
    [Flags]
    public enum ComboModifier
    {
        None        = 0x0,
        Ctrl        = 0x1,
        Alt         = 0x2,
        Shift       = 0x4,
        LeftClick   = 0x8,
        MiddleClick = 0x10,
        RightClick  = 0x20,
    }
    public class CameraInputCombo
    {
        public CameraInputCombo(EMouseButton type, ComboModifier modifiers)
        {
            _type = type;
            _modifiers = modifiers;
        }
        public CameraInputCombo(EMouseButton type)
        {
            _type = type;
            _modifiers = ComboModifier.None;
        }
        public EMouseButton _type;
        public ComboModifier _modifiers;
    }
    public class FlyingCameraPawn : Pawn
    {
        float _linearRight = 0.0f, _linearForward = 0.0f, _linearUp = 0.0f;
        float 
            _scrollSpeed = 5.0f, 
            _mouseRotateSpeed = 0.2f,
            _mouseTranslateSpeed = 1.0f,
            _gamepadRotateSpeed = 150.0f,
            _gamepadTranslateSpeed = 30.0f,
            _keyboardTranslateSpeed = 30.0f;

        float _pitch = 0.0f, _yaw = 0.0f;
        bool _rotating = false;
        bool _translating = false;
        bool _ctrl = false, _alt = false, _shift = false;
        Vec2 _cursorPos = Vec2.Zero;

        public FlyingCameraPawn() : base() { }
        public FlyingCameraPawn(PlayerIndex possessor) : base(possessor) { }

        CameraComponent CameraComponent { get { return RootComponent as CameraComponent; } }
        protected override void SetDefaults()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
            CameraComponent.Camera.TranslateAbsolute(new Vec3(0.0f, 160.0f, 120.0f));
            CameraComponent.Camera.Rotation.Pitch = -45.0f;
            base.SetDefaults();
        }
        protected override SceneComponent SetupComponents()
        {
            return new CameraComponent(false);
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolled);
            input.RegisterMouseMove(MouseMove, true);
            input.RegisterButtonPressed(EMouseButton.RightClick, OnRightClick);
            input.RegisterButtonPressed(EMouseButton.LeftClick, OnLeftClick);
            input.RegisterButtonPressed(EMouseButton.MiddleClick, OnMiddleClick);

            input.RegisterButtonPressed(EKey.A, MoveLeft);
            input.RegisterButtonPressed(EKey.W, MoveForward);
            input.RegisterButtonPressed(EKey.S, MoveBackward);
            input.RegisterButtonPressed(EKey.D, MoveRight);
            input.RegisterButtonPressed(EKey.Q, MoveDown);
            input.RegisterButtonPressed(EKey.E, MoveUp);
            input.RegisterButtonPressed(EKey.ControlLeft, OnControl);
            input.RegisterButtonPressed(EKey.ControlRight, OnControl);
            input.RegisterButtonPressed(EKey.AltLeft, OnAlt);
            input.RegisterButtonPressed(EKey.AltRight, OnAlt);
            input.RegisterButtonPressed(EKey.ShiftLeft, OnShift);
            input.RegisterButtonPressed(EKey.ShiftRight, OnShift);
            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, OnTogglePause);
            input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, OnTogglePause);

            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickX, OnRightStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickY, OnRightStickY, false);
            input.RegisterButtonPressed(GamePadButton.RightBumper, MoveUp);
            input.RegisterButtonPressed(GamePadButton.LeftBumper, MoveDown);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnGamepadSelect);
        }
        
        private void OnTogglePause()
        {
            Engine.TogglePause();
            //if (Engine.IsPaused)
            //{
            //    LocalPlayerController.EnqueuePosession(this);
            //    LocalPlayerController.ControlledPawn = GetViewport().HUD;
            //}
        }

        private void MoveDown(bool pressed) { _linearUp += _keyboardTranslateSpeed * (pressed ? -1.0f : 1.0f); }
        private void MoveUp(bool pressed) { _linearUp += _keyboardTranslateSpeed * (pressed ? 1.0f : -1.0f); }
        private void MoveLeft(bool pressed) { _linearRight += _keyboardTranslateSpeed * (pressed ? -1.0f : 1.0f); }
        private void MoveRight(bool pressed) { _linearRight += _keyboardTranslateSpeed * (pressed ? 1.0f : -1.0f); }
        private void MoveBackward(bool pressed) { _linearForward += _keyboardTranslateSpeed * (pressed ? -1.0f : 1.0f); }
        private void MoveForward(bool pressed) { _linearForward += _keyboardTranslateSpeed * (pressed ? 1.0f : -1.0f); }

        private void OnLeftStickX(float value) { _linearRight = value * _gamepadTranslateSpeed; }
        private void OnLeftStickY(float value) { _linearForward = value * _gamepadTranslateSpeed; }
        private void OnRightStickX(float value) { _yaw = value * _gamepadRotateSpeed; }
        private void OnRightStickY(float value) { _pitch = value * _gamepadRotateSpeed; }

        private void OnControl(bool pressed) { _ctrl = pressed; }
        private void OnAlt(bool pressed) { _alt = pressed; }
        private void OnShift(bool pressed) { _shift = pressed; }

        private void OnScrolled(bool up) { CameraComponent.Camera.Zoom(up ? _scrollSpeed : -_scrollSpeed); }
        private void OnLeftClick(bool pressed)
        {
            PickScene(false);
        }
        private void OnGamepadSelect()
        {
            PickScene(true);
        }
        private void OnRightClick(bool pressed)
        {
            if (pressed)
            {
                _rotating = !_alt;
                _translating = _alt;
            }
            else
                _translating = _rotating = false;
        }
        private void OnMiddleClick(bool pressed)
        {
            
        }
        //private void ExecuteCombo(EMouseButton button, bool pressed)
        //{
        //    //ComboModifier mod = GetModifier(button, _alt, _ctrl, _shift);
        //    //if (_combos.ContainsKey(mod))
        //    //    _combos[mod](pressed);
        //}
        public void MouseMove(float x, float y)
        {
            float xDiff = x - _cursorPos.X;
            float yDiff = y - _cursorPos.Y;
            if (_rotating)
                CameraComponent.Camera.Rotate(-yDiff * _mouseRotateSpeed, -xDiff * _mouseRotateSpeed);
            else if (_translating)
                CameraComponent.Camera.TranslateRelative(new Vec3(-xDiff * _mouseTranslateSpeed, yDiff * _mouseTranslateSpeed, 0.0f));
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
        private void PickScene(bool gamepad)
        {
            Viewport v = GetViewport();
            if (v == null)
                return;
            
            Actor actor = v.PickScene(gamepad ? v.Center : v.AbsoluteToRelative(_cursorPos));
            if (actor is EditorTransformTool)
            {

            }
            else if (actor is HudComponent)
            {

            }
            //else if (actor != null && actor.IsMovable && !actor.SimulatingPhysics)
            //{
            //    TransformTool tool = new TransformTool(actor);
            //    Engine.World.SpawnActor(tool, actor.WorldMatrix);
            //}
        }
        internal override void Tick(float delta)
        {
            CameraComponent.Camera.TranslateRelative(new Vec3(_linearRight, _linearUp, -_linearForward) * delta);
            CameraComponent.Camera.Rotate(_pitch * delta, _yaw * delta);
            HighlightScene(true);
        }

        //Dictionary<ComboModifier, Action<bool>> _combos = new Dictionary<ComboModifier, Action<bool>>();

        //private ComboModifier GetModifier(EMouseButton button, bool alt, bool ctrl, bool shift)
        //{
        //    ComboModifier mod = ComboModifier.None;

        //    if (button == EMouseButton.LeftClick)
        //        mod |= ComboModifier.LeftClick;
        //    else if (button == EMouseButton.RightClick)
        //        mod |= ComboModifier.RightClick;
        //    else if (button == EMouseButton.MiddleClick)
        //        mod |= ComboModifier.MiddleClick;

        //    if (_ctrl)
        //        mod |= ComboModifier.Ctrl;
        //    if (_alt)
        //        mod |= ComboModifier.Alt;
        //    if (_shift)
        //        mod |= ComboModifier.Shift;

        //    return mod;
        //}
        //public void SetInputCombo(Action<bool> func, EMouseButton button, bool alt, bool ctrl, bool shift)
        //{
        //    ComboModifier mod = GetModifier(button, alt, ctrl, shift);
        //    if (mod != ComboModifier.None)
        //        if (_combos.ContainsKey(mod))
        //            _combos[mod] = func;
        //        else
        //            _combos.Add(mod, func);
        //}
    }
}
