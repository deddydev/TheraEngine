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
            WriteMatrixUniforms(info._boneCount);
            Begin();
            return Finish();
        }
        private void WriteBuffers(PrimitiveBufferInfo info)
        {
            int layoutLocation = 0;
            for (int i = 0; i < info._positionCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Position" + i);
            for (int i = 0; i < info._normalCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Normal" + i);
            for (int i = 0; i < info._texcoordCount; ++i)
                WriteInVar(layoutLocation++, "vec2", "TexCoord" + i);
            for (int i = 0; i < info._colorCount; ++i)
                WriteInVar(layoutLocation++, "vec4", "Color" + i);
            for (int i = 0; i < info._tangentCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Tangent" + i);
            for (int i = 0; i < info._binormalCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Binormal" + i);
        }
        private void GenerateHeader()
        {
            string source = @"
#version 410

layout (location = 0) in vec3 Position0;
layout (location = 1) in vec3 Normal0;
layout (location = 2) in vec2 TexCoord0;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjMatrix;
uniform mat4 NormalMatrix; //transpose(inverse(modelMatrix))

out Data {
    vec3 Position;
    vec3 Normal;
} OutData;

void main()
{
    OutData.Position = (ModelMatrix * vec4(Position0, 1.0)).xyz;
    OutData.Normal = normalize((NormalMatrix * vec4(Normal0, 1.0)).xyz);

    gl_Position = ProjMatrix * ViewMatrix * ModelMatrix * vec4(Position0, 1.0);
}";
        }
    }
}
