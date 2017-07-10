using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering
{
    internal class GBufferMeshProgram : MeshProgram
    {
        EDrawBuffersAttachment[] _attachments;
        EFramebufferAttachment?[] _attachmentsPerTexture;
        GBuffer _buffer;
        int _width, _height;
        
        public GBufferMeshProgram(Material material, VertexShaderDesc info) : base(material, info) { }

        public void Update(GBuffer buffer, EFramebufferAttachment?[] attachmentsPerTexture, EDrawBuffersAttachment[] attachments, int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = buffer;
            _attachments = attachments;
            _attachmentsPerTexture = attachmentsPerTexture;
            
            _buffer.Bind(EFramebufferTarget.Framebuffer);
            for (int i = 0; i < Textures.Length; ++i)
            {
                Texture2D t = _textures[i];
                t.Index = i;
                t.PostPushData += TexturePostPushData;
                t.Generate();
            }

            FramebufferErrorCode c = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (c != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Problem compiling G-Buffer.");
            _buffer.Unbind(EFramebufferTarget.Framebuffer);
        }
        
        private void TexturePostPushData(int index)
        {
            if (_attachmentsPerTexture[index].HasValue)
                Engine.Renderer.AttachTextureToFrameBuffer(EFramebufferTarget.Framebuffer, _attachmentsPerTexture[index].Value, ETexTarget.Texture2D, Textures[index].BindingId, 0);
        }
        protected internal override void BindTextures()
        {
            //We want to bind textures to the gbuffer specifically
            _buffer.Bind(EFramebufferTarget.Framebuffer);
            base.BindTextures();
            //Make sure the shaders know what attachments to draw to
            Engine.Renderer.SetDrawBuffers(_attachments);
            //Unbind gbuffer
            _buffer.Unbind(EFramebufferTarget.Framebuffer);
        }
        /// <summary>
        /// Resizes the gbuffer's textures.
        /// Note that they will still fully cover the screen regardless of 
        /// if their dimensions match or not.
        /// </summary>
        public void Resized(int width, int height)
        {
            //Update each texture's dimensions
            foreach (Texture2D t in Textures)
                t.Resize(width, height);
        }

        public event Action SettingUniforms;

        public override void SetUniforms()
        {
            foreach (GLVar v in _parameters)
                v.SetUniform();

            SettingUniforms?.Invoke();

            if (Engine.Settings.ShadingStyle == ShadingStyle.Deferred)
                Engine.Scene.Lights.SetUniforms();

            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.RenderDelta), Engine.RenderDelta);
        }
    }
    internal class GBuffer : FrameBuffer
    {
        OrthographicCamera _quadCamera;
        PrimitiveManager<GBufferMeshProgram> _fullScreenTriangle;
        bool _forward;
        Viewport _parent;

        public Texture2D[] Textures => _fullScreenTriangle?.Program.Textures;

        EFramebufferAttachment?[] _attachmentsPerTexture;
        EDrawBuffersAttachment[] _colorAttachments;

        public GBuffer(Viewport viewport, bool forward)
        {
            _parent = viewport;
            _forward = forward;

            Vertex point0 = new Vec3(0.0f, 0.0f, 0.0f);
            Vertex point1 = new Vec3(2.0f, 0.0f, 0.0f);
            Vertex point2 = new Vec3(0.0f, 2.0f, 0.0f);
            VertexTriangle triangle1 = new VertexTriangle(point0, point1, point2);

            BoundingRectangle region = _parent.Region;

            _fullScreenTriangle = new PrimitiveManager<GBufferMeshProgram>(
                PrimitiveData.FromTriangles(Culling.None, VertexShaderDesc.JustPositions(), triangle1),
                GetGBufferMaterial(region.IntWidth, region.IntHeight, forward, this, DepthStencilUse.Depth32f));

            if (forward)
            {
                _attachmentsPerTexture = new EFramebufferAttachment?[]
                {
                    EFramebufferAttachment.ColorAttachment0, //OutputColor
                    EFramebufferAttachment.DepthAttachment, //Depth
                };
                _colorAttachments = new EDrawBuffersAttachment[]
                {
                    EDrawBuffersAttachment.ColorAttachment0, //OutputColor
                };
            }
            else
            {
                _attachmentsPerTexture = new EFramebufferAttachment?[]
                {
                    EFramebufferAttachment.ColorAttachment0, //AlbedoSpec
                    EFramebufferAttachment.ColorAttachment1, //Position
                    EFramebufferAttachment.ColorAttachment2, //Normal
                    EFramebufferAttachment.DepthAttachment, //Depth
                };
                _colorAttachments = new EDrawBuffersAttachment[]
                {
                    EDrawBuffersAttachment.ColorAttachment0, //AlbedoSpec
                    EDrawBuffersAttachment.ColorAttachment1, //Position
                    EDrawBuffersAttachment.ColorAttachment2, //Normal
                };
            }

            _fullScreenTriangle.Program.Update(this, _attachmentsPerTexture, _colorAttachments, _parent.Region.IntWidth, _parent.Region.IntHeight);
            _fullScreenTriangle.Program.SettingUniforms += Program_SettingUniforms;
            //_fullScreenTriangle.SettingUniforms += _fullScreenTriangle_SettingUniforms;

            _quadCamera = new OrthographicCamera() { NearZ = -0.5f, FarZ = 0.5f };
            _quadCamera.SetGraphStyle();
            _quadCamera.Resize(1.0f, 1.0f);
            //Resize(region);
        }

        private void Program_SettingUniforms()
        {
            _parent.Camera.PostProcessSettings.SetUniforms();
            AbstractRenderer.CurrentCamera.SetUniforms();
            Engine.Renderer.Uniform(Uniform.GetLocation(ECommonUniform.CameraPosition), _parent.Camera.WorldPoint);
        }

        public unsafe void Resize(int width, int height)
        {
            _fullScreenTriangle.Program.Resized(width, height);
        }

        public void Render()
        {
            AbstractRenderer.PushCurrentCamera(_quadCamera);
            _fullScreenTriangle.Render(Matrix4.Identity, Matrix3.Identity);
            AbstractRenderer.PopCurrentCamera();
        }
        public enum DepthStencilUse
        {
            None,
            Depth24,
            Depth32,
            Depth32f,
            Stencil8,
            Depth24Stencil8,
            Depth32Stencil8,
        }
        internal static Material GetGBufferMaterial(int width, int height, bool forward, GBuffer buffer, DepthStencilUse depthStencilUse)
        {
            //These are listed in order of appearance in the shader
            List<TextureReference> refs = forward ?
                new List<TextureReference>()
                {
                    new TextureReference("OutputColor", width, height,
                        EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte)
                    {
                        MinFilter = EMinFilter.Nearest,
                        MagFilter = EMagFilter.Nearest,
                        UWrap = ETexCoordWrap.Clamp,
                        VWrap = ETexCoordWrap.Clamp,
                    },
                    //new TextureReference("Text", width, height,
                    //    EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    //{
                    //    MinFilter = MinFilter.Nearest,
                    //    MagFilter = MagFilter.Nearest,
                    //    UWrap = TexCoordWrap.Clamp,
                    //    VWrap = TexCoordWrap.Clamp,
                    //},
                    new TextureReference("Depth", width, height,
                        EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float)
                    {
                        MinFilter = EMinFilter.Nearest,
                        MagFilter = EMagFilter.Nearest,
                        UWrap = ETexCoordWrap.Clamp,
                        VWrap = ETexCoordWrap.Clamp,
                    },
                }
                :
                new List<TextureReference>()
                {
                    new TextureReference("AlbedoSpec", width, height,
                        EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte)
                    {
                        MinFilter = EMinFilter.Nearest,
                        MagFilter = EMagFilter.Nearest,
                        UWrap = ETexCoordWrap.Clamp,
                        VWrap = ETexCoordWrap.Clamp,
                    },
                    new TextureReference("Position", width, height,
                        EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float)
                    {
                        MinFilter = EMinFilter.Nearest,
                        MagFilter = EMagFilter.Nearest,
                        UWrap = ETexCoordWrap.Clamp,
                        VWrap = ETexCoordWrap.Clamp,
                    },
                    new TextureReference("Normal", width, height,
                        EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float)
                    {
                        MinFilter = EMinFilter.Nearest,
                        MagFilter = EMagFilter.Nearest,
                        UWrap = ETexCoordWrap.Clamp,
                        VWrap = ETexCoordWrap.Clamp,
                    },
                    //new TextureReference("Text", width, height,
                    //    EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    //{
                    //    MinFilter = MinFilter.Nearest,
                    //    MagFilter = MagFilter.Nearest,
                    //    UWrap = TexCoordWrap.Clamp,
                    //    VWrap = TexCoordWrap.Clamp,
                    //},
                    new TextureReference("Depth", width, height,
                        EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float)
                    {
                        MinFilter = EMinFilter.Nearest,
                        MagFilter = EMagFilter.Nearest,
                        UWrap = ETexCoordWrap.Clamp,
                        VWrap = ETexCoordWrap.Clamp,
                    },
                };

            return new Material("GBufferMaterial", new List<GLVar>(), refs, forward ? GBufferShaderForward() : GBufferShaderDeferred());
        }
        internal static Shader GBufferShaderDeferred()
        {
            string source = @"

#version 450

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D Texture3;

in vec3 FragPos;

out vec4 OutColor;

uniform vec3 CameraPosition;
uniform vec3 CameraForward;
uniform float CameraNearZ;
uniform float CameraFarZ;
uniform float ScreenWidth;
uniform float ScreenHeight;
uniform float ScreenOrigin;
uniform float ProjOrigin;
uniform float ProjRange;
uniform float InvViewMatrix;
uniform float InvProjMatrix;

" + PostProcessSettings.ShaderSetup() + @"
" + ShaderHelpers.LightingSetupBasic() + @"

void main()
{
    vec2 uv = FragPos.xy;
    vec4 AlbedoSpec = texture(Texture0, uv);
    vec3 FragPos = texture(Texture1, uv).rgb;
    vec3 Normal = texture(Texture2, uv).rgb;
    float Depth = texture(Texture3, uv).r;

    " + ShaderHelpers.LightingCalc("totalLight", "GlobalAmbient", "Normal", "FragPos", "AlbedoSpec.rgb", "AlbedoSpec.a") + @"

    vec3 hdrSceneColor = AlbedoSpec.rgb * totalLight;

    " + PostProcessPart() + @"
}";
            return new Shader(ShaderMode.Fragment, source);
        }
        private static string PostProcessPart()
        {
            return @"
    //Color grading
    hdrSceneColor *= ColorGrade.Tint;

    //Tone mapping
    vec3 ldrSceneColor = vec3(1.0) - exp(-hdrSceneColor * ColorGrade.Exposure);
    
    //Vignette
    //float alpha = clamp(pow(distance(uv, vec2(0.5)), Vignette.Intensity), 0.0, 1.0);
    //vec4 smoothed = smoothstep(vec4(1.0), Vignette.Color, Vignette.Color * vec4(alpha));
    //ldrSceneColor = mix(ldrSceneColor, smoothed.rgb, alpha * smoothed.a);

    //Gamma-correct
    vec3 gammaCorrected = pow(ldrSceneColor, vec3(1.0 / ColorGrade.Gamma));

    OutColor = vec4(gammaCorrected, 1.0);";
        }
        internal static Shader GBufferShaderForward()
        {
            string source = @"

#version 450

uniform sampler2D Texture0;
uniform sampler2D Texture1;

in vec3 FragPos;

out vec4 OutColor;

uniform vec3 CameraPosition;
uniform vec3 CameraForward;
uniform float CameraNearZ;
uniform float CameraFarZ;
uniform float ScreenWidth;
uniform float ScreenHeight;
uniform float ScreenOrigin;

void main()
{
    vec2 uv = FragPos.xy;
    vec3 hdrSceneColor = texture(Texture0, uv).rgb;
    float Depth = texture(Texture1, uv).r;

    " + PostProcessPart() + @"
}";
            return new Shader(ShaderMode.Fragment, source);
        }
    }
}
