using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;
using EnumsNET;

namespace TheraEngine.Rendering.OpenGL
{
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

        //public override void RenderLine(string name, Vec3 start, Vec3 end, ColorF4 color, float lineWidth = 5)
        //{
        //    //base.RenderLine(name, start, end, color, lineWidth);
        //    GL.EnableVertexArrayAttrib(vaoId, index);

        //    GL.VertexArrayAttribFormat(vaoId, index, componentCount, VertexAttribType.Byte + componentType, buffer._normalize, 0);
        //    GL.NamedBufferData(buffer.BindingId, buffer._componentCount, buffer._data.Address, BufferUsageHint.StreamDraw + (int)buffer._usage);
        //    GL.VertexArrayAttribBinding(vaoId, index, index);
        //    GL.VertexArrayVertexBuffer(vaoId, index, buffer.BindingId, IntPtr.Zero, buffer.Stride);
        //}
        //public override void RenderPoint(string name, Vec3 position, ColorF4 color, float pointSize = DefaultPointSize)
        //{
        //    SetPointSize(pointSize);
        //    UseProgram(null);

        //}
        //public override void RenderLineLoop(bool closedLoop, params Vec3[] points)
        //{

        //}
        //public override void RenderLineLoop(bool closedLoop, PropAnimVec3 points)
        //{

        //}

        public override void ClearColor(ColorF4 color)
        {
            GL.ClearColor(color.R, color.G, color.B, color.A);
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
                    throw new Exception("Call CreateQueries instead.");
                case EObjectType.Renderbuffer:
                    GL.CreateRenderbuffers(count, ids);
                    break;
                case EObjectType.Sampler:
                    GL.CreateSamplers(count, ids);
                    break;
                case EObjectType.Texture:
                    throw new Exception("Call CreateTextures instead.");
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
            GL.CreateTextures((TextureTarget)(int)target, count, ids);
            return ids;
        }
        public override int[] CreateQueries(int type, int count)
        {
            int[] ids = new int[count];
            GL.CreateQueries((QueryTarget)type, count, ids);
            return ids;
        }
        #endregion

        #region Shaders
        public override void SetShaderMode(ShaderMode type)
        {
            switch (type)
            {
                case ShaderMode.Fragment:
                    _currentShaderMode = ShaderType.FragmentShader;
                    break;
                case ShaderMode.Vertex:
                    _currentShaderMode = ShaderType.VertexShader;
                    break;
                case ShaderMode.Geometry:
                    _currentShaderMode = ShaderType.GeometryShader;
                    break;
                case ShaderMode.TessControl:
                    _currentShaderMode = ShaderType.TessControlShader;
                    break;
                case ShaderMode.TessEvaluation:
                    _currentShaderMode = ShaderType.TessEvaluationShader;
                    break;
                case ShaderMode.Compute:
                    _currentShaderMode = ShaderType.ComputeShader;
                    break;
            }
        }
        public override void SetBindFragDataLocation(int bindingId, int location, string name)
        {
            GL.BindFragDataLocation(bindingId, location, name);
        }
        public override int GenerateShader(params string[] source)
        {
            int handle = GL.CreateShader(_currentShaderMode);
            if (handle == 0)
            {
                Engine.LogWarning("GL.CreateShader did not return a valid binding id.");
                return 0;
            }

            foreach (string s in source)
                GL.ShaderSource(handle, s);
            GL.CompileShader(handle);

#if DEBUG
            GL.GetShader(handle, ShaderParameter.CompileStatus, out int status);
            if (status == 0 || ForceShaderOutput)
            {
                GL.GetShaderInfoLog(handle, out string info);

                if (string.IsNullOrEmpty(info) && status == 0)
                    Engine.LogWarning("Unable to compile shader, but no error was returned.");
                else
                {
                    Engine.LogWarning(info);
                    for (int i = 0; i < source.Length; ++i)
                    {
                        Engine.PrintLine("\n\nSource{0}\n", i.ToString());

                        //Split the source by new lines
                        string[] s = source[i].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                        //Add the line number to the source so we can go right to errors on specific lines
                        int lineNumber = 1;
                        foreach (string line in s)
                            Engine.PrintLine(string.Format("{0}: {1}", (lineNumber++).ToString().PadLeft(s.Length.ToString().Length, '0'), line));
                    }
                    Engine.PrintLine("\n\n");
                }
            }
#endif
            return handle;
        }
        public override void SetProgramParameter(int programBindingId, EProgParam parameter, int value)
            => GL.ProgramParameter(programBindingId, (ProgramParameterName)(int)parameter, value);
        public override void BindPipeline(int pipelineBindingId)
        {
            GL.BindProgramPipeline(pipelineBindingId);
        }
        public override void SetPipelineStage(int pipelineBindingId, EProgramStageMask mask, int programBindingId)
        {
            GL.UseProgramStages(pipelineBindingId, (ProgramStageMask)(int)mask, programBindingId);
        }
        public override void ActiveShaderProgram(int pipelineBindingId, int programBindingId)
        {
            GL.ActiveShaderProgram(pipelineBindingId, programBindingId);
        }
        public override int GenerateProgram(int[] shaderHandles, bool separable)
        {
            int handle = GL.CreateProgram();
            GL.ProgramParameter(handle, ProgramParameterName.ProgramSeparable, separable ? 1 : 0);

            foreach (int i in shaderHandles)
                GL.AttachShader(handle, i);
            
            GL.LinkProgram(handle);
            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                GL.GetProgramInfoLog(handle, out string info);
                if (string.IsNullOrEmpty(info))
                    Engine.PrintLine("Unable to link program, but no error was returned.");
                else
                    Engine.PrintLine(info);
            }

            //We don't need these anymore now that they're part of the program
            foreach (int i in shaderHandles)
            {
                GL.DetachShader(handle, i);
                GL.DeleteShader(i);
            }
            
            return handle;
        }
        public override void SetActiveTexture(int unit)
        {
            if (unit < 0)
                throw new InvalidOperationException("Unit needs to be >= 0.");
            if (unit >= Engine.MaxTextureUnits)
                throw new InvalidOperationException("Unit needs to be less than " + Engine.MaxTextureUnits.ToString());
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
        }
        public override void UseProgram(int programBindingId)
        {
            GL.UseProgram(programBindingId);
        }
        public override void ApplyRenderParams(RenderingParameters r)
        {
            Engine.Renderer.ColorMask(r.WriteRed, r.WriteGreen, r.WriteBlue, r.WriteAlpha);
            Engine.Renderer.Cull(r.CullMode);

            if (r.DepthTest.Enabled)
            {
                GL.Enable(EnableCap.DepthTest);
                DepthFunc(r.DepthTest.Function);
                GL.DepthMask(r.DepthTest.UpdateDepth);
            }
            else
                GL.Disable(EnableCap.DepthTest);

            if (r.Blend.Enabled)
            {
                GL.Enable(EnableCap.Blend);
                BlendEquation(r.Blend.RgbEquation, r.Blend.AlphaEquation);
                BlendFuncSeparate(r.Blend.RgbSrcFactor, r.Blend.RgbDstFactor, r.Blend.AlphaSrcFactor, r.Blend.AlphaDstFactor);
            }
            else
                GL.Disable(EnableCap.Blend);

            GL.PointSize(r.PointSize);
            GL.LineWidth(r.LineWidth);
        }

        public override int GetAttribLocation(int programBindingId, string name)
        {
            return GL.GetAttribLocation(programBindingId, name);
        }
        public override int GetUniformLocation(int programBindingId, string name)
        {
            return GL.GetUniformLocation(programBindingId, name);
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
        public override void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel)
        {
            GL.NamedFramebufferTexture(frameBufferBindingId, (FramebufferAttachment)(int)attachment, textureBindingId, mipLevel);
        }
        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, ETexTarget texTarget, int textureBindingId, int mipLevel)
        {
            GL.FramebufferTexture2D((FramebufferTarget)(int)target, (FramebufferAttachment)(int)attachment, (TextureTarget)(int)texTarget, textureBindingId, mipLevel);
        }
        public override void SetDrawBuffer(EDrawBuffersAttachment attachment)
        {
            GL.DrawBuffer((DrawBufferMode)(int)attachment);
        }
        public override void SetDrawBuffer(int bindingId, EDrawBuffersAttachment attachment)
        {
            GL.NamedFramebufferDrawBuffer(bindingId, (DrawBufferMode)(int)attachment);
        }
        public override void SetReadBuffer(EDrawBuffersAttachment attachment)
        {
            GL.ReadBuffer((ReadBufferMode)(int)attachment);
        }
        public override void SetReadBuffer(int bindingId, EDrawBuffersAttachment attachment)
        {
            GL.NamedFramebufferReadBuffer(bindingId, (ReadBufferMode)(int)attachment);
        }
        public override void SetDrawBuffers(EDrawBuffersAttachment[] attachments)
        {
            GL.DrawBuffers(attachments.Length, attachments.Select(x => (DrawBuffersEnum)(int)x).ToArray());
        }
        public override void SetDrawBuffers(int bindingId, EDrawBuffersAttachment[] attachments)
        {
            GL.NamedFramebufferDrawBuffers(bindingId, attachments.Length, attachments.Select(x => (DrawBuffersEnum)(int)x).ToArray());
        }
        public override void BindRenderBuffer(int bindingId)
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, bindingId);
        }
        public override void RenderbufferStorage(ERenderBufferStorage storage, int width, int height)
        {
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)(int)storage, width, height);
        }
        public override void FramebufferRenderBuffer(EFramebufferTarget target, EFramebufferAttachment attachement, int renderBufferBindingId)
        {
            GL.FramebufferRenderbuffer((FramebufferTarget)(int)target, (FramebufferAttachment)(int)attachement, RenderbufferTarget.Renderbuffer, renderBufferBindingId);
        }
        public override void FramebufferRenderBuffer(int frameBufferBindingId, EFramebufferAttachment attachement, int renderBufferBindingId)
        {
            GL.NamedFramebufferRenderbuffer(frameBufferBindingId, (FramebufferAttachment)(int)attachement, RenderbufferTarget.Renderbuffer, renderBufferBindingId);
        }
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

        public override int GetStencilIndex(float x, float y)
        {
            int val = 0;
            GL.ReadPixels((int)x, (int)(RenderPanel.RenderingPanel.Height - y), 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthStencil, PixelType.UnsignedInt248, ref val);
            return val & 0xFF;
        }
        public override float GetDepth(float x, float y)
        {
            //GL.ReadBuffer(ReadBufferMode.FrontAndBack);

            //Viewport v = Engine.CurrentPanel.GetViewport(0);
            //float[] pixels = new float[(int)v.Width * (int)v.Height];
            //v._gBuffer.Textures[4].Bind();
            //GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, pixels);
            //return pixels[(int)y * (int)v.Width + (int)x];

            float val = 0;
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
        
        //public override void UniformMaterial(int matID, int location, params IUniformable4Int[] p)
        //{
        //    const int count = 4;

        //    if (location < 0)
        //        return;

        //    float[] values = new float[p.Length << 2];

        //    for (int i = 0; i < p.Length; ++i)
        //        for (int x = 0; x < count; ++x)
        //            values[i << 2 + x] = p[i].Data[x];

        //    GL.ProgramUniform4(matID, location, p.Length, values);
        //}
        
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
        public override void Cull(Culling culling)
        {
            if (culling == Culling.None)
                GL.Disable(EnableCap.CullFace);
            else
            {
                GL.Enable(EnableCap.CullFace);
                switch (culling)
                {
                    case Culling.Back:
                        GL.CullFace(CullFaceMode.Back);
                        break;
                    case Culling.Front:
                        GL.CullFace(CullFaceMode.Front);
                        break;
                    case Culling.Both:
                        GL.CullFace(CullFaceMode.FrontAndBack);
                        break;
                }
            }
        }
        public override void MapBufferData(VertexBuffer buffer)
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
            buffer._data.Dispose();
            buffer._data = new DataSource(GL.MapNamedBufferRange(buffer.BindingId, IntPtr.Zero, length,
                BufferAccessMask.MapPersistentBit |
                BufferAccessMask.MapCoherentBit |
                BufferAccessMask.MapReadBit |
                BufferAccessMask.MapWriteBit), length);
            //buffer._data = new DataSource(GL.MapNamedBuffer(buffer.BindingId, BufferAccess.ReadWrite), length);
        }
        public override void PushBufferData(VertexBuffer buffer)
        {
            //GL.BufferData(_target, (IntPtr)_data.Length, _data.Address, BufferUsageHint.StaticDraw);
            GL.NamedBufferData(buffer.BindingId, buffer._componentCount, buffer._data.Address, BufferUsageHint.StreamDraw + (int)buffer._usage);
        }
        public override void InitializeBuffer(VertexBuffer buffer)
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

                    GL.BindBuffer(buffer._target == EBufferTarget.DataArray ? BufferTarget.ArrayBuffer : BufferTarget.ElementArrayBuffer, buffer.BindingId);
                    GL.EnableVertexAttribArray(index);
                    if (integral)
                        GL.VertexAttribIPointer(index, componentCount, VertexAttribIntegerType.Byte + componentType, 0, buffer._data.Address);
                    else
                        GL.VertexAttribPointer(index, componentCount, VertexAttribPointerType.Byte + componentType, buffer._normalize, 0, 0);

                    if (VertexBuffer.MapData)
                        MapBufferData(buffer);
                    else
                        PushBufferData(buffer);

                    break;

                case 1:

                    GL.BindVertexBuffer(index, buffer.BindingId, IntPtr.Zero, buffer.Stride);
                    GL.EnableVertexAttribArray(index);
                    if (integral)
                        GL.VertexAttribIFormat(index, componentCount, VertexAttribIntegerType.Byte + componentType, 0);
                    else
                        GL.VertexAttribFormat(index, componentCount, VertexAttribType.Byte + componentType, buffer._normalize, 0);

                    if (VertexBuffer.MapData)
                        MapBufferData(buffer);
                    else
                        PushBufferData(buffer);

                    GL.VertexAttribBinding(index, index);

                    break;

                case 2:

                    GL.EnableVertexArrayAttrib(vaoId, index);
                    if (integral)
                        GL.VertexArrayAttribIFormat(vaoId, index, componentCount, VertexAttribType.Byte + componentType, 0);
                    else
                        GL.VertexArrayAttribFormat(vaoId, index, componentCount, VertexAttribType.Byte + componentType, buffer._normalize, 0);

                    if (VertexBuffer.MapData)
                        MapBufferData(buffer);
                    else
                        PushBufferData(buffer);

                    GL.VertexArrayAttribBinding(vaoId, index, index);
                    GL.VertexArrayVertexBuffer(vaoId, index, buffer.BindingId, IntPtr.Zero, buffer.Stride);

                    break;
            }
        }
        /// <summary>
        /// Requires 4.5 or ARB_direct_state_access
        /// </summary>
        public override void UnmapBufferData(VertexBuffer buffer)
        {
            //GL.UnmapBuffer(buffer._target);
            GL.UnmapNamedBuffer(buffer.BindingId);
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
        public override void LinkRenderIndices(IPrimitiveManager manager, VertexBuffer indexBuffer)
        {
            if (indexBuffer._target != EBufferTarget.DrawIndices)
                throw new Exception("IndexBuffer needs target type of " + EBufferTarget.DrawIndices.ToString() + ".");
            GL.VertexArrayElementBuffer(manager.BindingId, indexBuffer.BindingId);
        }
        /// <summary>
        /// Requires 1.1
        /// </summary>
        public override void RenderCurrentPrimitiveManager()
        {
            if (_currentPrimitiveManager != null)
                GL.DrawElements(
                    PrimitiveType.Points + (int)_currentPrimitiveManager.Data._type,
                    _currentPrimitiveManager.IndexBuffer.ElementCount,
                    DrawElementsType.UnsignedByte + (int)_currentPrimitiveManager.ElementType,
                    0);

        }
        #endregion

        public override Bitmap GetScreenshot(Rectangle region, bool withTransparency)
        {
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
            TextureTarget tt = (TextureTarget)texTarget.ConvertByName(typeof(TextureTarget));
            PixelInternalFormat pit = (PixelInternalFormat)internalFormat.ConvertByName(typeof(PixelInternalFormat));
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)format.ConvertByName(typeof(OpenTK.Graphics.OpenGL.PixelFormat));
            PixelType pt = (PixelType)type.ConvertByName(typeof(PixelType));
            GL.TexImage2D(tt, mipLevel, pit, width, height, 0, pf, pt, data);
        }
        public override void PushTextureData(
            ETexTarget texTarget,
            int mipLevel,
            EPixelInternalFormat internalFormat,
            int width,
            int height,
            EPixelFormat format,
            EPixelType type,
            byte[] data)
        {
            TextureTarget tt = (TextureTarget)texTarget.ConvertByName(typeof(TextureTarget));
            PixelInternalFormat pit = (PixelInternalFormat)internalFormat.ConvertByName(typeof(PixelInternalFormat));
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)format.ConvertByName(typeof(OpenTK.Graphics.OpenGL.PixelFormat));
            PixelType pt = (PixelType)type.ConvertByName(typeof(PixelType));
            GL.TexImage2D(tt, mipLevel, pit, width, height, 0, pf, pt, data);
        }

        public override void BindTexture(ETexTarget texTarget, int bindingId)
            => GL.BindTexture((TextureTarget)texTarget.ConvertByName(typeof(TextureTarget)), bindingId);

        #region Blending Methods
        public override void BlendColor(ColorF4 color)
            => GL.BlendColor(color.R, color.G, color.B, color.A);
        public override void BlendFunc(EBlendingFactor srcFactor, EBlendingFactor destFactor)
            => GL.BlendFunc((BlendingFactorSrc)(int)srcFactor, (BlendingFactorDest)(int)destFactor);
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

        public override void CheckFrameBufferErrors()
        {
            FramebufferErrorCode c = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (c != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Problem compiling framebuffer: " + c.ToString());
        }

        public override void ClearTexImage(int bindingId, int level, EPixelFormat format, EPixelType type, VoidPtr clearColor)
        {
            OpenTK.Graphics.OpenGL.PixelFormat pf = (OpenTK.Graphics.OpenGL.PixelFormat)format.ConvertByName(typeof(OpenTK.Graphics.OpenGL.PixelFormat));
            PixelType pt = (PixelType)type.ConvertByName(typeof(PixelType));
            GL.ClearTexImage(bindingId, level, pf, pt, clearColor);
        }

        public override void ColorMask(bool r, bool g, bool b, bool a)
        {
            GL.ColorMask(r, g, b, a);
        }

        public override void GenerateMipmap(ETexTarget target)
        {
            GL.GenerateMipmap((GenerateMipmapTarget)target.ConvertByName(typeof(GenerateMipmapTarget)));
        }
        public override void GenerateTextureMipmap(int textureBindingId)
        {
            GL.GenerateTextureMipmap(textureBindingId);
        }
    }
}
