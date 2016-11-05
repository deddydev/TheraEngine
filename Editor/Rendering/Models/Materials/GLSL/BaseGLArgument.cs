using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLArgument
    {
        protected string _name;

        public BaseGLArgument(string name)
        {
            _name = name;
        }

        public bool CanConnectTo(BaseGLArgument other)
        {
            if (other == null)
                return true;

            if ((this is IGLArgument && other is IGLArgument) || 
                (this is IGLOutput && other is IGLOutput))
                return false;

            if (this is IGLArgument)
            {
                IGLArgument arg = (IGLArgument)this;
                IGLOutput output = (IGLOutput)other;
                if (arg.GetArgType() != output.GetOutType())
                    return false;
            }
            else
            {
                IGLArgument arg = (IGLArgument)other;
                IGLOutput output = (IGLOutput)this;
                if (arg.GetArgType() != output.GetOutType())
                    return false;
            }

            return true;
        }
    }
}
