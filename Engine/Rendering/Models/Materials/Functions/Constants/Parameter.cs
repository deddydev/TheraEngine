using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    [FunctionDefinition(
                "Constants",
                "Parameter Value",
                "Provides an animatable value to the shader.",
                "constant scalar vector parameter value uniform animatable animate animation")]
    public class ParameterFunc<T> : BaseParameterFunc where T : ShaderVar, new()
    {
        public ParameterFunc() : this(default) { }
        public ParameterFunc(T value) : base()
        {
            Value = value;
            HasGlobalVarDec = true;
            Overloads[0].Outputs[0] = (EGenShaderVarType)ShaderVar.TypeAssociations[typeof(T)];
        }
        
        T _value;

        public T Value
        {
            get => _value;
            set
            {
                _value = value ?? new T();
                _value.Renamed += _value_Renamed;
                _headerString.Text = _value.Name ?? "<Nameless Parameter>";
                ArrangeControls();
            }
        }

        private void _value_Renamed(TObject node, string oldName)
        {
            _headerString.Text = node.Name;
            ArrangeControls();
        }

        //private void _value_ValueChanged(ShaderVar obj)
        //    => _headerString.Text = obj.GenericValue.ToString();

        protected override string GetOperation() => _value.GetShaderValueString();
        public override string GetGlobalVarDec() => _value.GetUniformDeclaration();
        public override void SetUniform(RenderProgram programBindingId)
            => _value?.SetProgramUniform(programBindingId);

        public override ShaderVar GetVar() => Value;

        public override void GetDefinition(out string[] inputNames, out string[] outputNames, out MatFuncOverload[] overloads)
        {
            inputNames = new string[] { };
            outputNames = new string[] { string.Empty };
            overloads = new MatFuncOverload[] 
            {
                new MatFuncOverload(EGLSLVersion.Ver_110, EGenShaderVarType.Float)
            };
        }
    }
    public abstract class BaseParameterFunc : ShaderMethod
    {
        public BaseParameterFunc() : base(true) { }
        
        public abstract void SetUniform(RenderProgram programBindingId);
        public abstract ShaderVar GetVar();
    }
}
