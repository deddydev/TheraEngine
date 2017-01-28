using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public static class VertexShaderGenerator
    {
        private const string BoneCountDef = "BONE_COUNT";
        private const string MorphCountDef = "MORPH_COUNT";
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

            //Write #definitions
            _generator.WriteVersion();
            if (_info.IsWeighted)
                wl("#define {0} = {1};", BoneCountDef, _info._boneCount);
            if (allowMeshMorphing && _info._pnbtCount > 1)
                wl("#define {0} = {1};", MorphCountDef, _info._pnbtCount);
            wl();

            //Write header in fields (from buffers)
            WriteBuffers(true);
            wl();
            //Write header uniforms
            WriteMatrixUniforms();
            wl();
            //Write header out fields (to fragment shader)
            WriteOutData();
            wl();

            //Write the beginning of the main function
            _generator.Begin();

            if (_info.IsWeighted)
                WriteRiggedPNTB(allowMeshMorphing ? singleMorphRig : true);
            else
                WriteStaticPNTB(allowMeshMorphing);

            return new Shader(ShaderMode.Vertex, _generator.Finish());
        }
        private static void WriteMatrixUniforms()
        {
            WriteUniform(GLTypeName._mat4, "ModelMatrix");
            WriteUniform(GLTypeName._mat4, "NormalMatrix");
            WriteUniform(GLTypeName._mat4, "ViewMatrix");
            WriteUniform(GLTypeName._mat4, "ProjMatrix");
            if (_info.IsWeighted)
            {
               WriteUniform(GLTypeName._mat4, "BoneMatrices[" + BoneCountDef + "]");
               WriteUniform(GLTypeName._mat4, "BoneMatricesIT[" + BoneCountDef + "]");
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
            wl("} OutData;");
        }
        private static void WriteRiggedPNTB(bool singleRig = true)
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

            if (singleRig)
            {
                wl("for (int i = 0; i < 4; ++i)");
                wl("{");
                wl("OutData.Position += {0}[{1}[i]] * finalPosition * {2}[i];", Uniform.BoneMatricesName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasNormals)
                    wl("OutData.Normal += {0}[{1}[i]] * finalNormal * {2}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasBinormals)
                    wl("OutData.Binormal += {0}[{1}[i]] * finalBinormal * {2}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasTangents)
                    wl("OutData.Tangent += {0}[{1}[i]] * finalTangent * {2}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                wl("}");
            }
            else
            {
                wl("float totalWeight = 0;");
                //foreach (float f in weights)
                //    totalWeight += f;

                //float baseWeight = 1.0f - totalWeight;
                //float total = totalWeight + baseWeight;

                wl("for (int i = 0; i < 4; ++i)");
                wl("{");
                wl("OutData.Position += {0}[{1}[i]] * finalPosition * {2}[i];", Uniform.BoneMatricesName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasNormals)
                    wl("OutData.Normal += {0}[{1}[i]] * finalNormal * {2}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasBinormals)
                    wl("OutData.Binormal += {0}[{1}[i]] * finalBinormal * {2}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                if (_info.HasTangents)
                    wl("OutData.Tangent += {0}[{1}[i]] * finalTangent * {2}[i];", Uniform.BoneMatricesITName, BufferType.MatrixIds, BufferType.MatrixWeights);
                wl("}");
            }
        }
        private static void WriteStaticPNTB(bool morphed)
        {
            wl("vec4 finalPosition = ModelMatrix * vec4(Position0, 1.0);");
            wl("OutData.Position = finalPosition.xyz;");
            wl("gl_Position = ProjMatrix * ViewMatrix * finalPosition;");
            if (_info.HasNormals)
                wl("OutData.Normal = normalize((NormalMatrix * vec4(Normal0, 1.0)).xyz);");
            if (_info.HasBinormals)
                wl("OutData.Binormal = normalize((NormalMatrix * vec4(Binormal0, 1.0)).xyz);");
            if (_info.HasTangents)
                wl("OutData.Tangent = normalize((NormalMatrix * vec4(Tangent0, 1.0)).xyz);");
        }
        private static void WriteBuffers(bool singleMorphRig)
        {
            int layoutLocation = 0;

            //Write mesh buffers first
            for (int i = 0; i < _info._pnbtCount; ++i)
            {
                WriteInVar(layoutLocation++, GLTypeName._vec3, BufferType.Position.ToString() + i);
                if (_info.HasNormals)
                    WriteInVar(layoutLocation++, GLTypeName._vec3, BufferType.Normal.ToString() + i);
                if (_info.HasBinormals)
                    WriteInVar(layoutLocation++, GLTypeName._vec3, BufferType.Binormal.ToString() + i);
                if (_info.HasTangents)
                    WriteInVar(layoutLocation++, GLTypeName._vec3, BufferType.Tangent.ToString() + i);
            }
            //Then colors and texcoords
            for (int i = 0; i < _info._colorCount; ++i)
                WriteInVar(layoutLocation++, GLTypeName._vec4, BufferType.Color.ToString() + i);
            for (int i = 0; i < _info._texcoordCount; ++i)
                WriteInVar(layoutLocation++, GLTypeName._vec2, BufferType.TexCoord.ToString() + i);

            //Barycentric coord, for wireframe rendering
            if (_info._hasBarycentricCoord)
                WriteInVar(layoutLocation++, GLTypeName._vec3, BufferType.Barycentric.ToString());

            //And finally influence buffers
            if (_info.IsWeighted)
            {
                if (singleMorphRig)
                {
                    WriteInVar(layoutLocation++, GLTypeName._ivec4, BufferType.MatrixIds.ToString());
                    WriteInVar(layoutLocation++, GLTypeName._vec4, BufferType.MatrixWeights.ToString());
                }
                else
                {
                    for (int i = 0; i < _info._pnbtCount; ++i)
                    {
                        WriteInVar(layoutLocation++, GLTypeName._ivec4, BufferType.MatrixIds.ToString() + i);
                        WriteInVar(layoutLocation++, GLTypeName._vec4, BufferType.MatrixWeights.ToString() + i);
                    }
                }
            }
        }
    }
}
