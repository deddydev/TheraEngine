﻿using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderBVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._bvec2;
        [Category(CategoryName)]
        public BoolVec2 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private BoolVec2 _value;

        public ShaderBVec2() : this(new BoolVec2(), NoName) { }
        public ShaderBVec2(BoolVec2 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderBVec2(BoolVec2 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderBool(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderBool(defaultValue.Y, "Y", this));
        }
    }
    public class ShaderVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._vec2;
        [Category(CategoryName)]
        public Vec2 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private Vec2 _value;

        public ShaderVec2() : this(new Vec2(), NoName) { }
        public ShaderVec2(Vec2 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderVec2(Vec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderFloat(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderFloat(defaultValue.Y, "Y", this));
        }
    }
    public class ShaderDVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._dvec2;
        [Category(CategoryName)]
        public DVec2 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private DVec2 _value;

        public ShaderDVec2() : this(new DVec2(), NoName) { }
        public ShaderDVec2(DVec2 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderDVec2(DVec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderDouble(defaultValue.Y, "Y", this));
        }
    }
    public class ShaderIVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._ivec2;
        [Category(CategoryName)]
        public IVec2 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private IVec2 _value;

        public ShaderIVec2() : this(new IVec2(), NoName) { }
        public ShaderIVec2(IVec2 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderIVec2(IVec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderInt(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderInt(defaultValue.Y, "Y", this));
        }
    }
    public class ShaderUVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._uvec2;
        [Category(CategoryName)]
        public UVec2 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private UVec2 _value;

        public ShaderUVec2() : this(new UVec2(), NoName) { }
        public ShaderUVec2(UVec2 defaultValue, string name)
            : this(defaultValue, name, null) { }
        public ShaderUVec2(UVec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderUInt(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderUInt(defaultValue.Y, "Y", this));
        }
    }
}
