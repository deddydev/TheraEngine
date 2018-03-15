using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class AppendVector : ShaderMethod
    {
        protected override List<MatFuncValueInput> GetValueInputs()
        {
            return base.GetValueInputs();
        }
        protected override List<MatFuncValueOutput> GetValueOutputs()
        {
            return new List<MatFuncValueOutput>()
            {
                new MatFuncValueOutput("")
            };
        }
    }
    public class JoinVec3 : MaterialFunction
    {
        protected override string GetOperation()
        {

        }
    }
    public class JoinVec4 : MaterialFunction
    {
        protected override string GetOperation()
        {

        }
    }
}
