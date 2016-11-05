using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLBool : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._bool; } }
        public bool Value { get { return _value; } set { _value = value; } }
        
        private bool _value;

        public GLBool(bool defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLFloat : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._float; } }
        public float Value { get { return _value; } set { _value = value; } }

        private float _value;

        public GLFloat(float defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLDouble : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._double; } }
        public double Value { get { return _value; } set { _value = value; } }

        private double _value;

        public GLDouble(double defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLInt : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._int; } }
        public int Value { get { return _value; } set { _value = value; } }

        private int _value;

        public GLInt(int defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLUInt : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._uint; } }
        public uint Value { get { return _value; } set { _value = value; } }

        private uint _value;

        public GLUInt(uint defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
}
