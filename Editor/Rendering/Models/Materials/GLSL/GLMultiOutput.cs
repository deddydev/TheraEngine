using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLMultiOutput : BaseGLOutput
    {
        GLMultiArgument[] _syncedArgs = new GLMultiArgument[0];
        private Type _currentArgType = null;

        public GLMultiOutput(string name, params GLTypeName[] types) : base(name) { }
        
        public void SetSyncedOutputs(params GLMultiArgument[] args) { _syncedArgs = args; }

        protected BaseGLArgument _connectedTo = null;
        protected BaseGLArgument ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }
        public override Type GetOutType()
        {
            return _currentArgType;
        }

        public override Type[] GetPossibleOutTypes()
        {
            return _syncedArgs.Select(x => x.GetType()).ToArray();
        }

        public override GLTypeName GetTypeName()
        {
            throw new NotImplementedException();
        }
    }
}
