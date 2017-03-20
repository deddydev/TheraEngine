using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class GBuffer : FrameBuffer
    {
        PrimitiveManager _fullScreenQuad;

        Texture[] Textures => _fullScreenQuad?.Program.Textures;

        DrawBuffersAttachment[] _attachments = new DrawBuffersAttachment[]
        {
            DrawBuffersAttachment.ColorAttachment0, //AlbedoSpec
            DrawBuffersAttachment.ColorAttachment1, //Position
            DrawBuffersAttachment.ColorAttachment2, //Normal
            DrawBuffersAttachment.ColorAttachment3, //Texcoord
            DrawBuffersAttachment.ColorAttachment4, //Stencil
            DrawBuffersAttachment.ColorAttachment5, //Text
            DrawBuffersAttachment.DepthAttachement, //Depth
        };

        public GBuffer(int width, int height)
        {
            _fullScreenQuad = new PrimitiveManager(
                PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), 
                VertexQuad.ZUpQuad(width, height)),
                Material.GetGBufferMaterial(width, height));
            _fullScreenQuad.SettingUniforms += Engine.Renderer.Scene.SetUniforms;
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
                OpenTK.Graphics.OpenGL.FramebufferTarget.Framebuffer,
                OpenTK.Graphics.OpenGL.FramebufferAttachment.Depth,
                OpenTK.Graphics.OpenGL.TextureTarget.Texture2D);

            //Bind other textures
            for (int i = 1; i < Textures.Length; ++i)
            {
                Textures[i].AttachToFrameBuffer(
                    OpenTK.Graphics.OpenGL.FramebufferTarget.Framebuffer,
                    OpenTK.Graphics.OpenGL.FramebufferAttachment.ColorAttachment0 + i,
                    OpenTK.Graphics.OpenGL.TextureTarget.Texture2D);
            }
            
            Engine.Renderer.DrawBuffers(_attachments);
            Unbind(FramebufferType.Write);
        }
        public unsafe void Resize(float width, float height)
        {
            // 3--2
            // |\ |
            // | \|
            // 0--1
            //0 1 3 3 1 2

            //Vec3 bottomLeft = new Vec3(0.0f, 0.0f, 0.0f);
            Vec3 bottomRight = new Vec3(width, 0.0f, 0.0f);
            Vec3 topRight = new Vec3(width, height, 0.0f);
            Vec3 topLeft = new Vec3(0.0f, height, 0.0f);

            Vec3* data = (Vec3*)_fullScreenQuad.Data[0].Address;
            //data[0] = bottomLeft;
            data[1] = data[4] = bottomRight;
            data[2] = data[3] = topLeft;
            data[5] = topRight;
        }

        public void Render()
        {
            _fullScreenQuad.Render(Matrix4.Identity, Matrix3.Identity);
        }
    }
}
