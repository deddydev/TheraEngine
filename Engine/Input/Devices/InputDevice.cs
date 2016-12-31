using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public abstract class InputDevice : ObjectBase
    {
        public static Dictionary<InputDeviceType, InputDevice[]> CurrentDevices =
            new Dictionary<InputDeviceType, InputDevice[]>()
        {
            { InputDeviceType.Gamepad, new InputDevice[4] },
            { InputDeviceType.Keyboard, new InputDevice[4] },
            { InputDeviceType.Mouse, new InputDevice[4] },
        };

        protected ButtonManager[] _buttonStates;
        protected AxisManager[] _axisStates;

        protected int _index;
        protected bool _isConnected;

        public ConnectedStateChange ConnectionStateChanged;

        public bool IsConnected { get { return _isConnected; } }
        public int Index { get { return _index; } }

        public InputDevice(int index)
        {
            _index = index;
            Console.WriteLine(GetType().ToString() + _index + " created.");
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
            ResetStates();
        }
        protected abstract int GetButtonCount();
        protected abstract int GetAxisCount();
        private void ResetStates()
        {
            _buttonStates = new ButtonManager[GetButtonCount()];
            _axisStates = new AxisManager[GetAxisCount()];
        }
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
            return _isConnected;
        }
        internal override void Tick(float delta) { UpdateStates(delta); }
        public static void RegisterButtonEvent(ButtonManager m, ButtonInputType type, Action func, bool unregister)
        {
            if (m != null)
                switch (type)
                {
                    case ButtonInputType.Pressed:
                        m.RegisterPressed(func, unregister);
                        break;
                    case ButtonInputType.Released:
                        m.RegisterReleased(func, unregister);
                        break;
                    case ButtonInputType.Held:
                        m.RegisterHeld(func, unregister);
                        break;
                    case ButtonInputType.DoublePressed:
                        m.RegisterDoublePressed(func, unregister);
                        break;
                }
        }
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
        public void RegisterPressed(Action func, bool unregister)
        {
            if (unregister)
                _onPressed.Remove(func);
            else
                _onPressed.Add(func);
        }
        public void RegisterReleased(Action func, bool unregister)
        {
            if (unregister)
                _onReleased.Remove(func);
            else
                _onReleased.Add(func);
        }
        public void RegisterHeld(Action func, bool unregister)
        {
            if (unregister)
                _onHeld.Remove(func);
            else
                _onHeld.Add(func);
        }
        public void RegisterDoublePressed(Action func, bool unregister)
        {
            if (unregister)
                _onDoublePressed.Remove(func);
            else
                _onDoublePressed.Add(func);
        }
        public void RegisterPressedState(DelButtonState func, bool unregister)
        {
            if (unregister)
                _onStateChanged.Remove(func);
            else
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
            int i = _onPressed.Count;
            for (int x = 0; x < i; ++x)
                _onPressed[x]();
            int j = _onStateChanged.Count;
            for (int x = 0; x < j; ++x)
                _onStateChanged[x](true);
            //Console.WriteLine(_name + ": PRESSED");
        }
        private void OnReleased()
        {
            int i = _onReleased.Count;
            for (int x = 0; x < i; ++x)
                _onReleased[x]();
            int j = _onStateChanged.Count;
            for (int x = 0; x < j; ++x)
                _onStateChanged[x](false);
            //Console.WriteLine(_name + ": RELEASED");
        }
        private void OnHeld()
        {
            int i = _onHeld.Count;
            for (int x = 0; x < i; ++x)
                _onHeld[x]();
            //Console.WriteLine(_name + ": HELD");
        }
        private void OnDoublePressed()
        {
            int i = _onDoublePressed.Count;
            for (int x = 0; x < i; ++x)
                _onDoublePressed[x]();
            //Console.WriteLine(_name + ": DOUBLE PRESSED");
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
        public void RegisterAxis(DelAxisValue func, bool continuousUpdate, bool unregister)
        {
            if (unregister)
            {
                if (continuousUpdate)
                    _continuous.Remove(func);
                else
                    _onUpdate.Remove(func);
            }
            else
            {
                if (continuousUpdate)
                    _continuous.Add(func);
                else
                    _onUpdate.Add(func);
            }
        }
        private void OnAxisChanged(float realValue)
        {
            int i = _onUpdate.Count;
            for (int x = 0; x < i; ++x)
                _onUpdate[x](realValue);
            //Console.WriteLine(_name + ": " + realValue.ToString());
        }
        private void OnAxisValue(float realValue)
        {
            int i = _continuous.Count;
            for (int x = 0; x < i; ++x)
                _continuous[x](realValue);
        }
        public override void UnregisterAll()
        {
            base.UnregisterAll();
            _continuous.Clear();
            _onUpdate.Clear();
        }
    }
}
