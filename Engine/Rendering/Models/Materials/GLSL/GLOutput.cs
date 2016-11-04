using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public interface IGLOutput { Type GetOutType(); }
    public class GLOutput<T> : BaseGLArgument, IGLOutput where T : GLVar
    {
        public GLOutput(string name) : base(name) { }

        protected GLArgument<T> _connectedTo = null;
        protected GLArgument<T> ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }
        public Type GetOutType() { return typeof(T); }
    }
}
