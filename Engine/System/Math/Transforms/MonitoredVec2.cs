namespace System
{
    public class MonitoredVec2
    {
        public MonitoredVec2() { }
        public MonitoredVec2(Vec2 value) { _value = value; }
        
        private Vec2 _value;

        public event Action XChanged;
        public event Action YChanged;
        public event ValueChange XValueChanged;
        public event ValueChange YValueChanged;
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

        public static implicit operator Vec2(MonitoredVec2 v) { return v._value; }
        public static implicit operator MonitoredVec2(Vec2 v) { return new MonitoredVec2(v); }
    }
}