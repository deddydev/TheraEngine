using System;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLInput<T> : BaseGLArgument where T : GLVar
    {
        public GLInput(string name) : base(name) { }
        public override bool IsOutput { get { return false; } }
        public override GLTypeName GetArgType() { return GLVar.TypeAssociations[GetType()]; }

        public BaseGLArgument ConnectedTo { get { return _connectedTo; } }

        protected BaseGLArgument _connectedTo = null;

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
