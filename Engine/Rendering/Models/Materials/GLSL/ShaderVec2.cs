﻿using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderBVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._bvec2; } }
        public BoolVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private BoolVec2 _value;

        public ShaderBVec2(BoolVec2 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderBool(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderBool(defaultValue.Y, "Y", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._vec2; } }
        public Vec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private Vec2 _value;

        public ShaderVec2(Vec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderFloat(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderFloat(defaultValue.Y, "Y", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderDVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._dvec2; } }
        public DVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private DVec2 _value;

        public ShaderDVec2(DVec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderDouble(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderDouble(defaultValue.Y, "Y", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderIVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._ivec2; } }
        public IVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private IVec2 _value;

        public ShaderIVec2(IVec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderInt(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderInt(defaultValue.Y, "Y", this));
        }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderUVec2 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return ShaderVarType._uvec2; } }
        public UVec2 Value { get { return _value; } set { _value = value; } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() { return _value.ToString(); }

        private UVec2 _value;

        public ShaderUVec2(UVec2 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add(".x", new ShaderUInt(defaultValue.X, "X", this));
            _fields.Add(".y", new ShaderUInt(defaultValue.Y, "Y", this));
        }
    }
}
