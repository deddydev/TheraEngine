using System;

namespace CustomEngine.Audio
{
    public class AudioParameters
    {
        public UsableValue<Vec3> Position;
    }
    public struct UsableValue<T> where T : struct
    {
        public UsableValue(T value, bool use = false)
        {
            _value = value;
            _use = use;
        }

        public T _value;
        public bool _use;

        public static implicit operator UsableValue<T>(T value)
        {
            return new UsableValue<T>(value);
        }
        public static implicit operator UsableValue<T>(bool use)
        {
            return new UsableValue<T>(default(T), use);
        }
        public static implicit operator T(UsableValue<T> value)
        {
            if (!value._use)
                throw new Exception("Value not set.");
            return value._value;
        }
    }
}