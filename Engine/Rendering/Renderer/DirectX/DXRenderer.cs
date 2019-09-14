using System;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Memory;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using DX11 = SharpDX.Direct3D11;

namespace TheraEngine.Rendering.DirectX
{
    internal unsafe class DXRenderer : AbstractRenderer
    {
        public DXRenderer()
        {

        }
        
        public DXWindowContext DxCtx => CurrentContext as DXWindowContext;
        public RasterizerState RasterState => DxCtx.RasterState;
        public DX11.Device Device => DxCtx.Device;
        public DeviceContext DeviceContext => DxCtx.DeviceContext;
        public SwapChain SwapChain => DxCtx.SwapChain;
        public Texture2D DepthStencilBuffer => DxCtx.DepthStencilBuffer;
        public RenderTargetView RenderTargetView => DxCtx.RenderTargetView;
        public DepthStencilState DepthStencilState => DxCtx.DepthStencilState;
        public DepthStencilView DepthStencilView => DxCtx.DepthStencilView;

        public override ERenderLibrary RenderLibrary => ERenderLibrary.Direct3D11;
        
        public override void Clear(EFBOTextureType clearBufferMask)
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
            int[] ids = new int[count];
            switch (type)
            {
                case EObjectType.Buffer:
                    break;
                case EObjectType.Framebuffer:
                    for (int i = 0; i < count; ++i)
                    {
                        RenderTargetViewDescription desc = new RenderTargetViewDescription()
                        {

                        };
                        RenderTargetView fbo = new RenderTargetView(Device, null, desc);
                    }
                    break;
                case EObjectType.Program:
                    break;
                case EObjectType.ProgramPipeline:
                    break;
                case EObjectType.Query:
                    break;
                case EObjectType.Renderbuffer:
                    break;
                case EObjectType.Sampler:
                    break;
                case EObjectType.Shader:
                    break;
                case EObjectType.Texture:
                    break;
                case EObjectType.TransformFeedback:
                    break;
                case EObjectType.VertexArray:
                    break;
            }
            return ids;
        }
        
        public override void RenderCurrentPrimitiveManager(int instances)
        {
            if (_currentPrimitiveManager is null)
                return;

            EPrimitiveType type = _currentPrimitiveManager.Data._type;
            int count = _currentPrimitiveManager.IndexBuffer.ElementCount;
            EDrawElementType elemType = _currentPrimitiveManager.ElementType;

        }
        
        public override void PushBufferData(DataBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void MapBufferData(DataBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void UnmapBufferData(DataBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void SetFaceCulling(ECulling culling)
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

        public override void SetShaderMode(EGLSLType type)
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

        public override byte GetStencilIndex(float x, float y)
        {
            throw new NotImplementedException();
        }

        public override int[] CreateTextures(ETexTarget target, int count)
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

        public override void LinkRenderIndices(IPrimitiveManager manager, DataBuffer indexBuffer)
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
        
        public override void SetProgramParameter(int program, EProgParam parameter, int value)
        {
            throw new NotImplementedException();
        }

        public override void UseProgram(int program)
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

        internal override int OnGetAttribLocation(int program, string name)
        {
            throw new NotImplementedException();
        }

        internal override int OnGetUniformLocation(int program, string name)
        {
            throw new NotImplementedException();
        }
        
        public override void ActiveShaderProgram(int pipelineBindingId, int program)
        {
            throw new NotImplementedException();
        }

        public override void CheckFrameBufferErrors()
        {
            throw new NotImplementedException();
        }
        
        public override void ClearTexImage(int bindingId, int level, EPixelFormat format, EPixelType type, VoidPtr clearColor)
        {
            throw new NotImplementedException();
        }

        public override void ColorMask(bool r, bool g, bool b, bool a)
        {
            throw new NotImplementedException();
        }

        public override void GenerateMipmap(ETexTarget target)
        {
            throw new NotImplementedException();
        }

        public override void GenerateMipmap(int textureBindingId)
        {
            throw new NotImplementedException();
        }

        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, ETexTarget texTarget, int textureBindingId, int mipLevel)
        {
            throw new NotImplementedException();
        }

        public override void CheckErrors()
        {
            throw new NotImplementedException();
        }

        public override void EnableDepthTest(bool enabled)
        {
            throw new NotImplementedException();
        }

        public override int[] CreateQueries(EQueryTarget type, int count)
        {
            throw new NotImplementedException();
        }

        public override void SetShaderSource(int bindingId, params string[] sources)
        {
            throw new NotImplementedException();
        }

        public override bool CompileShader(int bindingId, out string info)
        {
            throw new NotImplementedException();
        }

        public override int GenerateProgram(bool separable)
        {
            throw new NotImplementedException();
        }

        public override void AttachShader(int shaderBindingId, int program)
        {
            throw new NotImplementedException();
        }

        public override void DetachShader(int shaderBindingId, int program)
        {
            throw new NotImplementedException();
        }

        public override bool LinkProgram(int bindingId, out string info)
        {
            throw new NotImplementedException();
        }

        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, int textureBindingId, int mipLevel)
        {
            throw new NotImplementedException();
        }

        public override void MemoryBarrier(EMemoryBarrierFlags flags)
        {
            throw new NotImplementedException();
        }

        public override void MemoryBarrierByRegion(EMemoryBarrierRegionFlags flags)
        {
            throw new NotImplementedException();
        }

        public override void DispatchCompute(int numGroupsX, int numGroupsY, int numGroupsZ)
        {
            throw new NotImplementedException();
        }

        public override void DispatchComputeIndirect(int offset)
        {
            throw new NotImplementedException();
        }

        public override void PushTextureData<T>(ETexTarget texTarget, int mipLevel, EPixelInternalFormat internalFormat, int width, int height, EPixelFormat pixelFormat, EPixelType type, T[] data)
        {
            throw new NotImplementedException();
        }

        public override void PushTextureSubData(ETexTarget texTarget, int mipLevel, int xOffset, int yOffset, int width, int height, EPixelFormat format, EPixelType type, VoidPtr data)
        {
            throw new NotImplementedException();
        }

        public override void PushTextureSubData<T>(ETexTarget texTarget, int mipLevel, int xOffset, int yOffset, int width, int height, EPixelFormat format, EPixelType type, T[] data)
        {
            throw new NotImplementedException();
        }

        public override void ClearStencil(int value)
        {
            throw new NotImplementedException();
        }

        public override void StencilOp(EStencilOp fail, EStencilOp zFail, EStencilOp zPass)
        {
            throw new NotImplementedException();
        }

        public override void TextureView(int bindingId, ETexTarget target, int origTextureId, EPixelInternalFormat fmt, int minLevel, int numLevels, int minLayer, int numLayers)
        {
            throw new NotImplementedException();
        }

        public override void SetTextureStorage(ETexTarget2D texTarget, int mipLevels, ESizedInternalFormat internalFormat, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void SetTextureStorage(int bindingId, int mipLevels, ESizedInternalFormat internalFormat, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void StencilMask(int value)
        {
            throw new NotImplementedException();
        }

        public override void SetMipmapParams(int bindingId, int minLOD, int maxLOD, int largestMipmapLevel, int smallestAllowedMipmapLevel)
        {
            throw new NotImplementedException();
        }

        public override void SetMipmapParams(ETexTarget target, int minLOD, int maxLOD, int largestMipmapLevel, int smallestAllowedMipmapLevel)
        {
            throw new NotImplementedException();
        }

        public override void GetTexImage<T>(ETexTarget texture2D, int smallestMipmapLevel, EPixelFormat pixelFormat, EPixelType pixelType, T[] rgba)
        {
            throw new NotImplementedException();
        }

        public override void UniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding)
        {
            throw new NotImplementedException();
        }

        public override void BeginConditionalRender(RenderQuery query, EConditionalRenderType type)
        {
            throw new NotImplementedException();
        }

        public override void EndConditionalRender()
        {
            throw new NotImplementedException();
        }

        public override void AttachTextureToFrameBuffer(int frameBufferBindingId, EFramebufferAttachment attachment, int textureBindingId, int mipLevel, int layer)
        {
            throw new NotImplementedException();
        }

        public override void AttachTextureToFrameBuffer(EFramebufferTarget target, EFramebufferAttachment attachment, int textureBindingId, int mipLevel, int layer)
        {
            throw new NotImplementedException();
        }

        public override void BeginQuery(int bindingId, EQueryTarget target)
        {
            throw new NotImplementedException();
        }

        public override void EndQuery(EQueryTarget target)
        {
            throw new NotImplementedException();
        }

        public override void QueryCounter(int bindingId)
        {
            throw new NotImplementedException();
        }

        public override int GetQueryObjectInt(int bindingId, EGetQueryObject obj)
        {
            throw new NotImplementedException();
        }

        public override long GetQueryObjectLong(int bindingId, EGetQueryObject obj)
        {
            throw new NotImplementedException();
        }

        public override void AttributeDivisor(int attributeLocation, int divisor)
        {
            throw new NotImplementedException();
        }

        public override void BindBufferBase(EBufferRangeTarget rangeTarget, int blockIndex, int bufferBindingId)
        {
            throw new NotImplementedException();
        }

        public override int GetUniformBlockIndex(int program, string name)
        {
            throw new NotImplementedException();
        }

        public override void InitializeBuffer(DataBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public override void PushBufferSubData(DataBuffer buffer, int offset, int length)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable4Int[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable4Float[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable4Double[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable4UInt[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable4Bool[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable3Int[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable3Float[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable2Int[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable2Float[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable1Int[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params IUniformable1Float[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params int[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params float[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, Matrix4 p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params Matrix4[] p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, Matrix3 p)
        {
            throw new NotImplementedException();
        }

        internal override void Uniform(int programBindingId, int location, params Matrix3[] p)
        {
            throw new NotImplementedException();
        }

        public override void GetTexImage(ETexTarget target, int level, EPixelFormat pixelFormat, EPixelType pixelType, IntPtr pixels)
        {
            throw new NotImplementedException();
        }

        public override void GetTexImage<T>(int textureBindingId, int level, EPixelFormat pixelFormat, EPixelType pixelType, T[] pixels)
        {
            throw new NotImplementedException();
        }

        public override void GetTexImage(int textureBindingId, int level, EPixelFormat pixelFormat, EPixelType pixelType, int bufSize, IntPtr pixels)
        {
            throw new NotImplementedException();
        }

        public override void TexParameter(int texBindingId, ETexParamName texParam, float paramData)
        {
            throw new NotImplementedException();
        }

        public override void TexParameter(int texBindingId, ETexParamName texParam, int paramData)
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

        public override EWaitSyncStatus ClientWaitSync(IntPtr sync, long timeout)
        {
            throw new NotImplementedException();
        }

        public override void WaitSync(IntPtr sync, long timeout)
        {
            throw new NotImplementedException();
        }

        public override IntPtr FenceSync()
        {
            throw new NotImplementedException();
        }
    }
}
