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
        public string DisplayName => _displayName;

        private string _condition, _displayName;
        public GridCallable(string displayName = null, string condition = null)
        {
            _displayName = displayName;
            _condition = condition;
        }
        public bool Evaluate(object owningObject) 
            => _condition == null ? true : ExpressionParser.Evaluate<bool>(_condition, owningObject);
    }
}