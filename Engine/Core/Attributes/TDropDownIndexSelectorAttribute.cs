using System;

namespace TheraEngine.Core.Attributes
{
    public class TDropDownIndexSelectorAttribute : Attribute
    {
        public string Target { get; }

        public TDropDownIndexSelectorAttribute(string target)
        {
            Target = target;
        }
    }
}