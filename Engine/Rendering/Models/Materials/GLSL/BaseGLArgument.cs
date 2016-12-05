namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLArgument
    {
        public BaseGLArgument(string name) { _name = name; }

        public string Name { get { return _name; } }
        public abstract bool IsOutput { get; }

        protected string _name;
        public abstract GLTypeName GetArgType();

        public bool TryConnectTo(BaseGLArgument other)
        {
            if (!CanConnectTo(other))
                return false;
            DoConnection(other);
            return true;
        }
        public abstract void ClearConnection(BaseGLArgument other);
        protected abstract void DoConnection(BaseGLArgument other);
        public abstract bool CanConnectTo(BaseGLArgument other);
    }
}
