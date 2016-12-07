using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLMultiOutput : BaseGLMultiInput
    {
        protected List<BaseGLArgument> _connectedTo = new List<BaseGLArgument>();

        public override bool IsOutput { get { return false; } }

        public GLMultiOutput(string name, params GLTypeName[] types) : base(name)
        {
            _allowedArgTypes = types;
        }
        public GLMultiOutput(string name, BaseGLMultiInput linkedMultiArg) : base(name, linkedMultiArg)
        {
            _syncedArgs.Add(linkedMultiArg);
            _allowedArgTypes = linkedMultiArg._allowedArgTypes;
        }
        public override void ClearConnection(BaseGLArgument other)
        {
            if (_connectedTo == null || _connectedTo != other)
                return;

            BaseGLArgument o = _connectedTo;
            _connectedTo = null;
            o.ClearConnection(this);
        }

        protected override void DoConnection(BaseGLArgument other)
        {
            throw new NotImplementedException();
        }
    }
}
