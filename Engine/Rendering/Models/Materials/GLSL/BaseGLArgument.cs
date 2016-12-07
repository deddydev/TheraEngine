using CustomEngine.Rendering.HUD;
namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLArgument : HudComponent
    {
        public BaseGLArgument(string name) : base(null) { _name = name; }
        public BaseGLArgument(string name, MaterialFunction parent) : base(parent) { _name = name; }
        
        public abstract bool IsOutput { get; }
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
        public virtual bool CanConnectTo(BaseGLArgument other)
        {
            return 
                other != null &&
                other.IsOutput != IsOutput &&
                GetArgType() == other.GetArgType();
        }
    }
}
