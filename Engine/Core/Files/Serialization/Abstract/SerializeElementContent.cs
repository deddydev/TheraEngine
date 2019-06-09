using System;

namespace TheraEngine.Core.Files.Serialization
{
    public class SerializeElementContent : TObject
    {
        public SerializeElementContent() { }
        public SerializeElementContent(object value) => SetValueAsObject(value);
        
        public SerializeElement Parent { get; internal set; }

        private object _value;
        private string _stringValue;
        private Type _valueType;

        public bool IsNotNull => _value != null || _stringValue != null;
        public bool SetValueAsObject(object o)
        {
            _value = o;
            _valueType = _value?.GetType();

            if (_value == null)
            {
                IsNonStringObject = true;
                return false;
            }

            var serializer = Engine.DomainProxy.DetermineObjectSerializer(_valueType, true);

            string str = null;
            IsNonStringObject = serializer == null || !serializer.ObjectToString(_value, out str);
            _stringValue = str;
            
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
            BaseObjectSerializer serializer = Engine.DomainProxy.DetermineObjectSerializer(type, true);
            if (serializer == null)
            {
                _valueType = null;
                return false;
            }

            IsNonStringObject = !serializer.ObjectFromString(type, _stringValue, out _value);
            _valueType = type;
            
            return true;
        }
        public bool GetObject(Type expectedType, out object value)
        {
            if (expectedType == null)
            {
                value = _value;
                return _value != null && !IsUnparsedString;
            }

            bool success = IsNotNull && (IsUnparsedString ? ParseStringToObject(expectedType) : (_value == null || expectedType.IsInstanceOfType(_value)));

            value = success ? _value : default;

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
