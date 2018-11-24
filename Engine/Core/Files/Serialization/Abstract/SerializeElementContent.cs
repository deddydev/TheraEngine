using System;

namespace TheraEngine.Core.Files.Serialization
{
    public class SerializeElementContent
    {
        public SerializeElementContent() { }
        public SerializeElementContent(object value) => SetValueAsObject(value);
        
        private object _value;
        private string _stringValue;
        private Type _valueType;

        public bool IsNotNull => _value != null || _stringValue != null;
        public bool SetValueAsObject(object o)
        {
            _value = o;
            _valueType = _value?.GetType();
            IsNonStringObject = _value != null ? !SerializationCommon.GetString(_value, _valueType, out _stringValue) : false;
            return !IsNonStringObject;
        }
        public void SetValueAsString(string o)
        {
            _stringValue = o;
            _valueType = null;
            IsNonStringObject = false;
        }
        public bool SetValueAsString(string o, Type type)
        {
            _stringValue = o;
            _valueType = null;
            IsNonStringObject = false;
            return ParseStringToObject(type);
        }

        public bool IsNonStringObject { get; private set; } = false;
        public bool IsUnparsedString => _valueType == null;

        private bool ParseStringToObject(Type type)
        {
            bool canParse = SerializationCommon.IsSerializableAsString(type);
            if (canParse)
            {
                _value = SerializationCommon.ParseString(_stringValue, type);
                _valueType = type;
            }
            else
                _valueType = null;
            IsNonStringObject = false;
            return canParse;
        }
        public bool GetObject(Type expectedType, out object value)
        {
            if (expectedType == null)
            {
                value = _value;
                return _value != null && !IsUnparsedString;
            }

            bool success = IsNotNull && (IsUnparsedString ?
                ParseStringToObject(expectedType) : 
                (_value == null ? true : expectedType.IsAssignableFrom(_value.GetType())));

            if (success)
                value = _value;
            else
                value = default;

            return success;
        }
        public bool GetObjectAs<T>(out T value)
        {
            bool success = IsNotNull && (IsUnparsedString ? ParseStringToObject(typeof(T)) : _value is T);
            if (success)
                value = (T)_value;
            else
                value = default;
            return success;
        }
        public bool GetString(out string value)
        {
            if (!IsNotNull || IsNonStringObject)
            {
                value = null;
                return false;
            }
            value = _stringValue;
            return true;
        }

        public static SerializeElementContent FromString(string value)
        {
            SerializeElementContent attrib = new SerializeElementContent(null);
            attrib.SetValueAsString(value);
            return attrib;
        }
        public static SerializeElementContent FromString(string value, Type objectType, out bool parseSucceeded)
        {
            SerializeElementContent attrib = new SerializeElementContent(null);
            parseSucceeded = attrib.SetValueAsString(value, objectType);
            return attrib;
        }
    }
}
