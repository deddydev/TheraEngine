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
        public GridCallable(string condition = null)
            => Condition = condition;

        public string Condition { get; set; }

        public bool Evaluate(object owningObject) 
            => Condition == null ? true : ExpressionParser.Evaluate<bool>(Condition, owningObject);
    }
}