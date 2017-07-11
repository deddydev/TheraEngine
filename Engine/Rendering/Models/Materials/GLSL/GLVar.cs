using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum ShaderType : int
    {
        Invalid = -1,

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
    public abstract class ShaderVar : IShaderVarOwner
    {
        internal static Dictionary<Type, ShaderType> TypeAssociations = new Dictionary<Type, ShaderType>()
        {
            { typeof(GLBool),   ShaderType._bool   },
            { typeof(GLInt),    ShaderType._int    },
            { typeof(GLUInt),   ShaderType._uint   },
            { typeof(GLFloat),  ShaderType._float  },
            { typeof(GLDouble), ShaderType._double },
            { typeof(GLVec2),   ShaderType._vec2   },
            { typeof(GLVec3),   ShaderType._vec3   },
            { typeof(GLVec4),   ShaderType._vec4   },
            { typeof(GLMat3),   ShaderType._mat3   },
            { typeof(GLMat4),   ShaderType._mat4   },
            { typeof(GLIVec2),  ShaderType._ivec2  },
            { typeof(GLIVec3),  ShaderType._ivec3  },
            { typeof(GLIVec4),  ShaderType._ivec4  },
            { typeof(GLUVec2),  ShaderType._uvec2  },
            { typeof(GLUVec3),  ShaderType._uvec3  },
            { typeof(GLUVec4),  ShaderType._uvec4  },
            { typeof(GLDVec2),  ShaderType._dvec2  },
            { typeof(GLDVec3),  ShaderType._dvec3  },
            { typeof(GLDVec4),  ShaderType._dvec4  },
            { typeof(GLBVec2),  ShaderType._bvec2  },
            { typeof(GLBVec3),  ShaderType._bvec3  },
            { typeof(GLBVec4),  ShaderType._bvec4  },
        };
        internal static Dictionary<ShaderType, Type> GLTypeAssociations = new Dictionary<ShaderType, Type>()
        {
            { ShaderType._bool,   typeof(GLBool)   },
            { ShaderType._int,    typeof(GLInt)    },
            { ShaderType._uint,   typeof(GLUInt)   },
            { ShaderType._float,  typeof(GLFloat)  },
            { ShaderType._double, typeof(GLDouble) },
            { ShaderType._vec2,   typeof(GLVec2)   },
            { ShaderType._vec3,   typeof(GLVec3)   },
            { ShaderType._vec4,   typeof(GLVec4)   },
            { ShaderType._mat3,   typeof(GLMat3)   },
            { ShaderType._mat4,   typeof(GLMat4)   },
            { ShaderType._ivec2,  typeof(GLIVec2)  },
            { ShaderType._ivec3,  typeof(GLIVec3)  },
            { ShaderType._ivec4,  typeof(GLIVec4)  },
            { ShaderType._uvec2,  typeof(GLUVec2)  },
            { ShaderType._uvec3,  typeof(GLUVec3)  },
            { ShaderType._uvec4,  typeof(GLUVec4)  },
            { ShaderType._dvec2,  typeof(GLDVec2)  },
            { ShaderType._dvec3,  typeof(GLDVec3)  },
            { ShaderType._dvec4,  typeof(GLDVec4)  },
            { ShaderType._bvec2,  typeof(GLBVec2)  },
            { ShaderType._bvec3,  typeof(GLBVec3)  },
            { ShaderType._bvec4,  typeof(GLBVec4)  },
        };

        protected bool _canSwizzle = true;
        protected IShaderVarOwner _owner;
        protected string _name;
        protected Dictionary<string, ShaderVar> _fields = new Dictionary<string, ShaderVar>();

        internal IShaderVarOwner Owner => _owner;
        public abstract ShaderType TypeName { get; }

        public string Name
        {
            get => _name;
            set
            {
                //Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value)
                _name = value.ReplaceWhitespace("");
            }
        }

        internal void SetProgramUniform(int programBindingId, string name) 
            => SetProgramUniform(programBindingId, Engine.Renderer.GetUniformLocation(programBindingId, name));

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

        /// <summary>
        /// Ex: layout (location = 0) uniform float potato;
        /// </summary>
        internal string GetUniformDeclaration(int layoutId = -1)
        {
            string line = "";
            if (layoutId >= 0)
                line = string.Format("layout (location = {0}) ", layoutId);
            return line + string.Format("uniform {0};", GetDeclaration());
        }
        internal string GetDeclaration()
        {
            return string.Format("{0} {1}", TypeName.ToString().Substring(1), Name);
        }
        internal abstract string GetValueString();
        /// <summary>
        /// Ex: this is float '.x', parent is vec4 '[0]', parent is mat4 'tomato': tomato[0].x
        /// </summary>
        /// <returns></returns>
        internal virtual string AccessorTree()
        {
            return Name;
        }
    }
}
