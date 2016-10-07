using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Calls the given event with a parameter of the property's type containing the previous value when the property is changed.
    /// </summary>
    public class PostChanged : Attribute
    {
        public string _methodName;
        public PostChanged(string methodName) { _methodName = methodName; }
    }
    /// <summary>
    /// Calls the given event with a parameter of the property's type containing the previous value when the property is changed.
    /// </summary>
    public class PreChanged : Attribute
    {
        public string _methodName;
        public PreChanged(string methodName) { _methodName = methodName; }
    }
}
