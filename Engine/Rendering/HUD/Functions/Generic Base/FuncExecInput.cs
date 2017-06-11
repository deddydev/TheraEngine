using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TheraEngine.Rendering.HUD;

namespace TheraEngine.Rendering
{
    public interface IFuncExecInput : IBaseFuncExec
    {
        void SetConnection(IFuncExecOutput other);
    }
    public class FuncExecInput<TOutput, TParent> : BaseFuncExec<TOutput>, IFuncExecInput
        where TOutput : HudComponent, IFuncExecOutput where TParent : HudComponent, IFunction
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
