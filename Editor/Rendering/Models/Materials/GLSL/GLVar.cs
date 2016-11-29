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
        protected Dictionary<string, GLVar> _fields = new Dictionary<string, GLVar>();

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
