using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PropGridControlForAttribute : Attribute//, ISerializable
    {
        public Type[] Types { get; set; }
        public bool ForceMainControl { get; set; }

        public PropGridControlForAttribute(params Type[] types) : this(false, types) { }
        public PropGridControlForAttribute(bool forceMainControl, params Type[] types)
        {
            Types = types;
            ForceMainControl = forceMainControl;
        }

        //protected PropGridControlForAttribute(SerializationInfo info, StreamingContext context)
        //{
        //    if (info is null)
        //        throw new ArgumentNullException(nameof(info));

        //    int count = info.GetInt32("Count");
        //    for (int i = 0; i < count; ++i)
        //        Types[i] = (Type)TypeProxy.GetType(info.GetString(nameof(Types) + $"[{i}]"));
        //    ForceMainControl = info.GetBoolean(nameof(ForceMainControl));
        //}
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    if (info is null)
        //        throw new ArgumentNullException(nameof(info));

        //    info.AddValue("Count", Types.Length);
        //    for (int i = 0; i < Types.Length; ++i)
        //        info.AddValue(nameof(Types) + $"[{i}]", Types[i].AssemblyQualifiedName);
        //    info.AddValue(nameof(ForceMainControl), ForceMainControl);
        //}
    }
}
