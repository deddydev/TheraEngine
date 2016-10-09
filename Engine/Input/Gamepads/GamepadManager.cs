using System;
using System.Collections.Generic;

namespace CustomEngine.Input.Gamepads
{
    public enum GamePadButton
    {
        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight,
        FaceUp,
        FaceDown,
        FaceLeft,
        FaceRight,
        LeftStick,
        RightStick,
        SpecialLeft,
        SpecialRight,
        LeftBumper,
        RightBumper
    }
    public enum GamePadAxis
    {

    }
    public delegate void AxisEvent(float value);
    public abstract class GamepadManager : ObjectBase
    {
        protected bool _isConnected;
        protected int _controllerIndex;

        protected Dictionary<GamePadButton, ButtonState> _buttonStates = new Dictionary<GamePadButton, ButtonState>();
        protected Dictionary<GamePadAxis, AxisState> _axisStates = new Dictionary<GamePadAxis, AxisState>();

        public ButtonState DPadUp { get { return _buttonStates[GamePadButton.DPadUp]; } }
        public ButtonState DPadDown { get { return _buttonStates[GamePadButton.DPadDown]; } }
        public ButtonState DPadLeft { get { return _buttonStates[GamePadButton.DPadLeft]; } }
        public ButtonState DPadRight { get { return _buttonStates[GamePadButton.DPadRight]; } }

        public ButtonState FaceUp { get { return _buttonStates[GamePadButton.FaceUp]; } }
        public ButtonState FaceDown { get { return _buttonStates[GamePadButton.FaceDown]; } }
        public ButtonState FaceLeft { get { return _buttonStates[GamePadButton.FaceLeft]; } }
        public ButtonState FaceRight { get { return _buttonStates[GamePadButton.FaceRight]; } }

        public ButtonState LeftStick { get { return _buttonStates[GamePadButton.LeftStick]; } }
        public ButtonState RightStick { get { return _buttonStates[GamePadButton.RightStick]; } }
        public ButtonState LeftBumper { get { return _buttonStates[GamePadButton.LeftBumper]; } }
        public ButtonState RightBumper { get { return _buttonStates[GamePadButton.RightBumper]; } }

        public ButtonState SpecialLeft { get { return _buttonStates[GamePadButton.SpecialLeft]; } }
        public ButtonState SpecialRight { get { return _buttonStates[GamePadButton.SpecialRight]; } }

        protected AxisState
            LeftTrigger, RightTrigger,
            LeftStickY, LeftStickX,
            RightStickY, RightStickX;
        
        public bool IsConnected { get { return _isConnected; } }

        public GamepadManager()
        {
            _tickGroup = System.TickGroup.PrePhysics;
            _tickOrder = System.TickOrder.Input;
            CreateStates();
            RegisterTick();
        }

        internal override void Tick(float delta) { UpdateStates(); }
        protected abstract void UpdateStates();
        protected abstract void CreateStates();
        public abstract void Vibrate(bool left);
    }
    public class ButtonState
    {
        public ButtonState(bool exists)
        {
            _exists = exists;
        }

        private float _holdDelaySeconds = 0.2f;
        private float _secondsBetweenTaps = 0.1f;

        private bool _isPressed;
        private bool _exists;

        public event Action Pressed;
        public event Action Released;
        public event Action Held;
        public event Action DoubleTapped;
        
        public bool Exists { get { return _exists; } }
        public bool IsPressed { get { return _isPressed; } }
    }
    public class AxisState
    {
        private float _holdDelaySeconds = 0.2f;
        private float _pressedThreshold = 0.95f;
        private float _deadZoneThreshold = 0.05f;
        private bool _isPressed;
        private bool _exists;

        public float DeadZoneThreshold
        {
            get { return _deadZoneThreshold; }
            set { _deadZoneThreshold = value; }
        }

        public event Action Pressed;
        public event Action Released;
        public event Action Held;

        public bool Exists { get { return _exists; } }
        public bool IsPressed { get { return _isPressed; } }
    }
}
