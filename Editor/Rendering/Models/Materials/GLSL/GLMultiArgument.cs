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
        public GLMultiArgument(string name, params GLTypeName[] types) : base(name) { }
        
        protected BaseGLOutput _connectedTo = null;
        protected GLOutput<T> ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }

        public override Type[] GetArgType()
        {
            throw new NotImplementedException();
        }
    }
}
