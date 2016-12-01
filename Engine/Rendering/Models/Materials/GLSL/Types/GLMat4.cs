using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLMat4 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._mat4; } }
        public Matrix4 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }

        private Matrix4 _value;

        public GLMat4(Matrix4 defaultValue, string name, IGLVarOwner owner) 
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("[0]", new GLVec4(defaultValue.Row0, "Row0", this));
            _fields.Add("[1]", new GLVec4(defaultValue.Row1, "Row1", this));
            _fields.Add("[2]", new GLVec4(defaultValue.Row2, "Row2", this));
            _fields.Add("[3]", new GLVec4(defaultValue.Row3, "Row3", this));
        }
    }
    public class GLMat3 : GLVar
    {
        public override GLTypeName TypeName { get { return GLTypeName._mat3; } }
        public Matrix3 Value { get { return _value; } set { _value = value; } }
        public override void SetUniform(int location) { Engine.Renderer.Uniform(location, _value); }
        public override string GetValueString() { return _value.ToString(); }
        
        private Matrix3 _value;
        
        public GLMat3(Matrix3 defaultValue, string name, IGLVarOwner owner)
            : base(name, owner)
        {
            _value = defaultValue;
            _fields.Add("[0]", new GLVec3(defaultValue.Row0, "Row0", this));
            _fields.Add("[1]", new GLVec3(defaultValue.Row1, "Row1", this));
            _fields.Add("[2]", new GLVec3(defaultValue.Row2, "Row2", this));
        }
    }
}
