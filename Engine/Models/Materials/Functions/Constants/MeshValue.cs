using Extensions;
using System;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class MeshParam
    {
        public event Action Changed;

        public MeshParam()
        {
            Value = EMeshValue.FragPos;
            Index = 0;
        }
        public MeshParam(EMeshValue value, int index)
        {
            Value = value;
            Index = index;
        }

        private EMeshValue _value;
        private int _index;

        private void Update()
        {
            switch (_value)
            {
                default:
                case EMeshValue.FragPos:
                    Type = EShaderVarType._vec3;
                    ShaderBaseLocation = VertexShaderGenerator.FragPosBaseLoc;
                    MaxCount = 1;
                    Index = Index.ClampMax(MaxCount - 1);
                    ShaderLocation = ShaderBaseLocation + Index;
                    break;
                case EMeshValue.FragNorm:
                    Type = EShaderVarType._vec3;
                    ShaderBaseLocation = VertexShaderGenerator.FragNormBaseLoc;
                    MaxCount = 1;
                    Index = Index.ClampMax(MaxCount - 1);
                    ShaderLocation = ShaderBaseLocation + Index;
                    break;
                case EMeshValue.FragBinorm:
                    Type = EShaderVarType._vec3;
                    ShaderBaseLocation = VertexShaderGenerator.FragBinormBaseLoc;
                    MaxCount = 1;
                    Index = Index.ClampMax(MaxCount - 1);
                    ShaderLocation = ShaderBaseLocation + Index;
                    break;
                case EMeshValue.FragTan:
                    Type = EShaderVarType._vec3;
                    ShaderBaseLocation = VertexShaderGenerator.FragTanBaseLoc;
                    MaxCount = 1;
                    Index = Index.ClampMax(MaxCount - 1);
                    ShaderLocation = ShaderBaseLocation + Index;
                    break;
                case EMeshValue.FragUV:
                    Type = EShaderVarType._vec2;
                    ShaderBaseLocation = VertexShaderGenerator.FragUVBaseLoc;
                    MaxCount = VertexShaderDesc.MaxTexCoords;
                    Index = Index.ClampMax(MaxCount - 1);
                    ShaderLocation = ShaderBaseLocation + Index;
                    break;
                case EMeshValue.FragColor:
                    Type = EShaderVarType._vec4;
                    ShaderBaseLocation = VertexShaderGenerator.FragColorBaseLoc;
                    MaxCount = VertexShaderDesc.MaxColors;
                    Index = Index.ClampMax(MaxCount - 1);
                    ShaderLocation = ShaderBaseLocation + Index;
                    break;
            }
            Changed?.Invoke();
        }

        public int Index
        {
            get => _index;
            set
            {
                int temp = value.Clamp(0, MaxCount - 1);
                if (_index != temp)
                {
                    _index = temp;
                    Update();
                }
            }
        }
        public EMeshValue Value
        {
            get => _value;
            set
            {
                _value = value;
                Update();
            }
        }

        public int MaxCount { get; private set; }
        public EShaderVarType Type { get; private set; }
        public int ShaderBaseLocation { get; private set; }
        public int ShaderLocation { get; private set; }

        public string GetVariableName() => Value.ToString() + (MaxCount > 1 ? Index.ToString() : "");
        public string GetVariableInDeclaration()
            => "layout(location = " + ShaderLocation + ") in " + Type.ToString().Substring(1) + " " + GetVariableName() + ";";

        public override int GetHashCode() => ShaderLocation;
        public override bool Equals(object obj)
        {
            if (!(obj is MeshParam p))
                return false;
            return p.ShaderLocation == ShaderLocation;
        }
    }
    [FunctionDefinition(
        "Constants",
        "Mesh Value",
        "Provides a value from the mesh to the shader.",
        "mesh value")]
    public class MeshValueFunc : ShaderMethod
    {
        public MeshValueFunc() : this(EMeshValue.FragPos) { }
        public MeshValueFunc(EMeshValue value) : base()
        {
            Param.Changed += Param_Changed;
            Param.Value = value;
            NecessaryMeshParams.Add(Param);
        }

        private void Param_Changed()
        {
            //OutputArguments[0].AllowedArgumentTypes = new int[] { (int)Param.Type };
            Overloads[0].Outputs[0] = (EGenShaderVarType)Param.Type;
            _headerString.Text = Param.GetVariableName();
            ArrangeControls();
        }

        public MeshParam Param { get; } = new MeshParam();

        public static EShaderVarType GetType(EMeshValue value)
        {
            switch (value)
            {
                default:
                case EMeshValue.FragPos:
                case EMeshValue.FragNorm:
                case EMeshValue.FragBinorm:
                case EMeshValue.FragTan:
                    return EShaderVarType._vec3;
                case EMeshValue.FragUV:
                    return EShaderVarType._vec2;
                case EMeshValue.FragColor:
                    return EShaderVarType._vec4;
            }
        }

        //public override string GetGlobalVarDec() => Param.GetVariableInDeclaration();
        protected override string GetOperation() => Param.GetVariableName();

        public override void GetDefinition(out string[] inputNames, out string[] outputNames, out MatFuncOverload[] overloads)
        {
            inputNames = new string[]
            {

            };
            outputNames = new string[]
            {
                string.Empty,
            };
            overloads = new MatFuncOverload[]
            {
                new MatFuncOverload(EGLSLVersion.Ver_110, EGenShaderVarType.Vec3, false),
            };
        }
    }
}
