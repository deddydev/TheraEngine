using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public interface IGLArgument { Type GetArgType(); }
    public class GLArgument<T> : BaseGLArgument, IGLArgument where T : GLVar
    {
        public GLArgument(string name) : base(name) { }

        protected GLOutput<T> _connectedTo = null;
        protected GLOutput<T> ConnectedTo
        {
            get { return _connectedTo; }
            set
            {
                if (CanConnectTo(value))
                    _connectedTo = value;
            }
        }
        public Type GetArgType() { return typeof(T); }
    }
}
