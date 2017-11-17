using System;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.DirectX
{
    internal unsafe class DXRenderer : AbstractRenderer
    {
        public static DXRenderer Instance;
        public DXRenderer()
        {

        }

        public override RenderLibrary RenderLibrary => RenderLibrary.Direct3D11;
        
        public override void Clear(EBufferClear clearBufferMask)
        {
            throw new NotImplementedException();
        }

        public override void DeleteObjects(EObjectType type, int[] bindingIds)
        {
            throw new NotImplementedException();
        }

        public override void SetBindFragDataLocation(int bindingId, int location, string name)
        {
            throw new NotImplementedException();
        }

        public override void BindFrameBuffer(EFramebufferTarget type, int bindingId)
        {
            throw new NotImplementedException();
        }
        public override void SetDrawBuffers(EDrawBuffersAttachment[] attachments)
        {
            throw new NotImplementedException();
        }

        public override int[] CreateObjects(EObjectType type, int count)
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

        //public override int GenerateProgram(int[] shaderHandles, VertexShaderDesc info)
        //{
        //    throw new NotImplementedException();
        //}

        //public override int GetAttribLocation(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //public override int GetUniformLocation(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable4Int[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable4Float[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable3Int[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable3Float[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable2Int[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable2Float[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable1Int[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params IUniformable1Float[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params int[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params float[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, Matrix4 p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params Matrix4[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, Matrix3 p)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Uniform(int location, params Matrix3[] p)
        //{
        //    throw new NotImplementedException();
        //}
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

        public override void DeleteObject(EObjectType type, int bindingId)
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
        

        public override void SetDrawBuffer(EDrawBuffersAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public override void SetDrawBuffer(int bindingId, EDrawBuffersAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public override void SetDrawBuffers(int bindingId, EDrawBuffersAttachment[] attachments)
        {
            throw new NotImplementedException();
        }

        public override void SetReadBuffer(EDrawBuffersAttachment attachment)
        {
            throw new NotImplementedException();
        }

        public override void SetReadBuffer(int bindingId, EDrawBuffersAttachment attachment)
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

        public override void SetActiveTexture(int unit)
        {
            throw new NotImplementedException();
        }

        public override void LinkRenderIndices(IPrimitiveManager manager, VertexBuffer indexBuffer)
        {
            throw new NotImplementedException();
        }

        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, ETexTarget texTarget, int textureBindingId, int mipLevel)
        {
            throw new NotImplementedException();
        }

        public override void AllowDepthWrite(bool allow)
        {
            throw new NotImplementedException();
        }

        public override void DepthFunc(EComparison func)
        {
            throw new NotImplementedException();
        }

        public override void DepthRange(double near, double far)
        {
            throw new NotImplementedException();
        }

        public override void BlendColor(ColorF4 color)
        {
            throw new NotImplementedException();
        }

        public override void BlendFunc(EBlendingFactor srcFactor, EBlendingFactor destFactor)
        {
            throw new NotImplementedException();
        }

        public override void BlendFuncSeparate(EBlendingFactor srcFactorRGB, EBlendingFactor destFactorRGB, EBlendingFactor srcFactorAlpha, EBlendingFactor destFactorAlpha)
        {
            throw new NotImplementedException();
        }

        public override void BlendEquation(EBlendEquationMode rgb, EBlendEquationMode alpha)
        {
            throw new NotImplementedException();
        }

        public override void BlendEquationSeparate(EBlendEquationMode rgb, EBlendEquationMode alpha)
        {
            throw new NotImplementedException();
        }

        public override void ClearDepth(float defaultDepth)
        {
            throw new NotImplementedException();
        }

        public override void BlitFrameBuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, EClearBufferMask mask, EBlitFramebufferFilter filter)
        {
            throw new NotImplementedException();
        }

        public override void ClearColor(ColorF4 color)
        {
            throw new NotImplementedException();
        }
        
        public override void SetProgramParameter(int programBindingId, EProgParam parameter, int value)
        {
            throw new NotImplementedException();
        }

        public override void UseProgram(int programBindingId)
        {
            throw new NotImplementedException();
        }

        public override void BindPipeline(int pipelineBindingId)
        {
            throw new NotImplementedException();
        }

        public override void SetPipelineStage(int pipelineBindingId, EProgramStageMask mask, int programBindingId)
        {
            throw new NotImplementedException();
        }

        public override void ApplyRenderParams(RenderingParameters r)
        {
            throw new NotImplementedException();
        }

        public override int GetAttribLocation(int programBindingId, string name)
        {
            throw new NotImplementedException();
        }

        public override int GetUniformLocation(int programBindingId, string name)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable4Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable4Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable3Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable3Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable2Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable2Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable1Int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable1Float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params int[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params float[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, Matrix4 p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params Matrix4[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, Matrix3 p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params Matrix3[] p)
        {
            throw new NotImplementedException();
        }

        public override int GenerateProgram(int[] shaderHandles, bool separable)
        {
            throw new NotImplementedException();
        }

        public override void ActiveShaderProgram(int pipelineBindingId, int programBindingId)
        {
            throw new NotImplementedException();
        }

        public override void CheckFrameBufferErrors()
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable4Double[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable4UInt[] p)
        {
            throw new NotImplementedException();
        }

        public override void Uniform(int programBindingId, int location, params IUniformable4Bool[] p)
        {
            throw new NotImplementedException();
        }

        public override int GenerateShader(params string[] source)
        {
            throw new NotImplementedException();
        }

        public override void ClearTexImage(int bindingId, ETexTarget textureTarget, ColorF4 clearColor)
        {
            throw new NotImplementedException();
        }
    }
}
