using System;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Reflection.Attributes
{
    public class BrowsableIf : Attribute
    {
        private string _condition;
        public BrowsableIf(string condition)
            => _condition = condition;
        public bool Evaluate(object owningObject) 
            => ExpressionParser.Evaluate<bool>(_condition, owningObject);
    }
}