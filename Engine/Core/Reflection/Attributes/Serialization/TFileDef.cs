using System;
using System.Runtime.Serialization;

namespace TheraEngine.ComponentModel
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TFileDef : Attribute, ISerializable
    {
        public string UserFriendlyName { get; set; }
        public string Description { get; set; }

        public TFileDef(string userFriendlyName, string description = null)
        {
            UserFriendlyName = userFriendlyName;
            Description = description;
        }
        public TFileDef(SerializationInfo info, StreamingContext context)
        {
            UserFriendlyName = info.GetString(nameof(UserFriendlyName));
            Description = info.GetString(nameof(Description));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(UserFriendlyName), UserFriendlyName);
            info.AddValue(nameof(Description), Description);
        }
    }
}
