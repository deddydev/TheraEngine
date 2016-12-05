using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLOutput<T> : BaseGLArgument where T : GLVar
    {
        public GLOutput(string name) : base(name) { }

        public override bool IsOutput { get { return true; } }

        protected List<BaseGLArgument> _connectedTo = null;
        protected List<BaseGLArgument> ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }
        public override GLTypeName GetArgType() { return GLVar.TypeAssociations[GetType()]; }

        public override void ClearConnection(BaseGLArgument other)
        {
            if (_connectedTo == null || _connectedTo != other)
                return;

            BaseGLArgument o = _connectedTo;
            _connectedTo = null;
            o.ClearConnection(this);
        }
        public override bool CanConnectTo(BaseGLArgument other)
        {
            return other != null && GetArgType() == other.GetArgType() && IsOutput != other.IsOutput;
        }
        protected override void DoConnection(BaseGLArgument other)
        {
            if (_connectedTo != null)
                _connectedTo.ClearConnection(this);
            _connectedTo = other;
        }
    }
}
