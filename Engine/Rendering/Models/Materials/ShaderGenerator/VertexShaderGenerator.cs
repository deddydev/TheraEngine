using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class VertexShaderGenerator : ShaderGenerator
    {
        public override string Generate(ResultBasicFunc end)
        {
            throw new NotImplementedException();
        }
        public string Generate(bool weighted, string[] bufferNames)
        {
            WriteVersion();
            foreach (GLVar u in Uniform.CommonUniforms)
            {

            }
            Begin();
            return Finish();
        }
    }
}
