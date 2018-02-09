using TheraEngine.Input;
using TheraEngine.Input.Devices;
using System;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Components.Scene;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Core.Shapes;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Worlds;

namespace TheraEngine.Actors.Types.Pawns
{
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
    public class FlyingCameraPawn : Pawn<TRLaggedComponent>
    {
        public FlyingCameraPawn() : base() { }
        public FlyingCameraPawn(LocalPlayerIndex possessor) : base(false, possessor) { }

        protected override TRLaggedComponent OnConstruct()
        {
            TRLaggedComponent root = new TRLaggedComponent();
            ScreenShakeComponent = new ScreenShake3DComponent();
            Camera = new PerspectiveCamera();
            CameraComponent cam = new CameraComponent(Camera);
            ScreenShakeComponent.ChildComponents.Add(cam);
            root.ChildComponents.Add(ScreenShakeComponent);
            return root;
        }

        public PerspectiveCamera Camera { get; private set; }
        public ScreenShake3DComponent ScreenShakeComponent { get; private set; }
        
        float 
            _linearRight = 0.0f,
            _linearForward = 0.0f,
            _linearUp = 0.0f,
            _pitch = 0.0f,
            _yaw = 0.0f;

        bool 
            _ctrl = false,
            _alt = false,
            _shift = false,
            _rightClickPressed = false, 
            _middleClickPressed = false,
            _leftClickPressed = false;
        
        [Browsable(false)]
        bool Rotating => _rightClickPressed && _ctrl;
        [Browsable(false)]
        bool Translating => _rightClickPressed && !_ctrl;

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

        //protected override void PostConstruct()
        //{
        //    //RootComponent.Translation = new Vec3(0.0f, 20.0f, -40.0f);
        //    //RootComponent.Rotation.Pitch = -10.0f;
        //    //Camera_TransformChanged();
        //    base.PostConstruct();
        //}

        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolled, EInputPauseType.TickAlways);
            input.RegisterMouseMove(MouseMove, true, EInputPauseType.TickAlways);

            input.RegisterButtonPressed(EMouseButton.RightClick, OnRightClick, EInputPauseType.TickAlways);

            input.RegisterButtonPressed(EKey.A, MoveLeft, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.W, MoveForward, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.S, MoveBackward, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.D, MoveRight, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.Q, MoveDown, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.E, MoveUp, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(EKey.T, ButtonInputType.Pressed, AddTrauma, EInputPauseType.TickAlways);

            input.RegisterButtonPressed(EKey.ControlLeft, OnControl, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.ControlRight, OnControl, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.AltLeft, OnAlt, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.AltRight, OnAlt, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.ShiftLeft, OnShift, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(EKey.ShiftRight, OnShift, EInputPauseType.TickAlways);

            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, OnTogglePause, EInputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, OnTogglePause, EInputPauseType.TickAlways);

            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false, EInputPauseType.TickAlways);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false, EInputPauseType.TickAlways);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickX, OnRightStickX, false, EInputPauseType.TickAlways);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickY, OnRightStickY, false, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(GamePadButton.RightBumper, MoveUp, EInputPauseType.TickAlways);
            input.RegisterButtonPressed(GamePadButton.LeftBumper, MoveDown, EInputPauseType.TickAlways);
        }

        private void AddTrauma()
        {
            ScreenShakeComponent.Trauma += 0.4f;
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

        private void OnControl(bool pressed)
            => _ctrl = pressed;
        private void OnAlt(bool pressed)
            => _alt = pressed;
        private void OnShift(bool pressed)
            => _shift = pressed;

        private void OnScrolled(bool up)
        {
            if (_alt || _shift)
                return;

            if (_ctrl)
                Engine.TimeDilation *= up ? 0.8f : 1.2f;
            else
            {
                RootComponent.TranslateRelative(0.0f, 0.0f, up ? ScrollSpeed : -ScrollSpeed);
            }
        }
        
        private void OnRightClick(bool pressed)
        {
            _rightClickPressed = pressed;
            Viewport v = LocalPlayerController.Viewport;
            Vec2 viewportPoint = v.AbsoluteToRelative(HUD.CursorPosition);

            Segment s = v.GetWorldSegment(viewportPoint);
            RayTraceClosest c = new RayTraceClosest(s.StartPoint, s.EndPoint, 0, 0xFFFF);
            if (_hasHit = c.Trace())
                _screenPoint = Camera.WorldToScreen(c.HitPointWorld);
        }
        
        bool _hasHit = false;
        Vec3 _screenPoint;
        public void MouseMove(float x, float y)
        {
            if (Rotating)
            {
                float pitch = -y * MouseRotateSpeed;
                float yaw = -x * MouseRotateSpeed;
                if (_hasHit)
                    RootComponent.Pivot(pitch, yaw, Camera.ScreenToWorld(_screenPoint));
                else
                    RootComponent.DesiredRotation.AddRotations(pitch, yaw, 0.0f);
                //RootComponent.DesiredRotation.RemapToRange(-180.0f, 180.0f);
            }
            else if (Translating)
            {
                if (_hasHit)
                {
                    Vec3 oldPoint = Camera.ScreenToWorld(_screenPoint);
                    _screenPoint.X += -x;
                    _screenPoint.Y += y;
                    Vec3 newPoint = Camera.ScreenToWorld(_screenPoint);
                    Vec3 diff = newPoint - oldPoint;
                    RootComponent.DesiredTranslation += diff;
                }
                else
                    RootComponent.TranslateRelative(-x * MouseTranslateSpeed, y * MouseTranslateSpeed, 0.0f);
            }
        }
        public override void OnSpawnedPostComponentSetup()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
            base.OnSpawnedPostComponentSetup();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, Tick);
            base.OnDespawned();
        }
        private void Tick(float delta)
        {
            bool translate = !(_linearRight.IsZero() && _linearUp.IsZero() && _linearForward.IsZero());
            bool rotate = !(_pitch.IsZero() && _yaw.IsZero());
            if (translate)
                RootComponent.TranslateRelative(new Vec3(_linearRight, _linearUp, -_linearForward) * delta);
            if (rotate)
            {
                RootComponent.DesiredRotation.AddRotations(_pitch * delta, _yaw * delta, 0.0f);
                //RootComponent.DesiredRotation.RemapToRange(-180.0f, 180.0f);
            }
        }

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
}
