using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class VertexShaderGenerator : ShaderGenerator
    {
        public VertexShaderGenerator(PrimitiveBufferInfo info) : base(info) { }

        protected const string BoneCountDef = "BONE_COUNT";
        public override string Generate(ResultBasicFunc end)
        {
            throw new NotImplementedException();
        }
        public string Generate(string code)
        {
            WriteVersion();
            if (_info.IsWeighted)
                wl("#define {0} = {1};", BoneCountDef, _info._boneCount);
            WriteBuffers();
            WriteMatrixUniforms();
            WriteOutData();
            Begin();
            WriteVecDefines(false);
            if (_info.IsWeighted)
                WriteRiggedVNTB();
            else
                WriteStaticVNTB();
            wl(code);
            return Finish();
        }
        private void WriteVecDefines(bool morphable)
        {
            wl("vec4 finalPosition;");
            if (_info.HasNormals)
                wl("vec4 finalNormal;");
            if (_info.HasBinormals)
                wl("vec4 finalBinormal;");
            if (_info.HasTangents)
                wl("vec4 finalTangent;");
            if (morphable)
            {
                for (int i = 0; i < _info._positionCount; ++i)
                {

                }
            }
            if (_info.IsWeighted)
            {
                wl("OutData.Position = 0.0;");
                wl("vec4 finalPosition;");
                if (_info.HasNormals)
                {
                    wl("OutData.Normal = 0.0;");
                    wl("vec4 finalNormal;");
                }
                if (_info.HasBinormals)
                {
                    wl("OutData.Binormal = 0.0;");
                    wl("vec4 finalBinormal = vec4(Binormal0, 1.0);");
                }
                if (_info.HasTangents)
                {
                    wl("OutData.Tangent = 0.0;");
                    wl("vec4 finalTangent = vec4(Tangent0, 1.0);");
                }
            }
            else
            {

            }
        }
        private void WriteMatrixUniforms()
        {
            WriteUniform("mat4", "ModelMatrix");
            WriteUniform("mat4", "ViewMatrix");
            WriteUniform("mat4", "ProjMatrix");
            Comment("transpose(inverse(modelMatrix))");
            WriteUniform("mat4", "NormalMatrix");
            if (_info.IsWeighted)
            {
                WriteUniform("mat4", "BoneMatrices[" + BoneCountDef + "]");
                WriteUniform("mat4", "BoneMatricesIT[" + BoneCountDef + "]");
            }
        }
        /// <summary>
        /// This information is sent to the fragment shader.
        /// </summary>
        private void WriteOutData()
        {
            wl("out Data {");
            wl("vec3 Position;");
            if (_info.HasNormals)
                wl("vec3 Normal;");
            if (_info.HasTangents)
                wl("vec3 Tangent;");
            if (_info.HasBinormals)
                wl("vec3 Binormal;");
            wl("} OutData;");
        }
        private void WriteRiggedVNTB()
        {
            wl("for (int i = 0; i < 4; ++i)");
            wl("{");
            wl("OutData.Position += BoneMatrices[MatrixIds[i]] * finalPosition * MatrixWeights[i];");
            if (_info.HasNormals)
                wl("OutData.Normal += BoneMatricesIT[MatrixIds[i]] * finalNormal * MatrixWeights[i];");
            if (_info.HasBinormals)
                wl("OutData.Binormal += BoneMatricesIT[MatrixIds[i]] * finalBinormal * MatrixWeights[i];");
            if (_info.HasTangents)
                wl("OutData.Tangent += BoneMatricesIT[MatrixIds[i]] * finalTangent * MatrixWeights[i];");
            wl("}");

        }
        private void WriteStaticVNTB()
        {
            wl("finalPosition = ModelMatrix * vec4(Position0, 1.0);");
            wl("OutData.Position = tempPos.xyz;");
            wl("gl_Position = ProjMatrix * ViewMatrix * tempPos;");
            if (_info.HasNormals)
                wl("OutData.Normal = normalize((NormalMatrix * vec4(Normal0, 1.0)).xyz);");
            if (_info.HasBinormals)
                wl("OutData.Binormal = normalize((NormalMatrix * vec4(Binormal0, 1.0)).xyz);");
            if (_info.HasTangents)
                wl("OutData.Tangent = normalize((NormalMatrix * vec4(Tangent0, 1.0)).xyz);");
        }
        private void WriteBuffers()
        {
            int layoutLocation = 0;
            for (int i = 0; i < _info._positionCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Position" + i);
            for (int i = 0; i < _info._normalCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Normal" + i);
            for (int i = 0; i < _info._colorCount; ++i)
                WriteInVar(layoutLocation++, "vec4", "Color" + i);
            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteInVar(layoutLocation++, "vec2", "TexCoord" + i);
            for (int i = 0; i < _info._tangentCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Tangent" + i);
            for (int i = 0; i < _info._binormalCount; ++i)
                WriteInVar(layoutLocation++, "vec3", "Binormal" + i);
            if (_info.IsWeighted)
            {
                WriteInVar(layoutLocation++, "ivec4", "MatrixIds");
                WriteInVar(layoutLocation++, "vec4", "MatrixWeights");
            }
        }
    }
}
