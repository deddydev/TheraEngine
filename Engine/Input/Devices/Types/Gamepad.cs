using System;
using System.Collections.Generic;
using System.Linq;
using CustomEngine.Input.Devices.OpenTK;
using CustomEngine.Input.Devices.DirectX;

namespace CustomEngine.Input.Devices
{
    public enum GamePadButton : int
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
    public enum GamePadAxis : int
    {
        LeftTrigger,
        RightTrigger,
        LeftThumbstickX,
        LeftThumbstickY,
        RightThumbstickX,
        RightThumbstickY,
    }
    public delegate void ConnectedStateChange(bool nowConnected);
    /// <summary>
    /// Input for local
    /// </summary>
    public abstract class Gamepad : InputDevice
    {
        protected bool _isConnected;
        protected int _controllerIndex;
        protected ButtonManager[] _buttonStates = new ButtonManager[14];
        protected AxisManager[] _axisStates = new AxisManager[6];
        protected bool _hasCreatedStates = false;

        public ButtonManager[] ButtonStates { get { return _buttonStates; } }
        public AxisManager[] AxisStates { get { return _axisStates; } }

        public ConnectedStateChange ConnectionStateChanged;

        public int ControllerIndex { get { return _controllerIndex; } }
        public bool IsConnected { get { return _isConnected; } }

        public ButtonManager DPadUp { get { return _buttonStates[(int)GamePadButton.DPadUp]; } }
        public ButtonManager DPadDown { get { return _buttonStates[(int)GamePadButton.DPadDown]; } }
        public ButtonManager DPadLeft { get { return _buttonStates[(int)GamePadButton.DPadLeft]; } }
        public ButtonManager DPadRight { get { return _buttonStates[(int)GamePadButton.DPadRight]; } }

        public ButtonManager FaceUp { get { return _buttonStates[(int)GamePadButton.FaceUp]; } }
        public ButtonManager FaceDown { get { return _buttonStates[(int)GamePadButton.FaceDown]; } }
        public ButtonManager FaceLeft { get { return _buttonStates[(int)GamePadButton.FaceLeft]; } }
        public ButtonManager FaceRight { get { return _buttonStates[(int)GamePadButton.FaceRight]; } }

        public ButtonManager LeftStick { get { return _buttonStates[(int)GamePadButton.LeftStick]; } }
        public ButtonManager RightStick { get { return _buttonStates[(int)GamePadButton.RightStick]; } }
        public ButtonManager LeftBumper { get { return _buttonStates[(int)GamePadButton.LeftBumper]; } }
        public ButtonManager RightBumper { get { return _buttonStates[(int)GamePadButton.RightBumper]; } }

        public ButtonManager SpecialLeft { get { return _buttonStates[(int)GamePadButton.SpecialLeft]; } }
        public ButtonManager SpecialRight { get { return _buttonStates[(int)GamePadButton.SpecialRight]; } }

        public AxisManager LeftTrigger { get { return _axisStates[(int)GamePadAxis.LeftTrigger]; } }
        public AxisManager RightTrigger { get { return _axisStates[(int)GamePadAxis.RightTrigger]; } }
        public AxisManager LeftThumbstickY { get { return _axisStates[(int)GamePadAxis.LeftThumbstickY]; } }
        public AxisManager LeftThumbstickX { get { return _axisStates[(int)GamePadAxis.LeftThumbstickX]; } }
        public AxisManager RightThumbstickY { get { return _axisStates[(int)GamePadAxis.RightThumbstickY]; } }
        public AxisManager RightThumbstickX { get { return _axisStates[(int)GamePadAxis.RightThumbstickX]; } }
        
        public static Gamepad NewInstance(int controllerIndex)
        {
            switch (Engine.InputLibrary)
            {
                case InputLibrary.OpenTK:
                    return new TKGamepad(controllerIndex);
                case InputLibrary.XInput:
                    return new DXGamepad(controllerIndex);
            }
            return null;
        } 

        public Gamepad(int controllerIndex) : base(controllerIndex)
        {
            _controllerIndex = controllerIndex;
            CreateStates();
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
        }
        protected void SetButton(GamePadButton b, bool exists)
        {
            _buttonStates[(int)b] = exists ? new ButtonManager(b) : null;
        }
        protected void SetAxis(GamePadAxis a, bool exists)
        {
            _axisStates[(int)a] = exists ? new AxisManager(a) : null;
        }
        protected abstract void CreateStates();
        protected abstract void UpdateStates(float delta);
        /// <summary>
        /// Returns true if connected.
        /// </summary>
        protected bool UpdateConnected(bool isConnected)
        {
            if (_isConnected == isConnected)
                return _isConnected;
           
            _isConnected = isConnected;
            ConnectionStateChanged?.Invoke(_isConnected);

            if (_isConnected)
            {
                if (!_hasCreatedStates)
                    CreateStates();
                return true;
            }
            else if (_hasCreatedStates)
            {
                _buttonStates = new ButtonManager[14];
                _axisStates = new AxisManager[6];
                _hasCreatedStates = false;
                Console.WriteLine("Gamepad input states destroyed.");
            }
            return false;
        }
        internal override void Tick(float delta) { UpdateStates(delta); }
        /// <summary>
        /// Left motor is low freq, right motor is high freq.
        /// They are NOT the same.
        /// </summary>
        /// <param name="left">Low frequency motor speed, 0 - 1.</param>
        /// <param name="right">High frequency motor speed, 0 - 1.</param>
        public abstract void Vibrate(float left, float right);
        public void ClearVibration() { Vibrate(0.0f, 0.0f); }

        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func)
        {
            ButtonManager m = _buttonStates[(int)button];
            switch (type)
            {
                case ButtonInputType.Pressed:
                    m.Pressed += func;
                    break;
                case ButtonInputType.Released:
                    m.Released += func;
                    break;
                case ButtonInputType.Held:
                    m.Held += func;
                    break;
                case ButtonInputType.DoublePressed:
                    m.DoublePressed += func;
                    break;
            }
        }
        public void RegisterButtonEvent(GamePadAxis axis, ButtonInputType type, Action func)
        {
            AxisManager m = _axisStates[(int)axis];
            switch (type)
            {
                case ButtonInputType.Pressed:
                    m.Pressed += func;
                    break;
                case ButtonInputType.Released:
                    m.Released += func;
                    break;
                case ButtonInputType.Held:
                    m.Held += func;
                    break;
                case ButtonInputType.DoublePressed:
                    m.DoublePressed += func;
                    break;
            }
        }
        public void RegisterButtonPressed(GamePadButton button, Action<bool> func)
        {
            _buttonStates[(int)button].PressedState += func;
        }
        public void RegisterButtonPressed(GamePadAxis axis, Action<bool> func)
        {
            _axisStates[(int)axis].PressedState += func;
        }
        public void RegisterAxisUpdate(GamePadAxis axis, Action<float> func, bool continuousUpdate)
        {
            if (continuousUpdate)
                _axisStates[(int)axis].AxisValue += func;
            else
                _axisStates[(int)axis].AxisChanged += func;
        }
    }
    public class ButtonManager
    {
        public ButtonManager(GamePadButton button) { _button = button; }

        private const float TimerMax = 0.5f;

        private float _holdDelaySeconds = 0.2f;
        private float _maxSecondsBetweenPresses = 0.2f;
        private float _timer;
        private bool _isPressed;
        private GamePadButton _button;

        public event Action Pressed;
        public event Action Released;
        public event Action Held;
        public event Action DoublePressed;
        public event Action<bool> PressedState;

        public bool IsPressed { get { return _isPressed; } }

        internal void Tick(bool isPressed, float delta)
        {
            if (_isPressed != isPressed)
            {
                if (_isPressed = isPressed)
                {
                    if (_timer <= _maxSecondsBetweenPresses)
                    {
                        DoublePressed?.Invoke();
                        Console.WriteLine(_button + " DOUBLECLICKED");
                    }

                    _timer = 0.0f;
                    Pressed?.Invoke();
                    PressedState?.Invoke(true);
                }
                else
                {
                    Released?.Invoke();
                    PressedState?.Invoke(false);
                }
                Console.WriteLine(_button + ": " + _isPressed.ToString());
            }
            else if (_timer < TimerMax)
            {
                _timer += delta;
                if (_isPressed && _timer >= _holdDelaySeconds)
                {
                    _timer = TimerMax;
                    Held?.Invoke();
                    Console.WriteLine(_button + " HELD");
                }
            }
        }
    }
    public class AxisManager
    {
        public AxisManager(GamePadAxis axis) { _axis = axis; }

        private const float TimerMax = 0.5f;

        private float _updateThreshold = 0.0001f;
        private float _holdDelaySeconds = 0.2f;
        private float _maxSecondsBetweenPresses = 0.2f;
        private float _pressedThreshold = 0.95f;
        private float _deadZoneThreshold = 0.05f;
        private float _value = 0.0f, _prevValue = 0.0f;
        private float _timer;
        private bool _isPressed;
        //private bool _continuousUpdates = false;
        private GamePadAxis _axis;

        public float Value
        {
            get { return Math.Abs(_value) > _deadZoneThreshold ? _value : 0.0f; }
        }
        public float PreviousValue
        {
            get { return Math.Abs(_prevValue) > _deadZoneThreshold ? _prevValue : 0.0f; }
        }
        public float PressedThreshold
        {
            get { return _pressedThreshold; }
            set { _pressedThreshold = value; }
        }
        public float DeadZoneThreshold
        {
            get { return _deadZoneThreshold; }
            set { _deadZoneThreshold = value; }
        }
        public float UpdateThreshold
        {
            get { return _updateThreshold; }
            set { _updateThreshold = value; }
        }
        //public bool ContinuousUpdates
        //{
        //    get { return _continuousUpdates; }
        //    set { _continuousUpdates = value; }
        //}
        public bool IsPressed { get { return _isPressed; } }

        public event Action<float> AxisChanged;
        public event Action<float> AxisValue;
        public event Action Pressed;
        public event Action Released;
        public event Action Held;
        public event Action DoublePressed;
        public event Action<bool> PressedState;

        internal void Tick(float value, float delta)
        {
            _prevValue = _value;
            _value = value;

            float realValue = Value;
            float realPrev = PreviousValue;

            bool wasPressed = Math.Abs(realPrev) > _pressedThreshold;
            bool nowPressed = Math.Abs(realValue) > _pressedThreshold;

            AxisValue?.Invoke(realValue);
            if (Math.Abs(realValue - realPrev) > _updateThreshold)
            {
                AxisChanged?.Invoke(realValue);
                Console.WriteLine(_axis + ": " + realValue.ToString());
            }

            if (wasPressed != nowPressed)
            {
                if (nowPressed)
                {
                    if (_timer <= _maxSecondsBetweenPresses)
                    {
                        DoublePressed?.Invoke();
                        Console.WriteLine(_axis + " DOUBLECLICKED");
                    }

                    _timer = 0.0f;
                    _isPressed = true;
                    Pressed?.Invoke();
                    PressedState?.Invoke(true);
                    Console.WriteLine(_axis + " PRESSED");
                }
                else
                {
                    _isPressed = false;
                    Released?.Invoke();
                    PressedState?.Invoke(false);
                    Console.WriteLine(_axis + " RELEASED");
                }
            }
            else if (_timer < TimerMax)
            {
                _timer += delta;
                if (_isPressed && _timer >= _holdDelaySeconds)
                {
                    _timer = TimerMax;
                    Held?.Invoke();
                    Console.WriteLine(_axis + " HELD");
                }
            }
        }
    }
}
