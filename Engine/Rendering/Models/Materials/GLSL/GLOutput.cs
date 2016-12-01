using System;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLOutput<T> : BaseGLOutput where T : GLVar
    {
        public GLOutput(string name) : base(name) { }

        protected GLArgument<T> _connectedTo = null;
        protected GLArgument<T> ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }
        public override Type GetOutType() { return typeof(T); }
        public override Type[] GetPossibleOutTypes() { return new Type[] { GetOutType() }; }

        public override GLTypeName GetTypeName()
        {
            throw new NotImplementedException();
        }
    }
}
