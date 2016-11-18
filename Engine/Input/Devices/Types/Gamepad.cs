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
    public abstract class CGamePad : InputDevice
    {
        public CGamePad(int index) : base(index) { }
        public static CGamePad NewInstance(int index)
        {
            switch (Engine.InputLibrary)
            {
                case InputLibrary.OpenTK: return new TKGamepad(index);
                case InputLibrary.XInput: return new DXGamepad(index);
            }
            return null;
        }
        protected override int GetButtonCount() { return 14; }
        protected override int GetAxisCount() { return 6; }
        protected abstract bool ButtonExists(GamePadButton button);
        protected abstract List<bool> ButtonsExist(List<GamePadButton> buttons);
        protected abstract bool AxistExists(GamePadAxis axis);
        protected abstract List<bool> AxesExist(List<GamePadAxis> axes);
        private ButtonManager CacheButton(GamePadButton button)
        {
            int b = (int)button;
            if (_buttonStates[b] == null && ButtonExists(button))
               _buttonStates[b] = new ButtonManager(button.ToString());
            return _buttonStates[b];
        }
        private AxisManager CacheAxis(GamePadAxis axis)
        {
            int a = (int)axis;
            if (_axisStates[a] == null && AxistExists(axis))
                _axisStates[a] = new AxisManager(axis.ToString());
            return _axisStates[a];
        }
        private void RegisterButtonEvent(ButtonManager m, ButtonInputType type, Action func)
        {
            if (m != null)
                switch (type)
                {
                    case ButtonInputType.Pressed:
                        m.RegisterPressed(func);
                        break;
                    case ButtonInputType.Released:
                        m.RegisterReleased(func);
                        break;
                    case ButtonInputType.Held:
                        m.RegisterHeld(func);
                        break;
                    case ButtonInputType.DoublePressed:
                        m.RegisterDoublePressed(func);
                        break;
                }
        }
        public void RegisterButtonEvent(GamePadButton button, ButtonInputType type, Action func)
        {
            RegisterButtonEvent(CacheButton(button), type, func);
        }
        public void RegisterButtonEvent(GamePadAxis axis, ButtonInputType type, Action func)
        {
            RegisterButtonEvent(CacheAxis(axis), type, func);
        }
        public void RegisterButtonState(GamePadButton button, DelButtonState func)
        {
            CacheButton(button)?.RegisterPressedState(func);
        }
        public void RegisterButtonState(GamePadAxis axis, DelButtonState func)
        {
            CacheAxis(axis)?.RegisterPressedState(func);
        }
        public void RegisterAxisUpdate(GamePadAxis axis, DelAxisValue func, bool continuousUpdate)
        {
            CacheAxis(axis)?.RegisterAxis(func, continuousUpdate);
        }
        /// <summary>
        /// Left motor is low freq, right motor is high freq.
        /// They are NOT the same.
        /// </summary>
        /// <param name="left">Low frequency motor speed, 0 - 1.</param>
        /// <param name="right">High frequency motor speed, 0 - 1.</param>
        public abstract void Vibrate(float lowFreq, float highFreq);
        public void ClearVibration() { Vibrate(0.0f, 0.0f); }

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
    }
    public delegate void DelButtonState(bool pressed);
    public class ButtonManager
    {
        public ButtonManager(string name) { _name = name; }

        public List<Action>
            _onPressed = new List<Action>(),
            _onReleased = new List<Action>(),
            _onHeld = new List<Action>(),
            _onDoublePressed = new List<Action>();
        public List<DelButtonState> _onStateChanged = new List<DelButtonState>();
        
        static readonly float TimerMax = 0.5f;

        protected float _holdDelaySeconds = 0.2f;
        protected float _maxSecondsBetweenPresses = 0.2f;
        protected float _timer;
        protected bool _isPressed;
        protected string _name;

        public string Name { get { return _name; } }
        public bool IsPressed { get { return _isPressed; } }

        internal void Tick(bool isPressed, float delta)
        {
            if (_isPressed != isPressed)
            {
                if (_isPressed = isPressed)
                {
                    if (_timer <= _maxSecondsBetweenPresses)
                        OnDoublePressed();

                    _timer = 0.0f;
                    OnPressed();
                }
                else
                    OnReleased();
            }
            else if (_timer < TimerMax)
            {
                _timer += delta;
                if (_isPressed && _timer >= _holdDelaySeconds)
                {
                    _timer = TimerMax;
                    OnHeld();
                }
            }
        }
        public void RegisterPressed(Action func)
        {
            _onPressed.Add(func);
        }
        public void RegisterReleased(Action func)
        {
            _onReleased.Add(func);
        }
        public void RegisterHeld(Action func)
        {
            _onHeld.Add(func);
        }
        public void RegisterDoublePressed(Action func)
        {
            _onDoublePressed.Add(func);
        }
        public void RegisterPressedState(DelButtonState func)
        {
            _onStateChanged.Add(func);
        }
        public virtual void UnregisterAll()
        {
            _onPressed.Clear();
            _onReleased.Clear();
            _onHeld.Clear();
            _onDoublePressed.Clear();
            _onStateChanged.Clear();
        }
        private void OnPressed()
        {
            _onPressed.ForEach(del => del());
            _onStateChanged.ForEach(del => del(true));
            Console.WriteLine(_name + " PRESSED");
        }
        private void OnReleased()
        {
            _onReleased.ForEach(del => del());
            _onStateChanged.ForEach(del => del(false));
            Console.WriteLine(_name + " RELEASED");
        }
        private void OnHeld()
        {
            _onHeld.ForEach(del => del());
            Console.WriteLine(_name + " HELD");
        }
        private void OnDoublePressed()
        {
            _onDoublePressed.ForEach(del => del());
            Console.WriteLine(_name + " DOUBLE PRESSED");
        }
    }
    public delegate void DelAxisValue(float value);
    public class AxisManager : ButtonManager
    {
        public AxisManager(string name) : base(name) { }

        public List<DelAxisValue> 
            _continuous = new List<DelAxisValue>(), 
            _onUpdate = new List<DelAxisValue>();

        private float _updateThreshold = 0.0001f;
        private float _pressedThreshold = 0.9f;
        private float _deadZoneThreshold = 0.15f;
        private float _value = 0.0f;

        public float Value
        {
            get { return Math.Abs(_value) > _deadZoneThreshold ? _value : 0.0f; }
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
        internal void Tick(float value, float delta)
        {
            float prev = Value;
            _value = value;
            float realValue = Value;

            OnAxisValue(realValue);
            if (Math.Abs(realValue - prev) > _updateThreshold)
                OnAxisChanged(realValue);

            Tick(Math.Abs(realValue) > _pressedThreshold, delta);
        }
        public void RegisterAxis(DelAxisValue func, bool continuousUpdate)
        {
            if (continuousUpdate)
                _continuous.Add(func);
            else
                _onUpdate.Add(func);
        }
        private void OnAxisChanged(float realValue)
        {
            _onUpdate.ForEach(del => del(realValue));
            Console.WriteLine(_name + ": " + realValue.ToString());
        }
        private void OnAxisValue(float realValue)
        {
            _continuous.ForEach(del => del(realValue));
        }
        public override void UnregisterAll()
        {
            base.UnregisterAll();
            _continuous.Clear();
            _onUpdate.Clear();
        }
    }
}
