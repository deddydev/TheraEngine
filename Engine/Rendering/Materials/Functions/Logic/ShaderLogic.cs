using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public abstract class ShaderLogic : MaterialFunction
    {
        public ShaderLogic() : base() { }

        public abstract string GetLogicFormat();
    }
}
