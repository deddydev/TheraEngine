using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLMultiArgument : BaseGLArgument
    {
        GLMultiArgument[] _syncedArgs = new GLMultiArgument[0];
        private Type _currentArgType = null;

        public GLMultiArgument(string name, params GLTypeName[] types) : base(name) { }
        
        public void SetSyncedArguments(params GLMultiArgument[] args) { _syncedArgs = args; }

        protected BaseGLOutput _connectedTo = null;
        protected BaseGLOutput ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }
        public override Type[] GetPossibleArgTypes()
        {
            return _syncedArgs.Select(x => x.GetType()).ToArray();
        }
        public override Type GetArgType()
        {
            return _currentArgType;
        }

        public override GLTypeName GetTypeName()
        {
            throw new NotImplementedException();
        }
    }
}
