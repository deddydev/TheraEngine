using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridControlForAttribute : Attribute
    {
        public Type[] Types { get; set; }
        public PropGridControlForAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
