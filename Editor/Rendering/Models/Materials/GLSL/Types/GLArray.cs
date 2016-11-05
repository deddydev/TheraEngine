using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLArray<T> : GLVar where T : GLVar
    {
        public override GLTypeName TypeName { get { return _elementTypeName; } }
        public T[] Value { get { return _value; } set { _value = value; } }

        public GLTypeName _elementTypeName;
        private T[] _value;

        public GLArray(T[] defaultValue, GLTypeName elementType, string name, IGLVarOwner owner) 
            : base(name, owner) { _value = defaultValue; _elementTypeName = elementType; }
    }
}
