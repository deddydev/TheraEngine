using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemAttribute : Attribute
    {
        public Type[] Types { get; set; }
        public PropGridItemAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
