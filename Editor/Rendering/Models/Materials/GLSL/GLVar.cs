using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
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
        protected IGLVarOwner _owner;
        protected string _name;
        protected Dictionary<string, GLSubVar> _fields = new Dictionary<string, GLSubVar>();

        public IGLVarOwner Owner { get { return _owner; } }
        public abstract GLTypeName TypeName { get; }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value).ReplaceWhitespace("");
            }
        }

        private GLTypeName GetSubType(GLTypeName type)
        {
            string id = type.ToString().Substring(1, 1);
            switch (id)
            {
                case "i":
                    return GLTypeName._int;
                case "u":
                    return GLTypeName._uint;
                case "b":
                    return GLTypeName._bool;
                case "d":
                    return GLTypeName._double;
                default:
                    return GLTypeName._float;
            }
        }

        public GLVar(string userName, IGLVarOwner owner)
        {
            _owner = owner;
            Name = userName;
            GLTypeName t;
            switch (typeName)
            {
                case GLTypeName._float:
                case GLTypeName._double:
                case GLTypeName._bool:
                case GLTypeName._int:
                case GLTypeName._uint:
                    break;
                case GLTypeName._vec2:
                case GLTypeName._bvec2:
                case GLTypeName._ivec2:
                case GLTypeName._uvec2:
                case GLTypeName._dvec2:
                    t = GetSubType(typeName);
                    _fields.Add(new GLSubVar(t, "X", ".x", this));
                    _fields.Add(new GLSubVar(t, "Y", ".y", this));
                    break;
                case GLTypeName._vec3:
                case GLTypeName._bvec3:
                case GLTypeName._ivec3:
                case GLTypeName._uvec3:
                case GLTypeName._dvec3:
                    t = GetSubType(typeName);
                    _fields.Add(new GLSubVar(t, "X", ".x", this));
                    _fields.Add(new GLSubVar(t, "Y", ".y", this));
                    _fields.Add(new GLSubVar(t, "Z", ".z", this));
                    break;
                case GLTypeName._vec4:
                case GLTypeName._bvec4:
                case GLTypeName._ivec4:
                case GLTypeName._uvec4:
                case GLTypeName._dvec4:
                    t = GetSubType(typeName);
                    _fields.Add(new GLSubVar(t, "X", ".x", this));
                    _fields.Add(new GLSubVar(t, "Y", ".y", this));
                    _fields.Add(new GLSubVar(t, "Z", ".z", this));
                    _fields.Add(new GLSubVar(t, "W", ".w", this));
                    break;
                case GLTypeName._mat3:
                    _fields.Add(new GLSubVar(GLTypeName._vec3, "Column0", "[0]", this));
                    _fields.Add(new GLSubVar(GLTypeName._vec3, "Column1", "[1]", this));
                    _fields.Add(new GLSubVar(GLTypeName._vec3, "Column2", "[2]", this));
                    break;
                case GLTypeName._mat4:
                    _fields.Add(new GLSubVar(GLTypeName._vec4, "Column0", "[0]", this));
                    _fields.Add(new GLSubVar(GLTypeName._vec4, "Column1", "[1]", this));
                    _fields.Add(new GLSubVar(GLTypeName._vec4, "Column2", "[2]", this));
                    _fields.Add(new GLSubVar(GLTypeName._vec4, "Column3", "[3]", this));
                    break;
            }
        }

        /// <summary>
        /// Ex: layout (location = 0) uniform float potato;
        /// </summary>
        public string GetGlobalFieldLine(int layoutId, EQualifier qualifier)
        {
            return string.Format("layout (location = {0}) {1} {2};", 
                layoutId, qualifier.ToString().Substring(1), GetDeclaration());
        }
        /// <summary>
        /// Ex: uniform float potato -> "float potato"
        /// </summary>
        public string GetDeclaration()
        {
            return string.Format("{0} {1}", _typeName.ToString().Substring(1), Name);
        }
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
