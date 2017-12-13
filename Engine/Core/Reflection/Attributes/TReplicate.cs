using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core.Reflection.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TReplicate : Attribute
    {
        public string EventMethodName { get; set; }

        public TReplicate() { }
        public TReplicate(string eventMethodName) 
            => EventMethodName = eventMethodName;
    }
}
