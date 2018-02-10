using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderMat4 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._mat4;
        [Category(CategoryName)]
        public Matrix4 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private Matrix4 _value;

        public ShaderMat4(Matrix4 defaultValue, string name, IShaderVarOwner owner) 
            : base(name, owner)
        {
            _canSwizzle = false;
            _value = defaultValue;
            _fields.Add("[0]", new ShaderVec4(defaultValue.Row0, "Row0", this));
            _fields.Add("[1]", new ShaderVec4(defaultValue.Row1, "Row1", this));
            _fields.Add("[2]", new ShaderVec4(defaultValue.Row2, "Row2", this));
            _fields.Add("[3]", new ShaderVec4(defaultValue.Row3, "Row3", this));
        }
    }
    public class ShaderMat3 : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName => ShaderVarType._mat3;
        [Category(CategoryName)]
        public Matrix3 Value { get => _value; set { _value = value; OnValueChanged(); } }
        internal override void SetProgramUniform(int programBindingId, int location) => Engine.Renderer.Uniform(programBindingId, location, _value);
        internal override string GetValueString() => _value.ToString();

        [TSerialize(ValueName, IsXmlElementString = true)]
        private Matrix3 _value;
        
        public ShaderMat3(Matrix3 defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner)
        {
            _canSwizzle = false;
            _value = defaultValue;
            _fields.Add("[0]", new ShaderVec3(defaultValue.Row0, "Row0", this));
            _fields.Add("[1]", new ShaderVec3(defaultValue.Row1, "Row1", this));
            _fields.Add("[2]", new ShaderVec3(defaultValue.Row2, "Row2", this));
        }
    }
}
