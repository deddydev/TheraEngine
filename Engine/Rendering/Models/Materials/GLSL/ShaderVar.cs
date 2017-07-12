﻿using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum ShaderVarType : int
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
        internal static Dictionary<Type, ShaderVarType> TypeAssociations = new Dictionary<Type, ShaderVarType>()
        {
            { typeof(ShaderBool),   ShaderVarType._bool   },
            { typeof(ShaderInt),    ShaderVarType._int    },
            { typeof(ShaderUInt),   ShaderVarType._uint   },
            { typeof(ShaderFloat),  ShaderVarType._float  },
            { typeof(ShaderDouble), ShaderVarType._double },
            { typeof(ShaderVec2),   ShaderVarType._vec2   },
            { typeof(ShaderVec3),   ShaderVarType._vec3   },
            { typeof(ShaderVec4),   ShaderVarType._vec4   },
            { typeof(ShaderMat3),   ShaderVarType._mat3   },
            { typeof(ShaderMat4),   ShaderVarType._mat4   },
            { typeof(ShaderIVec2),  ShaderVarType._ivec2  },
            { typeof(ShaderIVec3),  ShaderVarType._ivec3  },
            { typeof(ShaderIVec4),  ShaderVarType._ivec4  },
            { typeof(ShaderUVec2),  ShaderVarType._uvec2  },
            { typeof(ShaderUVec3),  ShaderVarType._uvec3  },
            { typeof(ShaderUVec4),  ShaderVarType._uvec4  },
            { typeof(ShaderDVec2),  ShaderVarType._dvec2  },
            { typeof(ShaderDVec3),  ShaderVarType._dvec3  },
            { typeof(ShaderDVec4),  ShaderVarType._dvec4  },
            { typeof(ShaderBVec2),  ShaderVarType._bvec2  },
            { typeof(ShaderBVec3),  ShaderVarType._bvec3  },
            { typeof(ShaderBVec4),  ShaderVarType._bvec4  },
        };
        internal static Dictionary<ShaderVarType, Type> GLTypeAssociations = new Dictionary<ShaderVarType, Type>()
        {
            { ShaderVarType._bool,   typeof(ShaderBool)   },
            { ShaderVarType._int,    typeof(ShaderInt)    },
            { ShaderVarType._uint,   typeof(ShaderUInt)   },
            { ShaderVarType._float,  typeof(ShaderFloat)  },
            { ShaderVarType._double, typeof(ShaderDouble) },
            { ShaderVarType._vec2,   typeof(ShaderVec2)   },
            { ShaderVarType._vec3,   typeof(ShaderVec3)   },
            { ShaderVarType._vec4,   typeof(ShaderVec4)   },
            { ShaderVarType._mat3,   typeof(ShaderMat3)   },
            { ShaderVarType._mat4,   typeof(ShaderMat4)   },
            { ShaderVarType._ivec2,  typeof(ShaderIVec2)  },
            { ShaderVarType._ivec3,  typeof(ShaderIVec3)  },
            { ShaderVarType._ivec4,  typeof(ShaderIVec4)  },
            { ShaderVarType._uvec2,  typeof(ShaderUVec2)  },
            { ShaderVarType._uvec3,  typeof(ShaderUVec3)  },
            { ShaderVarType._uvec4,  typeof(ShaderUVec4)  },
            { ShaderVarType._dvec2,  typeof(ShaderDVec2)  },
            { ShaderVarType._dvec3,  typeof(ShaderDVec3)  },
            { ShaderVarType._dvec4,  typeof(ShaderDVec4)  },
            { ShaderVarType._bvec2,  typeof(ShaderBVec2)  },
            { ShaderVarType._bvec3,  typeof(ShaderBVec3)  },
            { ShaderVarType._bvec4,  typeof(ShaderBVec4)  },
        };

        protected bool _canSwizzle = true;
        protected IShaderVarOwner _owner;
        protected string _name;
        protected Dictionary<string, ShaderVar> _fields = new Dictionary<string, ShaderVar>();

        internal IShaderVarOwner Owner => _owner;
        public abstract ShaderVarType TypeName { get; }

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
