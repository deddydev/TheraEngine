using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheraEngine.Rendering
{
    internal class GBuffer : FrameBuffer
    {
        OrthographicCamera _quadCamera;
        PrimitiveManager _fullScreenTriangle;
        bool _forward;
        Viewport _parent;

        public TextureReference[] Textures => _fullScreenTriangle?.Material.TexRefs;
        
        public GBuffer(Viewport viewport, bool forward)
        {
            _parent = viewport;
            _forward = forward;

            Vertex point0 = new Vec3(0.0f, 0.0f, 0.0f);
            Vertex point1 = new Vec3(2.0f, 0.0f, 0.0f);
            Vertex point2 = new Vec3(0.0f, 2.0f, 0.0f);
            VertexTriangle triangle1 = new VertexTriangle(point0, point1, point2);

            BoundingRectangle region = _parent.Region;

            Material m = GetGBufferMaterial(region.IntWidth, region.IntHeight, forward, this, DepthStencilUse.Depth32f);
            m.FrameBuffer = this;

            _fullScreenTriangle = new PrimitiveManager(PrimitiveData.FromTriangles(Culling.None, VertexShaderDesc.JustPositions(), triangle1), m);
            
            Bind(EFramebufferTarget.Framebuffer);
            _fullScreenTriangle.Material.GenerateTextures();
            FramebufferErrorCode c = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (c != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Problem compiling G-Buffer.");
            Unbind(EFramebufferTarget.Framebuffer);

            _fullScreenTriangle.SettingUniforms += SetUniforms;

            _quadCamera = new OrthographicCamera() { NearZ = -0.5f, FarZ = 0.5f };
            _quadCamera.SetGraphStyle();
            _quadCamera.Resize(1.0f, 1.0f);
        }

        private void SetUniforms()
        {
            int fragId = Engine.Settings.AllowShaderPipelines ? _fullScreenTriangle.Material.Program.BindingId : _fullScreenTriangle._vsFragProgram.BindingId;

            _parent.Camera.PostProcessSettings.SetUniforms(fragId);
            
            Engine.Renderer.ProgramUniform(fragId, Uniform.GetLocation(fragId, ECommonUniform.CameraPosition), _parent.Camera.WorldPoint);

            if (Engine.Settings.ShadingStyle == ShadingStyle.Deferred)
                Engine.Scene.Lights.SetUniforms(fragId);
        }

        public unsafe void Resize(int width, int height)
        {
            _fullScreenTriangle.Material.ResizeTextures(width, height);
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
            TextureReference[] refs = forward ?
                new TextureReference[]
                {
                    new TextureReference("OutputColor", width, height,
                        EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Clamp,
                        VWrap = ETexWrapMode.Clamp,
                        FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                    },
                    new TextureReference("Depth", width, height,
                        EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Clamp,
                        VWrap = ETexWrapMode.Clamp,
                        FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                    },
                } : 
                new TextureReference[]
                {
                    new TextureReference("AlbedoSpec", width, height,
                        EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Clamp,
                        VWrap = ETexWrapMode.Clamp,
                        FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                    },
                    new TextureReference("Position", width, height,
                        EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Clamp,
                        VWrap = ETexWrapMode.Clamp,
                        FrameBufferAttachment = EFramebufferAttachment.ColorAttachment1,
                    },
                    new TextureReference("Normal", width, height,
                        EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Clamp,
                        VWrap = ETexWrapMode.Clamp,
                        FrameBufferAttachment = EFramebufferAttachment.ColorAttachment2,
                    },
                    new TextureReference("Depth", width, height,
                        EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Clamp,
                        VWrap = ETexWrapMode.Clamp,
                        FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                    },
                };

            Shader shader = forward ? GBufferShaderForward() : GBufferShaderDeferred();
            //Debug.WriteLine(shader._source);
            return new Material("GBufferMaterial", new ShaderVar[0], refs, shader);
        }
        internal static Shader GBufferShaderDeferred()
        {
            string source = @"
#version 450
//GBUFFER FRAG SHADER

out vec4 OutColor;

in vec3 FragPos;

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D Texture3;

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
        internal static Shader GBufferShaderForward()
        {
            string source = @"
#version 450
//GBUFFER FRAG SHADER

out vec4 OutColor;

in vec3 FragPos;

uniform sampler2D Texture0;
uniform sampler2D Texture1;

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
        /// <summary>
        /// Takes the HDR scene color, tone maps to LDR and performs post processing.
        /// </summary>
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
    }
}
