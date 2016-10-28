using System;
using System.Collections.Generic;

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
    public abstract class GamepadManager : ObjectBase
    {
        protected bool _isConnected;
        protected int _controllerIndex;
        protected ButtonState[] _buttonStates = new ButtonState[14];
        protected AxisState[] _axisStates = new AxisState[6];

        public ConnectedStateChange ConnectionStateChanged;

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
            _tickGroup = System.TickGroup.PrePhysics;
            _tickOrder = System.TickOrder.Input;
            CreateStates();
            RegisterTick();
        }
        protected void SetButton(GamePadButton b, bool exists)
        {
            _buttonStates[(int)b] = exists ? new ButtonState() : null;
        }
        protected void SetAxis(GamePadAxis a, bool exists)
        {
            _axisStates[(int)a] = exists ? new AxisState() : null;
        }
        protected abstract void CreateStates();
        protected abstract void UpdateStates(float delta);
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
        private float _holdDelaySeconds = 0.2f;
        private float _maxSecondsBetweenPresses = 0.1f;
        private float _timer;
        private bool _isPressed;

        public event Action Pressed;
        public event Action Released;
        public event Action Held;
        public event Action DoublePressed;
        
        public bool IsPressed { get { return _isPressed; } }

        internal void Tick(bool isPressed, float delta)
        {
            if (_isPressed != isPressed)
            {
                Console.WriteLine("Press state changed:" + _isPressed.ToString());
                if (_isPressed = isPressed)
                {
                    if (_timer <= _maxSecondsBetweenPresses)
                        DoublePressed?.Invoke();

                    _timer = 0.0f;
                    Pressed?.Invoke();
                }
                else
                    Released?.Invoke();
            }
            else
            {
                _timer += delta;
                if (_isPressed && _timer >= _holdDelaySeconds)
                    Held?.Invoke();
            }
        }
    }
    public delegate void AxisUpdate(float value);
    public class AxisState
    {
        private float _updateThreshold = 0.0001f;
        private float _holdDelaySeconds = 0.2f;
        private float _maxSecondsBetweenPresses = 0.1f;
        private float _pressedThreshold = 0.95f;
        private float _deadZoneThreshold = 0.05f;
        private float _value = 0.0f, _prevValue = 0.0f;
        private float _timer;
        private bool _isPressed;
        private bool _continuousUpdates = false;

        public float Value
        {
            get { return _value > _deadZoneThreshold ? _value : 0.0f; }
        }
        public float PreviousValue
        {
            get { return _prevValue > _deadZoneThreshold ? _prevValue : 0.0f; }
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
            bool wasPressed = _value > _pressedThreshold;
            bool nowPressed = value > _pressedThreshold;
            _prevValue = _value;
            _value = value;

            if (_continuousUpdates || Math.Abs(_value - _prevValue) > _updateThreshold)
            {
                Console.WriteLine("Axis state changed:" + _value.ToString());
                AxisUpdated?.Invoke(_value);
            }

            if (wasPressed != nowPressed)
            {
                if (nowPressed)
                {
                    if (_timer <= _maxSecondsBetweenPresses)
                        DoublePressed?.Invoke();

                    _timer = 0.0f;
                    _isPressed = true;
                    Pressed?.Invoke();
                }
                else
                {
                    _isPressed = false;
                    Released?.Invoke();
                }
            }
            else
            {
                _timer += delta;
                if (_isPressed && _timer >= _holdDelaySeconds)
                    Held?.Invoke();
            }
        }
    }
}
