using System;
using System.ComponentModel;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Actors.Types.Pawns
{
    public abstract class FlyingCameraPawnBase<T> : Pawn<T> where T : OriginRebasableComponent
    {
        public FlyingCameraPawnBase() : base() { }
        public FlyingCameraPawnBase(LocalPlayerIndex possessor) : base(false, possessor) { }
        
        public Camera Camera
        {
            get => CameraComp.Camera;
            set => CameraComp.Camera = value;
        }
        public CameraComponent CameraComp { get; set; }
        
        protected float 
            _linearRight = 0.0f,
            _linearForward = 0.0f,
            _linearUp = 0.0f,
            _pitch = 0.0f,
            _yaw = 0.0f;

        protected bool 
            _ctrl = false,
            _rightClickPressed = false;
        
        [Browsable(false)]
        public bool Rotating => _rightClickPressed && _ctrl;
        [Browsable(false)]
        public bool Translating => _rightClickPressed && !_ctrl;
        [Browsable(false)]
        public bool Moving => Rotating || Translating;

        [TSerialize]
        [Category("Movement")]
        public float ScrollSpeed { get; set; } = 2.0f;
        [TSerialize]
        [Category("Movement")]
        public float MouseRotateSpeed { get; set; } = 0.1f;
        [TSerialize]
        [Category("Movement")]
        public float MouseTranslateSpeed { get; set; } = 0.1f;
        [TSerialize]
        [Category("Movement")]
        public float GamepadRotateSpeed { get; set; } = 150.0f;
        [TSerialize]
        [Category("Movement")]
        public float GamepadTranslateSpeed { get; set; } = 30.0f;
        [TSerialize]
        [Category("Movement")]
        public float KeyboardTranslateSpeed { get; set; } = 10.0f;
        [TSerialize]
        [Category("Movement")]
        public float KeyboardRotateSpeed { get; set; } = 15.0f;

        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolled, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, MouseMoveType.Relative, EInputPauseType.TickAlways);

            input.RegisterButtonPressed(EMouseButton.RightClick, OnRightClick, EInputPauseType.TickAlways);

            input.RegisterButtonPressed(EKey.A, MoveLeft, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.W, MoveForward, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.S, MoveBackward, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.D, MoveRight, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.Q, MoveDown, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.E, MoveUp, EInputPauseType.TickAlways);

            input.RegisterButtonPressed(EKey.Up, PitchUp, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.Down, PitchDown, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.Left, YawLeft, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.Right, YawRight, EInputPauseType.TickAlways);
            
            input.RegisterButtonPressed(EKey.ControlLeft, OnControl, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.ControlRight, OnControl, EInputPauseType.TickAlways);

            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, OnTogglePause, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, OnTogglePause, EInputPauseType.TickAlways);

            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false, EInputPauseType.TickAlways);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false, EInputPauseType.TickAlways);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickX, OnRightStickX, false, EInputPauseType.TickAlways);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickY, OnRightStickY, false, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(GamePadButton.RightBumper, MoveUp, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(GamePadButton.LeftBumper, MoveDown, EInputPauseType.TickAlways);
        }

        private void OnTogglePause()
        {
            Engine.TogglePause(LocalPlayerController.LocalPlayerIndex);
            //if (Engine.IsPaused)
            //{
            //    LocalPlayerController.EnqueuePosession(this);
            //    LocalPlayerController.ControlledPawn = GetViewport().HUD;
            //}
        }

        private void MoveDown(bool pressed) 
            => _linearUp += KeyboardTranslateSpeed * (pressed ? -1.0f : 1.0f);
        private void MoveUp(bool pressed) 
            => _linearUp += KeyboardTranslateSpeed * (pressed ? 1.0f : -1.0f);
        private void MoveLeft(bool pressed)
            => _linearRight += KeyboardTranslateSpeed * (pressed ? -1.0f : 1.0f);
        private void MoveRight(bool pressed) 
            => _linearRight += KeyboardTranslateSpeed * (pressed ? 1.0f : -1.0f);
        private void MoveBackward(bool pressed) 
            => _linearForward += KeyboardTranslateSpeed * (pressed ? -1.0f : 1.0f);
        private void MoveForward(bool pressed)
            => _linearForward += KeyboardTranslateSpeed * (pressed ? 1.0f : -1.0f);

        private void OnLeftStickX(float value) 
            => _linearRight = value * GamepadTranslateSpeed;
        private void OnLeftStickY(float value)
            => _linearForward = value * GamepadTranslateSpeed;
        private void OnRightStickX(float value)
            => _yaw = -value * GamepadRotateSpeed;
        private void OnRightStickY(float value) 
            => _pitch = value * GamepadRotateSpeed;

        private void YawRight(bool pressed)
            => _yaw -= KeyboardRotateSpeed * (pressed ? 1.0f : -1.0f);
        private void YawLeft(bool pressed)
            => _yaw += KeyboardRotateSpeed * (pressed ? 1.0f : -1.0f);
        private void PitchDown(bool pressed)
            => _pitch -= KeyboardRotateSpeed * (pressed ? 1.0f : -1.0f);
        private void PitchUp(bool pressed)
            => _pitch += KeyboardRotateSpeed * (pressed ? 1.0f : -1.0f);

        private void OnControl(bool pressed)
            => _ctrl = pressed;
        
        protected virtual void OnRightClick(bool pressed)
        {
            _rightClickPressed = pressed;
        }

        public override void OnSpawnedPostComponentSpawn()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
            base.OnSpawnedPostComponentSpawn();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
            base.OnDespawned();
        }

        protected abstract void OnScrolled(bool up);
        protected abstract void MouseMove(float x, float y);
        protected abstract void Tick(float delta);

        #region Customizable Input
        //Dictionary<ComboModifier, Action<bool>> _combos = new Dictionary<ComboModifier, Action<bool>>();

        //private void ExecuteCombo(EMouseButton button, bool pressed)
        //{
        //    //ComboModifier mod = GetModifier(button, _alt, _ctrl, _shift);
        //    //if (_combos.ContainsKey(mod))
        //    //    _combos[mod](pressed);
        //}

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
        #endregion
    }

    //public enum CameraInputType
    //{
    //    TranslateXY,
    //    RotateYP,
    //    Select,
    //    ContextMenu,
    //}
    //[Flags]
    //public enum ComboModifier
    //{
    //    None        = 0x0,
    //    Ctrl        = 0x1,
    //    Alt         = 0x2,
    //    Shift       = 0x4,
    //    LeftClick   = 0x8,
    //    MiddleClick = 0x10,
    //    RightClick  = 0x20,
    //}
    //public class CameraInputCombo
    //{
    //    public CameraInputCombo(EMouseButton type, ComboModifier modifiers)
    //    {
    //        _type = type;
    //        _modifiers = modifiers;
    //    }
    //    public CameraInputCombo(EMouseButton type)
    //    {
    //        _type = type;
    //        _modifiers = ComboModifier.None;
    //    }
    //    public EMouseButton _type;
    //    public ComboModifier _modifiers;
    //}
}
