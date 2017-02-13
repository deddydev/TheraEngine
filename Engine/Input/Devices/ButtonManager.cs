using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
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
}
