using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using TheraEngine.Core.Memory;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;

namespace TheraEngine.Rendering.OpenGL
{
    public class MinGLVersion : Attribute
    {
        public EOpenGLVersion Version { get; }
        public MinGLVersion(EOpenGLVersion ver) => Version = ver;
    }
    public unsafe class GLRenderer : AbstractRenderer
    {
        public const bool ForceShaderOutput = false;

        // https://www.opengl.org/wiki/Rendering_Pipeline_Overview

        public override RenderLibrary RenderLibrary => RenderLibrary.OpenGL;

        public GLRenderer() { }

        private ShaderType _currentShaderMode;
        
        public override void Clear(EBufferClear mask)
        {
            ClearBufferMask newMask = 0;
            if (mask.HasFlag(EBufferClear.Color))
                newMask |= ClearBufferMask.ColorBufferBit;
            if (mask.HasFlag(EBufferClear.Depth))
                newMask |= ClearBufferMask.DepthBufferBit;
            if (mask.HasFlag(EBufferClear.Stencil))
                newMask |= ClearBufferMask.StencilBufferBit;
            if (mask.HasFlag(EBufferClear.Accum))
                newMask |= ClearBufferMask.AccumBufferBit;
            GL.Clear(newMask);
        }

        public override void SetPointSize(float size) => GL.PointSize(size);
        public override void SetLineSize(float size) => GL.LineWidth(size);
        
        [MinGLVersion(EOpenGLVersion.Ver_4_2)]
        public override void MemoryBarrier(EMemoryBarrierFlags flags)
            => GL.MemoryBarrier((MemoryBarrierFlags)flags);

        [MinGLVersion(EOpenGLVersion.Ver_4_5)]
        public override void MemoryBarrierByRegion(EMemoryBarrierRegionFlags flags)
            => GL.MemoryBarrierByRegion((MemoryBarrierRegionFlags)flags);

        [MinGLVersion(EOpenGLVersion.Ver_1_0)]
        public override void ClearColor(ColorF4 color)
            => GL.ClearColor(color.R, color.G, color.B, color.A);

        public override void CheckErrors()
        {
            //ErrorCode code;
            //string error = "";
            //string temp;
            //while ((code = GL.GetError()) != ErrorCode.NoError)
            //{
            //    temp = code.ToString();
            //    Prevent infinite loop
            //    if (!error.Contains(temp))
            //        error += temp;
            //    else
            //        break;
            //}
            //if (error.Length > 0)
            //    throw new Exception(error);
            //Engine.LogWarning(error);
        }

        #region Objects
        public override void DeleteObject(EObjectType type, int bindingId)
        {
            switch (type)
            {
                case EObjectType.Buffer:
                    GL.DeleteBuffer(bindingId);
                    break;
                case EObjectType.Framebuffer:
                    GL.DeleteFramebuffer(bindingId);
                    break;
                case EObjectType.Program:
                    GL.DeleteProgram(bindingId);
                    break;
                case EObjectType.ProgramPipeline:
                    GL.DeleteProgramPipeline(bindingId);
                    break;
                case EObjectType.Query:
                    GL.DeleteQuery(bindingId);
                    break;
                case EObjectType.Renderbuffer:
                    GL.DeleteRenderbuffer(bindingId);
                    break;
                case EObjectType.Sampler:
                    GL.DeleteSampler(bindingId);
                    break;
                case EObjectType.Texture:
                    GL.DeleteTexture(bindingId);
                    break;
                case EObjectType.TransformFeedback:
                    GL.DeleteTransformFeedback(bindingId);
                    break;
                case EObjectType.VertexArray:
                    GL.DeleteVertexArray(bindingId);
                    break;
                case EObjectType.Shader:
                    GL.DeleteShader(bindingId);
                    break;
            }
        }
        public override void DeleteObjects(EObjectType type, int[] bindingIds)
        {
            switch (type)
            {
                case EObjectType.Buffer:
                    GL.DeleteBuffers(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.Framebuffer:
                    GL.DeleteFramebuffers(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.Program:
                    foreach (int i in bindingIds)
                        GL.DeleteProgram(i);
                    break;
                case EObjectType.ProgramPipeline:
                    GL.DeleteProgramPipelines(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.Query:
                    GL.DeleteQueries(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.Renderbuffer:
                    GL.DeleteRenderbuffers(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.Sampler:
                    GL.DeleteSamplers(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.Texture:
                    GL.DeleteTextures(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.TransformFeedback:
                    GL.DeleteTransformFeedbacks(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.VertexArray:
                    GL.DeleteVertexArrays(bindingIds.Length, bindingIds);
                    break;
                case EObjectType.Shader:
                    foreach (int i in bindingIds)
                        GL.DeleteShader(i);
                    break;
            }
        }
        public override int[] CreateObjects(EObjectType type, int count)
        {
            int[] ids = new int[count];
            switch (type)
            {
                case EObjectType.Buffer:
                    GL.CreateBuffers(count, ids);
                    break;
                case EObjectType.Framebuffer:
                    GL.CreateFramebuffers(count, ids);
                    break;
                case EObjectType.Program:
                    for (int i = 0; i < count; ++i)
                        ids[i] = GL.CreateProgram();
                    break;
                case EObjectType.ProgramPipeline:
                    GL.CreateProgramPipelines(count, ids);
                    break;
                case EObjectType.Query:
                    //throw new InvalidOperationException("Call CreateQueries instead.");
                    GL.GenQueries(count, ids);
                    break;
                case EObjectType.Renderbuffer:
                    GL.CreateRenderbuffers(count, ids);
                    break;
                case EObjectType.Sampler:
                    GL.CreateSamplers(count, ids);
                    break;
                case EObjectType.Texture:
                    throw new InvalidOperationException("Call CreateTextures instead.");
                case EObjectType.TransformFeedback:
                    GL.CreateTransformFeedbacks(count, ids);
                    break;
                case EObjectType.VertexArray:
                    GL.CreateVertexArrays(count, ids);
                    break;
                case EObjectType.Shader:
                    for (int i = 0; i < count; ++i)
                        ids[i] = GL.CreateShader(_currentShaderMode);
                    break;
            }
            return ids;
        }
        public override int[] CreateTextures(ETexTarget target, int count)
        {
            int[] ids = new int[count];
            GL.GenTextures(count, ids);
            return ids;
        }
        public override int[] CreateQueries(EQueryTarget type, int count)
        {
            int[] ids = new int[count];
            GL.CreateQueries((QueryTarget)type, count, ids);
            return ids;
        }
        #endregion

        #region Shaders
        public override void SetShaderMode(EShaderMode type)
        {
            switch (type)
            {
                case EShaderMode.Fragment:
                    _currentShaderMode = ShaderType.FragmentShader;
                    break;
                case EShaderMode.Vertex:
                    _currentShaderMode = ShaderType.VertexShader;
                    break;
                case EShaderMode.Geometry:
                    _currentShaderMode = ShaderType.GeometryShader;
                    break;
                case EShaderMode.TessControl:
                    _currentShaderMode = ShaderType.TessControlShader;
                    break;
                case EShaderMode.TessEvaluation:
                    _currentShaderMode = ShaderType.TessEvaluationShader;
                    break;
                case EShaderMode.Compute:
                    _currentShaderMode = ShaderType.ComputeShader;
                    break;
            }
        }

        [MinGLVersion(EOpenGLVersion.Ver_4_3)]
        public override void DispatchCompute(int numGroupsX, int numGroupsY, int numGroupsZ)
            => GL.DispatchCompute(numGroupsX, numGroupsY, numGroupsZ);

        [MinGLVersion(EOpenGLVersion.Ver_4_3)]
        public override void DispatchComputeIndirect(int offset)
            => GL.DispatchComputeIndirect((IntPtr)offset);

        [MinGLVersion(EOpenGLVersion.Ver_3_0)]
        public override void SetBindFragDataLocation(int bindingId, int location, string name)
            => GL.BindFragDataLocation(bindingId, location, name);

        [MinGLVersion(EOpenGLVersion.Ver_2_0)]
        public override void SetShaderSource(int bindingId, params string[] sources)
            => GL.ShaderSource(bindingId, sources.Length, sources, sources.Select(x => x.Length).ToArray());
        
        [MinGLVersion(EOpenGLVersion.Ver_2_0)]
        public override bool CompileShader(int bindingId, out string info)
        {
            GL.CompileShader(bindingId);
            GL.GetShader(bindingId, ShaderParameter.CompileStatus, out int status);
            GL.GetShaderInfoLog(bindingId, out info);
            return status != 0;
        }

        [MinGLVersion(EOpenGLVersion.Ver_4_1)]
        public override void SetProgramParameter(int programBindingId, EProgParam parameter, int value)
            => GL.ProgramParameter(programBindingId, (ProgramParameterName)(int)parameter, value);

        [MinGLVersion(EOpenGLVersion.Ver_4_1)]
        public override void BindPipeline(int pipelineBindingId)
            => GL.BindProgramPipeline(pipelineBindingId);

        [MinGLVersion(EOpenGLVersion.Ver_4_1)]
        public override void SetPipelineStage(int pipelineBindingId, EProgramStageMask mask, int programBindingId)
            => GL.UseProgramStages(pipelineBindingId, (ProgramStageMask)(int)mask, programBindingId);

        [MinGLVersion(EOpenGLVersion.Ver_4_1)]
        public override void ActiveShaderProgram(int pipelineBindingId, int programBindingId)
            => GL.ActiveShaderProgram(pipelineBindingId, programBindingId);

        [MinGLVersion(EOpenGLVersion.Ver_4_1)]
        public override int GenerateProgram(bool separable)
        {
            int handle = GL.CreateProgram();
            GL.ProgramParameter(handle, ProgramParameterName.ProgramSeparable, separable ? 1 : 0);
            return handle;
        }
        public override void AttachShader(int shaderBindingId, int programBindingId)
        {
            GL.AttachShader(programBindingId, shaderBindingId);
        }
        public override void DetachShader(int shaderBindingId, int programBindingId)
        {
            GL.DetachShader(programBindingId, shaderBindingId);
        }
        public override bool LinkProgram(int bindingId, out string info)
        {
            GL.LinkProgram(bindingId);
            GL.GetProgram(bindingId, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                GL.GetProgramInfoLog(bindingId, out info);
                if (string.IsNullOrEmpty(info))
                    Engine.LogWarning("Unable to link program, but no error was returned.");
                else
                    Engine.LogWarning(info);
                return false;
            }
            else
            {
                info = null;
                return true;
            }
        }
        public override void UniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding)
        {
            GL.UniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding);
        }
        public override void SetActiveTexture(int unit)
        {
#if DEBUG
            int maxUnits = Engine.ComputerInfo.MaxTextureUnits;
            if (unit < 0)
                throw new InvalidOperationException("Unit needs to be >= 0.");
            if (unit >= maxUnits)
                throw new InvalidOperationException("Unit needs to be less than " + maxUnits.ToString());
#endif
            //Max texture unit is not limited by the number in this enum definition
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
        }
        public override void UseProgram(int bindingId)
        {
#if DEBUG
            if (bindingId <= BaseRenderState.NullBindingId)
                throw new InvalidOperationException($"{bindingId} is not a valid render state id.");
#endif
            GL.UseProgram(bindingId);
        }
        public override void EnableDepthTest(bool enabled)
        {
            if (enabled)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);
        }
        public override void ClearStencil(int value)
        {
            GL.ClearStencil(value);
        }
        /// <summary>
        /// Determines how the stencil buffer should be updated based on the current depth and stencil buffers and functions.
        /// </summary>
        /// <param name="fail">Action to take if the stencil test fails.</param>
        /// <param name="zFail">Action to take if the stencil test passes, but the depth test fails.</param>
        /// <param name="zPass">Action to take if both tests pass.</param>
        public override void StencilOp(EStencilOp fail, EStencilOp zFail, EStencilOp zPass)
        {
            GL.StencilOp((StencilOp)(int)fail, (StencilOp)(int)zFail, (StencilOp)(int)zPass);
        }
        public override void StencilMask(int mask)
        {
            GL.StencilMask(mask);
        }
        public override void ApplyRenderParams(RenderingParameters r)
        {
            if (r == null)
                return;

            Engine.Renderer.ColorMask(r.WriteRed, r.WriteGreen, r.WriteBlue, r.WriteAlpha);
            Engine.Renderer.Cull(r.CullMode);
            GL.PointSize(r.PointSize);
            GL.LineWidth(r.LineWidth.Clamp(0.0f, 1.0f));

            if (r.DepthTest.Enabled == ERenderParamUsage.Enabled)
            {
                GL.Enable(EnableCap.DepthTest);
                DepthFunc(r.DepthTest.Function);
                GL.DepthMask(r.DepthTest.UpdateDepth);
            }
            else if (r.DepthTest.Enabled == ERenderParamUsage.Disabled)
                GL.Disable(EnableCap.DepthTest);

            if (r.BlendMode.Enabled == ERenderParamUsage.Enabled)
            {
                GL.Enable(EnableCap.Blend);
                BlendEquation(r.BlendMode.RgbEquation, r.BlendMode.AlphaEquation);
                BlendFuncSeparate(r.BlendMode.RgbSrcFactor, r.BlendMode.RgbDstFactor, r.BlendMode.AlphaSrcFactor, r.BlendMode.AlphaDstFactor);
            }
            else if (r.BlendMode.Enabled == ERenderParamUsage.Disabled)
                GL.Disable(EnableCap.Blend);

            //if (r.AlphaTest.Enabled == ERenderParamUsage.Enabled)
            //{
            //    GL.Enable(EnableCap.AlphaTest);
            //    GL.AlphaFunc(AlphaFunction.Never + (int)r.AlphaTest.Comp, r.AlphaTest.Ref);
            //}
            //else if (r.AlphaTest.Enabled == ERenderParamUsage.Disabled)
            //    GL.Disable(EnableCap.AlphaTest);

            if (r.StencilTest.Enabled == ERenderParamUsage.Enabled)
            {
                StencilTest st = r.StencilTest;
                StencilTestFace b = st.BackFace;
                StencilTestFace f = st.FrontFace;
                GL.StencilOpSeparate(StencilFace.Back,
                    (StencilOp)(int)b.BothFailOp,
                    (StencilOp)(int)b.StencilPassDepthFailOp,
                    (StencilOp)(int)b.BothPassOp);
                GL.StencilOpSeparate(StencilFace.Front,
                    (StencilOp)(int)f.BothFailOp,
                    (StencilOp)(int)f.StencilPassDepthFailOp,
                    (StencilOp)(int)f.BothPassOp);
                GL.StencilMaskSeparate(StencilFace.Back, b.WriteMask);
                GL.StencilMaskSeparate(StencilFace.Front, f.WriteMask);
                GL.StencilFuncSeparate(StencilFace.Back,
                    StencilFunction.Never + (int)b.Func, b.Ref, b.ReadMask);
                GL.StencilFuncSeparate(StencilFace.Front,
                    StencilFunction.Never + (int)f.Func, f.Ref, f.ReadMask);
            }
            else if (r.StencilTest.Enabled == ERenderParamUsage.Disabled)
            {
                //GL.Disable(EnableCap.StencilTest);
                GL.StencilMask(0);
                GL.StencilOp(
                    OpenTK.Graphics.OpenGL.StencilOp.Keep,
                    OpenTK.Graphics.OpenGL.StencilOp.Keep,
                    OpenTK.Graphics.OpenGL.StencilOp.Keep);
                GL.StencilFunc(StencilFunction.Always, 0, 0);
            }
        }

        protected override int OnGetAttribLocation(int programBindingId, string name)
        {
            return GL.GetAttribLocation(programBindingId, name);
        }
        protected override int OnGetUniformLocation(int programBindingId, string name)
        {
            int loc = GL.GetUniformLocation(programBindingId, name);
            return loc;
        }

        #region Uniform
        //public override void Uniform(int location, params IUniformable4Int[] p)
        //{
        //    const int count = 4;

        //    if (location < 0)
        //        return;

        //    int[] values = new int[p.Length << 2];

        //    for (int i = 0; i < p.Length; ++i)
        //        for (int x = 0; x < count; ++x)
        //            values[(i << 2) + x] = p[i].Data[x];

        //    GL.Uniform4(location, p.Length, values);
        //}
        //public override void Uniform(int location, params IUniformable4Float[] p)
        //{
        //    const int count = 4;

        //    if (location < 0)
        //        return;

        //    float[] values = new float[p.Length << 2];

        //    for (int i = 0; i < p.Length; ++i)
        //        for (int x = 0; x < count; ++x)
        //            values[(i << 2) + x] = p[i].Data[x];

        //    GL.Uniform4(location, p.Length, values);
        //}
        //public override void Uniform(int location, params IUniformable3Int[] p)
        //{
        //    const int count = 3;

        //    if (location < 0)
        //        return;

        //    int[] values = new int[p.Length * 3];

        //    for (int i = 0; i < p.Length; ++i)
        //        for (int x = 0; x < count; ++x)
        //            values[i * 3 + x] = p[i].Data[x];

        //    GL.Uniform3(location, p.Length, values);
        //}
        //public override void Uniform(int location, params IUniformable3Float[] p)
        //{
        //    const int count = 3;

        //    if (location < 0)
        //        return;

        //    float[] values = new float[p.Length * 3];

        //    for (int i = 0; i < p.Length; ++i)
        //        for (int x = 0; x < count; ++x)
        //            values[i * 3 + x] = p[i].Data[x];

        //    GL.Uniform3(location, p.Length, values);
        //}
        //public override void Uniform(int location, params IUniformable2Int[] p)
        //{
        //    const int count = 2;

        //    if (location < 0)
        //        return;

        //    int[] values = new int[p.Length << 1];

        //    for (int i = 0; i < p.Length; ++i)
        //        for (int x = 0; x < count; ++x)
        //            values[(i << 1) + x] = p[i].Data[x];

        //    GL.Uniform2(location, p.Length, values);
        //}
        //public override void Uniform(int location, params IUniformable2Float[] p)
        //{
        //    const int count = 2;

        //    if (location < 0)
        //        return;

        //    float[] values = new float[p.Length << 1];

        //    for (int i = 0; i < p.Length; ++i)
        //        for (int x = 0; x < count; ++x)
        //            values[(i << 1) + x] = p[i].Data[x];

        //    GL.Uniform2(location, p.Length, values);
        //}
        //public override void Uniform(int location, params IUniformable1Int[] p)
        //{
        //    if (location > -1)
        //        GL.Uniform1(location, p.Length, p.Select(x => *x.Data).ToArray());
        //}
        //public override void Uniform(int location, params IUniformable1Float[] p)
        //{
        //    if (location > -1)
        //        GL.Uniform1(location, p.Length, p.Select(x => *x.Data).ToArray());
        //}
        //public override void Uniform(int location, params int[] p)
        //{
        //    if (location > -1)
        //        GL.Uniform1(location, p.Length, p);
        //}
        //public override void Uniform(int location, params float[] p)
        //{
        //    if (location > -1)
        //        GL.Uniform1(location, p.Length, p);
        //}
        //public override void Uniform(int location, Matrix4 p)
        //{
        //    if (location > -1)
        //        GL.UniformMatrix4(location, 1, false, p.Data);
        //}
        //public override void Uniform(int location, params Matrix4[] p)
        //{
        //    if (location > -1)
        //    {
        //        float[] values = new float[p.Length << 4];
        //        for (int i = 0; i < p.Length; ++i)
        //            for (int x = 0; x < 16; ++x)
        //                values[(i << 4) + x] = p[i].Data[x];
        //        GL.UniformMatrix4(location, p.Length, false, values);
        //    }
        //}
        //public override void Uniform(int location, Matrix3 p)
        //{
        //    if (location > -1)
        //        GL.UniformMatrix3(location, 1, false, p.Data);
        //}
        //public override void Uniform(int location, params Matrix3[] p)
        //{
        //    if (location > -1)
        //    {
        //        float[] values = new float[p.Length * 9];
        //        for (int i = 0; i < p.Length; ++i)
        //            for (int x = 0; x < 9; ++x)
        //                values[i * 9 + x] = p[i].Data[x];
        //        GL.UniformMatrix3(location, p.Length, false, values);
        //    }
        //}
        #endregion

        #region Program Uniform
        public override void Uniform(int programBindingId, int location, params IUniformable4Int[] p)
        {
            if (location < 0)
                return;
            int[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1], x.Data[2], x.Data[3] }).ToArray();
            GL.ProgramUniform4(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable4Float[] p)
        {
            if (location < 0)
                return;
            float[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1], x.Data[2], x.Data[3] }).ToArray();
            GL.ProgramUniform4(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable4UInt[] p)
        {
            throw new NotImplementedException();
            //if (location < 0)
            //    return;
            //uint[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1], x.Data[2], x.Data[3] }).ToArray();
            //GL.ProgramUniform4(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable4Double[] p)
        {
            if (location < 0)
                return;
            double[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1], x.Data[2], x.Data[3] }).ToArray();
            GL.ProgramUniform4(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable4Bool[] p)
        {
            throw new NotImplementedException();
            //if (location < 0)
            //    return;
            //bool[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1], x.Data[2], x.Data[3] }).ToArray();
            //GL.ProgramUniform4(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable3Int[] p)
        {
            if (location < 0)
                return;
            int[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1], x.Data[2] }).ToArray();
            GL.ProgramUniform3(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable3Float[] p)
        {
            if (location < 0)
                return;
            float[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1], x.Data[2] }).ToArray();
            GL.ProgramUniform3(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable2Int[] p)
        {
            if (location < 0)
                return;
            int[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1] }).ToArray();
            GL.ProgramUniform2(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable2Float[] p)
        {
            if (location < 0)
                return;
            float[] values = p.SelectMany(x => new[] { x.Data[0], x.Data[1] }).ToArray();
            GL.ProgramUniform2(programBindingId, location, p.Length, values);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable1Int[] p)
        {
            if (location < 0)
                return;
            int[] r = p.Select(x => *x.Data).ToArray();
            fixed (int* first = &r[0])
                GL.ProgramUniform1(programBindingId, location, p.Length, first);
        }
        public override void Uniform(int programBindingId, int location, params IUniformable1Float[] p)
        {
            if (location < 0)
                return;
            float[] r = p.Select(x => *x.Data).ToArray();
            fixed (float* first = &r[0])
                GL.ProgramUniform1(programBindingId, location, p.Length, first);
        }
        public override void Uniform(int programBindingId, int location, params int[] p)
        {
            if (location >= 0)
                fixed (int* first = &p[0])
                    GL.ProgramUniform1(programBindingId, location, p.Length, first);
        }
        public override void Uniform(int programBindingId, int location, params float[] p)
        {
            if (location >= 0)
                fixed (float* first = &p[0])
                    GL.ProgramUniform1(programBindingId, location, p.Length, first);
        }
        public override void Uniform(int programBindingId, int location, Matrix4 p)
        {
            if (location >= 0)
                GL.ProgramUniformMatrix4(programBindingId, location, 1, false, p.Data);
        }
        public override void Uniform(int programBindingId, int location, params Matrix4[] p)
        {
            if (location >= 0)
            {
                float[] values = new float[p.Length << 4];
                for (int i = 0; i < p.Length; ++i)
                    for (int x = 0; x < 16; ++x)
                        values[(i << 4) + x] = p[i].Data[x];
                GL.ProgramUniformMatrix4(programBindingId, location, p.Length, false, values);
            }
        }
        public override void Uniform(int programBindingId, int location, Matrix3 p)
        {
            if (location >= 0)
                GL.ProgramUniformMatrix3(programBindingId, location, 1, false, p.Data);
        }
        public override void Uniform(int programBindingId, int location, params Matrix3[] p)
        {
            if (location >= 0)
            {
                float[] values = new float[p.Length * 9];
                for (int i = 0; i < p.Length; ++i)
                    for (int x = 0; x < 9; ++x)
                        values[i * 9 + x] = p[i].Data[x];
                GL.ProgramUniformMatrix3(programBindingId, location, p.Length, false, values);
            }
        }
        #endregion

        #endregion

        #region Framebuffers

        public override void CheckFrameBufferErrors()
        {
            FramebufferErrorCode c = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (c != FramebufferErrorCode.FramebufferComplete)
                //throw new Exception("Problem compiling framebuffer: " + c.ToString());
                Engine.LogWarning("Problem compiling framebuffer: " + c.ToString());
        }

        public override void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel)
            => GL.NamedFramebufferTexture(frameBufferBindingId, (FramebufferAttachment)(int)attachment, textureBindingId, mipLevel);
        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, ETexTarget texTarget, int textureBindingId, int mipLevel)
            => GL.FramebufferTexture2D((FramebufferTarget)(int)target, (FramebufferAttachment)(int)attachment, (TextureTarget)(int)texTarget, textureBindingId, mipLevel);
        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, int textureBindingId, int mipLevel)
            => GL.FramebufferTexture((FramebufferTarget)(int)target, (FramebufferAttachment)(int)attachment, textureBindingId, mipLevel);
        public override void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel, int layer)
            => GL.NamedFramebufferTextureLayer(frameBufferBindingId, (FramebufferAttachment)(int)attachment, textureBindingId, mipLevel, layer);
        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, int textureBindingId, int mipLevel, int layer)
            => GL.FramebufferTextureLayer((FramebufferTarget)(int)target, (FramebufferAttachment)(int)attachment, textureBindingId, mipLevel, layer);

        public override void SetDrawBuffer(EDrawBuffersAttachment attachment)
            => GL.DrawBuffer((DrawBufferMode)(int)attachment);
        public override void SetDrawBuffer(int bindingId, EDrawBuffersAttachment attachment)
            => GL.NamedFramebufferDrawBuffer(bindingId, (DrawBufferMode)(int)attachment);
        public override void SetDrawBuffers(EDrawBuffersAttachment[] attachments)
           => GL.DrawBuffers(attachments.Length, attachments.Select(x => (DrawBuffersEnum)(int)x).ToArray());
        public override void SetDrawBuffers(int bindingId, EDrawBuffersAttachment[] attachments)
           => GL.NamedFramebufferDrawBuffers(bindingId, attachments.Length, attachments.Select(x => (DrawBuffersEnum)(int)x).ToArray());

        public override void SetReadBuffer(EDrawBuffersAttachment attachment)
            => GL.ReadBuffer((ReadBufferMode)(int)attachment);
        public override void SetReadBuffer(int bindingId, EDrawBuffersAttachment attachment)
           => GL.NamedFramebufferReadBuffer(bindingId, (ReadBufferMode)(int)attachment);

        public override void BindRenderBuffer(int bindingId)
            => GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, bindingId);
        public override void RenderbufferStorage(ERenderBufferStorage storage, int width, int height)
            => GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)(int)storage, width, height);

        public override void FramebufferRenderBuffer(EFramebufferTarget target, EFramebufferAttachment attachement, int renderBufferBindingId)
            => GL.FramebufferRenderbuffer((FramebufferTarget)(int)target, (FramebufferAttachment)(int)attachement, RenderbufferTarget.Renderbuffer, renderBufferBindingId);
        public override void FramebufferRenderBuffer(int frameBufferBindingId, EFramebufferAttachment attachement, int renderBufferBindingId)
            => GL.NamedFramebufferRenderbuffer(frameBufferBindingId, (FramebufferAttachment)(int)attachement, RenderbufferTarget.Renderbuffer, renderBufferBindingId);
        
        public override void BlitFrameBuffer(
            int readBufferId, int writeBufferId,
            int srcX0, int srcY0,
            int srcX1, int srcY1,
            int dstX0, int dstY0,
            int dstX1, int dstY1,
            EClearBufferMask mask,
            EBlitFramebufferFilter filter)
        {
            ClearBufferMask maskgl = ClearBufferMask.None;
            if (mask.HasFlag(EClearBufferMask.AccumBufferBit))
                maskgl |= ClearBufferMask.AccumBufferBit;
            if (mask.HasFlag(EClearBufferMask.ColorBufferBit))
                maskgl |= ClearBufferMask.ColorBufferBit;
            if (mask.HasFlag(EClearBufferMask.CoverageBufferBitNv))
                maskgl |= ClearBufferMask.CoverageBufferBitNv;
            if (mask.HasFlag(EClearBufferMask.DepthBufferBit))
                maskgl |= ClearBufferMask.DepthBufferBit;
            if (mask.HasFlag(EClearBufferMask.StencilBufferBit))
                maskgl |= ClearBufferMask.StencilBufferBit;

            BlitFramebufferFilter filtergl = filter == EBlitFramebufferFilter.Linear ? 
                BlitFramebufferFilter.Linear : BlitFramebufferFilter.Nearest;

            GL.BlitNamedFramebuffer(
                readBufferId, writeBufferId,
                srcX0, srcY0,
                srcX1, srcY1,
                dstX0, dstY0,
                dstX1, dstY1,
                maskgl, filtergl);
        }
        public override void BlitFrameBuffer(
            int srcX0, int srcY0,
            int srcX1, int srcY1,
            int dstX0, int dstY0,
            int dstX1, int dstY1,
            EClearBufferMask mask,
            EBlitFramebufferFilter filter)
        {
            ClearBufferMask maskgl = ClearBufferMask.None;
            if (mask.HasFlag(EClearBufferMask.AccumBufferBit))
                maskgl |= ClearBufferMask.AccumBufferBit;
            if (mask.HasFlag(EClearBufferMask.ColorBufferBit))
                maskgl |= ClearBufferMask.ColorBufferBit;
            if (mask.HasFlag(EClearBufferMask.CoverageBufferBitNv))
                maskgl |= ClearBufferMask.CoverageBufferBitNv;
            if (mask.HasFlag(EClearBufferMask.DepthBufferBit))
                maskgl |= ClearBufferMask.DepthBufferBit;
            if (mask.HasFlag(EClearBufferMask.StencilBufferBit))
                maskgl |= ClearBufferMask.StencilBufferBit;

            BlitFramebufferFilter filtergl = filter == EBlitFramebufferFilter.Linear ?
                BlitFramebufferFilter.Linear : BlitFramebufferFilter.Nearest;

            GL.BlitFramebuffer(
                srcX0, srcY0,
                srcX1, srcY1,
                dstX0, dstY0,
                dstX1, dstY1,
                maskgl, filtergl);
        }
        public override void BindFrameBuffer(EFramebufferTarget type, int bindingId)
        {
            GL.BindFramebuffer((FramebufferTarget)type, bindingId);
        }
        #endregion

        public override byte GetStencilIndex(float x, float y)
        {
            byte val = 0;
            GL.ReadPixels((int)x, (int)y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.StencilIndex, PixelType.UnsignedByte, ref val);
            return val;
        }
        public override float GetDepth(float x, float y)
        {
            float val = 20;
            GL.ReadPixels((int)x, (int)y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, ref val);
            return val;

            //int val = 0;
            //GL.ReadPixels((int)x, (int)(Engine.CurrentPanel.Height - y), 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthStencil, PixelType.UnsignedInt248, ref val);
            //return (float)(val >> 8) / UInt24.MaxValue;
        }

        protected override void SetRenderArea(BoundingRectangle region)
            => GL.Viewport(region.IntX, region.IntY, region.IntWidth, region.IntHeight);
        public override void CropRenderArea(BoundingRectangle region)
            => GL.Scissor(region.IntX, region.IntY, region.IntWidth, region.IntHeight);
        
        #region Transform Feedback
        public override void BindTransformFeedback(int bindingId)
        {
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, bindingId);
        }
        public override void BeginTransformFeedback(FeedbackPrimitiveType type)
        {
            GL.BeginTransformFeedback((TransformFeedbackPrimitiveType)type.ConvertByName(typeof(TransformFeedbackPrimitiveType)));
        }
        public override void EndTransformFeedback()
        {
            GL.EndTransformFeedback();
        }
        public override void TransformFeedbackVaryings(int matId, string[] varNames)
        {
            GL.TransformFeedbackVaryings(matId, varNames.Length, varNames, TransformFeedbackMode.InterleavedAttribs);
        }
        #endregion

        #region Primitives
        public override void Cull(ECulling culling)
        {
            if (culling == ECulling.None)
                GL.Disable(EnableCap.CullFace);
            else
            {
                GL.Enable(EnableCap.CullFace);
                switch (culling)
                {
                    case ECulling.Back:
                        GL.CullFace(CullFaceMode.Back);
                        break;
                    case ECulling.Front:
                        GL.CullFace(CullFaceMode.Front);
                        break;
                    case ECulling.Both:
                        GL.CullFace(CullFaceMode.FrontAndBack);
                        break;
                }
            }
        }
        public override void MapBufferData(DataBuffer buffer)
        {
            //GL.BufferStorage(_target, _data.Length, _data.Address,
            //    BufferStorageFlags.MapWriteBit |
            //    BufferStorageFlags.MapReadBit |
            //    BufferStorageFlags.MapPersistentBit |
            //    BufferStorageFlags.MapCoherentBit |
            //    BufferStorageFlags.ClientStorageBit);
            //_data.Dispose();
            //_data = new DataSource(GL.MapBufferRange(_target, IntPtr.Zero, DataLength,
            //    BufferAccessMask.MapPersistentBit |
            //    BufferAccessMask.MapCoherentBit |
            //    BufferAccessMask.MapReadBit |
            //    BufferAccessMask.MapWriteBit), DataLength);
            
            int length = buffer._data.Length;
            GL.NamedBufferStorage(buffer.BindingId, length, buffer._data.Address,
                BufferStorageFlags.MapWriteBit |
                BufferStorageFlags.MapReadBit |
                BufferStorageFlags.MapPersistentBit |
                BufferStorageFlags.MapCoherentBit |
                BufferStorageFlags.ClientStorageBit);
            //buffer._data.Dispose();
            //buffer._data = new DataSource(GL.MapNamedBufferRange(buffer.BindingId, IntPtr.Zero, length,
            //    BufferAccessMask.MapPersistentBit |
            //    BufferAccessMask.MapCoherentBit |
            //    BufferAccessMask.MapReadBit |
            //    BufferAccessMask.MapWriteBit), length);
            //buffer._data = new DataSource(GL.MapNamedBuffer(buffer.BindingId, BufferAccess.ReadWrite), length);
        }
        /// <summary>
        /// Requires 4.5 or ARB_direct_state_access
        /// </summary>
        public override void UnmapBufferData(DataBuffer buffer)
        {
            //GL.UnmapBuffer(buffer._target);
            GL.UnmapNamedBuffer(buffer.BindingId);
        }
        public override void PushBufferData(DataBuffer buffer)
        {
            //GL.BufferData((BufferTarget)(int)buffer.Target, buffer.DataLength, buffer._data.Address, BufferUsageHint.StreamDraw + (int)buffer.Usage);
            GL.NamedBufferData(buffer.BindingId, buffer.DataLength, buffer._data.Address, BufferUsageHint.StreamDraw + (int)buffer.Usage);
        }
        public override void PushBufferSubData(DataBuffer buffer, int offset, int length)
        {
            GL.NamedBufferSubData(buffer.BindingId, (IntPtr)offset, length, buffer._data.Address);
        }
        /// <summary>
        /// Specifies attribute usage in a vertex shader.
        /// If divisor is 0, attribute advances per-vertex.
        /// If divisor is greater than 0, advances once per 'divisor' instances.
        /// </summary>
        public override void AttributeDivisor(int attributeLocation, int divisor)
        {
            GL.VertexAttribDivisor(attributeLocation, divisor);
        }
        public override void BindBufferBase(EBufferRangeTarget rangeTarget, int blockIndex, int bufferBindingId)
        {
            GL.BindBufferBase((BufferRangeTarget)(int)rangeTarget, blockIndex, bufferBindingId);
        }
        public override int GetUniformBlockIndex(int programBindingId, string name)
        {
            return GL.GetUniformBlockIndex(programBindingId, name);
        }
        public override void InitializeBuffer(DataBuffer buffer)
        {
            int glVer = 2;

            int index = buffer._location;
            int vaoId = buffer._vaoId;
            int componentType = (int)buffer._componentType;
            int componentCount = buffer._componentCount;
            bool integral = buffer._integral;
            switch (glVer)
            {
                case 0:

                    GL.BindBuffer((BufferTarget)(int)buffer.Target, buffer.BindingId);

                    if (buffer.Target == EBufferTarget.ArrayBuffer)
                    {
                        GL.EnableVertexAttribArray(index);
                        if (integral)
                            GL.VertexAttribIPointer(index, componentCount, VertexAttribIntegerType.Byte + componentType, 0, buffer._data.Address);
                        else
                            GL.VertexAttribPointer(index, componentCount, VertexAttribPointerType.Byte + componentType, buffer._normalize, 0, 0);
                    }

                    if (buffer.MapData)
                        MapBufferData(buffer);
                    else
                        PushBufferData(buffer);

                    break;

                case 1:

                    GL.BindVertexBuffer(index, buffer.BindingId, IntPtr.Zero, buffer.Stride);

                    if (buffer.Target == EBufferTarget.ArrayBuffer)
                    {
                        GL.EnableVertexAttribArray(index);
                        if (integral)
                            GL.VertexAttribIFormat(index, componentCount, VertexAttribIntegerType.Byte + componentType, 0);
                        else
                            GL.VertexAttribFormat(index, componentCount, VertexAttribType.Byte + componentType, buffer._normalize, 0);
                    }

                    if (buffer.MapData)
                        MapBufferData(buffer);
                    else
                        PushBufferData(buffer);

                    if (buffer.Target == EBufferTarget.ArrayBuffer)
                    {
                        GL.VertexAttribBinding(index, index);
                    }

                    break;

                case 2:

                    if (index >= 0)
                    {
                        if (buffer.Target == EBufferTarget.ArrayBuffer)
                        {
                            GL.EnableVertexArrayAttrib(vaoId, index);
                            if (integral)
                                GL.VertexArrayAttribIFormat(vaoId, index, componentCount, VertexAttribType.Byte + componentType, 0);
                            else
                                GL.VertexArrayAttribFormat(vaoId, index, componentCount, VertexAttribType.Byte + componentType, buffer._normalize, 0);
                        }
                        
                        if (buffer.MapData)
                            MapBufferData(buffer);
                        else
                            PushBufferData(buffer);

                        if (buffer.Target == EBufferTarget.ArrayBuffer)
                        {
                            GL.VertexArrayAttribBinding(vaoId, index, index);
                            GL.VertexArrayVertexBuffer(vaoId, index, buffer.BindingId, IntPtr.Zero, buffer.Stride);
                        }
                    }
                    else
                    {
                        if (buffer.MapData)
                            MapBufferData(buffer);
                        else
                            PushBufferData(buffer);
                    }

                    break;
            }
        }
        /// <summary>
        /// Requires 3.0 or ARB_vertex_array_object
        /// </summary>
        public override void BindPrimitiveManager(IPrimitiveManager manager)
        {
            GL.BindVertexArray(manager == null ? 0 : manager.BindingId);
            base.BindPrimitiveManager(manager);
        }
        /// <summary>
        /// Requires 4.5 or ARB_direct_state_access
        /// </summary>
        public override void LinkRenderIndices(IPrimitiveManager manager, DataBuffer indexBuffer)
        {
            if (indexBuffer.Target != EBufferTarget.ElementArrayBuffer)
                throw new Exception("IndexBuffer needs target type of " + EBufferTarget.ElementArrayBuffer.ToString() + ".");
            GL.VertexArrayElementBuffer(manager.BindingId, indexBuffer.BindingId);
        }
        /// <summary>
        /// Requires 1.1
        /// </summary>
        public override void RenderCurrentPrimitiveManager(int instances)
        {
            if (_currentPrimitiveManager != null)
            {
                PrimitiveType type = (PrimitiveType)(int)_currentPrimitiveManager.Data._type;
                int count = _currentPrimitiveManager.IndexBuffer.ElementCount;
                DrawElementsType elemType = DrawElementsType.UnsignedByte + (int)_currentPrimitiveManager.ElementType;
                //Engine.PrintLine("{0} {1} {2}", type.ToString(), count, elemType.ToString());
                //GL.DrawElements(type, count, elemType, 0);
                GL.DrawElementsInstancedBaseInstance(type, count, elemType, IntPtr.Zero, instances, 0);
                //GL.DrawElementsIndirect(ArbDrawIndirect.DrawIndirectBuffer, ArbDrawIndirect.DrawIndirectBuffer, IntPtr.Zero);
            }
        }
        #endregion

        #region Textures
        public override void TextureView(int bindingId, ETexTarget target, int origTextureId, EPixelInternalFormat fmt, int minLevel, int numLevels, int minLayer, int numLayers)
        {
            TextureTarget tt = (TextureTarget)target.ConvertByName(typeof(TextureTarget));
            PixelInternalFormat pit = (PixelInternalFormat)fmt.ConvertByName(typeof(PixelInternalFormat));
            GL.TextureView(bindingId, tt, origTextureId, pit, minLevel, numLevels, minLayer, numLayers);
        }
        public override void TexParameter(ETexTarget texTarget, ETexParamName texParam, float paramData)
        {
            GL.TexParameter(
                (TextureTarget)texTarget.ConvertByName(typeof(TextureTarget)),
                (TextureParameterName)texParam.ConvertByName(typeof(TextureParameterName)),
                paramData);
        }
        public override void TexParameter(ETexTarget texTarget, ETexParamName texParam, int paramData)
        {
            GL.TexParameter(
                (TextureTarget)texTarget.ConvertByName(typeof(TextureTarget)),
                (TextureParameterName)texParam.ConvertByName(typeof(TextureParameterName)),
                paramData);
        }

        public override void PushTextureData(
            ETexTarget texTarget,
            int mipLevel,
            EPixelInternalFormat internalFormat,
            int width,
            int height,
            EPixelFormat format,
            EPixelType type,
            VoidPtr data)
        {
            TextureTarget tt = (TextureTarget)(int)texTarget;
            PixelInternalFormat pit = (PixelInternalFormat)internalFormat.ConvertByName(typeof(PixelInternalFormat));
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)format.ConvertByName(typeof(OpenTK.Graphics.OpenGL.PixelFormat));
            PixelType pt = (PixelType)type.ConvertByName(typeof(PixelType));
            GL.TexImage2D(tt, mipLevel, pit, width, height, 0, pf, pt, data);
        }
        public override void PushTextureData<T>(
            ETexTarget texTarget,
            int mipLevel,
            EPixelInternalFormat internalFormat,
            int width,
            int height,
            EPixelFormat format,
            EPixelType type,
            T[] data)
        {
            TextureTarget tt = (TextureTarget)(int)texTarget;
            PixelInternalFormat pit = (PixelInternalFormat)internalFormat.ConvertByName(typeof(PixelInternalFormat));
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)format.ConvertByName(typeof(OpenTK.Graphics.OpenGL.PixelFormat));
            PixelType pt = (PixelType)type.ConvertByName(typeof(PixelType));
            GL.TexImage2D(tt, mipLevel, pit, width, height, 0, pf, pt, data);
        }
        public override void SetTextureStorage(
            ETexTarget2D texTarget,
            int mipLevels,
            ESizedInternalFormat internalFormat,
            int width,
            int height)
        {
            TextureTarget2d tt = (TextureTarget2d)texTarget.ConvertByName(typeof(TextureTarget2d));
            SizedInternalFormat it = (SizedInternalFormat)internalFormat.ConvertByName(typeof(SizedInternalFormat));
            GL.TexStorage2D(tt, mipLevels, it, width, height);
        }
        public override void SetTextureStorage(
            int bindingId,
            int mipLevels,
            ESizedInternalFormat internalFormat,
            int width,
            int height)
        {
            SizedInternalFormat it = (SizedInternalFormat)internalFormat.ConvertByName(typeof(SizedInternalFormat));
            GL.TextureStorage2D(bindingId, mipLevels, it, width, height);
        }
        public override void PushTextureSubData(
            ETexTarget texTarget,
            int mipLevel,
            int xOffset,
            int yOffset,
            int width,
            int height,
            EPixelFormat format,
            EPixelType type,
            VoidPtr data)
        {
            TextureTarget tt = (TextureTarget)(int)texTarget;
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)(int)format;
            PixelType pt = (PixelType)(int)type;
            GL.TexSubImage2D(tt, mipLevel, xOffset, yOffset, width, height, pf, pt, data);
        }
        public override void PushTextureSubData<T>(
            ETexTarget texTarget,
            int mipLevel,
            int xOffset,
            int yOffset,
            int width,
            int height,
            EPixelFormat format,
            EPixelType type,
            T[] data)
        {
            TextureTarget tt = (TextureTarget)(int)texTarget;
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)(int)format;
            PixelType pt = (PixelType)(int)type;
            GL.TexSubImage2D(tt, mipLevel, xOffset, yOffset, width, height, pf, pt, data);
        }
        public override void BindTexture(ETexTarget texTarget, int bindingId)
            => GL.BindTexture((TextureTarget)texTarget.ConvertByName(typeof(TextureTarget)), bindingId);
        public override void ClearTexImage(int bindingId, int level, EPixelFormat format, EPixelType type, VoidPtr clearColor)
        {
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)format.ConvertByName(typeof(OpenTK.Graphics.OpenGL.PixelFormat));
            PixelType pt = (PixelType)type.ConvertByName(typeof(PixelType));
            GL.ClearTexImage(bindingId, level, pf, pt, clearColor);
        }
        public override void GenerateMipmap(ETexTarget target)
            => GL.GenerateMipmap((GenerateMipmapTarget)(int)target);
        public override void GenerateMipmap(int textureBindingId)
            => GL.GenerateTextureMipmap(textureBindingId);
        public override void SetMipmapParams(int bindingId, int minLOD, int maxLOD, int largestMipmapLevel, int smallestAllowedMipmapLevel)
        {
            GL.TextureParameterI(bindingId, TextureParameterName.TextureBaseLevel, ref largestMipmapLevel);
            GL.TextureParameterI(bindingId, TextureParameterName.TextureMaxLevel, ref smallestAllowedMipmapLevel);
            GL.TextureParameterI(bindingId, TextureParameterName.TextureMinLod, ref minLOD);
            GL.TextureParameterI(bindingId, TextureParameterName.TextureMaxLod, ref maxLOD);
        }
        public override void SetMipmapParams(ETexTarget target, int minLOD, int maxLOD, int largestMipmapLevel, int smallestAllowedMipmapLevel)
        {
            TextureTarget t = (TextureTarget)(int)target;
            GL.TexParameterI(t, TextureParameterName.TextureBaseLevel, ref largestMipmapLevel);
            GL.TexParameterI(t, TextureParameterName.TextureMaxLevel, ref smallestAllowedMipmapLevel);
            GL.TexParameterI(t, TextureParameterName.TextureMinLod, ref minLOD);
            GL.TexParameterI(t, TextureParameterName.TextureMaxLod, ref maxLOD);
        }
        public override void GetTexImage<T>(ETexTarget target, int level, EPixelFormat pixelFormat, EPixelType pixelType, T[] pixels)
        {
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)(int)pixelFormat;
            PixelType pt = (PixelType)(int)pixelType;
            TextureTarget tt = (TextureTarget)(int)target;
            GL.GetTexImage(tt, level, pf, pt, pixels);
        }
        #endregion

        #region Blending Methods
        public override void BlendColor(ColorF4 color)
            => GL.BlendColor(color.R, color.G, color.B, color.A);
        public override void BlendFunc(EBlendingFactor srcFactor, EBlendingFactor destFactor)
            => GL.BlendFunc((BlendingFactor)(int)srcFactor, (BlendingFactor)(int)destFactor);
        public override void BlendFuncSeparate(EBlendingFactor srcFactorRGB, EBlendingFactor destFactorRGB, EBlendingFactor srcFactorAlpha, EBlendingFactor destFactorAlpha)
            => GL.BlendFuncSeparate((BlendingFactorSrc)(int)srcFactorRGB, (BlendingFactorDest)(int)destFactorRGB, (BlendingFactorSrc)(int)srcFactorAlpha, (BlendingFactorDest)(int)destFactorAlpha);
        public override void BlendEquation(EBlendEquationMode rgb, EBlendEquationMode alpha)
            => GL.BlendEquationSeparate((BlendEquationMode)(int)rgb, (BlendEquationMode)(int)alpha);
        public override void BlendEquationSeparate(EBlendEquationMode rgb, EBlendEquationMode alpha)
            => GL.BlendEquationSeparate((BlendEquationMode)(int)rgb, (BlendEquationMode)(int)alpha);
        #endregion

        #region Depth Methods
        public override void ClearDepth(float defaultDepth)
            => GL.ClearDepth(defaultDepth);
        public override void AllowDepthWrite(bool allow)
            => GL.DepthMask(allow);
        public override void DepthFunc(EComparison func)
            => GL.DepthFunc(GetDepthFunc(func));
        public override void DepthRange(double near, double far)
            => GL.DepthRange(near, far);
        private DepthFunction GetDepthFunc(EComparison comp)
        {
            switch (comp)
            {
                case EComparison.Never:
                    return DepthFunction.Never;
                case EComparison.Less:
                    return DepthFunction.Less;
                case EComparison.Equal:
                    return DepthFunction.Equal;
                case EComparison.Lequal:
                    return DepthFunction.Lequal;
                case EComparison.Greater:
                    return DepthFunction.Greater;
                case EComparison.Nequal:
                    return DepthFunction.Notequal;
                default:
                    return DepthFunction.Always;
            }
        }
        #endregion

        public override Bitmap GetScreenshot(Rectangle region, bool withTransparency)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            GL.ReadBuffer(ReadBufferMode.Front);
            Bitmap bmp = new Bitmap(region.Width, region.Height);
            BitmapData data;
            if (withTransparency)
            {
                data = bmp.LockBits(new Rectangle(0, 0, region.Width, region.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.ReadPixels(region.X, region.Y, region.Width, region.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            }
            else
            {
                data = bmp.LockBits(new Rectangle(0, 0, region.Width, region.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.ReadPixels(region.X, region.Y, region.Width, region.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            }
            bmp.UnlockBits(data);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        public override void ColorMask(bool r, bool g, bool b, bool a)
            => GL.ColorMask(r, g, b, a);

        public override void BeginConditionalRender(int queryObjectBindingId, EConditionalRenderType type)
        {
            GL.BeginConditionalRender(queryObjectBindingId, (ConditionalRenderType)(int)type);
        }
        public override void EndConditionalRender()
        {
            GL.EndConditionalRender();
        }

        #region Queries
        public override void BeginQuery(int bindingId, EQueryTarget target)
        {
            GL.BeginQuery((QueryTarget)(int)target, bindingId);
        }
        public override void EndQuery(EQueryTarget target)
        {
            GL.EndQuery((QueryTarget)(int)target);
        }
        public override int GetQueryObjectInt(int bindingId, EGetQueryObject obj)
        {
            GL.GetQueryObject(bindingId, (GetQueryObjectParam)(int)obj, out int val);
            return val;
        }
        public override long GetQueryObjectLong(int bindingId, EGetQueryObject obj)
        {
            GL.GetQueryObject(bindingId, (GetQueryObjectParam)(int)obj, out long val);
            return val;
        }
        public override void QueryCounter(int bindingId)
        {
            GL.QueryCounter(bindingId, QueryCounterTarget.Timestamp);
        }
        #endregion
    }
}
