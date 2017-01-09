using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public enum InputPauseType
    {
        TickAlways              = 0,
        TickOnlyWhenUnpaused    = 1,
        TickOnlyWhenPaused      = 2,
    }
    public abstract class InputDevice : ObjectBase
    {
        public static Dictionary<InputDeviceType, InputDevice[]> CurrentDevices =
            new Dictionary<InputDeviceType, InputDevice[]>()
        {
            { InputDeviceType.Gamepad,  new InputDevice[4] },
            { InputDeviceType.Keyboard, new InputDevice[4] },
            { InputDeviceType.Mouse,    new InputDevice[4] },
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
            //Console.WriteLine(GetType().ToString() + _index + " created.");
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
        public static void RegisterButtonEvent(ButtonManager m, ButtonInputType type, InputPauseType pauseType, Action func, bool unregister)
        {
            m?.Register(func, type, pauseType, unregister);
        }
    }
    public delegate void DelButtonState(bool pressed);
    public class ButtonManager
    {
        public ButtonManager(string name)
        {
            int count = 12;
            _actions = new List<Action>[count];
            _usedActions = new List<int>(count);
            _name = name;
        }

        public List<DelButtonState>[] _onStateChanged = new List<DelButtonState>[3];

        const float TimerMax = 0.5f;

        public string Name { get { return _name; } }
        public bool IsPressed { get { return _isPressed; } }

        protected float _holdDelaySeconds = 0.2f;
        protected float _maxSecondsBetweenPresses = 0.2f;
        protected float _timer;
        protected bool _isPressed;

        protected string _name;
        protected List<Action>[] _actions;
        protected List<int> _usedActions;

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
        public void Register(Action func, ButtonInputType type, InputPauseType pauseType, bool unregister)
        {
            int index = (int)type * (int)pauseType;
            if (unregister)
            {
                List<Action> list = _actions[index];
                if (list == null)
                    return;
                list.Remove(func);
                if (list.Count == 0)
                {
                    _actions[index] = null;
                    _usedActions.Remove(index);
                }
            }
            else
            {
                if (_actions[index] == null)
                {
                    _actions[index] = new List<Action>() { func };
                    _usedActions.Add(index);
                }
                else
                    _actions[index].Add(func);
            }
        }
        public void RegisterPressedState(DelButtonState func, InputPauseType pauseType, bool unregister)
        {
            int index = (int)pauseType;
            if (unregister)
            {
                List<DelButtonState> list = _onStateChanged[index];
                if (list == null)
                    return;
                list.Remove(func);
                if (list.Count == 0)
                    _onStateChanged[index] = null;
            }
            else
            {
                if (_onStateChanged[index] == null)
                    _onStateChanged[index] = new List<DelButtonState>() { func };
                else
                    _onStateChanged[index].Add(func);
            }
        }
        public virtual void UnregisterAll()
        {
            foreach (int i in _usedActions)
                _actions[i] = null;
            _usedActions.Clear();
            for (int i = 0; i < 3; ++i)
                _onStateChanged[i] = null;
        }
        private void OnPressed()
        {
            PerformAction(ButtonInputType.Pressed);
            PerformStateAction(true);
        }
        private void OnReleased()
        {
            PerformAction(ButtonInputType.Released);
            PerformStateAction(false);
        }
        private void OnHeld()
        {
            PerformAction(ButtonInputType.Held);
        }
        private void OnDoublePressed()
        {
            PerformAction(ButtonInputType.DoublePressed);
        }
        protected void PerformStateAction(bool pressed)
        {
            List<DelButtonState> list = _onStateChanged[(int)InputPauseType.TickAlways];
            if (list != null)
            {
                int i = list.Count;
                for (int x = 0; x < i; ++x)
                    list[x](pressed);
            }
            list = Engine.IsPaused ?
                _onStateChanged[(int)InputPauseType.TickOnlyWhenPaused] :
                _onStateChanged[(int)InputPauseType.TickOnlyWhenUnpaused];
            if (list != null)
            {
                int i = list.Count;
                for (int x = 0; x < i; ++x)
                    list[x](pressed);
            }
        }
        protected void PerformAction(ButtonInputType type)
        {
            int index = (int)type * 3;
            List<Action> list = _actions[index + (int)InputPauseType.TickAlways];
            if (list != null)
            {
                int i = list.Count;
                for (int x = 0; x < i; ++x)
                    list[x]();
            }
            list = Engine.IsPaused ?
                _actions[index + (int)InputPauseType.TickOnlyWhenPaused] :
                _actions[index + (int)InputPauseType.TickOnlyWhenUnpaused];
            if (list != null)
            {
                int i = list.Count;
                for (int x = 0; x < i; ++x)
                    list[x]();
            }
            //Console.WriteLine(_name + ": " + type.ToString());
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
        public void RegisterAxis(DelAxisValue func, InputPauseType pauseType, bool continuousUpdate, bool unregister)
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
