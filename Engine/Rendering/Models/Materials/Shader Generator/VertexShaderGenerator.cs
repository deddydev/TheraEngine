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

        private static void WriteLine(string str = "", params object[] args)
            => _generator.wl(str, args);
        private static void WriteUniform(int layoutLocation, GLTypeName type, string name) 
            => _generator.WriteUniform(layoutLocation, type, name);
        private static void WriteUniform(GLTypeName type, string name)
            => _generator.WriteUniform(type, name);
        private static void WriteInVar(int layoutLocation, GLTypeName type, string name)
            => _generator.WriteInVar(layoutLocation, type, name);
        private static void WriteInVar(GLTypeName type, string name)
            => _generator.WriteInVar(type, name);

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
            WriteLine();

            //Write header in fields (from buffers)
            WriteBuffers(allowMeshMorphing);
            WriteLine();

            //Write header uniforms
            WriteMatrixUniforms(allowMeshMorphing);
            WriteLine();

            //Write header out fields (to fragment shader)
            WriteOutData();
            WriteLine();

            //Write the beginning of the main function
            _generator.Begin();

            if (_info.IsWeighted && Engine.Settings.SkinOnGPU)
                WriteRiggedPNTB(allowMeshMorphing, allowMeshMorphing ? singleMorphRig : true);
            else
                WriteStaticPNTB(allowMeshMorphing);

            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteLine("OutData.MultiTexCoord{0} = TexCoord{0};", i);

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
                WriteInVar(GLTypeName._vec4, BufferType.Color.ToString() + i);

            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteInVar(GLTypeName._vec2, BufferType.TexCoord.ToString() + i);

            //Barycentric coord, for wireframe rendering
            if (_info._hasBarycentricCoord)
                WriteInVar(GLTypeName._vec3, BufferType.Barycentric.ToString());

            //And finally influence buffers
            if (Engine.Settings.SkinOnGPU && _info.IsWeighted)
                for (int i = 0; i < (morphed ? _info._morphCount + 1 : 1); ++i)
                {
                    WriteInVar(Engine.Settings.UseIntegerWeightingIds ? GLTypeName._ivec4 : GLTypeName._vec4, BufferType.MatrixIds.ToString() + i);
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
                if (Engine.Settings.SkinOnGPU)
                {
                    WriteUniform(GLTypeName._mat4, Uniform.BoneMatricesName + "[" + (_info._boneCount + 1) + "]");
                    WriteUniform(GLTypeName._mat4, Uniform.BoneMatricesITName + "[" + (_info._boneCount + 1) + "]");
                }
                if (morphed)
                    WriteUniform(GLTypeName._mat4, Uniform.MorphWeightsName + "[" + _info._morphCount + "]");
            }
        }
        /// <summary>
        /// This information is sent to the fragment shader.
        /// </summary>
        private static void WriteOutData()
        {
            WriteLine("out Data {");
            WriteLine("vec3 Position;");
            if (_info.HasNormals)
                WriteLine("vec3 Normal;");
            if (_info.HasTangents)
                WriteLine("vec3 Tangent;");
            if (_info.HasBinormals)
                WriteLine("vec3 Binormal;");
            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteLine("vec2 MultiTexCoord{0};", i);
            WriteLine("} OutData;");
        }
        private static void WriteRiggedPNTB(bool morphed, bool singleRig)
        {
            WriteLine("vec4 finalPosition = vec4(0.0);");
            WriteLine("vec4 basePosition = vec4(Position0, 1.0);");
            if (_info.HasNormals || _info.HasTangents || _info.HasBinormals)
            {
                if (_info.HasNormals)
                {
                    WriteLine("vec4 finalNormal = vec4(0.0);");
                    WriteLine("vec4 baseNormal = vec4(Normal0, 1.0);");
                }
                if (_info.HasBinormals)
                {
                    WriteLine("OutData.Binormal = vec3(0.0);");
                    WriteLine("vec3 baseBinormal = Binormal0;");
                }
                if (_info.HasTangents)
                {
                    WriteLine("OutData.Tangent = vec3(0.0);");
                    WriteLine("vec3 baseTangent = Tangent0;");
                }
            }
            WriteLine();
            if (singleRig)
            {
                WriteLine("int index;");
                WriteLine("float weight;");
                //wl("for (int i = 0; i < 4; ++i)");
                //wl("{");
                for (int i = 0; i < 4; ++i)
                {
                    string part = i == 0 ? "x" : i == 1 ? "y" : i == 2 ? "z" : "w";

                    if (Engine.Settings.UseIntegerWeightingIds)
                        WriteLine("index = {0}0.{1};", BufferType.MatrixIds.ToString(), part);
                    else
                        WriteLine("index = int({0}0.{1});", BufferType.MatrixIds.ToString(), part);

                    WriteLine("weight = {0}0.{1};", BufferType.MatrixWeights.ToString(), part);
                    WriteLine("finalPosition += ({0}[index] * basePosition) * weight;", Uniform.BoneMatricesName);
                    if (_info.HasNormals)
                        WriteLine("finalNormal += ({0}[index] * baseNormal) * weight;", Uniform.BoneMatricesITName);
                    if (_info.HasBinormals)
                        WriteLine("OutData.Binormal += ({0}[index] * baseBinormal) * weight;", Uniform.BoneMatricesITName);
                    if (_info.HasTangents)
                        WriteLine("OutData.Tangent += ({0}[index] * baseTangent) * weight;", Uniform.BoneMatricesITName);
                }
                //wl("}");
                WriteLine();
                if (_info.HasNormals)
                    WriteLine("OutData.Normal = normalize(finalNormal.xyz);");
                if (_info.HasBinormals)
                    WriteLine("OutData.Binormal = normalize(OutData.Binormal);");
                if (_info.HasTangents)
                    WriteLine("OutData.Tangent = normalize(OutData.Tangent);");
            }
            else
            {
                WriteLine("float totalWeight = 0.0;");
                WriteLine("for (int i = 0; i < {0}; ++i)", _info._morphCount);
                WriteLine("totalWeight += MorphWeights[i];");
                WriteLine();
                WriteLine("float baseWeight = 1.0 - totalWeight;");
                WriteLine("float total = totalWeight + baseWeight;");
                WriteLine();
                WriteLine("basePosition *= baseWeight;");
                if (_info.HasNormals)
                    WriteLine("baseNormal *= baseWeight;");
                if (_info.HasBinormals)
                    WriteLine("baseBinormal *= baseWeight;");
                if (_info.HasTangents)
                    WriteLine("baseTangent *= baseWeight;");
                WriteLine();
                WriteLine("for (int i = 0; i < 4; ++i)");
                WriteLine("{");
                for (int i = 0; i < _info._morphCount; ++i)
                {
                    WriteLine("finalPosition += {0}[{1}{3}[i]] * vec4(Position{5}, 1.0) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasNormals)
                        WriteLine("OutData.Normal += ({0}[{1}{3}[i]] * Normal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasBinormals)
                        WriteLine("OutData.Binormal += ({0}[{1}{3}[i]] * Binormal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasTangents)
                        WriteLine("OutData.Tangent += ({0}[{1}{3}[i]] * Tangent{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (i + 1 != _info._morphCount)
                        WriteLine();
                }
                WriteLine("}");
                WriteLine("finalPosition /= total;");
                if (_info.HasNormals)
                    WriteLine("OutData.Normal = normalize(OutData.Normal / total);");
                if (_info.HasBinormals)
                    WriteLine("OutData.Binormal normalize(OutData.Binormal / total);");
                if (_info.HasTangents)
                    WriteLine("OutData.Tangent normalize(OutData.Tangent / total);");
            }
            WriteLine("OutData.Position = finalPosition.xyz;");
            WriteLine("gl_Position = ProjMatrix * ViewMatrix * finalPosition;");
        }
        private static void WriteStaticPNTB(bool morphed)
        {
            WriteLine("vec4 position = vec4(Position0, 1.0);");
            if (_info.HasNormals)
                WriteLine("vec3 normal = Normal0;");
            if (_info.HasBinormals)
                WriteLine("vec3 binormal = Binormal0;");
            if (_info.HasTangents)
                WriteLine("vec3 tangent = Tangent0;");
            WriteLine();
            if (morphed)
            {
                WriteLine("float totalWeight = 0.0;");
                WriteLine("for (int i = 0; i < {0}; ++i)", _info._morphCount);
                WriteLine("totalWeight += {0}[i];", Uniform.MorphWeightsName);
                WriteLine("float baseWeight = 1.0 - totalWeight;");
                WriteLine("float invTotal = 1.0 / (totalWeight + baseWeight);");
                WriteLine();
                WriteLine("position *= baseWeight;");
                if (_info.HasNormals)
                    WriteLine("normal *= baseWeight;");
                if (_info.HasBinormals)
                    WriteLine("binormal *= baseWeight;");
                if (_info.HasTangents)
                    WriteLine("tangent *= baseWeight;");
                WriteLine();
                for (int i = 0; i < _info._morphCount; ++i)
                {
                    WriteLine("position += vec4(Position{0}, 1.0) * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasNormals)
                        WriteLine("normal += Normal{0} * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasBinormals)
                        WriteLine("binormal += Binormal{0} * MorphWeights[{1}];", i + 1, i);
                    if (_info.HasTangents)
                        WriteLine("tangent += Tangent{0} * MorphWeights[{1}];", i + 1, i);
                }
                WriteLine();
                WriteLine("position *= invTotal;");
                if (_info.HasNormals)
                    WriteLine("normal *= invTotal;");
                if (_info.HasBinormals)
                    WriteLine("binormal *= invTotal;");
                if (_info.HasTangents)
                    WriteLine("tangent *= invTotal;");
                WriteLine();
            }
            WriteLine("position = ModelMatrix * position;");
            WriteLine("OutData.Position = position.xyz;");
            WriteLine("gl_Position = ProjMatrix * ViewMatrix * position;");
            if (_info.HasNormals)
                WriteLine("OutData.Normal = normalize(NormalMatrix * normal);");
            if (_info.HasBinormals)
                WriteLine("OutData.Binormal = normalize(NormalMatrix * binormal);");
            if (_info.HasTangents)
                WriteLine("OutData.Tangent = normalize(NormalMatrix * tangent);");
        }
    }
}
