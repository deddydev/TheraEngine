using CustomEngine.Rendering.Cameras;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class GBuffer : FrameBuffer
    {
        PrimitiveManager _fullScreenQuad;
        OrthographicCamera _camera;

        Texture[] Textures => _fullScreenQuad?.Program.Textures;

        DrawBuffersAttachment[] _attachments = new DrawBuffersAttachment[]
        {
            DrawBuffersAttachment.ColorAttachment0, //AlbedoSpec
            DrawBuffersAttachment.ColorAttachment1, //Position
            DrawBuffersAttachment.ColorAttachment2, //Normal
            DrawBuffersAttachment.ColorAttachment3, //Texcoord
            DrawBuffersAttachment.ColorAttachment4, //Stencil
            DrawBuffersAttachment.ColorAttachment5, //Text
        };

        public GBuffer(BoundingRectangle region)
        {
            _fullScreenQuad = new PrimitiveManager(
                PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), 
                VertexQuad.ZUpQuad(region)),
                Material.GetGBufferMaterial(region.IntWidth, region.IntHeight));
            _fullScreenQuad.SettingUniforms += Engine.Renderer.Scene.SetUniforms;
            _camera = new OrthographicCamera();
            _camera.SetCenteredStyle();
        }
        ~GBuffer()
        {
            _fullScreenQuad.SettingUniforms -= Engine.Renderer.Scene.SetUniforms;
        }

        protected override void OnGenerated()
        {
            _fullScreenQuad.Generate();
            Bind(FramebufferType.Write);

            //Bind depth texture
            Textures[0].AttachToFrameBuffer(
                EFramebufferTarget.Framebuffer,
                EFramebufferAttachment.Depth,
                ETexTarget.Texture2D);

            //Bind other textures
            for (int i = 1; i < Textures.Length; ++i)
            {
                Textures[i].AttachToFrameBuffer(
                    EFramebufferTarget.Framebuffer,
                    EFramebufferAttachment.ColorAttachment0 + i,
                    ETexTarget.Texture2D);
            }
            
            Engine.Renderer.SetDrawBuffers(_attachments);
            Unbind(FramebufferType.Write);
        }
        public unsafe void SetRegion(BoundingRectangle region)
        {
            // 3--2
            // |\ |
            // | \|
            // 0--1
            //0 1 3 3 1 2
            //remapped ->
            //0 1 3 2

            VertexBuffer buffer = _fullScreenQuad.Data[0];
            Vec3* data = (Vec3*)buffer.Address;
            data[0] = region.BottomLeft;
            data[1] = region.BottomRight;
            data[2] = region.TopLeft;
            data[3] = region.TopRight;

            _camera.Resize(region.Width, region.Height);
        }

        public void Render()
        {
            AbstractRenderer.CurrentCamera = _camera;
            _fullScreenQuad.Render(Matrix4.Identity, Matrix3.Identity);
            AbstractRenderer.CurrentCamera = null;
        }
    }
}
