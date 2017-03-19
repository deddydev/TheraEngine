using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace CustomEngine.Rendering.OpenGL
{
    public unsafe class GLRenderer : AbstractRenderer
    {
        public const bool ForceShaderOutput = false;

        // https://www.opengl.org/wiki/Rendering_Pipeline_Overview

        public override RenderLibrary RenderLibrary => RenderLibrary.OpenGL;

        public GLRenderer() { }

        private ShaderType _currentShaderMode;
        
        public override void BindTextureData(int textureTargetEnum, int mipLevel, int pixelInternalFormatEnum, int width, int height, int pixelFormatEnum, int pixelTypeEnum, VoidPtr data)
        {
            // https://www.opengl.org/sdk/docs/man/html/glTexImage2D.xhtml
            GL.TexImage2D((TextureTarget)textureTargetEnum, mipLevel, (OpenTK.Graphics.OpenGL.PixelInternalFormat)pixelInternalFormatEnum, width, height, 0, (OpenTK.Graphics.OpenGL.PixelFormat)pixelFormatEnum, (PixelType)pixelTypeEnum, data);
        }
        public override void Clear(BufferClear mask)
        {
            ClearBufferMask newMask = 0;
            if (mask.HasFlag(BufferClear.Color))
                newMask |= ClearBufferMask.ColorBufferBit;
            if (mask.HasFlag(BufferClear.Depth))
                newMask |= ClearBufferMask.DepthBufferBit;
            if (mask.HasFlag(BufferClear.Stencil))
                newMask |= ClearBufferMask.StencilBufferBit;
            if (mask.HasFlag(BufferClear.Accum))
                newMask |= ClearBufferMask.AccumBufferBit;
            GL.Clear(newMask);
        }

        public override void SetPointSize(float size) => GL.PointSize(size);
        public override void SetLineSize(float size) => GL.LineWidth(size);

        #region Objects
        public override void DeleteObject(GenType type, int bindingId)
        {
            switch (type)
            {
                case GenType.Buffer:
                    GL.DeleteBuffer(bindingId);
                    break;
                case GenType.Framebuffer:
                    GL.DeleteFramebuffer(bindingId);
                    break;
                case GenType.Program:
                    GL.DeleteProgram(bindingId);
                    break;
                case GenType.ProgramPipeline:
                    GL.DeleteProgramPipeline(bindingId);
                    break;
                case GenType.Query:
                    GL.DeleteQuery(bindingId);
                    break;
                case GenType.Renderbuffer:
                    GL.DeleteRenderbuffer(bindingId);
                    break;
                case GenType.Sampler:
                    GL.DeleteSampler(bindingId);
                    break;
                case GenType.Texture:
                    GL.DeleteTexture(bindingId);
                    break;
                case GenType.TransformFeedback:
                    GL.DeleteTransformFeedback(bindingId);
                    break;
                case GenType.VertexArray:
                    GL.DeleteVertexArray(bindingId);
                    break;
                case GenType.Shader:
                    GL.DeleteShader(bindingId);
                    break;
            }
        }
        public override void DeleteObjects(GenType type, int[] bindingIds)
        {
            switch (type)
            {
                case GenType.Buffer:
                    GL.DeleteBuffers(bindingIds.Length, bindingIds);
                    break;
                case GenType.Framebuffer:
                    GL.DeleteFramebuffers(bindingIds.Length, bindingIds);
                    break;
                case GenType.Program:
                    foreach (int i in bindingIds)
                        GL.DeleteProgram(i);
                    break;
                case GenType.ProgramPipeline:
                    GL.DeleteProgramPipelines(bindingIds.Length, bindingIds);
                    break;
                case GenType.Query:
                    GL.DeleteQueries(bindingIds.Length, bindingIds);
                    break;
                case GenType.Renderbuffer:
                    GL.DeleteRenderbuffers(bindingIds.Length, bindingIds);
                    break;
                case GenType.Sampler:
                    GL.DeleteSamplers(bindingIds.Length, bindingIds);
                    break;
                case GenType.Texture:
                    GL.DeleteTextures(bindingIds.Length, bindingIds);
                    break;
                case GenType.TransformFeedback:
                    GL.DeleteTransformFeedbacks(bindingIds.Length, bindingIds);
                    break;
                case GenType.VertexArray:
                    GL.DeleteVertexArrays(bindingIds.Length, bindingIds);
                    break;
                case GenType.Shader:
                    foreach (int i in bindingIds)
                        GL.DeleteShader(i);
                    break;
            }
        }
        public override int[] CreateObjects(GenType type, int count)
        {
            int[] ids = new int[count];
            switch (type)
            {
                case GenType.Buffer:
                    GL.CreateBuffers(count, ids);
                    break;
                case GenType.Framebuffer:
                    GL.CreateFramebuffers(count, ids);
                    break;
                case GenType.Program:
                    for (int i = 0; i < count; ++i)
                        ids[i] = GL.CreateProgram();
                    break;
                case GenType.ProgramPipeline:
                    GL.CreateProgramPipelines(count, ids);
                    break;
                case GenType.Query:
                    throw new Exception("Call CreateQueries instead.");
                case GenType.Renderbuffer:
                    GL.CreateRenderbuffers(count, ids);
                    break;
                case GenType.Sampler:
                    GL.CreateSamplers(count, ids);
                    break;
                case GenType.Texture:
                    throw new Exception("Call CreateTextures instead.");
                case GenType.TransformFeedback:
                    GL.CreateTransformFeedbacks(count, ids);
                    break;
                case GenType.VertexArray:
                    GL.CreateVertexArrays(count, ids);
                    break;
                case GenType.Shader:
                    for (int i = 0; i < count; ++i)
                        ids[i] = GL.CreateShader(_currentShaderMode);
                    break;
            }
            return ids;
        }
        public override int[] CreateTextures(int target, int count)
        {
            int[] ids = new int[count];
            GL.CreateTextures((TextureTarget)target, count, ids);
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
        public override int GenerateShader(string source)
        {
            int handle = GL.CreateShader(_currentShaderMode);
            GL.ShaderSource(handle, source);
            GL.CompileShader(handle);
#if DEBUG
            GL.GetShader(handle, ShaderParameter.CompileStatus, out int status);
            if (status == 0 || ForceShaderOutput)
            {
                GL.GetShaderInfoLog(handle, out string info);

                if (string.IsNullOrEmpty(info))
                    info = source;

                Debug.WriteLine(info + "\n\n");

                //Split the source by new lines
                string[] s = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                //Add the line number to the source so we can go right to errors on specific lines
                int lineNumber = 1;
                foreach (string line in s)
                    Debug.WriteLine(string.Format("{0}: {1}", (lineNumber++).ToString().PadLeft(s.Length.ToString().Length, '0'), line));

                Debug.WriteLine("\n\n");
            }
#endif
            return handle;
        }
        public override int GenerateProgram(int[] shaderHandles, PrimitiveBufferInfo info)
        {
            int handle = GL.CreateProgram();
            foreach (int i in shaderHandles)
                GL.AttachShader(handle, i);

            //Have to bind 'in' attributes before linking
            int j = (int)BufferType.Position * VertexBuffer.MaxBufferCountPerType;
            for (int i = 0; i < info._morphCount + 1; ++i, ++j)
                GL.BindAttribLocation(handle, j, "Position" + i);

            if (info.HasNormals)
            {
                j = (int)BufferType.Normal * VertexBuffer.MaxBufferCountPerType;
                for (int i = 0; i < info._morphCount + 1; ++i, ++j)
                    GL.BindAttribLocation(handle, j, "Normal" + i);
            }
            if (info.HasBinormals)
            {
                j = (int)BufferType.Binormal * VertexBuffer.MaxBufferCountPerType;
                for (int i = 0; i < info._morphCount + 1; ++i, ++j)
                    GL.BindAttribLocation(handle, j, "Binormal" + i);
            }
            if (info.HasTangents)
            {
                j = (int)BufferType.Tangent * VertexBuffer.MaxBufferCountPerType;
                for (int i = 0; i < info._morphCount + 1; ++i, ++j)
                    GL.BindAttribLocation(handle, j, "Tangent" + i);
            }
            j = (int)BufferType.Color * VertexBuffer.MaxBufferCountPerType;
            for (int i = 0; i < info._colorCount; ++i, ++j)
                GL.BindAttribLocation(handle, j, "Color" + i);

            j = (int)BufferType.TexCoord * VertexBuffer.MaxBufferCountPerType;
            for (int i = 0; i < info._texcoordCount; ++i, ++j)
                GL.BindAttribLocation(handle, j, "TexCoord" + i);

            if (info.IsWeighted)
            {
                j = (int)BufferType.MatrixIds * VertexBuffer.MaxBufferCountPerType;
                for (int i = 0; i < info._morphCount + 1; ++i, ++j)
                    GL.BindAttribLocation(handle, j, "MatrixIds" + i);

                j = (int)BufferType.MatrixWeights * VertexBuffer.MaxBufferCountPerType;
                for (int i = 0; i < info._morphCount + 1; ++i, ++j)
                    GL.BindAttribLocation(handle, j, "MatrixWeights" + i);
            }

            if (info._hasBarycentricCoord)
            {
                j = (int)BufferType.Barycentric * VertexBuffer.MaxBufferCountPerType;
                GL.BindAttribLocation(handle, j++, "Barycentric");
            }

            GL.LinkProgram(handle);

            //We don't need these anymore now that they're part of the program
            foreach (int i in shaderHandles)
            {
                GL.DetachShader(handle, i);
                GL.DeleteShader(i);
            }

            return handle;
        }
        public override void UseProgram(MeshProgram program)
        {
            GL.UseProgram(program != null ? program.BindingId : BaseRenderState.NullBindingId);
            base.UseProgram(program);
            if (_currentMeshProgram != null)
            {
                if (_currentMeshProgram.Textures.Length > 0)
                {
                    GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
                    for (int i = 0; i < _currentMeshProgram.Textures.Length; ++i)
                    {
                        GL.ActiveTexture(TextureUnit.Texture0 + i);
                        Uniform("Texture" + i, i);
                        _currentMeshProgram.Textures[i].Bind();
                    }
                }
                else
                    GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
            }
        }

        #region Uniforms
        public override int GetAttribLocation(string name)
        {
            return GL.GetAttribLocation(_currentMeshProgram.BindingId, name);
        }
        public override int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(_currentMeshProgram.BindingId, name);
        }
        public override void Uniform(int location, params IUniformable4Int[] p)
        {
            const int count = 4;
            
            if (location < 0)
                return;

            int[] values = new int[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[(i << 2) + x] = p[i].Data[x];

            GL.Uniform4(location, p.Length, values);
        }
        public override void Uniform(int location, params IUniformable4Float[] p)
        {
            const int count = 4;
            
            if (location < 0)
                return;

            float[] values = new float[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[(i << 2) + x] = p[i].Data[x];

            GL.Uniform4(location, p.Length, values);
        }
        public override void Uniform(int location, params IUniformable3Int[] p)
        {
            const int count = 3;
            
            if (location < 0)
                return;

            int[] values = new int[p.Length * 3];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i * 3 + x] = p[i].Data[x];

            GL.Uniform3(location, p.Length, values);
        }
        public override void Uniform(int location, params IUniformable3Float[] p)
        {
            const int count = 3;
            
            if (location < 0)
                return;

            float[] values = new float[p.Length * 3];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i * 3 + x] = p[i].Data[x];

            GL.Uniform3(location, p.Length, values);
        }
        public override void Uniform(int location, params IUniformable2Int[] p)
        {
            const int count = 2;
            
            if (location < 0)
                return;

            int[] values = new int[p.Length << 1];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[(i << 1) + x] = p[i].Data[x];

            GL.Uniform2(location, p.Length, values);
        }
        public override void Uniform(int location, params IUniformable2Float[] p)
        {
            const int count = 2;
            
            if (location < 0)
                return;

            float[] values = new float[p.Length << 1];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[(i << 1) + x] = p[i].Data[x];

            GL.Uniform2(location, p.Length, values);
        }
        public override void Uniform(int location, params IUniformable1Int[] p)
        {
            if (location > -1)
                GL.Uniform1(location, p.Length, p.Select(x => *x.Data).ToArray());
        }
        public override void Uniform(int location, params IUniformable1Float[] p)
        {
            if (location > -1)
                GL.Uniform1(location, p.Length, p.Select(x => *x.Data).ToArray());
        }
        public override void Uniform(int location, params int[] p)
        {
            if (location > -1)
                GL.Uniform1(location, p.Length, p);
        }
        public override void Uniform(int location, params float[] p)
        {
            if (location > -1)
                GL.Uniform1(location, p.Length, p);
        }
        public override void Uniform(int location, Matrix4 p)
        {
            if (location > -1)
                GL.UniformMatrix4(location, 1, false, p.Data);
        }
        public override void Uniform(int location, params Matrix4[] p)
        {
            if (location > -1)
            {
                float[] values = new float[p.Length << 4];
                for (int i = 0; i < p.Length; ++i)
                    for (int x = 0; x < 16; ++x)
                        values[(i << 4) + x] = p[i].Data[x];
                GL.UniformMatrix4(location, p.Length, false, values);
            }
        }
        public override void Uniform(int location, Matrix3 p)
        {
            if (location > -1)
                GL.UniformMatrix3(location, 1, false, p.Data);
        }
        public override void Uniform(int location, params Matrix3[] p)
        {
            if (location > -1)
            {
                float[] values = new float[p.Length * 9];
                for (int i = 0; i < p.Length; ++i)
                    for (int x = 0; x < 9; ++x)
                        values[i * 9 + x] = p[i].Data[x];
                GL.UniformMatrix3(location, p.Length, false, values);
            }
        }
        #endregion

        #endregion

        public override void DrawBuffers(DrawBuffersAttachment[] attachments)
        {
            GL.DrawBuffers(attachments.Length, attachments.Select(x => (DrawBuffersEnum)x.Convert(typeof(DrawBuffersEnum))).ToArray());
        }

        public override float GetDepth(float x, float y)
        {
            float val = 0;
            GL.ReadPixels((int)x, (int)(Engine.CurrentPanel.Height - y), 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, ref val);
            return val;
        }
        protected override void SetRenderArea(Rectangle region)
        {
            GL.Viewport(region);
        }
        public override void CropRenderArea(Rectangle region)
        {
            GL.Scissor(region.X, region.Y, region.Width, region.Height);
        }
        public override void BindFrameBuffer(FramebufferType type, int bindingId)
        {
            switch (type)
            {
                case FramebufferType.ReadWrite:
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, bindingId);
                    break;
                case FramebufferType.Read:
                    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, bindingId);
                    break;
                case FramebufferType.Write:
                    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, bindingId);
                    break;
            }
        }

        public override void UniformMaterial(int matID, int location, params IUniformable4Int[] p)
        {
            const int count = 4;

            if (location < 0)
                return;

            float[] values = new float[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 2 + x] = p[i].Data[x];

            GL.ProgramUniform4(matID, location, p.Length, values);
        }

        public override void UniformMaterial(int matID, int location, params IUniformable4Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params IUniformable3Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params IUniformable3Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params IUniformable2Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params IUniformable2Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params IUniformable1Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params IUniformable1Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params int[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params float[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, Matrix4 p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params Matrix4[] p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, Matrix3 p)
        {
            throw new NotImplementedException();
        }

        public override void UniformMaterial(int matID, int location, params Matrix3[] p)
        {
            throw new NotImplementedException();
        }

        public override void BindTransformFeedback(int bindingId)
        {
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, bindingId);
        }
        public override void BeginTransformFeedback(FeedbackPrimitiveType type)
        {
            GL.BeginTransformFeedback((TransformFeedbackPrimitiveType)type.Convert(typeof(TransformFeedbackPrimitiveType)));
        }
        public override void EndTransformFeedback()
        {
            GL.EndTransformFeedback();
        }
        public override void TransformFeedbackVaryings(int matId, string[] varNames)
        {
            GL.TransformFeedbackVaryings(matId, varNames.Length, varNames, TransformFeedbackMode.InterleavedAttribs);
        }

        public override void Cull(Culling culling)
        {
            if (culling == Culling.None)
                GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.CullFace);
            else
            {
                GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.CullFace);
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
        }
        public override void PushBufferData(VertexBuffer buffer)
        {
            //GL.BufferData(_target, (IntPtr)_data.Length, _data.Address, BufferUsageHint.StaticDraw);
            GL.NamedBufferData(buffer.BindingId, buffer._componentCount, buffer._data.Address, BufferUsageHint.StreamDraw + (int)buffer._usage);
        }

        public override void InitializeBuffer(VertexBuffer buffer)
        {
            int glVer = 2;

            int index = buffer._index;
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

        public override void UnmapBufferData(VertexBuffer buffer)
        {
            //GL.UnmapBuffer(buffer._target);
            GL.UnmapNamedBuffer(buffer.BindingId);
        }

        public override void BindPrimitiveManager(PrimitiveManager manager)
        {
            GL.BindVertexArray(manager == null ? 0 : manager.BindingId);
            base.BindPrimitiveManager(manager);
        }

        /// <summary>
        /// REQUIRES OPENGL V4.5
        /// </summary>
        public override void LinkRenderIndices(PrimitiveManager manager, VertexBuffer indexBuffer)
        {
            if (indexBuffer._target != EBufferTarget.DrawIndices)
                throw new Exception("IndexBuffer needs target type of " + EBufferTarget.DrawIndices.ToString() + ".");
            GL.VertexArrayElementBuffer(manager.BindingId, indexBuffer.BindingId);
        }

        public override void RenderCurrentPrimitiveManager()
        {
            if (_currentPrimitiveManager != null)
                GL.DrawElements(
                    PrimitiveType.Points + (int)_currentPrimitiveManager.Data._type,
                    _currentPrimitiveManager._indexBuffer.ElementCount,
                    DrawElementsType.UnsignedByte + (int)_currentPrimitiveManager._elementType,
                    0);

        }

        public override Bitmap GetScreenshot(Rectangle region, bool withTransparency)
        {
            GL.ReadBuffer(ReadBufferMode.Back);
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

        internal override void DrawText(ScreenTextHandler text)
        {
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
            GL.Enable(OpenTK.Graphics.OpenGL.EnableCap.Blend);

            Bitmap b = text._bitmap;
            Viewport vp = text._viewport;
            int texId = text._texId;
            IVec2 size = text._size;

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Replace);

            if (size != (IVec2)vp.Region.Bounds ||
                vp.Region.Bounds.X.IsZero() ||
                vp.Region.Bounds.Y.IsZero())
            {
                size = (IVec2)vp.Region.Bounds;

                if (b != null)
                    b.Dispose();
                if (texId != -1)
                {
                    GL.DeleteTexture(texId);
                    texId = -1;
                }

                if (size.X == 0 || size.Y == 0)
                    return;

                //Create a texture over the whole model panel
                text._bitmap = b = new Bitmap(size.X, size.Y);

                b.MakeTransparent();

                text._texId = texId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, b.Width, b.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            }

            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                List<Vec2> _used = new List<Vec2>();

                foreach (ScreenTextHandler.TextData d in text._text.Values)
                    foreach (Vec3 v in d._positions)
                        if (v.X + d._string.Length * 10.0f > 0.0f && v.X < text._viewport.Width &&
                            v.Y > -10.0f && v.Y < text._viewport.Height &&
                            v.Z > 0.0f && v.Z < 1.0f && //near and far depth values
                            !_used.Contains(new Vec2(v.X, v.Y)))
                        {
                            g.DrawString(d._string, ScreenTextHandler._textFont, Brushes.Black, new PointF(v.X, v.Y));
                            _used.Add(new Vec2(v.X, v.Y));
                        }
            }

            GL.BindTexture(TextureTarget.Texture2D, text._texId);

            BitmapData data = text._bitmap.LockBits(new Rectangle(0, 0, text._bitmap.Width, text._bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, text._bitmap.Width, text._bitmap.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            text._bitmap.UnlockBits(data);
            
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(0.0f, 0.0f);

            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(text._size.X, 0.0f);

            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(text._size.X, text._size.Y);

            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(0.0f, text._size.Y);

            GL.End();

            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Blend);
            GL.Disable(OpenTK.Graphics.OpenGL.EnableCap.Texture2D);
        }
    }
}
