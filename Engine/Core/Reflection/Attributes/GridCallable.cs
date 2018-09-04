using System;
using TheraEngine.Core.Tools;

namespace TheraEngine.Core.Reflection.Attributes
{
    /// <summary>
    /// Informs the editor that this method can be called by the user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class GridCallable : Attribute
    {
        private string _condition;
        public GridCallable(string condition = null)
        {
            _condition = condition;
        }
        public bool Evaluate(object owningObject) 
            => _condition == null ? true : ExpressionParser.Evaluate<bool>(_condition, owningObject);
    }
}