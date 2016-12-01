using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class ConstantFunc<T> : MaterialFunction where T : GLVar
    {
        public ConstantFunc() : base() { }
        public ConstantFunc(T value) { _value = value; }
        
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
    }
}
