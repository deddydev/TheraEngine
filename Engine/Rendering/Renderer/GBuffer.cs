using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Rendering
{
    public class GBufferMeshProgram : MeshProgram
    {
        DrawBuffersAttachment[] _attachments;
        EFramebufferAttachment?[] _attachmentsPerTexture;
        GBuffer _buffer;
        //int _renderBufferId;
        int _width, _height;
        
        public GBufferMeshProgram(Material material, PrimitiveBufferInfo info) : base(material, info) { }

        public void Update(GBuffer buffer, EFramebufferAttachment?[] attachmentsPerTexture, DrawBuffersAttachment[] attachments, int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = buffer;
            _attachments = attachments;
            _attachmentsPerTexture = attachmentsPerTexture;
            
            _buffer.Bind(EFramebufferTarget.Framebuffer);
            for (int i = 0; i < Textures.Length; ++i)
            {
                Texture t = _textures[i];
                t.Index = i;
                t.PostPushData += TexturePostPushData;
                t.Generate();
            }
            //_renderBufferId = Engine.Renderer.CreateObjects(GenType.Renderbuffer, 1)[0];
            //Engine.Renderer.BindRenderBuffer(_renderBufferId);
            //Engine.Renderer.RenderbufferStorage(ERenderBufferStorage.DepthComponent, _width, _height);
            //Engine.Renderer.FramebufferRenderBuffer(BindingId, EFramebufferAttachment.DepthAttachment, _renderBufferId);
            FramebufferErrorCode c = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (c != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Problem compiling G-Buffer.");
            _buffer.Unbind(EFramebufferTarget.Framebuffer);
        }

        public override void SetMaterial(Material material)
        {
            _parameters = material.Parameters.ToArray();

            _textures = new Texture[material.Textures.Count];
            for (int i = 0; i < material.Textures.Count; ++i)
                _textures[i] = material.Textures[i].GetTexture();

            _fragmentShader = material._fragmentShader;
            _geometryShader = material._geometryShader;
            _tControlShader = material._tessellationControlShader;
            _tEvalShader = material._tessellationEvaluationShader;
            RemakeShaderArray();
        }
        private void TexturePostPushData(int index)
        {
            if (_attachmentsPerTexture[index].HasValue)
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, _attachmentsPerTexture[index].Value, ETexTarget.Texture2D, Textures[index].BindingId, 0);
        }
        protected internal override void BindTextures()
        {
            _buffer.Bind(EFramebufferTarget.Framebuffer);
            base.BindTextures();
            Engine.Renderer.SetDrawBuffers(_attachments);
            _buffer.Unbind(EFramebufferTarget.Framebuffer);
        }
        public void Resized(int width, int height)
        {
            foreach (Texture t in Textures)
                t.Resize(width, height);
        }
    }
    public class GBuffer : FrameBuffer
    {
        OrthographicCamera _quadCamera;
        PrimitiveManager<GBufferMeshProgram> _fullScreenTriangle;
        bool _forward;
        Viewport _parent;

        public Texture[] Textures => _fullScreenTriangle?.Program.Textures;

        EFramebufferAttachment?[] _attachmentsPerTexture;
        DrawBuffersAttachment[] _colorAttachments;

        public GBuffer(Viewport viewport, bool forward)
        {
            _parent = viewport;
            _forward = forward;

            Vertex point0 = new Vec3(0.0f, 0.0f, -2.0f);
            Vertex point1 = new Vec3(2.0f, 0.0f, -2.0f);
            Vertex point2 = new Vec3(0.0f, 2.0f, -2.0f);
            //Vertex point3 = new Vec3(0.0f, 1.0f, -2.0f);
            //Vertex point4 = new Vec3(1.0f, 0.0f, -2.0f);
            //Vertex point5 = new Vec3(1.0f, 1.0f, -2.0f);

            VertexTriangle triangle1 = new VertexTriangle(point0, point1, point2);
            //VertexTriangle triangle2 = new VertexTriangle(point3, point4, point5);

            BoundingRectangle region = _parent.Region;

            _fullScreenTriangle = new PrimitiveManager<GBufferMeshProgram>(
                PrimitiveData.FromTriangles(Culling.None, PrimitiveBufferInfo.JustPositions(), triangle1/*, triangle2*/),
                //PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), VertexQuad.ZUpQuad(region)),
                GetGBufferMaterial(region.IntWidth, region.IntHeight, forward, this));

            if (forward)
            {
                _attachmentsPerTexture = new EFramebufferAttachment?[]
                {
                    EFramebufferAttachment.ColorAttachment0, //OutputColor
                    null,
                    EFramebufferAttachment.DepthAttachment, //Depth
                };
                _colorAttachments = new DrawBuffersAttachment[]
                {
                    DrawBuffersAttachment.ColorAttachment0, //OutputColor
                };
            }
            else
            {
                _attachmentsPerTexture = new EFramebufferAttachment?[]
                {
                    EFramebufferAttachment.ColorAttachment0, //AlbedoSpec
                    EFramebufferAttachment.ColorAttachment1, //Position
                    EFramebufferAttachment.ColorAttachment2, //Normal
                    null,
                    EFramebufferAttachment.DepthAttachment, //Depth
                };
                _colorAttachments = new DrawBuffersAttachment[]
                {
                    DrawBuffersAttachment.ColorAttachment0, //AlbedoSpec
                    DrawBuffersAttachment.ColorAttachment1, //Position
                    DrawBuffersAttachment.ColorAttachment2, //Normal
                };
            }

            _fullScreenTriangle.Program.Update(this, _attachmentsPerTexture, _colorAttachments, region.IntWidth, region.IntHeight);
            _fullScreenTriangle.SettingUniforms += _fullScreenTriangle_SettingUniforms;

            _quadCamera = new OrthographicCamera();
            _quadCamera.SetGraphStyle();
            _quadCamera.Resize(1.0f, 1.0f);
            //Resize(region);
        }

        private void _fullScreenTriangle_SettingUniforms()
        {
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ViewMatrix), _quadCamera.InverseWorldMatrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ProjMatrix), _quadCamera.ProjectionMatrix);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenWidth), _parent.Width);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenHeight), _parent.Height);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.ScreenOrigin), _quadCamera.Origin);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraNearZ), _parent.Camera.NearZ);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraFarZ), _parent.Camera.FarZ);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraPosition), _parent.Camera.WorldPoint);
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraForward), _parent.Camera.GetForwardVector());
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraUp), _parent.Camera.GetUpVector());
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraRight), _parent.Camera.GetRightVector());

            Engine.Renderer.Scene.Lights.SetUniforms();
        }

        ~GBuffer()
        {
            _fullScreenTriangle.SettingUniforms -= _fullScreenTriangle_SettingUniforms;
        }
        public unsafe void Resize(int width, int height)
        {
            _fullScreenTriangle.Program.Resized(width, height);

            //VertexBuffer buffer = _fullScreenTriangle.Data[0];
            //Vec3* data = (Vec3*)buffer.Address;

            //data[0] = new Vec3(0.0f, 0.0f, -2.0f);
            //data[1] = new Vec3(region.Width, 0.0f, -2.0f);
            //data[2] = new Vec3(0.0f, region.Height, -2.0f);

            //Old method: full screen quad.
            //Not using this method because of the possible tear line between the two triangles without vsync
            //while waiting for BOTH triangles to finish rasterizing the given frame before displaying.
            // 3--2
            // |\ |
            // | \|
            // 0--1
            //0 1 3 3 1 2
            //remapped ->
            //0 1 3 2

            //data[0] = new Vec3(region.BottomLeft, 0.0f);
            //data[1] = new Vec3(region.BottomRight, 0.0f);
            //data[2] = new Vec3(region.TopLeft, 0.0f);
            //data[3] = new Vec3(region.TopRight, 0.0f);

            //_camera.Resize(region.Width, region.Height);
        }

        public void Render()
        {
            AbstractRenderer.CurrentCamera = _quadCamera;
            _fullScreenTriangle.Render(Matrix4.Identity, Matrix3.Identity);
            AbstractRenderer.CurrentCamera = null;
        }
        internal static Material GetGBufferMaterial(int width, int height, bool forward, GBuffer buffer)
        {
            //These are listed in order of appearance in the shader
            List<TextureReference> refs = forward ?
                new List<TextureReference>()
                {
                    new TextureReference("OutputColor", width, height,
                        EPixelInternalFormat.Srgb8, EPixelFormat.Bgr, EPixelType.UnsignedByte)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                    new TextureReference("Text", width, height,
                        EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                    new TextureReference("Depth", width, height,
                        EPixelInternalFormat.DepthComponent24, EPixelFormat.DepthComponent, EPixelType.Float)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                }
                :
                new List<TextureReference>()
                {
                    new TextureReference("AlbedoSpec", width, height,
                        EPixelInternalFormat.Srgb8Alpha8, EPixelFormat.Bgra, EPixelType.UnsignedByte, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                    new TextureReference("Position", width, height,
                        EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                    new TextureReference("Normal", width, height,
                        EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                    new TextureReference("Text", width, height,
                        EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                    new TextureReference("Depth", width, height,
                        EPixelInternalFormat.DepthComponent24, EPixelFormat.DepthComponent, EPixelType.Float)
                    {
                        MinFilter = MinFilter.Nearest,
                        MagFilter = MagFilter.Nearest,
                        UWrap = TexCoordWrap.Clamp,
                        VWrap = TexCoordWrap.Clamp,
                    },
                };

            return new Material("GBufferMaterial", PostProcessSettings.GetParameterList(), refs, forward ? GBufferShaderForward() : GBufferShaderDeferred());
        }
        public static Shader GBufferShaderDeferred()
        {
            string source = @"

#version 450

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D Texture3;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

out vec4 OutColor;

uniform vec3 CameraPosition;
uniform vec3 CameraForward;

" + Shader.LightingSetupBasic() + @"

void main()
{
    vec2 uv = InData.Position.xy;
    vec4 AlbedoSpec = texture(Texture0, uv);
    vec3 FragPos = texture(Texture1, uv).rgb;
    vec3 Normal = texture(Texture2, uv).rgb;
    vec4 Text = texture(Texture3, uv);

    " + Shader.LightingCalc("totalLight", "vec3(0.0)", "Normal", "FragPos", "AlbedoSpec.rgb", "AlbedoSpec.a") + @"

    OutColor = vec4(mix(totalLight, Text.rgb, Text.a), 1.0);
}";
            return new Shader(ShaderMode.Fragment, source);
        }

        public static Shader GBufferShaderForward()
        {
            string source = @"

#version 450

uniform sampler2D Texture0;
uniform sampler2D Texture1;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

out vec4 OutColor;

uniform vec3 CameraPosition;
uniform vec3 CameraForward;

void main()
{
    vec2 uv = InData.Position.xy;
    vec3 OutputColor = texture(Texture0, uv).rgb;
    vec4 Text = texture(Texture1, uv);

    OutColor = vec4(mix(OutputColor, Text.rgb, Text.a), 1.0);
}";
            return new Shader(ShaderMode.Fragment, source);
        }
    }
}
