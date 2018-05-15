using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class VertexShaderGenerator : MaterialGenerator
    {
        public const string FragPosName = "FragPos";
        public const string FragNormName = "FragNorm";
        public const string FragBinormName = "FragBinorm";
        public const string FragTanName = "FragTan";
        public const string FragColorName = "FragColor{0}";
        public const string FragUVName = "FragUV{0}";
        public const int FragPosBaseLoc = 0;
        public const int FragNormBaseLoc = 1;
        public const int FragTanBaseLoc = 2;
        public const int FragBinormBaseLoc = 3;
        public const int FragColorBaseLoc = 4;
        public const int FragUVBaseLoc = 6;

        private bool _morphsAllowed, _useMorphMultiRig;
        private VertexShaderDesc _info;
        private bool UseMorphs => _morphsAllowed && _info._morphCount > 0;
        private bool MultiRig => UseMorphs && _useMorphMultiRig;
        private TMaterial Material { get; set; }

        public GLSLShaderFile Generate(
            VertexShaderDesc info, 
            bool allowMeshMorphing, 
            bool useMorphMultiRig, 
            bool allowColorMorphing,
            TMaterial material)
        {
            _info = info;
            _morphsAllowed = allowMeshMorphing;
            _useMorphMultiRig = useMorphMultiRig;
            Material = material;

            //Write #definitions
            WriteVersion();
            Line();
            
            //Write header in fields (from buffers)
            WriteBuffers();
            Line();

            //Write header uniforms
            WriteMatrixUniforms();
            Line();

            //Write header out fields (to fragment shader)
            WriteOutData();
            Line();

            //For some reason, this is necessary
            if (Engine.Settings.AllowShaderPipelines)
            {
                Line("out gl_PerVertex");
                OpenBracket();
                Line("vec4 gl_Position;");
                Line("float gl_PointSize;");
                Line("float gl_ClipDistance[];");
                CloseBracket(true);
                Line();
            }
            
            StartMain();

            if (_info.IsWeighted && Engine.Settings.SkinOnGPU)
                WriteRiggedPNBT();
            else
                WriteStaticPNBT();

            for (int i = 0; i < _info._colorCount; ++i)
                Line("{0} = {2}{1};", string.Format(FragColorName, i), i, EBufferType.Color.ToString());
            for (int i = 0; i < _info._texcoordCount; ++i)
                Line("{0} = {2}{1};", string.Format(FragUVName, i), i, EBufferType.TexCoord.ToString());

            string source = EndMain();
            return new GLSLShaderFile(EShaderMode.Vertex, source);
        }
        private void WriteBuffers()
        {
            int meshCount = UseMorphs ? _info._morphCount + 1 : 1;
            bool weighted = Engine.Settings.SkinOnGPU && _info.IsWeighted;
            int location = 0;

            #region Positions
            EBufferType type = EBufferType.Position;
            for (int i = 0; i < meshCount; ++i)
                WriteInVar(location + i, EShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Normals
            type = EBufferType.Normal;
            if (_info.HasNormals)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, EShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Binormals
            type = EBufferType.Binormal;
            if (_info.HasBinormals)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, EShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Tangents
            type = EBufferType.Tangent;
            if (_info.HasTangents)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, EShaderVarType._vec3, VertexAttribInfo.GetAttribName(type, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region MatrixIds
            type = EBufferType.MatrixIds;
            if (weighted)
            {
                EShaderVarType varType = Engine.Settings.UseIntegerWeightingIds ? EShaderVarType._ivec4 : EShaderVarType._vec4;
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, varType, VertexAttribInfo.GetAttribName(type, i));
            }
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region MatrixWeights
            type = EBufferType.MatrixWeights;
            if (weighted)
                for (int i = 0; i < meshCount; ++i)
                    WriteInVar(location + i, EShaderVarType._vec4, VertexAttribInfo.GetAttribName(EBufferType.MatrixWeights, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region Colors
            type = EBufferType.Color;
            for (int i = 0; i < _info._colorCount; ++i)
                WriteInVar(location + i, EShaderVarType._vec4, VertexAttribInfo.GetAttribName(EBufferType.Color, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region TexCoords
            type = EBufferType.TexCoord;
            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteInVar(location + i, EShaderVarType._vec2, VertexAttribInfo.GetAttribName(EBufferType.TexCoord, i));
            //location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion
        }
        private void WriteMatrixUniforms()
        {
            WriteUniform(EShaderVarType._mat4, EEngineUniform.ModelMatrix.ToString());
            WriteUniform(EShaderVarType._mat3, EEngineUniform.NormalMatrix.ToString());
            WriteUniform(EShaderVarType._mat4, EEngineUniform.WorldToCameraSpaceMatrix.ToString());
            WriteUniform(EShaderVarType._mat4, EEngineUniform.ProjMatrix.ToString());
            if (_info.IsWeighted)
            {
                if (Engine.Settings.SkinOnGPU)
                {
                    WriteUniform(EShaderVarType._mat4, Uniform.BonePosMtxName + "[" + (_info._boneCount + 1) + "]");
                    WriteUniform(EShaderVarType._mat4, Uniform.BoneNrmMtxName + "[" + (_info._boneCount + 1) + "]");
                }
                if (UseMorphs)
                    WriteUniform(EShaderVarType._mat4, Uniform.MorphWeightsName + "[" + _info._morphCount + "]");
            }
        }

        /// <summary>
        /// This information is sent to the fragment shader.
        /// </summary>
        private void WriteOutData()
        {
            WriteOutVar(0, EShaderVarType._vec3, FragPosName);

            if (_info.HasNormals)
                WriteOutVar(1, EShaderVarType._vec3, FragNormName);

            if (_info.HasTangents)
                WriteOutVar(2, EShaderVarType._vec3, FragTanName);

            if (_info.HasBinormals)
                WriteOutVar(3, EShaderVarType._vec3, FragBinormName);

            for (int i = 0; i < _info._colorCount; ++i)
                WriteOutVar(4 + i, EShaderVarType._vec4, string.Format(FragColorName, i));

            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteOutVar(6 + i, EShaderVarType._vec2, string.Format(FragUVName, i));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowMorphs"></param>
        /// <param name="singleRig"></param>
        private void WriteRiggedPNBT()
        {
            Line("vec4 finalPosition = vec4(0.0f);");
            Line("vec4 basePosition = vec4(Position0, 1.0f);");
            if (_info.HasNormals || _info.HasTangents || _info.HasBinormals)
            {
                if (_info.HasNormals)
                {
                    Line("vec4 finalNormal = vec4(0.0f);");
                    Line("vec4 baseNormal = vec4(Normal0, 0.0f);");
                }
                if (_info.HasBinormals)
                {
                    Line("vec4 finalBinormal = vec4(0.0f);");
                    Line("vec4 baseBinormal = vec4(Binormal0, 0.0f);");
                }
                if (_info.HasTangents)
                {
                    Line("vec4 finalTangent = vec4(0.0f);");
                    Line("vec4 baseTangent = vec4(Tangent0, 0.0f);");
                }
            }
            Line();
            if (!MultiRig)
            {
                Line("int index;");
                Line("float weight;");

                Loop(4);
                OpenBracket();
                //for (int i = 0; i < 4; ++i)
                //{
                if (Engine.Settings.UseIntegerWeightingIds)
                        Line("index = {0}0[i];", EBufferType.MatrixIds.ToString());
                    else
                        Line("index = int({0}0[i]);", EBufferType.MatrixIds.ToString());
                    Line("weight = {0}0[i];", EBufferType.MatrixWeights.ToString());

                    Line($"finalPosition += ({Uniform.BonePosMtxName}[index] * basePosition) * weight;");
                    if (_info.HasNormals)
                        Line($"finalNormal += ({Uniform.BoneNrmMtxName}[index] * baseNormal) * weight;");
                    if (_info.HasBinormals)
                        Line($"finalBinormal += ({Uniform.BoneNrmMtxName}[index] * baseBinormal) * weight;");
                    if (_info.HasTangents)
                        Line($"finalTangent += ({Uniform.BoneNrmMtxName}[index] * baseTangent) * weight;");
                //}
                CloseBracket();

                Line();
                Line("finalPosition = ModelMatrix * vec4(finalPosition.xyz, 1.0f);");
                if (_info.HasNormals)
                    Line($"{FragNormName} = normalize(NormalMatrix * finalNormal.xyz);");
                if (_info.HasBinormals)
                    Line($"{FragBinormName} = normalize(NormalMatrix * finalBinormal.xyz);");
                if (_info.HasTangents)
                    Line($"{FragTanName} = normalize(NormalMatrix * finalTangent.xyz);");
            }
            else
            {
                Line("float totalWeight = 0.0f;");
                Line($"for (int i = 0; i < {_info._morphCount}; ++i)");
                Line("totalWeight += MorphWeights[i];");
                Line();
                Line("float baseWeight = 1.0f - totalWeight;");
                Line("float total = totalWeight + baseWeight;");
                Line();
                Line("basePosition *= baseWeight;");
                if (_info.HasNormals)
                    Line("baseNormal *= baseWeight;");
                if (_info.HasBinormals)
                    Line("baseBinormal *= baseWeight;");
                if (_info.HasTangents)
                    Line("baseTangent *= baseWeight;");
                Line();

                Loop(4);
                OpenBracket();
                for (int i = 0; i < _info._morphCount; ++i)
                {
                    Line("finalPosition += {0}[{1}{3}[i]] * vec4(Position{5}, 1.0f) * {2}{3}[i] * {4}[i];", Uniform.BonePosMtxName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasNormals)
                        Line("finalNormal += ({0}[{1}{3}[i]] * Normal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneNrmMtxName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasBinormals)
                        Line("finalBinorm += ({0}[{1}{3}[i]] * Binormal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneNrmMtxName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasTangents)
                        Line("finalTangent += ({0}[{1}{3}[i]] * Tangent{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneNrmMtxName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (i + 1 != _info._morphCount)
                        Line();
                }
                CloseBracket();

                Line("finalPosition = ModelMatrix * (finalPosition / vec4(total, total, total, 1.0f));");
                if (_info.HasNormals)
                    Line($"{FragNormName} = normalize(NormalMatrix * (finalNormal / total));");
                if (_info.HasBinormals)
                    Line($"{FragBinormName} = normalize(NormalMatrix * (finalBinormal / total));");
                if (_info.HasTangents)
                    Line($"{FragTanName} = normalize(NormalMatrix * (finalTangent / total));");
            }
            Line($"{FragPosName} = finalPosition.xyz;");
            Line("gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * finalPosition;");
        }

        /// <summary>
        /// Writes positions, and optionally normals, tangents, and binormals for a static mesh.
        /// </summary>
        /// <param name="allowMorphs">If the mesh should be morphable or not, regardless of whether or not the mesh actually has morphs.</param>
        private void WriteStaticPNBT()
        {
            Line("vec4 position = vec4(Position0, 1.0f);");
            if (_info.HasNormals)
                Line("vec3 normal = Normal0;");
            if (_info.HasBinormals)
                Line("vec3 binormal = Binormal0;");
            if (_info.HasTangents)
                Line("vec3 tangent = Tangent0;");
            Line();

            if (UseMorphs)
            {
                Line("float totalWeight = 0.0f;");
                Loop(_info._morphCount);
                Line($"totalWeight += {Uniform.MorphWeightsName}[i];");

                Line("float baseWeight = 1.0f - totalWeight;");
                Line("float invTotal = 1.0f / (totalWeight + baseWeight);");
                Line();

                Line("position *= baseWeight;");
                if (_info.HasNormals)
                    Line("normal *= baseWeight;");
                if (_info.HasBinormals)
                    Line("binormal *= baseWeight;");
                if (_info.HasTangents)
                    Line("tangent *= baseWeight;");
                Line();

                for (int i = 0; i < _info._morphCount; ++i)
                {
                    Line($"position += vec4(Position{i + 1}, 1.0f) * MorphWeights[{i}];");
                    if (_info.HasNormals)
                        Line($"normal += Normal{i + 1} * MorphWeights[{i}];");
                    if (_info.HasBinormals)
                        Line($"binormal += Binormal{i + 1} * MorphWeights[{i}];");
                    if (_info.HasTangents)
                        Line($"tangent += Tangent{i + 1} * MorphWeights[{i}];");
                }
                Line();
                Line("position *= invTotal;");
                if (_info.HasNormals)
                    Line("normal *= invTotal;");
                if (_info.HasBinormals)
                    Line("binormal *= invTotal;");
                if (_info.HasTangents)
                    Line("tangent *= invTotal;");
                Line();
            }

            Line("position = ModelMatrix * position;");
            Line($"{FragPosName} = position.xyz;");
            Line("gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * position;");
            if (_info.HasNormals)
                Line($"{FragNormName} = normalize(NormalMatrix * normal);");
            if (_info.HasBinormals)
                Line($"{FragBinormName} = normalize(NormalMatrix * binormal);");
            if (_info.HasTangents)
                Line($"{FragTanName} = normalize(NormalMatrix * tangent);");
        }
    }
}
