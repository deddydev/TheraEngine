using System;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public static class VertexShaderGenerator
    {
        public const string FragPosName = "FragPos";
        public const string FragNormName = "FragNorm";
        public const string FragBinormName = "FragBinorm";
        public const string FragTanName = "FragTan";
        public const string FragColorName = "FragColor{0}";
        public const string FragUVName = "FragUV{0}";

        private static ShaderGenerator _generator = new ShaderGenerator();
        private static VertexShaderDesc _info;

        private static void WriteLine(string str = "", params object[] args)
            => _generator.wl(str, args);
        private static void WriteUniform(int layoutLocation, ShaderVarType type, string name) 
            => _generator.WriteUniform(layoutLocation, type, name);
        private static void WriteUniform(ShaderVarType type, string name)
            => _generator.WriteUniform(type, name);
        private static void WriteInVar(int layoutLocation, ShaderVarType type, string name)
            => _generator.WriteInVar(layoutLocation, type, name);
        private static void WriteInVar(ShaderVarType type, string name)
            => _generator.WriteInVar(type, name);

        public static Shader Generate(ResultBasicFunc end)
        {
            throw new NotImplementedException();
        }
        public static Shader Generate(VertexShaderDesc info, bool allowMeshMorphing, bool singleMorphRig, bool allowColorMorphing)
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

            if (Engine.Settings.AllowShaderPipelines)
            {
                WriteLine("out gl_PerVertex");
                WriteLine("{");
                WriteLine("vec4 gl_Position;");
                WriteLine("float gl_PointSize;");
                WriteLine("float gl_ClipDistance[];");
                WriteLine("};");
                WriteLine();
            }

            //Write the beginning of the main function
            _generator.Begin();

            if (_info.IsWeighted && Engine.Settings.SkinOnGPU)
                WriteRiggedPNTB(allowMeshMorphing, allowMeshMorphing ? singleMorphRig : true);
            else
                WriteStaticPNTB(allowMeshMorphing);

            for (int i = 0; i < _info._colorCount; ++i)
                WriteLine("{0} = {2}{1};", string.Format(FragColorName, i), i, BufferType.Color.ToString());
            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteLine("{0} = {2}{1};", string.Format(FragUVName, i), i, BufferType.TexCoord.ToString());

            string source = _generator.Finish();
            return new Shader(ShaderMode.Vertex, source);
        }
        private static void WriteBuffers(bool allowMorphs)
        {
            int meshCount = allowMorphs ? _info._morphCount + 1 : 1;
            bool weighted = Engine.Settings.SkinOnGPU && _info.IsWeighted;
            int location = 0;

            #region Positions
            BufferType type = BufferType.Position;
            for (int i = 0; i < meshCount; ++i)
                WriteInVar(location + i, ShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Normals
            type = BufferType.Normal;
            if (_info.HasNormals)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, ShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Binormals
            type = BufferType.Binormal;
            if (_info.HasBinormals)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, ShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Tangents
            type = BufferType.Tangent;
            if (_info.HasTangents)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, ShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region MatrixIds
            type = BufferType.MatrixIds;
            if (weighted)
            {
                ShaderVarType varType = Engine.Settings.UseIntegerWeightingIds ? ShaderVarType._ivec4 : ShaderVarType._vec4;
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, varType, VertexAttribInfo.GetAttribName(type, i));
            }
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region MatrixWeights
            type = BufferType.MatrixWeights;
            if (weighted)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, ShaderVarType._vec4, VertexAttribInfo.GetAttribName(BufferType.MatrixWeights, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Colors
            type = BufferType.Color;
            for (int i = 0; i < _info._colorCount; ++i)
                WriteInVar(location + i, ShaderVarType._vec4, VertexAttribInfo.GetAttribName(BufferType.Color, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region TexCoords
            type = BufferType.TexCoord;
            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteInVar(location + i, ShaderVarType._vec2, VertexAttribInfo.GetAttribName(BufferType.TexCoord, i));
            //location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion
        }
        private static void WriteMatrixUniforms(bool morphed)
        {
            WriteUniform(ShaderVarType._mat4, ECommonUniform.ModelMatrix.ToString());
            WriteUniform(ShaderVarType._mat3, ECommonUniform.NormalMatrix.ToString());
            WriteUniform(ShaderVarType._mat4, ECommonUniform.WorldToCameraSpaceMatrix.ToString());
            WriteUniform(ShaderVarType._mat4, ECommonUniform.ProjMatrix.ToString());
            if (_info.IsWeighted)
            {
                if (Engine.Settings.SkinOnGPU)
                {
                    WriteUniform(ShaderVarType._mat4, Uniform.BonePosMtxName + "[" + (_info._boneCount + 1) + "]");
                    WriteUniform(ShaderVarType._mat4, Uniform.BoneNrmMtxName + "[" + (_info._boneCount + 1) + "]");
                }
                if (morphed)
                    WriteUniform(ShaderVarType._mat4, Uniform.MorphWeightsName + "[" + _info._morphCount + "]");
            }
        }
        /// <summary>
        /// This information is sent to the fragment shader.
        /// </summary>
        private static void WriteOutData()
        {
            WriteLine("out vec3 {0};", FragPosName);
            if (_info.HasNormals)
                WriteLine("out vec3 {0};", FragNormName);
            if (_info.HasTangents)
                WriteLine("out vec3 {0};", FragTanName);
            if (_info.HasBinormals)
                WriteLine("out vec3 {0};", FragBinormName);
            for (int i = 0; i < _info._colorCount; ++i)
                WriteLine("out vec4 {0};", string.Format(FragColorName, i));
            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteLine("out vec2 {0};", string.Format(FragUVName, i));
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
                    WriteLine("vec4 baseNormal = vec4(Normal0, 0.0);");
                }
                if (_info.HasBinormals)
                {
                    WriteLine("vec4 finalBinormal = vec4(0.0);");
                    WriteLine("vec4 baseBinormal = vec4(Binormal0, 0.0);");
                }
                if (_info.HasTangents)
                {
                    WriteLine("vec4 finalTangent = vec4(0.0);");
                    WriteLine("vec4 baseTangent = vec4(Tangent0, 0.0);");
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

                    WriteLine("finalPosition += ({0}[index] * basePosition) * weight;", Uniform.BonePosMtxName.ToString());
                    if (_info.HasNormals)
                        WriteLine("finalNormal += ({0}[index] * baseNormal) * weight;", Uniform.BoneNrmMtxName.ToString());
                    if (_info.HasBinormals)
                        WriteLine("finalBinormal += ({0}[index] * baseBinormal) * weight;", Uniform.BoneNrmMtxName.ToString());
                    if (_info.HasTangents)
                        WriteLine("finalTangent += ({0}[index] * baseTangent) * weight;", Uniform.BoneNrmMtxName.ToString());
                }
                //wl("}");
                WriteLine();
                WriteLine("finalPosition = ModelMatrix * vec4(finalPosition.xyz, 1.0);");
                if (_info.HasNormals)
                    WriteLine("{0} = normalize(NormalMatrix * finalNormal.xyz);", FragNormName);
                if (_info.HasBinormals)
                    WriteLine("{0} = normalize(NormalMatrix * finalBinormal.xyz);", FragBinormName);
                if (_info.HasTangents)
                    WriteLine("{0} = normalize(NormalMatrix * finalTangent.xyz);", FragTanName);
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
                    WriteLine("finalPosition += {0}[{1}{3}[i]] * vec4(Position{5}, 1.0) * {2}{3}[i] * {4}[i];", Uniform.BonePosMtxName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasNormals)
                        WriteLine("finalNormal += ({0}[{1}{3}[i]] * Normal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneNrmMtxName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasBinormals)
                        WriteLine("finalBinorm += ({0}[{1}{3}[i]] * Binormal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneNrmMtxName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasTangents)
                        WriteLine("finalTangent += ({0}[{1}{3}[i]] * Tangent{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneNrmMtxName, BufferType.MatrixIds, BufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (i + 1 != _info._morphCount)
                        WriteLine();
                }
                WriteLine("}");
                WriteLine("finalPosition = ModelMatrix * (finalPosition / vec4(total, total, total, 1.0));");
                if (_info.HasNormals)
                    WriteLine("{0} = normalize(NormalMatrix * (finalNormal / total));", FragNormName);
                if (_info.HasBinormals)
                    WriteLine("{0} = normalize(NormalMatrix * (finalBinormal / total));", FragBinormName);
                if (_info.HasTangents)
                    WriteLine("{0} = normalize(NormalMatrix * (finalTangent / total));", FragTanName);
            }
            WriteLine("{0} = finalPosition.xyz;", FragPosName);
            WriteLine("gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * finalPosition;");
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
            WriteLine("{0} = position.xyz;", FragPosName);
            WriteLine("gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * position;");
            if (_info.HasNormals)
                WriteLine("{0} = normalize(NormalMatrix * normal);", FragNormName);
            if (_info.HasBinormals)
                WriteLine("{0} = normalize(NormalMatrix * binormal);", FragBinormName);
            if (_info.HasTangents)
                WriteLine("{0} = normalize(NormalMatrix * tangent);", FragTanName);
        }
    }
}
