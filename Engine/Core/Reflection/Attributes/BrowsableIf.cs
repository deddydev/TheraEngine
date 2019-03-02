using System;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Reflection.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BrowsableIf : Attribute
    {
        public string Condition { get; }

        public BrowsableIf(string condition)
            => Condition = condition;

        public bool Evaluate(object owningObject) 
            => ExpressionParser.Evaluate<bool>(Condition, owningObject);
    }
}