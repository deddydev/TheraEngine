using System;

namespace TheraEditor.Windows.Forms
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EditorForAttribute : Attribute
    {
        public Type[] DataTypes { get; set; }
        public EditorForAttribute(params Type[] dataTypes)
        {
            DataTypes = dataTypes;
        }
    }
}