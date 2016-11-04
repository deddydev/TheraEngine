using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLArgument : GLVar
    {
        protected bool _isOutput;
        protected GLVar _connectedTo;
        public GLArgument(
            GLTypeName type,
            string name, 
            IGLVarOwner owner,
            bool isOutput,
            bool isArray = false) 
            : base(type, name, owner, isArray)
        {
            _isOutput = isOutput;
        }
        public void Setup(bool output, MaterialFunction owner)
        {
            _isOutput = output;
            _owner = owner;
        }

    }
}
