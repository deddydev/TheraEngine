using System;
using System.Collections.Generic;
using Extensions;

namespace TheraEngine.Input.Devices
{
    public delegate void DelMouseScroll(bool down);
    [Serializable]
    public class ScrollWheelManager
    {
        List<(EInputPauseType PauseType, DelMouseScroll Method)> _onUpdate = new List<(EInputPauseType, DelMouseScroll)>();

        float _lastValue = 0.0f;

        public static float ScrollSpeed { get; set; } = 60.0f;

        internal void Tick(float value, float delta)
        {
            if (value.EqualTo(_lastValue))
                return;
            if (value < _lastValue)
            {
                OnUpdate(true);
                _lastValue = value;
            }
            else if (value > _lastValue)
            {
                OnUpdate(false);
                _lastValue = value;
            }
        }
        public void Register(DelMouseScroll func, EInputPauseType pauseType, bool unregister)
        {
            lock (_onUpdate)
            {
                if (unregister)
                {
                    int index = _onUpdate.FindIndex(x => x.Method == func);
                    if (index >= 0 && index < _onUpdate.Count)
                        _onUpdate.RemoveAt(index);
                }
                else
                    _onUpdate.Add((pauseType, func));
            }
        }
        private void OnUpdate(bool down)
        {
            lock (_onUpdate)
            {
                int i = _onUpdate.Count;
                for (int x = 0; x < i; ++x)
                {
                    var (PauseType, Method) = _onUpdate[x];
                    if (PauseType == EInputPauseType.TickAlways ||
                        (PauseType == EInputPauseType.TickOnlyWhenUnpaused && !Engine.IsPaused) ||
                        (PauseType == EInputPauseType.TickOnlyWhenPaused && Engine.IsPaused))
                        Method(down);
                }
            }
        }
    }
}
