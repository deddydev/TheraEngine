using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Input.Devices
{
    public delegate void DelSendButtonPressedState(int buttonIndex, int listIndex, bool pressed);
    public delegate void DelSendButtonAction(int buttonIndex, int listIndex);
    public delegate void DelButtonState(bool pressed);
    [Serializable]
    public class ButtonManager
    {
        private const float TimerMax = 0.5f;

        private readonly DelSendButtonPressedState SendStateToServer;
        private readonly DelSendButtonAction SendActionToServer;

        public ButtonManager(int index, string name, DelSendButtonPressedState onPressState, DelSendButtonAction onAction)
        {
            const int count = 12;

            _actions = new List<Action>[count];
            _usedActions = new List<int>(count);

            Name = name;
            SendStateToServer = onPressState;
            SendActionToServer = onAction;
            Index = index;
        }

        public int Index { get; }
        public string Name { get; }
        
        public bool IsPressed { get; protected set; }
        public bool IsHeld { get; protected set; }
        public bool IsDoublePressed { get; protected set; }

        protected List<DelButtonState>[] _onStateChanged = new List<DelButtonState>[3];
        protected List<Action>[] _actions;
        protected List<int> _usedActions;

        protected float _holdDelaySeconds = 0.2f;
        protected float _maxSecondsBetweenPresses = 0.2f;
        protected float _timer;

        #region Registration
        public virtual bool IsEmpty() => _usedActions.Count == 0 && _onStateChanged.All(x => x is null || x.Count == 0);
        public void Register(Action func, EButtonInputType type, EInputPauseType pauseType, bool unregister)
        {
            int index = (int)type * 3 + (int)pauseType;
            if (unregister)
            {
                List<Action> list = _actions[index];
                if (list is null)
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
                if (_actions[index] is null)
                {
                    _actions[index] = new List<Action>() { func };
                    _usedActions.Add(index);
                }
                else
                    _actions[index].Add(func);
            }
        }
        public bool GetState(EButtonInputType type)
        {
            switch (type)
            {
                case EButtonInputType.Pressed:       return IsPressed;
                case EButtonInputType.Released:      return !IsPressed;
                case EButtonInputType.Held:          return IsHeld;
                case EButtonInputType.DoublePressed: return IsDoublePressed;
                default:
                    Engine.LogWarning($"Invalid {nameof(EButtonInputType)} {nameof(type)}.");
                    return false;
            }
        }
        public void RegisterPressedState(DelButtonState func, EInputPauseType pauseType, bool unregister)
        {
            int index = (int)pauseType;
            if (unregister)
            {
                List<DelButtonState> list = _onStateChanged[index];
                if (list is null)
                    return;
                list.Remove(func);
                if (list.Count == 0)
                    _onStateChanged[index] = null;
            }
            else
            {
                if (_onStateChanged[index] is null)
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
        #endregion

        #region Actions
        internal void Tick(bool isPressed, float delta)
        {
            if (IsPressed != isPressed)
            {
                if (isPressed)
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
                if (IsPressed && _timer >= _holdDelaySeconds)
                {
                    _timer = TimerMax;
                    OnHeld();
                }
            }
        }
        private void OnPressed()
        {
            IsPressed = true;
            PerformAction(EButtonInputType.Pressed);
            PerformStateAction(true);
        }
        private void OnReleased()
        {
            IsPressed = false;
            IsHeld = false;
            IsDoublePressed = false;
            PerformAction(EButtonInputType.Released);
            PerformStateAction(false);
        }
        private void OnHeld()
        {
            IsHeld = true;
            PerformAction(EButtonInputType.Held);
        }
        private void OnDoublePressed()
        {
            IsDoublePressed = true;
            PerformAction(EButtonInputType.DoublePressed);
        }
        protected void PerformStateAction(bool pressed)
        {
            int index = (int)EInputPauseType.TickAlways;

            //Always ticking list
            ExecutePressedStateList(index, pressed);

            index = Engine.IsPaused ?
                (int)EInputPauseType.TickOnlyWhenPaused :
                (int)EInputPauseType.TickOnlyWhenUnpaused;

            //Pause dependent list
            ExecutePressedStateList(index, pressed);
        }
        protected void PerformAction(EButtonInputType type)
        {
            int index = (int)type * 3;

            //Always ticking list
            ExecuteActionList(index);

            index += Engine.IsPaused ?
                (int)EInputPauseType.TickOnlyWhenPaused : 
                (int)EInputPauseType.TickOnlyWhenUnpaused;

            //Pause dependent list
            ExecuteActionList(index);
        }
        private void ExecuteActionList(int listIndex)
        {
            List<Action> list = _actions[listIndex];
            if (list is null)
                return;

            //Inform the server of the input
            SendActionToServer?.Invoke(Index, listIndex);

            //Run the input locally
            int i = list.Count;
            for (int x = 0; x < i; ++x)
                list[x]();
        }
        private void ExecutePressedStateList(int listIndex, bool pressed)
        {
            List<DelButtonState> list = _onStateChanged[listIndex];
            if (list is null)
                return;

            //Inform the server of the input
            SendStateToServer?.Invoke(Index, listIndex, pressed);

            //Run the input locally
            int i = list.Count;
            for (int x = 0; x < i; ++x)
                list[x](pressed);
        }
        #endregion

        public override string ToString() => Name;
    }
}
