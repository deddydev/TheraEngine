using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PropGridControlForAttribute : Attribute
    {
        public Type[] Types { get; set; }
        public bool ForceMainControl { get; set; }

        public PropGridControlForAttribute(params Type[] types) : this(false, types) { }
        public PropGridControlForAttribute(bool forceMainControl, params Type[] types)
        {
            Types = types;
            ForceMainControl = forceMainControl;
        }
    }
}
