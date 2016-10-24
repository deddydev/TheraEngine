using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public enum GLTypeName
    {
        _float,
        _vec2,
        _vec3,
        _vec4,
        _mat3,
        _mat4,
    }
    public abstract class GLVar : IGLVarOwner
    {
        GLTypeName _typeName;
        string _name;
        bool _isOutput;
        GLVar _connectedTo;
        IGLVarOwner _owner;
        protected List<GLVar> _fields = new List<GLVar>();

        public GLVar(GLTypeName typeName, string name)
        {
            _typeName = typeName;
            _name = name;
        }

        public void Setup(bool output, MaterialFunction owner)
        {
            _isOutput = output;
            _owner = owner;
        }

        public GLTypeName TypeName { get { return _typeName; } }
        public string Name { get { return _name; } }

        public bool IsOutput { get { return _isOutput; } }
        public IGLVarOwner Owner { get { return _owner; } }

        public GLVar ConnectedTo
        {
            get { return _connectedTo; }
            set { _connectedTo = FindConnectable(value, this); }
        }
        public GLVar FindConnectable(GLVar target, GLVar compared)
        {
            if (target.CanConnectTo(compared))
                return target;
            foreach (GLVar v in _fields)
                if (FindConnectable(v, compared) != null)
                    return v;
            return null;
        }
        public bool CanConnectTo(GLVar other)
        {
            if (other.IsOutput == _isOutput)
                return false;
            if (other._typeName != _typeName)
                return false;
            return true;
        }
    }
}
