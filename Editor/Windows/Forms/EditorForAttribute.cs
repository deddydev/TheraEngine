using System;
using System.Linq;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms
{        
    /// <summary>
    /// Declares that this editor should always be used to edit the specified file types.
    /// NOTE that the class this attribute is on must have a constructors for each type included in this attribute: ClassName(dataType value)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EditorForAttribute : Attribute
    {
        public TypeProxy[] DataTypes { get; set; }
        /// <summary>
        /// NOTE that the class this attribute is on must have a constructors for each type included in this attribute: ClassName(dataType value)
        /// </summary>
        /// <param name="dataTypes">All types this editor can edit.</param>
        public EditorForAttribute(params Type[] dataTypes)
            => DataTypes = dataTypes.Select(x => TypeProxy.Get(x)).ToArray();
    }
}