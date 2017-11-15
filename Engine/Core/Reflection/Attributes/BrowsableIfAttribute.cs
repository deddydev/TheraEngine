using System;
using TheraEngine.Tools;

namespace TheraEngine.Core.Reflection.Attributes
{
    public class BrowsableIfAttribute : Attribute
    {
        private string _condition;
        public BrowsableIfAttribute(string condition)
            => _condition = condition;
        public bool Evaluate(object owningObject) 
            => ExpressionParser.Evaluate<bool>(_condition, owningObject);
    }
}