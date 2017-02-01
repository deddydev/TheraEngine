using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public static class VertexShaderGenerator
    {
        //private const string BoneCountDef = "BONE_COUNT";
        //private const string MorphCountDef = "MORPH_COUNT";
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
            //if (_info.IsWeighted)
            //    wl("#define {0} {1};", BoneCountDef, _info._boneCount);
            //if (allowMeshMorphing)
            //    wl("#define {0} {1};", MorphCountDef, _info._morphCount);
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
            int layoutLocation = 0;

            //Write mesh buffers first
            for (int i = 0; i < (morphed ? _info._morphCount + 1 : 1); ++i)
            {
                WriteInVar(layoutLocation++, GLTypeName._vec3, BufferType.Position.ToString() + i);
                if (_info.HasNormals)
                    WriteInVar(layoutLocation, GLTypeName._vec3, BufferType.Normal.ToString() + i);
                ++layoutLocation;
                if (_info.HasBinormals)
                    WriteInVar(layoutLocation, GLTypeName._vec3, BufferType.Binormal.ToString() + i);
                ++layoutLocation;
                if (_info.HasTangents)
                    WriteInVar(layoutLocation, GLTypeName._vec3, BufferType.Tangent.ToString() + i);
                ++layoutLocation;
            }

            //Then colors and texcoords
            layoutLocation = (int)BufferType.Color * VertexBuffer.MaxBufferCountPerType;
            for (int i = 0; i < _info._colorCount; ++i)
            {
                WriteInVar(layoutLocation, GLTypeName._vec4, BufferType.Color.ToString() + i);
            }
            layoutLocation = (int)BufferType.TexCoord * VertexBuffer.MaxBufferCountPerType;
            for (int i = 0; i < _info._texcoordCount; ++i)
            {
                WriteInVar(layoutLocation, GLTypeName._vec2, BufferType.TexCoord.ToString() + i);
            }
            //Barycentric coord, for wireframe rendering
            layoutLocation = (int)BufferType.Barycentric * VertexBuffer.MaxBufferCountPerType;
            if (_info._hasBarycentricCoord)
            {
                WriteInVar(layoutLocation, GLTypeName._vec3, BufferType.Barycentric.ToString());
            }
            ++layoutLocation;

            //And finally influence buffers
            if (_info.IsWeighted)
                for (int i = 0; i < (morphed ? _info._morphCount + 1 : 1); ++i)
                {
                    WriteInVar(layoutLocation++, GLTypeName._ivec4, BufferType.MatrixIds.ToString() + i);
                    WriteInVar(layoutLocation++, GLTypeName._vec4, BufferType.MatrixWeights.ToString() + i);
                }
        }
        private static void WriteMatrixUniforms(bool morphed)
        {
            WriteUniform(GLTypeName._mat4, "ModelMatrix");
            WriteUniform(GLTypeName._mat4, "NormalMatrix");
            WriteUniform(GLTypeName._mat4, "ViewMatrix");
            WriteUniform(GLTypeName._mat4, "ProjMatrix");
            if (_info.IsWeighted)
            {
                WriteUniform(GLTypeName._mat4, Uniform.BoneMatricesName + "[" + _info._boneCount + "]");
                WriteUniform(GLTypeName._mat4, Uniform.BoneMatricesITName + "[" + _info._boneCount + "]");
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
            wl("OutData.Position = vec3(0.0);");
            wl("vec4 basePosition = vec4(Position0, 1.0);");
            if (_info.HasNormals)
            {
                wl("OutData.Normal = vec3(0.0);");
                wl("vec4 baseNormal = vec4(Normal0, 1.0);");
            }
            if (_info.HasBinormals)
            {
                wl("OutData.Binormal = vec3(0.0);");
                wl("vec4 baseBinormal = vec4(Binormal0, 1.0);");
            }
            if (_info.HasTangents)
            {
                wl("OutData.Tangent = vec3(0.0);");
                wl("vec4 baseTangent = vec4(Tangent0, 1.0);");
            }
            wl();
            if (singleRig)
            {
                wl("for (int i = 0; i < 4; ++i)");
                wl("{");
                wl("OutData.Position += (({0}[{1}0[i]] * basePosition) * {2}0[i]).xyz;", Uniform.BoneMatricesName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasNormals)
                    wl("OutData.Normal += normalize(({0}[{1}0[i]] * baseNormal) * {2}0[i]).xyz;", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasBinormals)
                    wl("OutData.Binormal += normalize(({0}[{1}0[i]] * baseBinormal) * {2}0[i]).xyz;", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasTangents)
                    wl("OutData.Tangent += normalize(({0}[{1}0[i]] * baseTangent) * {2}0[i]).xyz;", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                wl("}");
                wl();
                if (_info.HasNormals)
                    wl("OutData.Normal = normalize(OutData.Normal);");
                if (_info.HasBinormals)
                    wl("OutData.Binormal = normalize(OutData.Normal);");
                if (_info.HasTangents)
                    wl("OutData.Tangent = normalize(OutData.Normal);");
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
                    wl("OutData.Position += {0}[{1}{3}[i]] * vec4(Position{5}, 1.0) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasNormals)
                        wl("OutData.Normal += {0}[{1}{3}[i]] * vec4(Normal{5}, 1.0) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasBinormals)
                        wl("OutData.Binormal += {0}[{1}{3}[i]] * vec4(Binormal{5}, 1.0) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasTangents)
                        wl("OutData.Tangent += {0}[{1}{3}[i]] * vec4(Tangent{5}, 1.0) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (i + 1 != _info._morphCount)
                        wl();
                }
                wl("}");
                wl("OutData.Position /= total;");
                if (_info.HasNormals)
                    wl("OutData.Normal /= total;");
                if (_info.HasBinormals)
                    wl("OutData.Binormal /= total;");
                if (_info.HasTangents)
                    wl("OutData.Tangent /= total;");
            }
        }
        private static void WriteStaticPNTB(bool morphed)
        {
            wl("vec4 position = ModelMatrix * vec4(Position0, 1.0);");
            if (_info.HasNormals)
                wl("vec4 normal = NormalMatrix * vec4(Normal0, 1.0);");
            if (_info.HasBinormals)
                wl("vec4 binormal = NormalMatrix * vec4(Binormal0, 1.0);");
            if (_info.HasTangents)
                wl("vec4 tangent = NormalMatrix * vec4(Tangent0, 1.0);");
            wl();
            if (morphed)
            {
                wl("float totalWeight = 0.0;");
                wl("for (int i = 0; i < {0}; ++i)", _info._morphCount);
                wl("totalWeight += {0}[i];", Uniform.MorphWeightsName);

                wl("float baseWeight = 1.0 - totalWeight;");
                wl("float total = totalWeight + baseWeight;");

                wl("position *= baseWeight;");
                if (_info.HasNormals)
                    wl("normal *= baseWeight;");
                if (_info.HasBinormals)
                    wl("binormal *= baseWeight;");
                if (_info.HasTangents)
                    wl("tangent *= baseWeight;");

                for (int i = 0; i < _info._morphCount; ++i)
                {
                    wl("position += ModelMatrix * vec4(Position{0}, 1.0) * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasNormals)
                        wl("normal += NormalMatrix * vec4(Normal{0}, 1.0) * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasBinormals)
                        wl("binormal += NormalMatrix * vec4(Binormal{0}, 1.0) * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasTangents)
                        wl("tangent += NormalMatrix * vec4(Tangent{0}, 1.0) * MorphWeights[{1}];", i + 1, i);
                }

                wl("position /= total;");
                wl("OutData.Position = position.xyz;");
                wl("gl_Position = ProjMatrix * ViewMatrix * position;");
                if (_info.HasNormals)
                    wl("OutData.Normal = normalize(normal.xyz / total);");
                if (_info.HasBinormals)
                    wl("OutData.Binormal = normalize(binormal.xyz / total);");
                if (_info.HasTangents)
                    wl("OutData.Tangent = normalize(tangent.xyz / total);");
            }
            else
            {
                wl("OutData.Position = position.xyz;");
                wl("gl_Position = ProjMatrix * ViewMatrix * position;");
                if (_info.HasNormals)
                    wl("OutData.Normal = normalize(normal.xyz);");
                if (_info.HasBinormals)
                    wl("OutData.Binormal = normalize(binormal.xyz);");
                if (_info.HasTangents)
                    wl("OutData.Tangent = normalize(tangent.xyz);");
            }
        }
    }
}
