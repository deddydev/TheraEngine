using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Input.Devices
{
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
