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
        public string Generate(PrimitiveBufferInfo info)
        {
            WriteVersion();
            WriteBuffers(info);
            if (info._boneCount > 0)
            {

            }
            else
            {

            }
            Begin();
            return Finish();
        }
        private void WriteBuffers(PrimitiveBufferInfo info)
        {
            if (info._positionCount > 0)
            {

            }
        }
    }
}
