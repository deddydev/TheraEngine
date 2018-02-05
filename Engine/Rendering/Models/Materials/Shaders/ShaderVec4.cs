﻿using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderBVec4 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._bvec4;
        public BoolVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize("Value", IsXmlElementString = true)]
        private BoolVec4 _value;

        public ShaderBVec4() : this(new BoolVec4(), NoName) { }
        public ShaderBVec4(BoolVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderBVec4(BoolVec4 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderBool(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderBool(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderBool(defaultValue.Z, "Z", this));
            _fields.Add("w", new ShaderBool(defaultValue.W, "W", this));
        }
    }
    public class ShaderVec4 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._vec4;
        public Vec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) 
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize("Value", IsXmlElementString = true)]
        private Vec4 _value;

        public ShaderVec4() : this(new Vec4(), NoName) { }
        public ShaderVec4(Vec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderVec4(Vec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderFloat(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderFloat(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderFloat(defaultValue.Z, "Z", this));
            _fields.Add("w", new ShaderFloat(defaultValue.W, "W", this));
        }
    }
    public class ShaderDVec4 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._dvec4;
        public DVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location)
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize("Value", IsXmlElementString = true)]
        private DVec4 _value;

        public ShaderDVec4() : this(new DVec4(), NoName) { }
        public ShaderDVec4(DVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderDVec4(DVec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderDouble(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderDouble(defaultValue.Z, "Z", this));
            _fields.Add("w", new ShaderDouble(defaultValue.W, "W", this));
        }
    }
    public class ShaderIVec4 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._ivec4;
        public IVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) 
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize("Value", IsXmlElementString = true)]
        private IVec4 _value;

        public ShaderIVec4() : this(new IVec4(), NoName) { }
        public ShaderIVec4(IVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderIVec4(IVec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderInt(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderInt(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderInt(defaultValue.Z, "Z", this));
            _fields.Add("w", new ShaderInt(defaultValue.W, "W", this));
        }
    }
    public class ShaderUVec4 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._uvec4;
        public UVec4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) 
            => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize("Value", IsXmlElementString = true)]
        private UVec4 _value;

        public ShaderUVec4() : this(new UVec4(), NoName) { }
        public ShaderUVec4(UVec4 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderUVec4(UVec4 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("x", new ShaderUInt(defaultValue.X, "X", this));
            _fields.Add("y", new ShaderUInt(defaultValue.Y, "Y", this));
            _fields.Add("z", new ShaderUInt(defaultValue.Z, "Z", this));
            _fields.Add("w", new ShaderUInt(defaultValue.W, "W", this));
        }
    }
}
