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
            throw new NotImplementedException();
        }
    }
}
