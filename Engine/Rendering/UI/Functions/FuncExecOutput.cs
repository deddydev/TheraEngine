namespace TheraEngine.Rendering.UI.Functions
{
    public interface IFuncExecOutput : IBaseFuncExec
    {
        void SetConnection(IFuncExecInput other);
    }
    public class FuncExecOutput<TInput, TParent> : BaseFuncExec<TInput>, IFuncExecOutput
        where TInput : UIComponent, IFuncExecInput where TParent : UIComponent, IFunction
    {
        public override bool IsOutput => true;

        public FuncExecOutput(string name)
            : base(name) { }
        public FuncExecOutput(string name, TParent parent)
            : base(name, parent) { }
        
        public virtual void SetConnection(IFuncExecInput other)
            => SetConnection(other as TInput);
        public virtual void SetConnection(TInput other)
        {
            if (other == _connectedTo)
                return;
            _connectedTo?.ClearConnection();
            _connectedTo = other;
            _connectedTo?.SetConnection(this);
        }
    }
}
