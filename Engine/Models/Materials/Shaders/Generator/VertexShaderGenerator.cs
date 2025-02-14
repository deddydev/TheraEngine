﻿using System;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Generates a typical vertex shader for use with most models.
    /// </summary>
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
        private bool UseMorphs => _morphsAllowed && _info.MorphCount > 0;
        private bool MultiRig => UseMorphs && _useMorphMultiRig;
        private TMaterial Material { get; set; }

        public GLSLScript Generate(
            VertexShaderDesc info,
            TMaterial material,
            bool allowMeshMorphing = true, 
            bool useMorphMultiRig = false, 
            bool allowColorMorphing = true)
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
                CloseBracket(null, true);
                Line();
            }
            
            StartMain();

            if (_info.IsWeighted && Engine.Settings.SkinOnGPU)
                WriteRiggedPNBT();
            else
                WriteStaticPNBT();

            for (int i = 0; i < _info.ColorCount; ++i)
                Line("{0} = {2}{1};", string.Format(FragColorName, i), i, EBufferType.Color.ToString());
            for (int i = 0; i < _info.TexcoordCount; ++i)
                Line("{0} = {2}{1};", string.Format(FragUVName, i), i, EBufferType.TexCoord.ToString());

            string source = EndMain();
            //Engine.PrintLine(source);
            return new GLSLScript(EGLSLType.Vertex, source);
        }
        private void WriteBuffers()
        {
            int meshCount = UseMorphs ? _info.MorphCount + 1 : 1;
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
            for (int i = 0; i < _info.ColorCount; ++i)
                WriteInVar(location + i, EShaderVarType._vec4, VertexAttribInfo.GetAttribName(EBufferType.Color, i));
            location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion

            #region TexCoords
            type = EBufferType.TexCoord;
            for (int i = 0; i < _info.TexcoordCount; ++i)
                WriteInVar(location + i, EShaderVarType._vec2, VertexAttribInfo.GetAttribName(EBufferType.TexCoord, i));
            //location += VertexAttribInfo.GetMaxBuffersForType(type);
            #endregion
        }
        private void WriteMatrixUniforms()
        {
            WriteUniform(EShaderVarType._mat4, EEngineUniform.ModelMatrix.ToString());
            WriteUniform(EShaderVarType._mat3, EEngineUniform.NormalMatrix.ToString());
            WriteUniform(EShaderVarType._mat4, EEngineUniform.WorldToCameraSpaceMatrix.ToString());
            if (_info.CameraTransformFlags != ECameraTransformFlags.None)
                WriteUniform(EShaderVarType._mat4, EEngineUniform.CameraToWorldSpaceMatrix.ToString());
            WriteUniform(EShaderVarType._mat4, EEngineUniform.ProjMatrix.ToString());
            if (_info.IsWeighted)
            {
                if (Engine.Settings.SkinOnGPU)
                {
                    StartUniformBlock("Bones");
                    WriteUniform(EShaderVarType._mat4, Uniform.BoneTransformsName + "[" + (_info.BoneCount + 1) + "]");
                    EndUniformBlock("BoneDef");
                }
                if (UseMorphs)
                    WriteUniform(EShaderVarType._mat4, Uniform.MorphWeightsName + "[" + _info.MorphCount + "]");
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

            if (_info.HasBinormals)
                WriteOutVar(2, EShaderVarType._vec3, FragBinormName);

            if (_info.HasTangents)
                WriteOutVar(3, EShaderVarType._vec3, FragTanName);

            for (int i = 0; i < _info.ColorCount; ++i)
                WriteOutVar(4 + i, EShaderVarType._vec4, string.Format(FragColorName, i));

            for (int i = 0; i < _info.TexcoordCount; ++i)
                WriteOutVar(6 + i, EShaderVarType._vec2, string.Format(FragUVName, i));
        }

        /// <summary>
        /// Calculates positions, and optionally normals, tangents, and binormals for a rigged mesh.
        /// </summary>
        private void WriteRiggedPNBT()
        {
            bool hasNBT = _info.HasNormals || _info.HasTangents || _info.HasBinormals;

            Line("vec4 finalPosition = vec4(0.0f);");
            Line("vec4 basePosition = vec4(Position0, 1.0f);");

            if (_info.HasNormals)
            {
                Line("vec3 finalNormal = vec3(0.0f);");
                Line("vec3 baseNormal = Normal0;");
            }
            if (_info.HasBinormals)
            {
                Line("vec3 finalBinormal = vec3(0.0f);");
                Line("vec3 baseBinormal = Binormal0;");
            }
            if (_info.HasTangents)
            {
                Line("vec3 finalTangent = vec3(0.0f);");
                Line("vec3 baseTangent = Tangent0;");
            }
            
            Line();
            if (!MultiRig)
            {
                Line("int index;");
                Line("float weight;");

                OpenLoop(4);
                {
                    if (Engine.Settings.UseIntegerWeightingIds)
                        Line("index = {0}0[i];", EBufferType.MatrixIds.ToString());
                    else
                        Line("index = int({0}0[i]);", EBufferType.MatrixIds.ToString());

                    Line("weight = {0}0[i];", EBufferType.MatrixWeights.ToString());

                    Line($"finalPosition += (BoneDef.{Uniform.BoneTransformsName}[index] * basePosition) * weight;");
                    if (hasNBT)
                    {
                        Line($"mat3 nrmMtx = mat3(transpose(inverse(BoneDef.{Uniform.BoneTransformsName}[index])));");
                        if (_info.HasNormals)
                            Line($"finalNormal += (nrmMtx * baseNormal) * weight;");
                        if (_info.HasBinormals)
                            Line($"finalBinormal += (nrmMtx * baseBinormal) * weight;");
                        if (_info.HasTangents)
                            Line($"finalTangent += (nrmMtx * baseTangent) * weight;");
                    }
                }
                CloseBracket();

                Line();
                if (_info.HasNormals)
                    Line($"{FragNormName} = normalize(NormalMatrix * finalNormal);");
                if (_info.HasBinormals)
                    Line($"{FragBinormName} = normalize(NormalMatrix * finalBinormal);");
                if (_info.HasTangents)
                    Line($"{FragTanName} = normalize(NormalMatrix * finalTangent);");
            }
            else
            {
                Line("float totalWeight = 0.0f;");
                Line($"for (int i = 0; i < {_info.MorphCount}; ++i)");
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

                OpenLoop(4);
                for (int i = 0; i < _info.MorphCount; ++i)
                {
                    Line("finalPosition += BoneDef.{0}[{1}{3}[i]] * vec4(Position{5}, 1.0f) * {2}{3}[i] * {4}[i];", Uniform.BoneTransformsName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasNormals)
                        Line("finalNormal += (mat3(BoneDef.{0}[{1}{3}[i]]) * Normal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneTransformsName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasBinormals)
                        Line("finalBinorm += (mat3(BoneDef.{0}[{1}{3}[i]]) * Binormal{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneTransformsName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (_info.HasTangents)
                        Line("finalTangent += (mat3(BoneDef.{0}[{1}{3}[i]]) * Tangent{5}) * {2}{3}[i] * {4}[i];", Uniform.BoneTransformsName, EBufferType.MatrixIds, EBufferType.MatrixWeights, i, Uniform.MorphWeightsName, i + 1);
                    if (i + 1 != _info.MorphCount)
                        Line();
                }
                CloseBracket();

                if (_info.HasNormals)
                    Line($"{FragNormName} = normalize(NormalMatrix * (finalNormal / total));");
                if (_info.HasBinormals)
                    Line($"{FragBinormName} = normalize(NormalMatrix * (finalBinormal / total));");
                if (_info.HasTangents)
                    Line($"{FragTanName} = normalize(NormalMatrix * (finalTangent / total));");
                Line("finalPosition /= vec4(vec3(total), 1.0f);");
            }
            ResolvePosition("finalPosition");
        }
        /// <summary>
        /// Calculates positions, and optionally normals, tangents, and binormals for a static mesh.
        /// </summary>
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
                OpenLoop(_info.MorphCount);
                Line($"totalWeight += {Uniform.MorphWeightsName}[i];");
                CloseBracket();

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

                for (int i = 0; i < _info.MorphCount; ++i)
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

            ResolvePosition("position");
            if (_info.HasNormals)
                Line($"{FragNormName} = normalize(NormalMatrix * normal);");
            if (_info.HasBinormals)
                Line($"{FragBinormName} = normalize(NormalMatrix * binormal);");
            if (_info.HasTangents)
                Line($"{FragTanName} = normalize(NormalMatrix * tangent);");
        }
        private void ResolvePosition(string posName)
        {
            Line("mat4 ViewMatrix = WorldToCameraSpaceMatrix;");
            if (_info.CameraTransformFlags == ECameraTransformFlags.None)
            {
                Line($"{posName} = ModelMatrix * vec4({posName}.xyz, 1.0f);");
                Line($"{FragPosName} = {posName}.xyz;");
                Line($"gl_Position = ProjMatrix * ViewMatrix * {posName};");
                return;
            }
            Line("mat4 BillboardMatrix = CameraToWorldSpaceMatrix;");
            if (_info.CameraTransformFlags.HasFlag(ECameraTransformFlags.RotateX))
            {
                //Do not align X column to be stationary from camera's viewpoint
                Line("ViewMatrix[0][0] = 1.0f;");
                Line("ViewMatrix[0][1] = 0.0f;");
                Line("ViewMatrix[0][2] = 0.0f;");

                //Do not fix Y column to rotate with camera
                Line("BillboardMatrix[1][0] = 0.0f;");
                Line("BillboardMatrix[1][1] = 1.0f;");
                Line("BillboardMatrix[1][2] = 0.0f;");

                //Do not fix Z column to rotate with camera
                Line("BillboardMatrix[2][0] = 0.0f;");
                Line("BillboardMatrix[2][1] = 0.0f;");
                Line("BillboardMatrix[2][2] = 1.0f;");
            }
            if (_info.CameraTransformFlags.HasFlag(ECameraTransformFlags.RotateY))
            {
                //Do not fix X column to rotate with camera
                Line("BillboardMatrix[0][0] = 1.0f;");
                Line("BillboardMatrix[0][1] = 0.0f;");
                Line("BillboardMatrix[0][2] = 0.0f;");

                //Do not align Y column to be stationary from camera's viewpoint
                Line("ViewMatrix[1][0] = 0.0f;");
                Line("ViewMatrix[1][1] = 1.0f;");
                Line("ViewMatrix[1][2] = 0.0f;");

                //Do not fix Z column to rotate with camera
                Line("BillboardMatrix[2][0] = 0.0f;");
                Line("BillboardMatrix[2][1] = 0.0f;");
                Line("BillboardMatrix[2][2] = 1.0f;");
            }
            if (_info.CameraTransformFlags.HasFlag(ECameraTransformFlags.RotateZ))
            {
                //Do not fix X column to rotate with camera
                Line("BillboardMatrix[0][0] = 1.0f;");
                Line("BillboardMatrix[0][1] = 0.0f;");
                Line("BillboardMatrix[0][2] = 0.0f;");

                //Do not fix Y column to rotate with camera
                Line("BillboardMatrix[1][0] = 0.0f;");
                Line("BillboardMatrix[1][1] = 1.0f;");
                Line("BillboardMatrix[1][2] = 0.0f;");

                //Do not align Z column to be stationary from camera's viewpoint
                Line("ViewMatrix[2][0] = 0.0f;");
                Line("ViewMatrix[2][1] = 0.0f;");
                Line("ViewMatrix[2][2] = 1.0f;");
            }
            if (_info.CameraTransformFlags.HasFlag(ECameraTransformFlags.ConstrainTranslationX))
            {
                //Clear X translation
                Line("ViewMatrix[3][0] = 0.0f;");
                Line("BillboardMatrix[3][0] = 0.0f;");
            }
            if (_info.CameraTransformFlags.HasFlag(ECameraTransformFlags.ConstrainTranslationY))
            {
                //Clear Y translation
                Line("ViewMatrix[3][1] = 0.0f;");
                Line("BillboardMatrix[3][1] = 0.0f;");
            }
            if (_info.CameraTransformFlags.HasFlag(ECameraTransformFlags.ConstrainTranslationZ))
            {
                //Clear Z translation
                Line("ViewMatrix[3][2] = 0.0f;");
                Line("BillboardMatrix[3][2] = 0.0f;");
            }

            Line($"{posName} = ModelMatrix * vec4({posName}.xyz, 1.0f);");
            Line($"{FragPosName} = (BillboardMatrix * {posName}).xyz;");
            Line($"gl_Position = ProjMatrix * ViewMatrix * {posName};");
            return;
        }
    }
    /// <summary>
    /// Determines how vertices should rotate and scale in relation to the camera.
    /// </summary>
    [Flags]
    public enum ECameraTransformFlags
    {
        /// <summary>
        /// No billboarding, all vertices are static.
        /// </summary>
        None = 0,

        /// <summary>
        /// If set, the X axis will rotate to face the camera position.
        /// If not set, the X axis will rotate to face the camera screen plane.
        /// </summary>
        PointRotationX = 0x001,
        /// <summary>
        /// If set, the Y axis will rotate to face the camera position.
        /// If not set, the Y axis will rotate to face the camera screen plane.
        /// </summary>
        PointRotationY = 0x002,
        /// <summary>
        /// If set, the Z axis will rotate to face the camera position.
        /// If not set, the Z axis will rotate to face the camera screen plane.
        /// </summary>
        PointRotationZ = 0x004,

        /// <summary>
        /// If set, the X axis will scale according to the distance to the camera position.
        /// If not set, the X axis will scale according to the distance to the camera screen plane.
        /// </summary>
        PointScaleX = 0x008,
        /// <summary>
        /// If set, the Y axis will scale according to the distance to the camera position.
        /// If not set, the Y axis will scale according to the distance to the camera screen plane.
        /// </summary>
        PointScaleY = 0x010,
        /// <summary>
        /// If set, the Z axis will scale according to the distance to the camera position.
        /// If not set, the Z axis will scale according to the distance to the camera screen plane.
        /// </summary>
        PointScaleZ = 0x020,

        /// <summary>
        /// If set, the X axis will rotate to face the camera.
        /// </summary>
        RotateX = 0x040,
        /// <summary>
        /// If set, the Y axis will rotate to face the camera.
        /// </summary>
        RotateY = 0x080,
        /// <summary>
        /// If set, the Z axis will rotate to face the camera.
        /// </summary>
        RotateZ = 0x100,

        /// <summary>
        /// If set, the X axis will scale according to camera.
        /// </summary>
        ScaleX = 0x200,
        /// <summary>
        /// If set, the Y axis will rotate to face the camera.
        /// </summary>
        ScaleY = 0x400,
        /// <summary>
        /// If set, the Z axis will rotate to face the camera.
        /// </summary>
        ScaleZ = 0x800,
        /// <summary>
        /// If set, the X axis translation will not move with the camera.
        /// </summary>
        ConstrainTranslationX = 0x1000,
        /// <summary>
        /// If set, the Y axis translation will not move with the camera.
        /// </summary>
        ConstrainTranslationY = 0x2000,
        /// <summary>
        /// If set, the Z axis translation will not move with the camera.
        /// </summary>
        ConstrainTranslationZ = 0x4000,

        /// <summary>
        /// If set, the position on all axes will not move with the camera.
        /// </summary>
        ConstrainTranslations = ConstrainTranslationX | ConstrainTranslationY | ConstrainTranslationZ,
    }
}
