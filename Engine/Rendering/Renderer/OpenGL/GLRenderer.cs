using System;
using OpenTK.Graphics.OpenGL;
using System.Linq;
using System.Drawing;
using CustomEngine.Rendering.Models.Materials;
using System.Collections.Generic;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Rendering.OpenGL
{
    public unsafe class GLRenderer : AbstractRenderer
    {
        // https://www.opengl.org/wiki/Rendering_Pipeline_Overview

        public static GLRenderer Instance = new GLRenderer();

        public override RenderLibrary RenderLibrary { get { return RenderLibrary.OpenGL; } }

        public GLRenderer() { }

        #region Shapes
        //public override void DrawBoxWireframe(System.Vec3 min, System.Vec3 max)
        //{
        //    GL.Begin(PrimitiveType.LineStrip);

        //    GL.Vertex3(max.X, max.Y, max.Z);
        //    GL.Vertex3(max.X, max.Y, min.Z);
        //    GL.Vertex3(min.X, max.Y, min.Z);
        //    GL.Vertex3(min.X, min.Y, min.Z);
        //    GL.Vertex3(min.X, min.Y, max.Z);
        //    GL.Vertex3(max.X, min.Y, max.Z);
        //    GL.Vertex3(max.X, max.Y, max.Z);

        //    GL.End();

        //    GL.Begin(PrimitiveType.Lines);

        //    GL.Vertex3(min.X, max.Y, max.Z);
        //    GL.Vertex3(max.X, max.Y, max.Z);
        //    GL.Vertex3(min.X, max.Y, max.Z);
        //    GL.Vertex3(min.X, min.Y, max.Z);
        //    GL.Vertex3(min.X, max.Y, max.Z);
        //    GL.Vertex3(min.X, max.Y, min.Z);
        //    GL.Vertex3(max.X, min.Y, min.Z);
        //    GL.Vertex3(min.X, min.Y, min.Z);
        //    GL.Vertex3(max.X, min.Y, min.Z);
        //    GL.Vertex3(max.X, max.Y, min.Z);
        //    GL.Vertex3(max.X, min.Y, min.Z);
        //    GL.Vertex3(max.X, min.Y, max.Z);

        //    GL.End();
        //}
        //public override void DrawBoxSolid(System.Vec3 min, System.Vec3 max)
        //{
        //    GL.Begin(PrimitiveType.QuadStrip);

        //    GL.Vertex3(min.X, min.Y, min.Z);
        //    GL.Vertex3(min.X, max.Y, min.Z);
        //    GL.Vertex3(max.X, min.Y, min.Z);
        //    GL.Vertex3(max.X, max.Y, min.Z);
        //    GL.Vertex3(max.X, min.Y, max.Z);
        //    GL.Vertex3(max.X, max.Y, max.Z);
        //    GL.Vertex3(min.X, min.Y, max.Z);
        //    GL.Vertex3(min.X, max.Y, max.Z);
        //    GL.Vertex3(min.X, min.Y, min.Z);
        //    GL.Vertex3(min.X, max.Y, min.Z);

        //    GL.End();

        //    GL.Begin(PrimitiveType.Quads);

        //    GL.Vertex3(min.X, max.Y, min.Z);
        //    GL.Vertex3(min.X, max.Y, max.Z);
        //    GL.Vertex3(max.X, max.Y, max.Z);
        //    GL.Vertex3(max.X, max.Y, min.Z);

        //    GL.Vertex3(min.X, min.Y, min.Z);
        //    GL.Vertex3(min.X, min.Y, max.Z);
        //    GL.Vertex3(max.X, min.Y, max.Z);
        //    GL.Vertex3(max.X, min.Y, min.Z);

        //    GL.End();
        //}
        //public override void DrawCapsuleWireframe(System.Single radius, System.Single halfHeight)
        //{
        //    throw new NotImplementedException();
        //}
        //public override void DrawCapsuleSolid(System.Single radius, System.Single halfHeight)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        public override void BindTextureData(int textureTargetEnum, int mipLevel, int pixelInternalFormatEnum, int width, int height, int pixelFormatEnum, int pixelTypeEnum, VoidPtr data)
        {
            // https://www.opengl.org/sdk/docs/man/html/glTexImage2D.xhtml
            GL.TexImage2D((TextureTarget)textureTargetEnum, mipLevel, (OpenTK.Graphics.OpenGL.PixelInternalFormat)pixelInternalFormatEnum, width, height, 0, (PixelFormat)pixelFormatEnum, (PixelType)pixelTypeEnum, data);
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

        //#region Conversion
        //private OpenTK.Matrix4 GLMat4(Matrix4 matrix4)
        //{
        //    return *(OpenTK.Matrix4*)&matrix4;
        //}
        //private OpenTK.Vector4 GLVec4(System.Vec4 vec4)
        //{
        //    return *(OpenTK.Vector4*)&vec4;
        //}
        //private OpenTK.Vector3 GLVec3(System.Vec3 vec3)
        //{
        //    return *(OpenTK.Vector3*)&vec3;
        //}
        //private OpenTK.Vector2 GLVec2(System.Vec2 vec2)
        //{
        //    return *(OpenTK.Vector2*)&vec2;
        //}
        //private OpenTK.Quaternion GLQuat(System.Quaternion quat)
        //{
        //    return *(OpenTK.Quaternion*)&quat;
        //}
        //#endregion

        #region Objects
        public override void DeleteObject(GenType type, int bindingId)
        {
            switch (type)
            {
                case GenType.Buffer:
                    GL.DeleteBuffer(bindingId);
                    break;
                case GenType.DisplayList:
                    GL.DeleteLists(bindingId, 1);
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
                case GenType.DisplayList:
                    foreach (int i in bindingIds)
                        GL.DeleteLists(i, 1);
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
        public override int GenObject(GenType type)
        {
            switch (type)
            {
                case GenType.Buffer:
                    return GL.GenBuffer();
                case GenType.DisplayList:
                    return GL.GenLists(1);
                case GenType.Framebuffer:
                    return GL.GenFramebuffer();
                case GenType.Program:
                    return GL.CreateProgram();
                case GenType.ProgramPipeline:
                    return GL.GenProgramPipeline();
                case GenType.Query:
                    return GL.GenQuery();
                case GenType.Renderbuffer:
                    return GL.GenRenderbuffer();
                case GenType.Sampler:
                    return GL.GenSampler();
                case GenType.Texture:
                    return GL.GenTexture();
                case GenType.TransformFeedback:
                    return GL.GenTransformFeedback();
                case GenType.VertexArray:
                    return GL.GenVertexArray();
                case GenType.Shader:
                    return GL.CreateShader(_currentShaderMode);
            }
            return 0;
        }
        public override int[] GenObjects(GenType type, int count)
        {
            int[] ids = new int[count];
            switch (type)
            {
                case GenType.Buffer:
                    GL.GenBuffers(count, ids);
                    break;
                case GenType.Framebuffer:
                    GL.GenFramebuffers(count, ids);
                    break;
                case GenType.Program:
                    for (int i = 0; i < count; ++i)
                        ids[i] = GL.CreateProgram();
                    break;
                case GenType.ProgramPipeline:
                    GL.GenProgramPipelines(count, ids);
                    break;
                case GenType.Query:
                    GL.GenQueries(count, ids);
                    break;
                case GenType.Renderbuffer:
                    GL.GenRenderbuffers(count, ids);
                    break;
                case GenType.Sampler:
                    GL.GenSamplers(count, ids);
                    break;
                case GenType.Texture:
                    GL.GenTextures(count, ids);
                    break;
                case GenType.TransformFeedback:
                    GL.GenTransformFeedbacks(count, ids);
                    break;
                case GenType.VertexArray:
                    GL.GenVertexArrays(count, ids);
                    break;
                case GenType.Shader:
                    for (int i = 0; i < count; ++i)
                        ids[i] = GL.CreateShader(_currentShaderMode);
                    break;
                case GenType.DisplayList:
                    return new int[] { GL.GenLists(count) };
            }
            return ids;
        }
        #endregion

        #region Display Lists
        public override int CreateDisplayList()
        {
            return GL.GenLists(1);
        }
        public override void BeginDisplayList(int id, DisplayListMode mode)
        {
            GL.NewList(id, mode == DisplayListMode.Compile ? ListMode.Compile : ListMode.CompileAndExecute);
        }
        public override void EndDisplayList()
        {
            GL.EndList();
        }
        public override void CallDisplayList(int id)
        {
            GL.CallList(id);
        }
        public override void DeleteDisplayList(int id)
        {
            GL.DeleteLists(id, 1);
        }
        #endregion

        #region Drawing
        public override void Begin(EPrimitive type)
        {
            GL.Begin((PrimitiveType)(int)type);
        }
        public override void Vertex3(Vec3 value)
        {
            GL.Vertex3(value.X, value.Y, value.Z);
        }
        public override void Vertex2(Vec2 value)
        {
            GL.Vertex2(value.X, value.Y);
        }
        public override void Normal3(Vec3 value)
        {
            GL.Normal3(value.X, value.Y, value.Z);
        }
        public override void TexCoord2(Vec2 value)
        {
            GL.TexCoord2(value.X, value.Y);
        }
        public override void MultiTexCoord2(int unit, Vec2 value)
        {
            GL.MultiTexCoord2(TextureUnit.Texture0 + unit, value.X, value.Y);
        }
        public override void Color4(ColorF4 value)
        {
            GL.Color4(value.R, value.G, value.B, value.A);
        }
        public override void Color3(ColorF3 value)
        {
            GL.Color3(value.R, value.G, value.B);
        }
        public override void End()
        {
            GL.End();
        }
        public override void SetPointSize(float size)
        {
            GL.PointSize(size);
        }
        public override void SetLineSize(float size)
        {
            GL.LineWidth(size);
        }

        #endregion

        #region Shaders
        private ShaderType _currentShaderMode;
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
            int status;
            GL.GetShader(handle, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                string info;
                GL.GetShaderInfoLog(handle, out info);
                Console.WriteLine(info + "\n\n");

                //Split the source by new lines
                string[] s = source.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                //Add the line number to the source so we can go right to errors on specific lines
                int lineNumber = 1;
                foreach (string line in s)
                    Console.WriteLine(string.Format("{0}: {1}", (lineNumber++).ToString().PadLeft(s.Length.ToString().Length, '0'), line));

                Console.WriteLine("\n\n");
            }
#endif
            return handle;
        }
        public override int GenerateMaterial(int[] shaderHandles)
        {
            int handle = GL.CreateProgram();
            foreach (int i in shaderHandles)
                GL.AttachShader(handle, i);

            //Have to bind 'in' attributes before linking
            int k = 0;
            for (int i = 0; i < VertexBuffer.BufferTypeCount; ++i)
            {
                BufferType type = (BufferType)i;
                for (int j = 0; j < VertexBuffer.MaxBufferCountPerType; ++j)
                    GL.BindAttribLocation(handle, k++, type.ToString() + j);
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
        public override void UseMaterial(Material material)
        {
            GL.UseProgram(material.BindingId);
            base.UseMaterial(material);
        }

        #region Uniforms
        public override int GetAttribLocation(string name)
        {
            return GL.GetAttribLocation(_programHandle, name);
        }
        public override int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(_programHandle, name);
        }
        public override void Uniform(int location, params IUniformable4Int[] p)
        {
            const int count = 4;
            
            if (location < 0)
                return;

            int[] values = new int[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 2 + x] = p[i].Data[x];

            GL.Uniform3(location, p.Length, values);
        }
        public override void Uniform(int location, params IUniformable4Float[] p)
        {
            const int count = 4;
            
            if (location < 0)
                return;

            float[] values = new float[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 2 + x] = p[i].Data[x];

            GL.Uniform3(location, p.Length, values);
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
                    values[i << 1 + x] = p[i].Data[x];

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
                    values[i << 1 + x] = p[i].Data[x];

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
                        values[i << 4 + x] = p[i].Data[x];
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
            GL.ReadPixels((int)x, (int)(Engine.CurrentPanel.Height - y), 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref val);
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

        public override void Uniform(int matID, int location, params IUniformable4Int[] p)
        {
            const int count = 4;

            if (location < 0)
                return;

            float[] values = new float[p.Length << 2];

            for (int i = 0; i < p.Length; ++i)
                for (int x = 0; x < count; ++x)
                    values[i << 2 + x] = p[i].Data[x];

            GL.ProgramUniform3(matID, location, p.Length, values);
        }

        public override void Uniform(int matID, int location, params IUniformable4Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params IUniformable3Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params IUniformable3Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params IUniformable2Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params IUniformable2Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params IUniformable1Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params IUniformable1Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, Matrix4 p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params Matrix4[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, Matrix3 p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int matID, int location, params Matrix3[] p)
        {
            throw new NotImplementedException();
        }
    }
}
