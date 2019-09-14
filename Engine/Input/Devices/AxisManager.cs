using System;
using System.Collections.Generic;

namespace TheraEngine.Input.Devices
{
    public delegate void DelSendAxisValue(int axisIndex, int listIndex, float value);
    public delegate void DelAxisValue(float value);
    [Serializable]
    public class AxisManager : ButtonManager
    {
        private readonly DelSendAxisValue SendAxisToServer;
        
        public AxisManager(int index, string name, DelSendButtonPressedState onPressState, DelSendButtonAction onAction, DelSendAxisValue onAxis) 
            : base(index, name, onPressState, onAction)
        {
            SendAxisToServer = onAxis;

            int count = 6; //2 input types * 3 pause types
            _axisActions = new List<DelAxisValue>[count];
            _usedAxisActions = new List<int>(count);
        }

        public List<DelAxisValue>[] _axisActions;
        protected List<int> _usedAxisActions;
        
        private float _value = 0.0f;

        public float Value => Math.Abs(_value) > DeadZoneThreshold ? _value : 0.0f;

        public float PressedThreshold { get; set; } = 0.9f;
        public float DeadZoneThreshold { get; set; } = 0.15f;
        public float UpdateThreshold { get; set; } = 0.0001f;

        #region Registration
        public override bool IsEmpty() => base.IsEmpty() && _usedAxisActions.Count == 0;
        public void RegisterAxis(DelAxisValue func, EInputPauseType pauseType, bool continuousUpdate, bool unregister)
        {
            int index = (continuousUpdate ? 0 : 3) + (int)pauseType;
            if (unregister)
            {
                List<DelAxisValue> list = _axisActions[index];
                if (list is null)
                    return;
                list.Remove(func);
                if (list.Count == 0)
                {
                    _axisActions[index] = null;
                    _usedAxisActions.Remove(index);
                }
            }
            else
            {
                if (_actions[index] is null)
                {
                    _axisActions[index] = new List<DelAxisValue>() { func };
                    _usedAxisActions.Add(index);
                }
                else
                    _axisActions[index].Add(func);
            }
        }
        public override void UnregisterAll()
        {
            base.UnregisterAll();
            foreach (int i in _usedAxisActions)
                _axisActions[i] = null;
            _usedAxisActions.Clear();
        }
        #endregion

        #region Actions
        internal void Tick(float value, float delta)
        {
            float prev = Value;
            _value = value;
            float realValue = Value;

            OnContinuousAxisUpdate(realValue);
            if (Math.Abs(realValue - prev) > UpdateThreshold)
                OnAxisChanged(realValue);

            //Tick button events using a pressed threshold so the axis can also be used as a button
            Tick(Math.Abs(realValue) > PressedThreshold, delta);
        }
        private void OnAxisChanged(float value)
        {
            int index = 3;

            ExecuteList(index, value);

            index += Engine.IsPaused ?
                (int)EInputPauseType.TickOnlyWhenPaused :
                (int)EInputPauseType.TickOnlyWhenUnpaused;

            ExecuteList(index, value);
        }
        private void OnContinuousAxisUpdate(float value)
        {
            int index = 0;

            ExecuteList(index, value);

            index += Engine.IsPaused ?
                (int)EInputPauseType.TickOnlyWhenPaused :
                (int)EInputPauseType.TickOnlyWhenUnpaused;

            ExecuteList(index, value);
        }
        private void ExecuteList(int listIndex, float value)
        {
            List<DelAxisValue> list = _axisActions[listIndex];
            if (list != null)
            {
                SendAxisToServer(Index, listIndex, value);

                int i = list.Count;
                for (int x = 0; x < i; ++x)
                    list[x](value);
            }
        }
        #endregion

        public override string ToString() => Name;
    }
}
