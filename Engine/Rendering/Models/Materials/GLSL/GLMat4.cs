﻿using System;

namespace TheraEngine.Rendering.Models.Materials
{
    public class GLMat4 : ShaderVar
    {
        public override ShaderType TypeName => ShaderType._mat4;
        public Matrix4 Value { get => _value; set => _value = value; }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        private Matrix4 _value;

        public GLMat4(Matrix4 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _canSwizzle = false;
            _value = defaultValue;
            _fields.Add("[0]", new GLVec4(defaultValue.Row0, "Row0", this));
            _fields.Add("[1]", new GLVec4(defaultValue.Row1, "Row1", this));
            _fields.Add("[2]", new GLVec4(defaultValue.Row2, "Row2", this));
            _fields.Add("[3]", new GLVec4(defaultValue.Row3, "Row3", this));
        }
    }
    public class GLMat3 : ShaderVar
    {
        public override ShaderType TypeName => ShaderType._mat3;
        public Matrix3 Value { get => _value; set => _value = value; }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.ProgramUniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();
        
        private Matrix3 _value;
        
        public GLMat3(Matrix3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _canSwizzle = false;
            _value = defaultValue;
            _fields.Add("[0]", new GLVec3(defaultValue.Row0, "Row0", this));
            _fields.Add("[1]", new GLVec3(defaultValue.Row1, "Row1", this));
            _fields.Add("[2]", new GLVec3(defaultValue.Row2, "Row2", this));
        }
    }
}
