using System;

namespace TheraEngine.Rendering.Models.Materials
{
    public class GLArray<T> : ShaderVar where T : ShaderVar
    {
        public override ShaderType TypeName { get { return _elementTypeName; } }
        public ArrayHandler<T> Value { get { return _value; } set { _value = value; } }
        public int Length { get { return _value.Length; } }
        internal override void SetProgramUniform(int programBindingId, int location)
        {
            throw new NotImplementedException();
            //Engine.Renderer.Uniform();
        }
        internal override string GetValueString() { return _value.ToString(); }

        public ShaderType _elementTypeName;
        private ArrayHandler<T> _value;

        public GLArray(ArrayHandler<T> defaultValue, ShaderType elementType, string name, IShaderVarOwner owner) 
            : base(name, owner) { _value = defaultValue; _elementTypeName = elementType; }
    }
    public class ArrayHandler<T> : IUniformableArray where T : ShaderVar
    {
        public int Length { get { return Values.Length; } }
        public IUniformable[] Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
