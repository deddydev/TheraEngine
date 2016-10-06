using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class ChangedEvent : Attribute
    {
        public string _methodName;
        public ChangedEvent(string methodName) { _methodName = methodName; }
    }
}
