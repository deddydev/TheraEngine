using System;
using TheraEngine.Components;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IBaseFuncExec : IUIComponent
    {
        void ClearConnection();
    }
    public abstract class BaseFuncExec : BaseFuncArg
    {
        public static ColorF4 DefaultColor { get; } = new ColorF4(0.7f, 1.0f);

        public BaseFuncExec(string name) : base(name, DefaultColor) { }
        public BaseFuncExec(string name, IFunction parent) : base(name, parent, DefaultColor) { }
    }
    public abstract class BaseFuncExec<TInput> : BaseFuncExec where TInput : UIComponent, IBaseFuncExec
    {
        public TInput ConnectedTo => _connectedTo;

        protected TInput _connectedTo;

        public BaseFuncExec(string name) : base(name) { }
        public BaseFuncExec(string name, IFunction parent) : base(name, parent) { }
        
        public virtual void ClearConnection()
        {
            TInput temp = _connectedTo;
            _connectedTo = null;
            temp?.ClearConnection();
        }
    }
}
