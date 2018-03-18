using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models.Materials.Functions
{
    public class AppendVector : ShaderMethod
    {
        protected override string GetOperation()
        {
            throw new NotImplementedException();
        }

        protected override MatFuncValueInput[] GetValueInputs()
        {
            return base.GetValueInputs();
        }
        protected override MatFuncValueOutput[] GetValueOutputs()
        {
            return new MatFuncValueOutput[]
            {
                new MatFuncValueOutput("")
            };
        }
    }
}
