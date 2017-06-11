using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum GLTypeName : int
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
    public abstract class GLVar : IGLVarOwner
    {
        public static Dictionary<Type, GLTypeName> TypeAssociations = new Dictionary<Type, GLTypeName>()
        {
            { typeof(GLBool),   GLTypeName._bool   },
            { typeof(GLInt),    GLTypeName._int    },
            { typeof(GLUInt),   GLTypeName._uint   },
            { typeof(GLFloat),  GLTypeName._float  },
            { typeof(GLDouble), GLTypeName._double },
            { typeof(GLVec2),   GLTypeName._vec2   },
            { typeof(GLVec3),   GLTypeName._vec3   },
            { typeof(GLVec4),   GLTypeName._vec4   },
            { typeof(GLMat3),   GLTypeName._mat3   },
            { typeof(GLMat4),   GLTypeName._mat4   },
            { typeof(GLIVec2),  GLTypeName._ivec2  },
            { typeof(GLIVec3),  GLTypeName._ivec3  },
            { typeof(GLIVec4),  GLTypeName._ivec4  },
            { typeof(GLUVec2),  GLTypeName._uvec2  },
            { typeof(GLUVec3),  GLTypeName._uvec3  },
            { typeof(GLUVec4),  GLTypeName._uvec4  },
            { typeof(GLDVec2),  GLTypeName._dvec2  },
            { typeof(GLDVec3),  GLTypeName._dvec3  },
            { typeof(GLDVec4),  GLTypeName._dvec4  },
            { typeof(GLBVec2),  GLTypeName._bvec2  },
            { typeof(GLBVec3),  GLTypeName._bvec3  },
            { typeof(GLBVec4),  GLTypeName._bvec4  },
        };
        public static Dictionary<GLTypeName, Type> GLTypeAssociations = new Dictionary<GLTypeName, Type>()
        {
            { GLTypeName._bool,   typeof(GLBool)   },
            { GLTypeName._int,    typeof(GLInt)    },
            { GLTypeName._uint,   typeof(GLUInt)   },
            { GLTypeName._float,  typeof(GLFloat)  },
            { GLTypeName._double, typeof(GLDouble) },
            { GLTypeName._vec2,   typeof(GLVec2)   },
            { GLTypeName._vec3,   typeof(GLVec3)   },
            { GLTypeName._vec4,   typeof(GLVec4)   },
            { GLTypeName._mat3,   typeof(GLMat3)   },
            { GLTypeName._mat4,   typeof(GLMat4)   },
            { GLTypeName._ivec2,  typeof(GLIVec2)  },
            { GLTypeName._ivec3,  typeof(GLIVec3)  },
            { GLTypeName._ivec4,  typeof(GLIVec4)  },
            { GLTypeName._uvec2,  typeof(GLUVec2)  },
            { GLTypeName._uvec3,  typeof(GLUVec3)  },
            { GLTypeName._uvec4,  typeof(GLUVec4)  },
            { GLTypeName._dvec2,  typeof(GLDVec2)  },
            { GLTypeName._dvec3,  typeof(GLDVec3)  },
            { GLTypeName._dvec4,  typeof(GLDVec4)  },
            { GLTypeName._bvec2,  typeof(GLBVec2)  },
            { GLTypeName._bvec3,  typeof(GLBVec3)  },
            { GLTypeName._bvec4,  typeof(GLBVec4)  },
        };

        protected bool _canSwizzle = true;
        protected IGLVarOwner _owner;
        protected string _name;
        protected Dictionary<string, GLVar> _fields = new Dictionary<string, GLVar>();

        public IGLVarOwner Owner { get { return _owner; } }
        public abstract GLTypeName TypeName { get; }

        public string Name
        {
            get { return _name; }
            set
            {
                //Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value)
                _name = value.ReplaceWhitespace("");
            }
        }

        public void SetUniform(string name) { SetUniform(Engine.Renderer.GetUniformLocation(name)); }
        public void SetUniform() { SetUniform(Engine.Renderer.GetUniformLocation(Name)); }
        public abstract void SetUniform(int location);

        public GLVar(string userName, IGLVarOwner owner)
        {
            _owner = owner;
            Name = userName;
        }

        /// <summary>
        /// Ex: layout (location = 0) uniform float potato;
        /// </summary>
        public string GetUniformDeclaration(int layoutId = -1)
        {
            string line = "";
            if (layoutId >= 0)
                line = string.Format("layout (location = {0}) ", layoutId);
            return line + string.Format("uniform {0};", GetDeclaration());
        }
        public string GetDeclaration()
        {
            return string.Format("{0} {1}", TypeName.ToString().Substring(1), Name);
        }
        public abstract string GetValueString();
        /// <summary>
        /// Ex: this is float '.x', parent is vec4 '[0]', parent is mat4 'tomato': tomato[0].x
        /// </summary>
        /// <returns></returns>
        public virtual string AccessorTree()
        {
            return Name;
        }
    }
}
