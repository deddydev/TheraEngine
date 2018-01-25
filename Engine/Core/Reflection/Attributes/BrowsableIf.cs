using System;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Reflection.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BrowsableIf : Attribute
    {
        private string _condition;
        public BrowsableIf(string condition)
            => _condition = condition;
        public bool Evaluate(object owningObject) 
            => ExpressionParser.Evaluate<bool>(_condition, owningObject);
    }
}