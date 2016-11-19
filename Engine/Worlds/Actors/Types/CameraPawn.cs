using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors.Components;
using System;
using CustomEngine.Rendering.Cameras;
using System.Linq;
using CustomEngine.Rendering;
using System.Collections.Generic;
using CustomEngine.Rendering.HUD;

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
    public class CameraPawn : Pawn
    {
        Dictionary<ComboModifier, Action<bool>> _combos = new Dictionary<ComboModifier, Action<bool>>();

        private ComboModifier GetModifier(EMouseButton button, bool alt, bool ctrl, bool shift)
        {
            ComboModifier mod = ComboModifier.None;

            if (button == EMouseButton.LeftClick)
                mod |= ComboModifier.LeftClick;
            else if (button == EMouseButton.RightClick)
                mod |= ComboModifier.RightClick;
            else if (button == EMouseButton.MiddleClick)
                mod |= ComboModifier.MiddleClick;

            if (_ctrl)
                mod |= ComboModifier.Ctrl;
            if (_alt)
                mod |= ComboModifier.Alt;
            if (_shift)
                mod |= ComboModifier.Shift;

            return mod;
        }
        public void SetInputCombo(Action<bool> func, EMouseButton button, bool alt, bool ctrl, bool shift)
        {
            ComboModifier mod = GetModifier(button, alt, ctrl, shift);
            if (mod != ComboModifier.None)
                if (_combos.ContainsKey(mod))
                    _combos[mod] = func;
                else
                    _combos.Add(mod, func);
        }

        float _linearSpeed = 1.0f, _linearRight = 0.0f, _linearForward = 0.0f, _linearUp = 0.0f;
        float _radialSpeed = 0.05f, _pitch = 0.0f, _yaw = 0.0f, _roll = 0.0f;
        bool _rotating = false;
        bool _translating = false;
        bool _ctrl = false, _alt = false, _shift = false;
        Vec2 _cursorPos = Vec2.Zero;

        public CameraPawn() : base() { }
        public CameraPawn(PlayerIndex possessor) : base(possessor) { }

        CameraComponent CameraComponent { get { return RootComponent as CameraComponent; } }
        protected override void SetDefaults()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
            base.SetDefaults();
        }
        protected override SceneComponent SetupComponents()
        {
            return new CameraComponent(false);
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolled);
            input.RegisterMouseMove(MouseMove, false, false);
            input.RegisterButtonPressed(EMouseButton.RightClick, OnRightClick);
            input.RegisterButtonPressed(EMouseButton.LeftClick, OnLeftClick);
            input.RegisterButtonPressed(EMouseButton.MiddleClick, OnMiddleClick);
            input.RegisterButtonPressed(EKey.A, MoveLeft);
            input.RegisterButtonPressed(EKey.W, MoveForward);
            input.RegisterButtonPressed(EKey.S, MoveBackward);
            input.RegisterButtonPressed(EKey.D, MoveRight);
            input.RegisterButtonPressed(EKey.Q, MoveUp);
            input.RegisterButtonPressed(EKey.E, MoveDown);
            input.RegisterButtonPressed(EKey.ControlLeft, OnControl);
            input.RegisterButtonPressed(EKey.ControlRight, OnControl);
            input.RegisterButtonPressed(EKey.AltLeft, OnAlt);
            input.RegisterButtonPressed(EKey.AltRight, OnAlt);
            input.RegisterButtonPressed(EKey.ShiftLeft, OnShift);
            input.RegisterButtonPressed(EKey.ShiftRight, OnShift);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickX, OnRightStickX, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickY, OnRightStickY, false);
            input.RegisterButtonPressed(GamePadButton.RightBumper, MoveUp);
            input.RegisterButtonPressed(GamePadButton.LeftBumper, MoveDown);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, SelectGamepad);
        }
        private void MoveDown(bool pressed) { _linearUp += pressed ? -1.0f : 1.0f; }
        private void MoveUp(bool pressed) { _linearUp += pressed ? 1.0f : -1.0f; }
        private void MoveLeft(bool pressed) { _linearRight += pressed ? -1.0f : 1.0f; }
        private void MoveRight(bool pressed) { _linearRight += pressed ? 1.0f : -1.0f; }
        private void MoveBackward(bool pressed) { _linearForward += pressed ? -1.0f : 1.0f; }
        private void MoveForward(bool pressed) { _linearForward += pressed ? 1.0f : -1.0f; }
        private void OnLeftStickX(float value) { _linearRight = value; }
        private void OnLeftStickY(float value) { _linearForward = value; }
        private void OnRightStickX(float value) { _yaw = value; }
        private void OnRightStickY(float value) { _pitch = value; }
        private void OnControl(bool pressed) { _ctrl = pressed; }
        private void OnAlt(bool pressed) { _alt = pressed; }
        private void OnShift(bool pressed) { _shift = pressed; }
        private void OnScrolled(bool up) { CameraComponent.Camera.Zoom(up ? _linearSpeed : -_linearSpeed); }
        private void OnLeftClick(bool pressed) { ExecuteCombo(EMouseButton.LeftClick, pressed); }
        private void OnRightClick(bool pressed) { ExecuteCombo(EMouseButton.RightClick, pressed); }
        private void OnMiddleClick(bool pressed) { ExecuteCombo(EMouseButton.MiddleClick, pressed); }
        private void ExecuteCombo(EMouseButton button, bool pressed)
        {
            switch (button)
            {
                case EMouseButton.LeftClick:
                    SelectMouse(pressed);
                    break;
                case EMouseButton.MiddleClick:
                    if (pressed)
                    {
                        _rotating = _alt;
                        _translating = !_alt;
                    }
                    else
                        _translating = _rotating = false;
                    break;
                case EMouseButton.RightClick:
                    ShowContextMenu();
                    break;
            }
            //ComboModifier mod = GetModifier(button, _alt, _ctrl, _shift);
            //if (_combos.ContainsKey(mod))
            //    _combos[mod](pressed);
        }
        public void MouseMove(float x, float y)
        {
            if (_rotating)
            {
                float xDiff = x - _cursorPos.X;
                float yDiff = y - _cursorPos.Y;
                CameraComponent.Camera.Rotate(xDiff * _radialSpeed, yDiff * _radialSpeed);
            }
            else if (_translating)
            {
                float xDiff = x - _cursorPos.X;
                float yDiff = y - _cursorPos.Y;
                CameraComponent.Camera.TranslateRelative(new Vec3(xDiff, yDiff, 0.0f) * _linearSpeed);
            }
            _cursorPos.X = x;
            _cursorPos.Y = y;
        }
        public void ShowContextMenu()
        {

        }
        private Viewport GetViewport()
        {
            LocalPlayerController player = LocalPlayerController;
            if (player == null)
                return null;
            return player.Viewport;
        }
        private void SelectGamepad()
        {
            Viewport v = GetViewport();
            if (v != null)
                Select(v, v.Center);
        }
        public void SelectMouse(bool pressed)
        {
            Viewport v = GetViewport();
            if (v != null)
                Select(v, v.AbsoluteToRelative(_cursorPos));
        }
        private void Select(Viewport v, Vec2 viewportPos)
        {

        }
        internal override void Tick(float delta)
        {
            if (_translating = 
                !_linearRight.IsZero() || 
                !_linearUp.IsZero() || 
                !_linearForward.IsZero())
            {
                float linearSpeed = _linearSpeed * delta;
                CameraComponent.Camera.TranslateRelative(new Vec3(_linearRight, _linearUp, _linearForward) * linearSpeed);
            }
            if (_rotating =
                !_yaw.IsZero() || 
                !_pitch.IsZero())
            {
                float radialSpeed = _radialSpeed * delta;
                CameraComponent.Camera.Rotate(_yaw * radialSpeed, _pitch * radialSpeed);
            }
        }
    }
}
