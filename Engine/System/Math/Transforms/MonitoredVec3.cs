using CustomEngine;

namespace System
{
    public delegate void ValueChange(float oldValue, float newValue);
    public class MonitoredVec3
    {
        public MonitoredVec3() { }
        public MonitoredVec3(Vec3 value) { _value = value; }

        private Vec3 _value;

        public event Action XChanged;
        public event Action YChanged;
        public event Action ZChanged;
        public event ValueChange XValueChanged;
        public event ValueChange YValueChanged;
        public event ValueChange ZValueChanged;
        public event Action Changed;

        public float X
        {
            get { return _value.X; }
            set
            {
                float oldX = _value.X;
                _value.X = value;
                XChanged?.Invoke();
                XValueChanged?.Invoke(oldX, _value.X);
                Changed?.Invoke();
            }
        }
        public float Y
        {
            get { return _value.Y; }
            set
            {
                float oldY = _value.Y;
                _value.Y = value;
                YChanged?.Invoke();
                YValueChanged?.Invoke(oldY, _value.Y);
                Changed?.Invoke();
            }
        }
        public float Z
        {
            get { return _value.Z; }
            set
            {
                float oldZ = _value.Z;
                _value.Z = value;
                ZChanged?.Invoke();
                ZValueChanged?.Invoke(oldZ, _value.Z);
                Changed?.Invoke();
            }
        }

        public Vec3 Value
        {
            get { return _value; }
            set
            {
                Vec3 oldValue = _value;
                _value = value;
                XChanged?.Invoke();
                XValueChanged?.Invoke(oldValue.X, _value.X);
                YChanged?.Invoke();
                YValueChanged?.Invoke(oldValue.Y, _value.Y);
                ZChanged?.Invoke();
                ZValueChanged?.Invoke(oldValue.Z, _value.Z);
                Changed?.Invoke();
            }
        }

        public static implicit operator Vec3(MonitoredVec3 v) { return v._value; }
        public static implicit operator MonitoredVec3(Vec3 v) { return new MonitoredVec3(v); }
    }
}