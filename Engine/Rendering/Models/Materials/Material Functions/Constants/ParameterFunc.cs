using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public interface IParameterFunc
    {
        void SetUniform();
    }
    [FunctionDefinition(
                "Constants",
                "Parameter Value",
                "Provides an animatable value to the shader.",
                "constant scalar vector parameter value uniform animatable animate animation")]
    public class ParameterFunc<T> : MaterialFunction, IParameterFunc where T : GLVar
    {
        public ParameterFunc() : base(true) { }
        public ParameterFunc(T value) : base(true) { _value = value; }
        
        T _value;

        public T Value
        {
            get => _value;
            set => _value = value;
        }
        protected override string GetOperation()
        {
            if (_value == null)
                return "";
            return _value.ToString();
        }
        public string GetUniformDeclaration(int layoutId = -1)
        {
            if (_value == null)
                return "";
            return _value.GetUniformDeclaration(layoutId);
        }
        public void SetUniform() 
            => _value?.SetUniform();
    }
}
