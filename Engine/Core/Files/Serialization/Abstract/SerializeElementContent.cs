using System;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.Serialization
{
    public class SerializeElementContent : TObjectSlim
    {
        public SerializeElementContent() { }
        public SerializeElementContent(object value) => SetValueAsObject(value);
        
        public SerializeElement Parent { get; internal set; }

        private object _value;
        private string _stringValue;
        private TypeProxy _valueType;

        public bool IsNotNull => _value != null || _stringValue != null;
        public bool SetValueAsObject(object o)
        {
            _value = o;
            _valueType = _value?.GetType();

            if (_value is null)
            {
                IsNonStringObject = true;
                return false;
            }

            var serializer = BaseObjectSerializer.DetermineObjectSerializer(_valueType, true);

            string str = null;
            IsNonStringObject = serializer is null || !serializer.ObjectToString(_value, out str);
            _stringValue = str;
            
            return !IsNonStringObject;
        }

        public void SetValueAsString(string o)
        {
            _stringValue = o;
            _valueType = null;
            IsNonStringObject = false;
        }
        public bool SetValueAsString(string o, TypeProxy type)
        {
            _stringValue = o;
            _valueType = null;
            IsNonStringObject = false;
            return ParseStringToObject(type);
        }

        public bool IsNonStringObject { get; private set; } = false;
        public bool IsUnparsedString => _valueType is null;

        private bool ParseStringToObject(TypeProxy type)
        {
            BaseObjectSerializer serializer = BaseObjectSerializer.DetermineObjectSerializer(type, true);
            if (serializer is null)
            {
                _valueType = null;
                return false;
            }

            IsNonStringObject = !serializer.ObjectFromString(type, _stringValue, out _value);
            _valueType = type;
            
            return true;
        }
        public bool GetObject(TypeProxy expectedType, out object value)
        {
            if (expectedType is null)
            {
                value = _value;
                return _value != null && !IsUnparsedString;
            }

            bool success = IsNotNull && (IsUnparsedString ? ParseStringToObject(expectedType) : (_value is null || expectedType.IsInstanceOfType(_value)));

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
