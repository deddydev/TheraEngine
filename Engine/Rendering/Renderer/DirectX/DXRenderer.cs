using System;
using CustomEngine.Rendering.DirectX;
using System.Collections.Generic;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using System.Drawing;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Rendering.DirectX
{
    internal unsafe class DXRenderer : AbstractRenderer
    {
        public static DXRenderer Instance;
        public DXRenderer()
        {

        }

        public override RenderLibrary RenderLibrary => RenderLibrary.Direct3D11;

        private class DLCompileInfo
        {
            public int _id;
            public bool _executeAfterCompiling;
            public bool _temporary;

            public DLCompileInfo(int id, bool execute, bool temporary)
            {
                _id = id;
                _executeAfterCompiling = execute;
                _temporary = temporary;
            }
        }

        private Dictionary<int, DXDisplayList> _displayLists = new Dictionary<int, DXDisplayList>();
        private Stack<DLCompileInfo> _compilingDisplayLists = new Stack<DLCompileInfo>();
        public DXDisplayList CurrentList { get { return _compilingDisplayLists.Count > 0 && _displayLists.Count > 0 ? _displayLists[_compilingDisplayLists.Peek()._id] : null; } }

        public override void Clear(BufferClear clearBufferMask)
        {
            throw new NotImplementedException();
        }

        public override void DeleteObjects(GenType type, int[] bindingIds)
        {
            throw new NotImplementedException();
        }

        public override void SetBindFragDataLocation(int bindingId, int location, string name)
        {
            throw new NotImplementedException();
        }

        public override void BindFrameBuffer(EFramebufferType type, int bindingId)
        {
            throw new NotImplementedException();
        }
        public override void SetDrawBuffers(DrawBuffersAttachment[] attachments)
        {
            throw new NotImplementedException();
        }

        public override int[] CreateObjects(GenType type, int count)
        {
            throw new NotImplementedException();
        }
        
        public override int[] CreateQueries(int type, int count)
        {
            throw new NotImplementedException();
        }

        public override void RenderCurrentPrimitiveManager()
        {
            throw new NotImplementedException();
        }

        public override void LinkRenderIndices(PrimitiveManager manager, VertexBuffer indexBuffer)
        {
            throw new NotImplementedException();
        }

        public override void InitializeBuffer(VertexBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void PushBufferData(VertexBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void MapBufferData(VertexBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void UnmapBufferData(VertexBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void Cull(Culling culling)
        {
            throw new NotImplementedException();
        }

        public override int GenerateProgram(int[] shaderHandles, PrimitiveBufferInfo info)
        {
            throw new NotImplementedException();
        }

        public override int GetAttribLocation(string name)
        {
            throw new NotImplementedException();
        }

        public override int GetUniformLocation(string name)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable4Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable4Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable3Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable3Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable2Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable2Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable1Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params IUniformable1Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, Matrix4 p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params Matrix4[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, Matrix3 p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int location, params Matrix3[] p)
        {
            throw new NotImplementedException();
        }
        public override void BindTransformFeedback(int bindingId)
        {
            throw new NotImplementedException();
        }

        public override void BeginTransformFeedback(FeedbackPrimitiveType type)
        {
            throw new NotImplementedException();
        }

        public override void EndTransformFeedback()
        {
            throw new NotImplementedException();
        }

        public override void TransformFeedbackVaryings(int program, string[] varNames)
        {
            throw new NotImplementedException();
        }

        public override void DeleteObject(GenType type, int bindingId)
        {
            throw new NotImplementedException();
        }

        public override void SetPointSize(float size)
        {
            throw new NotImplementedException();
        }

        public override void SetLineSize(float size)
        {
            throw new NotImplementedException();
        }

        public override void SetShaderMode(ShaderMode type)
        {
            throw new NotImplementedException();
        }

        public override int GenerateShader(string source)
        {
            throw new NotImplementedException();
        }

        public override float GetDepth(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override Bitmap GetScreenshot(Rectangle region, bool withTransparency)
        {
            throw new NotImplementedException();
        }

        public override void CropRenderArea(BoundingRectangle region)
        {
            throw new NotImplementedException();
        }

        protected override void SetRenderArea(BoundingRectangle region)
        {
            throw new NotImplementedException();
        }

        public override void BlitFrameBuffer(int readBufferId, int writeBufferId, int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, EClearBufferMask mask, EBlitFramebufferFilter filter)
        {
            throw new NotImplementedException();
        }

        public override void TexParameter(ETexTarget texTarget, ETexParamName texParam, float paramData)
        {
            throw new NotImplementedException();
        }

        public override void TexParameter(ETexTarget texTarget, ETexParamName texParam, int paramData)
        {
            throw new NotImplementedException();
        }
        

        public override void SetDrawBuffer(DrawBuffersAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public override void SetDrawBuffer(int bindingId, DrawBuffersAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public override void SetDrawBuffers(int bindingId, DrawBuffersAttachment[] attachments)
        {
            throw new NotImplementedException();
        }

        public override void SetReadBuffer(DrawBuffersAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public override void SetReadBuffer(int bindingId, DrawBuffersAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public override void BindTexture(ETexTarget texTarget, int bindingId)
        {
            throw new NotImplementedException();
        }

        public override void PushTextureData(ETexTarget texTarget, int mipLevel, EPixelInternalFormat internalFormat, int width, int height, EPixelFormat pixelFormat, EPixelType type, VoidPtr data)
        {
            throw new NotImplementedException();
        }

        public override int GetStencilIndex(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override int[] CreateTextures(ETexTarget target, int count)
        {
            throw new NotImplementedException();
        }

        public override void PushTextureData(ETexTarget texTarget, int mipLevel, EPixelInternalFormat internalFormat, int width, int height, EPixelFormat pixelFormat, EPixelType type, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel)
        {
            throw new NotImplementedException();
        }

        public override void BindRenderBuffer(int bindingId)
        {
            throw new NotImplementedException();
        }

        public override void RenderbufferStorage(ERenderBufferStorage storage, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void FramebufferRenderBuffer(EFramebufferTarget target, EFramebufferAttachment attachement, int renderBufferBindingId)
        {
            throw new NotImplementedException();
        }

        public override void FramebufferRenderBuffer(int frameBufferBindingId, EFramebufferAttachment attachement, int renderBufferBindingId)
        {
            throw new NotImplementedException();
        }
    }
}
