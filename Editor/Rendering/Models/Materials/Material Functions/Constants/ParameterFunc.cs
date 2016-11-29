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
                "Provides a constant value to the shader.", 
                "constant scalar parameter");
        }
        protected override string GetOperation()
        {
            return _value.ToString();
        }

        public void SetUniform() { Value?.SetUniform(); }
    }
}
