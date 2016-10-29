using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Input.Gamepads
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
    public abstract class GamepadAwaiter : ObjectBase, IDisposable
    {
        protected static List<GamepadAwaiter> CurrentAwaiters = new List<GamepadAwaiter>();

        public event Action<int> FoundController;

        public GamepadAwaiter(Action<int> uponFound)
        {
            FoundController += uponFound;
            RegisterTick(System.TickGroup.PrePhysics, System.TickOrder.Input);
        }
        ~GamepadAwaiter() { Dispose(); }

        public void Dispose()
        {
            CurrentAwaiters.Remove(this);
            UnregisterTick();
        }

        internal override void Tick(float delta)
        {
            List<int> connected = GetConnected();
            List<int> alreadyBound = GamepadManager.CurrentGamepads.Select(x => x.ControllerIndex).ToList();
            foreach (int i in connected)
                if (!alreadyBound.Contains(i))
                {
                    FoundController?.Invoke(i);
                    Dispose();
                }
        }

        protected abstract List<int> GetConnected();
    }
    public abstract class GamepadManager : ObjectBase
    {
        public static List<GamepadManager> CurrentGamepads = new List<GamepadManager>();

        protected bool _isConnected;
        protected int _controllerIndex;
        protected ButtonState[] _buttonStates = new ButtonState[14];
        protected AxisState[] _axisStates = new AxisState[6];
        protected bool _hasCreatedStates = false;

        public ConnectedStateChange ConnectionStateChanged;

        public int ControllerIndex { get { return _controllerIndex; } }
        public bool IsConnected { get { return _isConnected; } }

        public ButtonState DPadUp { get { return _buttonStates[(int)GamePadButton.DPadUp]; } }
        public ButtonState DPadDown { get { return _buttonStates[(int)GamePadButton.DPadDown]; } }
        public ButtonState DPadLeft { get { return _buttonStates[(int)GamePadButton.DPadLeft]; } }
        public ButtonState DPadRight { get { return _buttonStates[(int)GamePadButton.DPadRight]; } }

        public ButtonState FaceUp { get { return _buttonStates[(int)GamePadButton.FaceUp]; } }
        public ButtonState FaceDown { get { return _buttonStates[(int)GamePadButton.FaceDown]; } }
        public ButtonState FaceLeft { get { return _buttonStates[(int)GamePadButton.FaceLeft]; } }
        public ButtonState FaceRight { get { return _buttonStates[(int)GamePadButton.FaceRight]; } }

        public ButtonState LeftStick { get { return _buttonStates[(int)GamePadButton.LeftStick]; } }
        public ButtonState RightStick { get { return _buttonStates[(int)GamePadButton.RightStick]; } }
        public ButtonState LeftBumper { get { return _buttonStates[(int)GamePadButton.LeftBumper]; } }
        public ButtonState RightBumper { get { return _buttonStates[(int)GamePadButton.RightBumper]; } }

        public ButtonState SpecialLeft { get { return _buttonStates[(int)GamePadButton.SpecialLeft]; } }
        public ButtonState SpecialRight { get { return _buttonStates[(int)GamePadButton.SpecialRight]; } }

        public AxisState LeftTrigger { get { return _axisStates[(int)GamePadAxis.LeftTrigger]; } }
        public AxisState RightTrigger { get { return _axisStates[(int)GamePadAxis.RightTrigger]; } }
        public AxisState LeftThumbstickY { get { return _axisStates[(int)GamePadAxis.LeftThumbstickY]; } }
        public AxisState LeftThumbstickX { get { return _axisStates[(int)GamePadAxis.LeftThumbstickX]; } }
        public AxisState RightThumbstickY { get { return _axisStates[(int)GamePadAxis.RightThumbstickY]; } }
        public AxisState RightThumbstickX { get { return _axisStates[(int)GamePadAxis.RightThumbstickX]; } }
        
        public GamepadManager(int controllerIndex)
        {
            _controllerIndex = controllerIndex;
            CreateStates();
            RegisterTick(System.TickGroup.PrePhysics, System.TickOrder.Input);
            CurrentGamepads.Add(this);
        }
        ~GamepadManager() { CurrentGamepads.Remove(this); }
        protected void SetButton(GamePadButton b, bool exists)
        {
            _buttonStates[(int)b] = exists ? new ButtonState(b) : null;
        }
        protected void SetAxis(GamePadAxis a, bool exists)
        {
            _axisStates[(int)a] = exists ? new AxisState(a) : null;
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
                _buttonStates = new ButtonState[14];
                _axisStates = new AxisState[6];
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
    }
    public class ButtonState
    {
        public ButtonState(GamePadButton button) { _button = button; }

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
                }
                else
                    Released?.Invoke();
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
    public delegate void AxisUpdate(float value);
    public class AxisState
    {
        public AxisState(GamePadAxis axis) { _axis = axis; }

        private const float TimerMax = 0.5f;

        private float _updateThreshold = 0.0001f;
        private float _holdDelaySeconds = 0.2f;
        private float _maxSecondsBetweenPresses = 0.2f;
        private float _pressedThreshold = 0.95f;
        private float _deadZoneThreshold = 0.05f;
        private float _value = 0.0f, _prevValue = 0.0f;
        private float _timer;
        private bool _isPressed;
        private bool _continuousUpdates = false;
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
        public bool ContinuousUpdates
        {
            get { return _continuousUpdates; }
            set { _continuousUpdates = value; }
        }
        public bool IsPressed { get { return _isPressed; } }

        public event AxisUpdate AxisUpdated;
        public event Action Pressed;
        public event Action Released;
        public event Action Held;
        public event Action DoublePressed;

        internal void Tick(float value, float delta)
        {
            _prevValue = _value;
            _value = value;

            float realValue = Value;
            float realPrev = PreviousValue;

            bool wasPressed = Math.Abs(realPrev) > _pressedThreshold;
            bool nowPressed = Math.Abs(realValue) > _pressedThreshold;

            if (_continuousUpdates || Math.Abs(realValue - realPrev) > _updateThreshold)
            {
                AxisUpdated?.Invoke(realValue);
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
                    Console.WriteLine(_axis + " PRESSED");
                }
                else
                {
                    _isPressed = false;
                    Released?.Invoke();
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
