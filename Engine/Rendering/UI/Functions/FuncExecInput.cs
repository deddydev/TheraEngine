namespace TheraEngine.Rendering.UI.Functions
{
    public interface IFuncExecInput : IBaseFuncExec
    {
        void SetConnection(IFuncExecOutput other);
    }
    public class FuncExecInput<TOutput, TParent> : BaseFuncExec<TOutput>, IFuncExecInput
        where TOutput : UIComponent, IFuncExecOutput where TParent : UIComponent, IFunction
    {
        public override bool IsOutput => false;

        public FuncExecInput(string name)
            : base(name) { }
        public FuncExecInput(string name, TParent parent)
            : base(name, parent) { }
        
        public virtual void SetConnection(IFuncExecOutput other)
            => SetConnection(other as TOutput);
        public virtual void SetConnection(TOutput other)
        {
            if (other == _connectedTo)
                return;
            _connectedTo?.ClearConnection();
            _connectedTo = other;
            _connectedTo?.SetConnection(this);
        }
    }
}
