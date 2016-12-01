using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLArgument
    {
        public BaseGLArgument(string name) { _name = name; }

        public string Name { get { return _name; } }
        public bool IsOutput { get { return _isOutput; } }
        public BaseGLArgument ConnectedTo { get { return _connectedTo; } }

        protected BaseGLArgument _connectedTo = null;
        protected bool _isOutput;
        protected string _name;
        public abstract GLTypeName GetArgType();

        public bool TryConnectTo(BaseGLArgument other)
        {
            if (!CanConnectTo(other))
                return false;
            DoConnection(other);
            return true;
        }
        protected virtual void DoConnection(BaseGLArgument other)
        {
            if (_connectedTo != null)
            {
                _connectedTo.ClearConnection();
            }
            _connectedTo = other;
        }
        public virtual bool CanConnectTo(BaseGLArgument other)
        {
            return other != null && 
                GetArgType() == other.GetArgType() &&
                _isOutput != other._isOutput;
        }
    }
}
