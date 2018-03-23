using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Rendering.UI.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum EGenShaderVarType
    {
        Bool,
        Int,
        Uint,
        Float,
        Double,

        Vec2, Vec3, Vec4,
        BVec2, BVec3, BVec4,
        IVec2, IVec3, IVec4,
        UVec2, UVec3, UVec4,
        DVec2, DVec3, DVec4,
        Mat3, Mat4,

        GenBool,
        GenInt,
        GenUInt,
        GenFloat,
        GenDouble,

        VecBool,
        VecInt,
        VecUint,
        VecFloat,
        VecDouble,
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

        #region Type caches
        public EShaderVarType[] GetTypesMatching<T>() where T : IShaderVarType
        {
            Type varType = typeof(T);
            Type shaderType = typeof(ShaderVar);
            var types = Engine.FindTypes(t => t.IsSubclassOf(shaderType) && varType.IsAssignableFrom(t));
            return types.Select(x => TypeAssociations[x]).Distinct().ToArray();
        }
        public static Dictionary<Type, EShaderVarType> TypeAssociations = new Dictionary<Type, EShaderVarType>()
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

        public event Action<ShaderVar> ValueChanged;

        [TSerialize("CanSwizzle", XmlNodeType = EXmlNodeType.Attribute)]
        protected bool _canSwizzle = true;

        protected IShaderVarOwner _owner;

        //[TSerialize("Fields")]
        protected Dictionary<string, ShaderVar> _fields = new Dictionary<string, ShaderVar>();

        internal IShaderVarOwner Owner => _owner;
        public abstract EShaderVarType TypeName { get; }

        [TString(false, false, false, false)]
        [Category(CategoryName)]
        public override string Name
        {
            get => base.Name;
            set
            {
                //Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value)
                base.Name = (value ?? "").ReplaceWhitespace("");
            }
        }

        public abstract object GenericValue { get; }

        internal void SetProgramUniform(int programBindingId, string name)
        {
            int loc = Engine.Renderer.GetUniformLocation(programBindingId, name);
            if (loc >= 0)
                SetProgramUniform(programBindingId, loc);
            //else
            //    throw new Exception();
        }

        internal void SetProgramUniform(int programBindingId) 
            => SetProgramUniform(programBindingId, Name);

        //internal void SetUniform(string name) { SetUniform(Engine.Renderer.GetUniformLocation(programBindingId, name)); }
        //internal void SetUniform() { SetUniform(Engine.Renderer.GetUniformLocation(programBindingId, Name)); }

        internal abstract void SetProgramUniform(int programBindingId, int location);

        public ShaderVar(string userName, IShaderVarOwner owner)
        {
            _owner = owner;
            Name = userName;
        }

        protected void OnValueChanged()
        {
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
    }
}
