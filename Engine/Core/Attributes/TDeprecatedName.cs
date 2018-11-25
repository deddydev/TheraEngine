using System;

namespace TheraEngine.Core.Attributes
{
    public class TDeprecatedName : Attribute
    {
        public string Name { get; }
        
        public TDeprecatedName(string name) => Name = name;
    }
}