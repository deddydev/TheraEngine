using System;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLArgument<T> : BaseGLArgument where T : GLVar
    {
        public GLArgument(string name) : base(name) { }

        protected GLOutput<T> _connectedTo = null;
        protected GLOutput<T> ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }
        public override Type GetArgType() { return typeof(T); }
        public override Type[] GetPossibleArgTypes() { return new Type[] { GetArgType() }; }

        public override GLTypeName GetTypeName()
        {
            throw new NotImplementedException();
        }
    }
}
