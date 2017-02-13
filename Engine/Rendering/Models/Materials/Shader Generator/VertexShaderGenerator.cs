using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public static class VertexShaderGenerator
    {
        private static ShaderGenerator _generator = new ShaderGenerator();
        private static PrimitiveBufferInfo _info;
        
        private static void wl(string str = "", params object[] args) { _generator.wl(str, args); }
        private static void WriteUniform(int layoutLocation, GLTypeName type, string name) { _generator.WriteUniform(layoutLocation, type, name); }
        private static void WriteUniform(GLTypeName type, string name) { _generator.WriteUniform(type, name); }
        private static void WriteInVar(int layoutLocation, GLTypeName type, string name) { _generator.WriteInVar(layoutLocation, type, name); }
        private static void WriteInVar(GLTypeName type, string name) { _generator.WriteInVar(type, name); }

        public static Shader Generate(ResultBasicFunc end)
        {
            throw new NotImplementedException();
        }
        public static Shader Generate(PrimitiveBufferInfo info, bool allowMeshMorphing, bool singleMorphRig, bool allowColorMorphing)
        {
            _info = info;

            allowMeshMorphing = allowMeshMorphing && _info._morphCount > 0;

            //Write #definitions
            _generator.WriteVersion();
            wl();

            //Write header in fields (from buffers)
            WriteBuffers(allowMeshMorphing);
            wl();
            //Write header uniforms
            WriteMatrixUniforms(allowMeshMorphing);
            wl();
            //Write header out fields (to fragment shader)
            WriteOutData();
            wl();

            //Write the beginning of the main function
            _generator.Begin();

            if (_info.IsWeighted)
                WriteRiggedPNTB(allowMeshMorphing, allowMeshMorphing ? singleMorphRig : true);
            else
                WriteStaticPNTB(allowMeshMorphing);

            for (int i = 0; i < _info._texcoordCount; ++i)
                wl("OutData.MultiTexCoord{0} = TexCoord{0};", i);

            return new Shader(ShaderMode.Vertex, _generator.Finish());
        }
        private static void WriteBuffers(bool morphed)
        {
            //Write mesh buffers first
            for (int i = 0; i < (morphed ? _info._morphCount + 1 : 1); ++i)
            {
                WriteInVar(GLTypeName._vec3, BufferType.Position.ToString() + i);
                if (_info.HasNormals)
                    WriteInVar(GLTypeName._vec3, BufferType.Normal.ToString() + i);
                if (_info.HasBinormals)
                    WriteInVar(GLTypeName._vec3, BufferType.Binormal.ToString() + i);
                if (_info.HasTangents)
                    WriteInVar(GLTypeName._vec3, BufferType.Tangent.ToString() + i);
            }

            //Then colors and texcoords
            for (int i = 0; i < _info._colorCount; ++i)
            {
                WriteInVar(GLTypeName._vec4, BufferType.Color.ToString() + i);
            }
            for (int i = 0; i < _info._texcoordCount; ++i)
            {
                WriteInVar(GLTypeName._vec2, BufferType.TexCoord.ToString() + i);
            }
            //Barycentric coord, for wireframe rendering
            if (_info._hasBarycentricCoord)
            {
                WriteInVar(GLTypeName._vec3, BufferType.Barycentric.ToString());
            }

            //And finally influence buffers
            if (_info.IsWeighted)
                for (int i = 0; i < (morphed ? _info._morphCount + 1 : 1); ++i)
                {
                    WriteInVar(GLTypeName._ivec4, BufferType.MatrixIds.ToString() + i);
                    WriteInVar(GLTypeName._vec4, BufferType.MatrixWeights.ToString() + i);
                }
        }
        private static void WriteMatrixUniforms(bool morphed)
        {
            WriteUniform(GLTypeName._mat4, "ModelMatrix");
            WriteUniform(GLTypeName._mat3, "NormalMatrix");
            WriteUniform(GLTypeName._mat4, "ViewMatrix");
            WriteUniform(GLTypeName._mat4, "ProjMatrix");
            if (_info.IsWeighted)
            {
                WriteUniform(GLTypeName._mat4, Uniform.BoneMatricesName + "[" + (_info._boneCount + 1) + "]");
                WriteUniform(GLTypeName._mat3, Uniform.BoneMatricesITName + "[" + (_info._boneCount + 1) + "]");
                if (morphed)
                    WriteUniform(GLTypeName._mat4, Uniform.MorphWeightsName + "[" + _info._morphCount + "]");
            }
        }
        /// <summary>
        /// This information is sent to the fragment shader.
        /// </summary>
        private static void WriteOutData()
        {
            wl("out Data {");
            wl("vec3 Position;");
            if (_info.HasNormals)
                wl("vec3 Normal;");
            if (_info.HasTangents)
                wl("vec3 Tangent;");
            if (_info.HasBinormals)
                wl("vec3 Binormal;");
            for (int i = 0; i < _info._texcoordCount; ++i)
                wl("vec2 MultiTexCoord{0};", i);
            wl("} OutData;");
        }
        private static void WriteRiggedPNTB(bool morphed, bool singleRig)
        {
            wl("vec4 finalPosition = vec4(0.0);");
            wl("vec4 basePosition = vec4(Position0, 1.0);");
            if (_info.HasNormals)
            {
                wl("OutData.Normal = vec3(0.0);");
                wl("vec3 baseNormal = Normal0;");
            }
            if (_info.HasBinormals)
            {
                wl("OutData.Binormal = vec3(0.0);");
                wl("vec3 baseBinormal = Binormal0;");
            }
            if (_info.HasTangents)
            {
                wl("OutData.Tangent = vec3(0.0);");
                wl("vec3 baseTangent = Tangent0;");
            }
            wl();
            if (singleRig)
            {
                wl("for (int i = 0; i < 4; ++i)");
                wl("{");
                wl("finalPosition += ({0}[{1}0[i]] * basePosition) * {2}0[i];", Uniform.BoneMatricesName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasNormals)
                    wl("OutData.Normal += ({0}[{1}0[i]] * baseNormal) * {2}0[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasBinormals)
                    wl("OutData.Binormal += ({0}[{1}0[i]] * baseBinormal) * {2}0[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasTangents)
                    wl("OutData.Tangent += ({0}[{1}0[i]] * baseTangent) * {2}0[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                wl("}");
                wl();
                if (_info.HasNormals)
                    wl("OutData.Normal = normalize(OutData.Normal);");
                if (_info.HasBinormals)
                    wl("OutData.Binormal = normalize(OutData.Binormal);");
                if (_info.HasTangents)
                    wl("OutData.Tangent = normalize(OutData.Tangent);");
            }
            else
            {
                wl("float totalWeight = 0.0;");
                wl("for (int i = 0; i < {0}; ++i)", _info._morphCount);
                wl("totalWeight += MorphWeights[i];");
                wl();
                wl("float baseWeight = 1.0 - totalWeight;");
                wl("float total = totalWeight + baseWeight;");
                wl();
                wl("basePosition *= baseWeight;");
                if (_info.HasNormals)
                    wl("baseNormal *= baseWeight;");
                if (_info.HasBinormals)
                    wl("baseBinormal *= baseWeight;");
                if (_info.HasTangents)
                    wl("baseTangent *= baseWeight;");
                wl();
                wl("for (int i = 0; i < 4; ++i)");
                wl("{");
                for (int i = 0; i < _info._morphCount; ++i)
                {
                    wl("finalPosition += {0}[{1}{3}[i]] * vec4(Position{5}, 1.0) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasNormals)
                        wl("OutData.Normal += ({0}[{1}{3}[i]] * Normal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasBinormals)
                        wl("OutData.Binormal += ({0}[{1}{3}[i]] * Binormal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasTangents)
                        wl("OutData.Tangent += ({0}[{1}{3}[i]] * Tangent{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (i + 1 != _info._morphCount)
                        wl();
                }
                wl("}");
                wl("finalPosition /= total;");
                if (_info.HasNormals)
                    wl("OutData.Normal /= total;");
                if (_info.HasBinormals)
                    wl("OutData.Binormal /= total;");
                if (_info.HasTangents)
                    wl("OutData.Tangent /= total;");
            }
            wl("OutData.Position = (ModelMatrix * finalPosition).xyz;");
            if (_info.HasNormals)
                wl("OutData.Normal = normalize(NormalMatrix * OutData.Normal);");
            if (_info.HasBinormals)
                wl("OutData.Binormal = normalize(NormalMatrix * OutData.Binormal);");
            if (_info.HasTangents)
                wl("OutData.Tangent = normalize(NormalMatrix * OutData.Tangent);");
            wl("gl_Position = ProjMatrix * ViewMatrix * vec4(OutData.Position, 1.0);");
        }
        private static void WriteStaticPNTB(bool morphed)
        {
            wl("vec4 position = vec4(Position0, 1.0);");
            if (_info.HasNormals)
                wl("vec3 normal = Normal0;");
            if (_info.HasBinormals)
                wl("vec3 binormal = Binormal0;");
            if (_info.HasTangents)
                wl("vec3 tangent = Tangent0;");
            wl();
            if (morphed)
            {
                wl("float totalWeight = 0.0;");
                wl("for (int i = 0; i < {0}; ++i)", _info._morphCount);
                wl("totalWeight += {0}[i];", Uniform.MorphWeightsName);
                wl("float baseWeight = 1.0 - totalWeight;");
                wl("float invTotal = 1.0 / (totalWeight + baseWeight);");
                wl();
                wl("position *= baseWeight;");
                if (_info.HasNormals)
                    wl("normal *= baseWeight;");
                if (_info.HasBinormals)
                    wl("binormal *= baseWeight;");
                if (_info.HasTangents)
                    wl("tangent *= baseWeight;");
                wl();
                for (int i = 0; i < _info._morphCount; ++i)
                {
                    wl("position += vec4(Position{0}, 1.0) * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasNormals)
                        wl("normal += Normal{0} * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasBinormals)
                        wl("binormal += Binormal{0} * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasTangents)
                        wl("tangent += Tangent{0} * MorphWeights[{1}];", i + 1, i);
                }
                wl();
                wl("position *= invTotal;");
                if (_info.HasNormals)
                    wl("normal *= invTotal;");
                if (_info.HasBinormals)
                    wl("binormal *= invTotal;");
                if (_info.HasTangents)
                    wl("tangent *= invTotal;");
                wl();
            }
            wl("position = ModelMatrix * position;");
            wl("OutData.Position = position.xyz;");
            wl("gl_Position = ProjMatrix * ViewMatrix * position;");
            if (_info.HasNormals)
                wl("OutData.Normal = normalize(NormalMatrix * normal);");
            if (_info.HasBinormals)
                wl("OutData.Binormal = normalize(NormalMatrix * binormal);");
            if (_info.HasTangents)
                wl("OutData.Tangent = normalize(NormalMatrix * tangent);");
        }
    }
}
