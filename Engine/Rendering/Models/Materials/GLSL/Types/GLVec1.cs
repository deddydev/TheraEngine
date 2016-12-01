using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLBool : GLVar, IUniformable1Bool
    {
        public override GLTypeName TypeName { get { return GLTypeName._bool; } }
        public bool Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public unsafe bool* Data { get { fixed (bool* ptr = &_value) return ptr; } }
        public override string GetValueString() { return _value.ToString(); }

        private bool _value;

        public GLBool(bool defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLFloat : GLVar, IUniformable1Float
    {
        public override GLTypeName TypeName { get { return GLTypeName._float; } }
        public float Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public unsafe float* Data { get { fixed (float* ptr = &_value) return ptr; } }
        public override string GetValueString() { return _value.ToString(); }

        private float _value;

        public GLFloat(float defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLDouble : GLVar, IUniformable1Double
    {
        public override GLTypeName TypeName { get { return GLTypeName._double; } }
        public double Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public unsafe double* Data { get { fixed (double* ptr = &_value) return ptr; } }
        public override string GetValueString() { return _value.ToString(); }

        private double _value;

        public GLDouble(double defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLInt : GLVar, IUniformable1Int
    {
        public override GLTypeName TypeName { get { return GLTypeName._int; } }
        public int Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public unsafe int* Data { get { fixed (int* ptr = &_value) return ptr; } }
        public override string GetValueString() { return _value.ToString(); }

        private int _value;

        public GLInt(int defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
    public class GLUInt : GLVar, IUniformable1UInt
    {
        public override GLTypeName TypeName { get { return GLTypeName._uint; } }
        public uint Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public unsafe uint* Data { get { fixed (uint* ptr = &_value) return ptr; } }
        public override string GetValueString() { return _value.ToString(); }

        private uint _value;

        public GLUInt(uint defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
        }
    }
}
