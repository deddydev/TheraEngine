using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    [Flags]
    public enum EGenShaderVarType
    {
        Bool    = 0x000001,
        Int     = 0x000002,
        Uint    = 0x000004,
        Float   = 0x000008,
        Double  = 0x000010,

        Vec2    = 0x000020,
        Vec3    = 0x000040,
        Vec4    = 0x000080,

        BVec2   = 0x000100,
        BVec3   = 0x000200,
        BVec4   = 0x000400,

        IVec2   = 0x000800,
        IVec3   = 0x001000,
        IVec4   = 0x002000,

        UVec2   = 0x004000,
        UVec3   = 0x008000,
        UVec4   = 0x010000,

        DVec2   = 0x020000,
        DVec3   = 0x040000,
        DVec4   = 0x080000,

        Mat3    = 0x100000,
        Mat4    = 0x200000,

        GenBool     = Bool   | BVec2 | BVec3 | BVec4,
        GenInt      = Int    | IVec2 | IVec3 | IVec4,
        GenUInt     = Uint   | UVec2 | UVec3 | UVec4,
        GenFloat    = Float  |  Vec2 |  Vec3 |  Vec4,
        GenDouble   = Double | DVec2 | DVec3 | DVec4,

        VecBool     = BVec2 | BVec3 | BVec4,
        VecInt      = IVec2 | IVec3 | IVec4,
        VecUint     = UVec2 | UVec3 | UVec4,
        VecFloat    =  Vec2 |  Vec3 |  Vec4,
        VecDouble   = DVec2 | DVec3 | DVec4,
    }
    public enum EShaderVarType : int
    {
        _invalid = -1,

        _bool,
        _int,
        _uint,
        _float,
        _double,

        _vec2,
        _vec3,
        _vec4,

        _bvec2,
        _bvec3,
        _bvec4,

        _ivec2,
        _ivec3,
        _ivec4,

        _uvec2,
        _uvec3,
        _uvec4,
        
        _dvec2,
        _dvec3,
        _dvec4,

        _mat3,
        _mat4,
    }

    public interface IShaderVarType { }

    public interface IShaderNumericType : IShaderVarType { }
    public interface IShaderBooleanType : IShaderVarType { }
    public interface IShaderMatrixType : IShaderVarType { }
    public interface IShaderSignedIntType : IShaderVarType { }
    public interface IShaderUnsignedIntType : IShaderVarType { }
    public interface IShaderFloatType : IShaderVarType { }
    public interface IShaderDoubleType : IShaderVarType { }

    public interface IShaderNonVectorType : IShaderVarType { }
    public interface IShaderVectorType : IShaderVarType { }
    
    public interface IShaderVec2Type : IShaderVarType { }
    public interface IShaderVec3Type : IShaderVarType { }
    public interface IShaderVec4Type : IShaderVarType { }

    public interface IShaderVectorBoolType : IShaderVarType { }
    public interface IShaderVectorSignedIntType : IShaderVarType { }
    public interface IShaderVectorUnsignedIntType : IShaderVarType { }
    public interface IShaderVectorFloatType : IShaderVarType { }
    public interface IShaderVectorDoubleType : IShaderVarType { }
    
    public interface IShaderNonDecimalType : IShaderVarType { }
    public interface IShaderDecimalType : IShaderVarType { }

    public interface IShaderSignedType : IShaderVarType { }
    public interface IShaderUnsignedType : IShaderVarType { }

    public abstract class ShaderVar : TObject, IShaderVarOwner
    {
        internal const string CategoryName = "Material Parameter";
        internal const string ValueName = "Value";
        public const string NoName = "NoName";

        public event Action<ShaderVar> ValueChanged;

        //Determines if this var's components can be moved around
        protected bool _canSwizzle = true;
        protected IShaderVarOwner _owner;
        protected Dictionary<string, ShaderVar> _fields = new Dictionary<string, ShaderVar>();

        internal IShaderVarOwner Owner => _owner;
        public abstract EShaderVarType TypeName { get; }

        [Browsable(true)]
        [TString(false, false, false, false)]
        [Category(CategoryName)]
        [DisplayName("Uniform Name")]
        public override string Name
        {
            get => base.Name;
            set => base.Name = (value ?? "").ReplaceWhitespace("");
        }

        [Browsable(false)]
        public abstract object GenericValue { get; }

        internal void SetProgramUniform(RenderProgram program, string name)
        {
            if (_valueChanged)
            {
                int loc = program.GetUniformLocation(name);
                if (loc >= 0)
                    SetProgramUniform(program, loc);
                _valueChanged = false;
            }
            //else
            //    throw new Exception();
        }

        protected bool _valueChanged = true;
        internal void SetProgramUniform(RenderProgram program)
        {
            if (_valueChanged)
            {
                SetProgramUniform(program, Name);
                _valueChanged = false;
            }
        }

        //internal void SetUniform(string name) { SetUniform(Engine.Renderer.GetUniformLocation(programBindingId, name)); }
        //internal void SetUniform() { SetUniform(Engine.Renderer.GetUniformLocation(programBindingId, Name)); }

        internal abstract void SetProgramUniform(RenderProgram program, int location);

        public ShaderVar(string userName, IShaderVarOwner owner)
        {
            _owner = owner;
            Name = userName;
        }

        protected void OnValueChanged()
        {
            _valueChanged = true;
            ValueChanged?.Invoke(this);
        }

        /// <summary>
        /// Ex: layout (location = 0) uniform float potato;
        /// </summary>
        internal string GetUniformDeclaration(int bindingLocation = -1)
        {
            string line = "";
            if (bindingLocation >= 0)
                line = string.Format("layout (location = {0}) ", bindingLocation);
            return line + string.Format("uniform {0};", GetDeclaration());
        }
        internal string GetDeclaration()
        {
            return string.Format("{0} {1}", TypeName.ToString().Substring(1), Name);
        }
        internal abstract string GetShaderValueString();
        /// <summary>
        /// Ex: this is float '.x', parent is vec4 '[0]', parent is mat4 'tomato': tomato[0].x
        /// </summary>
        /// <returns></returns>
        internal virtual string AccessorTree()
        {
            return Name;
        }

        internal static Vec4 GetTypeColor(EShaderVarType argumentType)
        {
            switch (argumentType)
            {
                case EShaderVarType._bool:
                case EShaderVarType._bvec2:
                case EShaderVarType._bvec3:
                case EShaderVarType._bvec4:
                    return (ColorF4)Color.Red;
                case EShaderVarType._int:
                case EShaderVarType._ivec2:
                case EShaderVarType._ivec3:
                case EShaderVarType._ivec4:
                    return (ColorF4)Color.HotPink;
                case EShaderVarType._uint:
                case EShaderVarType._uvec2:
                case EShaderVarType._uvec3:
                case EShaderVarType._uvec4:
                    return (ColorF4)Color.Orange;
                case EShaderVarType._float:
                case EShaderVarType._vec2:
                case EShaderVarType._vec3:
                case EShaderVarType._vec4:
                    return (ColorF4)Color.Blue;
                case EShaderVarType._double:
                case EShaderVarType._dvec2:
                case EShaderVarType._dvec3:
                case EShaderVarType._dvec4:
                    return (ColorF4)Color.Green;
            }
            return BaseFuncValue.NoTypeColor;
        }

        #region Type caches
        public EShaderVarType[] GetTypesMatching<T>() where T : IShaderVarType
        {
            TypeProxy varType = typeof(T);
            TypeProxy shaderType = typeof(ShaderVar);
            var types = AppDomainHelper.FindTypes(t => t.IsSubclassOf(shaderType) && varType.IsAssignableFrom(t));
            return types.Select(x => TypeAssociations[x]).Distinct().ToArray();
        }
        public static Dictionary<TypeProxy, EShaderVarType> TypeAssociations = new Dictionary<TypeProxy, EShaderVarType>()
        {
            { typeof(ShaderBool),   EShaderVarType._bool   },
            { typeof(ShaderInt),    EShaderVarType._int    },
            { typeof(ShaderUInt),   EShaderVarType._uint   },
            { typeof(ShaderFloat),  EShaderVarType._float  },
            { typeof(ShaderDouble), EShaderVarType._double },
            { typeof(ShaderVec2),   EShaderVarType._vec2   },
            { typeof(ShaderVec3),   EShaderVarType._vec3   },
            { typeof(ShaderVec4),   EShaderVarType._vec4   },
            { typeof(ShaderMat3),   EShaderVarType._mat3   },
            { typeof(ShaderMat4),   EShaderVarType._mat4   },
            { typeof(ShaderIVec2),  EShaderVarType._ivec2  },
            { typeof(ShaderIVec3),  EShaderVarType._ivec3  },
            { typeof(ShaderIVec4),  EShaderVarType._ivec4  },
            { typeof(ShaderUVec2),  EShaderVarType._uvec2  },
            { typeof(ShaderUVec3),  EShaderVarType._uvec3  },
            { typeof(ShaderUVec4),  EShaderVarType._uvec4  },
            { typeof(ShaderDVec2),  EShaderVarType._dvec2  },
            { typeof(ShaderDVec3),  EShaderVarType._dvec3  },
            { typeof(ShaderDVec4),  EShaderVarType._dvec4  },
            { typeof(ShaderBVec2),  EShaderVarType._bvec2  },
            { typeof(ShaderBVec3),  EShaderVarType._bvec3  },
            { typeof(ShaderBVec4),  EShaderVarType._bvec4  },
        };
        public static Dictionary<EShaderVarType, Type> ShaderTypeAssociations = new Dictionary<EShaderVarType, Type>()
        {
            { EShaderVarType._bool,   typeof(ShaderBool)   },
            { EShaderVarType._int,    typeof(ShaderInt)    },
            { EShaderVarType._uint,   typeof(ShaderUInt)   },
            { EShaderVarType._float,  typeof(ShaderFloat)  },
            { EShaderVarType._double, typeof(ShaderDouble) },
            { EShaderVarType._vec2,   typeof(ShaderVec2)   },
            { EShaderVarType._vec3,   typeof(ShaderVec3)   },
            { EShaderVarType._vec4,   typeof(ShaderVec4)   },
            { EShaderVarType._mat3,   typeof(ShaderMat3)   },
            { EShaderVarType._mat4,   typeof(ShaderMat4)   },
            { EShaderVarType._ivec2,  typeof(ShaderIVec2)  },
            { EShaderVarType._ivec3,  typeof(ShaderIVec3)  },
            { EShaderVarType._ivec4,  typeof(ShaderIVec4)  },
            { EShaderVarType._uvec2,  typeof(ShaderUVec2)  },
            { EShaderVarType._uvec3,  typeof(ShaderUVec3)  },
            { EShaderVarType._uvec4,  typeof(ShaderUVec4)  },
            { EShaderVarType._dvec2,  typeof(ShaderDVec2)  },
            { EShaderVarType._dvec3,  typeof(ShaderDVec3)  },
            { EShaderVarType._dvec4,  typeof(ShaderDVec4)  },
            { EShaderVarType._bvec2,  typeof(ShaderBVec2)  },
            { EShaderVarType._bvec3,  typeof(ShaderBVec3)  },
            { EShaderVarType._bvec4,  typeof(ShaderBVec4)  },
        };
        public static Dictionary<EShaderVarType, Type> AssemblyTypeAssociations = new Dictionary<EShaderVarType, Type>()
        {
            { EShaderVarType._bool,   typeof(bool)       },
            { EShaderVarType._int,    typeof(int)        },
            { EShaderVarType._uint,   typeof(uint)       },
            { EShaderVarType._float,  typeof(float)      },
            { EShaderVarType._double, typeof(double)     },
            { EShaderVarType._vec2,   typeof(Vec2)       },
            { EShaderVarType._vec3,   typeof(Vec3)       },
            { EShaderVarType._vec4,   typeof(Vec4)       },
            { EShaderVarType._mat3,   typeof(Matrix3)    },
            { EShaderVarType._mat4,   typeof(Matrix4)    },
            { EShaderVarType._ivec2,  typeof(IVec2)      },
            { EShaderVarType._ivec3,  typeof(IVec3)      },
            { EShaderVarType._ivec4,  typeof(IVec4)      },
            { EShaderVarType._uvec2,  typeof(UVec2)      },
            { EShaderVarType._uvec3,  typeof(UVec3)      },
            { EShaderVarType._uvec4,  typeof(UVec4)      },
            { EShaderVarType._dvec2,  typeof(DVec2)      },
            { EShaderVarType._dvec3,  typeof(DVec3)      },
            { EShaderVarType._dvec4,  typeof(DVec4)      },
            { EShaderVarType._bvec2,  typeof(BoolVec2)   },
            { EShaderVarType._bvec3,  typeof(BoolVec3)   },
            { EShaderVarType._bvec4,  typeof(BoolVec4)   },
        };
        public static readonly EShaderVarType[] SignedIntTypes = new EShaderVarType[]
        {
            EShaderVarType._int,
            EShaderVarType._ivec2,
            EShaderVarType._ivec3,
            EShaderVarType._ivec4,
        };
        public static readonly EShaderVarType[] UnsignedIntTypes = new EShaderVarType[]
        {
            EShaderVarType._uint,
            EShaderVarType._uvec2,
            EShaderVarType._uvec3,
            EShaderVarType._uvec4,
        };
        public static readonly EShaderVarType[] IntegerTypes = new EShaderVarType[]
        {
            EShaderVarType._int,
            EShaderVarType._uint,
            EShaderVarType._ivec2,
            EShaderVarType._uvec2,
            EShaderVarType._ivec3,
            EShaderVarType._uvec3,
            EShaderVarType._ivec4,
            EShaderVarType._uvec4,
        };
        public static readonly EShaderVarType[] DecimalTypes = new EShaderVarType[]
        {
            EShaderVarType._float,
            EShaderVarType._double,
            EShaderVarType._vec2,
            EShaderVarType._dvec2,
            EShaderVarType._vec3,
            EShaderVarType._dvec3,
            EShaderVarType._vec4,
            EShaderVarType._dvec4,
        };
        public static readonly EShaderVarType[] FloatTypes = new EShaderVarType[]
        {
            EShaderVarType._float,
            EShaderVarType._vec2,
            EShaderVarType._vec3,
            EShaderVarType._vec4,
        };
        public static readonly EShaderVarType[] DoubleTypes = new EShaderVarType[]
        {
            EShaderVarType._double,
            EShaderVarType._dvec2,
            EShaderVarType._dvec3,
            EShaderVarType._dvec4,
        };
        public static readonly EShaderVarType[] NumericTypes = new EShaderVarType[]
        {
            EShaderVarType._float,
            EShaderVarType._double,
            EShaderVarType._int,
            EShaderVarType._uint,
            EShaderVarType._vec2,
            EShaderVarType._ivec2,
            EShaderVarType._uvec2,
            EShaderVarType._dvec2,
            EShaderVarType._vec3,
            EShaderVarType._ivec3,
            EShaderVarType._uvec3,
            EShaderVarType._dvec3,
            EShaderVarType._vec4,
            EShaderVarType._ivec4,
            EShaderVarType._uvec4,
            EShaderVarType._dvec4,
        };
        public static readonly EShaderVarType[] SignedTypes = new EShaderVarType[]
        {
            EShaderVarType._float,
            EShaderVarType._double,
            EShaderVarType._int,
            EShaderVarType._vec2,
            EShaderVarType._ivec2,
            EShaderVarType._dvec2,
            EShaderVarType._vec3,
            EShaderVarType._ivec3,
            EShaderVarType._dvec3,
            EShaderVarType._vec4,
            EShaderVarType._ivec4,
            EShaderVarType._dvec4,
        };
        public static readonly EShaderVarType[] BooleanTypes = new EShaderVarType[]
        {
            EShaderVarType._bool,
            EShaderVarType._bvec2,
            EShaderVarType._bvec3,
            EShaderVarType._bvec4,
        };
        public static readonly EShaderVarType[] VectorTypes = new EShaderVarType[]
        {
            EShaderVarType._vec2,
            EShaderVarType._ivec2,
            EShaderVarType._uvec2,
            EShaderVarType._dvec2,
            EShaderVarType._bvec2,
            EShaderVarType._vec3,
            EShaderVarType._ivec3,
            EShaderVarType._uvec3,
            EShaderVarType._dvec3,
            EShaderVarType._bvec3,
            EShaderVarType._vec4,
            EShaderVarType._ivec4,
            EShaderVarType._uvec4,
            EShaderVarType._dvec4,
            EShaderVarType._bvec4,
        };
        #endregion
    }
}
