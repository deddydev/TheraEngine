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
    public class ParameterFunc<T> : MaterialFunction, IParameterFunc where T : GLVar
    {
        public ParameterFunc() : base() { }
        public ParameterFunc(T value) { _value = value; }
        
        T _value;

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public static MaterialFuncInfo GetInfo()
        {
            return new MaterialFuncInfo(
                "Constants",
                "Parameter Value",
                "Provides an animatable value to the shader.", 
                "constant scalar parameter value");
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
        {
            _value?.SetUniform();
        }
    }
}
