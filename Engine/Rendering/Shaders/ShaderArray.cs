using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    public class ShaderArray<T> : ShaderVar where T : ShaderVar
    {
        [Browsable(false)]
        public override EShaderVarType TypeName => TypeAssociations[typeof(T)];
        public ShaderArrayValueHandler<T> Value { get => _value; set { _value = value; OnValueChanged(); } }
        public int Length => _value.Length;
        internal override void SetProgramUniform(RenderProgram program, int location)
        {
            //throw new NotImplementedException();
            //Engine.Renderer.Uniform();
        }
        internal override string GetShaderValueString() => _value.ToString();
        public override object GenericValue => Value;
        
        private ShaderArrayValueHandler<T> _value;

        public ShaderArray(string name)
            : this(name, null) { }
        public ShaderArray(string name, IShaderVarOwner owner)
            : base(name, owner) { _value = new ShaderArrayValueHandler<T>(); }
        public ShaderArray(ShaderArrayValueHandler<T> defaultValue, string name, IShaderVarOwner owner)
            : base(name, owner) { _value = defaultValue; }
    }
    public class ShaderArrayValueHandler<T> : IUniformableArray<T> where T : ShaderVar
    {
        public int Length => Values.Length;
        public T[] Values { get; set; }

        public ShaderArrayValueHandler()
        {
            Values = null;
        }
        public ShaderArrayValueHandler(int count)
        {
            Values = new T[count];
        }
    }
}
