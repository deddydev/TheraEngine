using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheraEngine.Files;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryEffects.Effect.ProfileCommon.Technique;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileClass("TMAT", "")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Material : FileObject
    {
        public event Action SettingUniforms;

        private List<Shader> _geometryShaders;
        private List<Shader> _tessEvalShaders;
        private List<Shader> _tessCtrlShaders;
        private List<Shader> _fragmentShaders;
        private Shader[] _shaders;
        private EDrawBuffersAttachment[] _fboAttachments;
        private bool _overrideAttachments = false;

        private RenderProgram _program;
        private FrameBuffer _frameBuffer;
        private UniformRequirements _requirements = UniformRequirements.None;

        protected ShaderVar[] _parameters;
        protected TextureReference[] _textures;

        private List<PrimitiveManager> _references = new List<PrimitiveManager>();
        private int _uniqueID = -1;

        private RenderingParameters _renderParams = new RenderingParameters();

        public RenderingParameters RenderParams
        {
            get => _renderParams;
            set => _renderParams = value;
        }
        /// <summary>
        /// Retrieves the material's uniform parameter at the given index.
        /// Use this to set uniform values to be passed to the fragment shader.
        /// </summary>
        public T2 Parameter<T2>(int index) where T2 : ShaderVar
        {
            if (index >= 0 && index < Parameters.Length)
                return Parameters[index] as T2;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// Retrieves the material's uniform parameter with the given name.
        /// Use this to set uniform values to be passed to the fragment shader.
        /// </summary>
        public T2 Parameter<T2>(string name) where T2 : ShaderVar
            => Parameters.FirstOrDefault(x => x.Name == name) as T2;
        
        public ShaderVar[] Parameters => _parameters;
        public TextureReference[] TexRefs
        {
            get => _textures;
            set
            {
                _textures = value;
                CollectFBOAttachments();
            }
        }
        public int UniqueID => _uniqueID;
        public RenderProgram Program
        {
            get
            {
                if (_program != null && !_program.IsActive)
                    _program.Generate();
                return _program;
            }
        }

        public FrameBuffer FrameBuffer
        {
            get => _frameBuffer;
            set
            {
                _frameBuffer = value;
                CollectFBOAttachments();
            }
        }

        public List<Shader> FragmentShaders => _fragmentShaders;

        public enum UniformRequirements
        {
            None,
            NeedsCamera,
            NeedsLightsAndCamera,
        }
        
        public UniformRequirements Requirements
        {
            get => _requirements;
            set => _requirements = value;
        }

        public EDrawBuffersAttachment[] FboAttachments
        {
            get => _fboAttachments;
            set
            {
                _fboAttachments = value;
                _overrideAttachments = true;
            }
        }

        public bool OverrideAttachments
        {
            get => _overrideAttachments;
            set
            {
                if (_overrideAttachments == value)
                    return;
                _overrideAttachments = value;
                if (!_overrideAttachments)
                    CollectFBOAttachments();
            }
        }

        internal void CollectFBOAttachments()
        {
            if (_frameBuffer != null && _textures != null && _textures.Length > 0)
            {
                List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
                foreach (TextureReference tref in _textures)
                {
                    tref.Material = this;
                    if (!tref.FrameBufferAttachment.HasValue || _overrideAttachments)
                        continue;
                    switch (tref.FrameBufferAttachment.Value)
                    {
                        case EFramebufferAttachment.Color:
                        case EFramebufferAttachment.Depth:
                        case EFramebufferAttachment.DepthAttachment:
                        case EFramebufferAttachment.DepthStencilAttachment:
                        case EFramebufferAttachment.Stencil:
                        case EFramebufferAttachment.StencilAttachment:
                            continue;
                    }
                    fboAttachments.Add((EDrawBuffersAttachment)(int)tref.FrameBufferAttachment.Value);
                }
                if (!_overrideAttachments)
                    _fboAttachments = fboAttachments.ToArray();
            }
            else if(!_overrideAttachments)
                _fboAttachments = null;
        }
        internal bool HasAttachment(EFramebufferAttachment value)
        {
            switch (value)
            {
                case EFramebufferAttachment.Color:
                case EFramebufferAttachment.Depth:
                case EFramebufferAttachment.DepthAttachment:
                case EFramebufferAttachment.DepthStencilAttachment:
                case EFramebufferAttachment.Stencil:
                case EFramebufferAttachment.StencilAttachment:
                    return true;
            }
            return _fboAttachments.Contains((EDrawBuffersAttachment)(int)value);
        }
        public void GenerateTextures()
        {
            if (_textures != null)
                foreach (TextureReference t in _textures)
                    if (t.Texture.IsActive)
                        t.Texture.PushData();
                    else
                        t.Texture.Generate();
        }
        internal void AddReference(PrimitiveManager user)
        {
            if (_references.Count == 0)
                _uniqueID = Engine.Scene.AddActiveMaterial(this);
            _references.Add(user);
        }
        internal void RemoveReference(PrimitiveManager user)
        {
            _references.Add(user);
            if (_references.Count == 0)
            {
                Engine.Scene.RemoveActiveMaterial(this);
                _uniqueID = -1;
            }
        }
        public void SetUniforms(int programBindingId = 0)
        {
            if (programBindingId <= 0)
                programBindingId = Program.BindingId;

            if (Requirements == UniformRequirements.NeedsLightsAndCamera)
            {
                AbstractRenderer.CurrentCamera.SetUniforms(programBindingId);
                Engine.Scene.Lights.SetUniforms(programBindingId);
            }
            else if (Requirements == UniformRequirements.NeedsCamera)
                AbstractRenderer.CurrentCamera.SetUniforms(programBindingId);
            
            if (RenderParams != null)
                Engine.Renderer.ApplyRenderParams(RenderParams);

            foreach (ShaderVar v in _parameters)
                v.SetProgramUniform(_program.BindingId);

            SetTextureUniforms(programBindingId);

            SettingUniforms?.Invoke();
        }
        /// <summary>
        /// Resizes the gbuffer's textures.
        /// Note that they will still fully cover the screen regardless of 
        /// if their dimensions match or not.
        /// </summary>
        public void ResizeTextures(int width, int height)
        {
            //Update each texture's dimensions
            foreach (TextureReference t in TexRefs)
                t.Resize(width, height);
        }
        private void SetTextureUniforms(int programBindingId)
        {
            for (int i = 0; i < TexRefs.Length; ++i)
                SetTextureUniform(i, i, "Texture" + i, programBindingId);
        }
        public void SetTextureUniform(int textureIndex, int textureUnit, string varName, int programBindingId)
        {
            Engine.Renderer.SetActiveTexture(textureUnit);
            Engine.Renderer.Uniform(programBindingId, varName, textureUnit);
            TexRefs[textureIndex].Texture.Bind();
        }

        public Material()
            : this("NewMaterial", new ShaderVar[0], new TextureReference[0]) { }

        public Material(string name, params Shader[] shaders) 
            : this(name, new ShaderVar[0], new TextureReference[0], shaders) { }
        public Material(string name, ShaderVar[] parameters, params Shader[] shaders)
            : this(name, parameters, new TextureReference[0], shaders) { }
        public Material(string name, TextureReference[] textures, params Shader[] shaders)
            : this(name, new ShaderVar[0], textures, shaders) { }
        public Material(string name, ShaderVar[] parameters, TextureReference[] textures, params Shader[] shaders)
        {
            _name = name;
            _parameters = parameters ?? new ShaderVar[0];
            TexRefs = textures ?? new TextureReference[0];

            _shaders = shaders;
            _fragmentShaders = new List<Shader>();
            _geometryShaders = new List<Shader>();
            _tessCtrlShaders = new List<Shader>();
            _tessEvalShaders = new List<Shader>();

            if (shaders != null)
                foreach (Shader s in shaders)
                {
                    switch (s.ShaderType)
                    {
                        case ShaderMode.Vertex:
                            throw new Exception();
                        case ShaderMode.Fragment:
                            _fragmentShaders.Add(s);
                            break;
                        case ShaderMode.Geometry:
                            _geometryShaders.Add(s);
                            break;
                        case ShaderMode.TessControl:
                            _tessCtrlShaders.Add(s);
                            break;
                        case ShaderMode.TessEvaluation:
                            _tessEvalShaders.Add(s);
                            break;
                    }
                }

            if (Engine.Settings.AllowShaderPipelines)
            {
                _program = new RenderProgram(_shaders);
                _program.Generated += _program_Generated;
            }
        }

        private void _program_Generated()
        {
            //foreach (ShaderVar v in _parameters)
            //{
            //    v.SetProgramUniform(_program.BindingId);
            //    v.ValueChanged += V_ValueChanged;
            //}
        }

        //private void V_ValueChanged(ShaderVar v)
        //{
        //    v.SetProgramUniform(_program.BindingId);
        //    ErrorCode r = GL.GetError();
        //    if (r != ErrorCode.NoError)
        //        Engine.DebugPrint(r.ToString());
        //}

        public static Material GetUnlitTextureMaterialForward(TextureReference texture)
        {
            return new Material("UnlitTextureMaterial",
                new TextureReference[] { texture },
                ShaderHelpers.UnlitTextureFragForward())
            {
                Requirements = UniformRequirements.None,
            };
        }
        public static Material GetUnlitTextureMaterialForward()
        {
            return new Material("UnlitTextureMaterial", ShaderHelpers.UnlitTextureFragForward())
            {
                Requirements = UniformRequirements.None,
            };
        }
        public static Material GetLitTextureMaterial() => GetLitTextureMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitTextureMaterial(bool deferred)
        {
            Shader frag = deferred ? ShaderHelpers.TextureFragDeferred() : ShaderHelpers.LitTextureFragForward();
            return new Material("LitTextureMaterial", frag)
            {
                Requirements = deferred ? UniformRequirements.None : UniformRequirements.NeedsLightsAndCamera
            };
        }
        public static Material GetLitTextureMaterial(TextureReference texture) => GetLitTextureMaterial(texture, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitTextureMaterial(TextureReference texture, bool deferred)
        {
            Shader frag = deferred ? ShaderHelpers.TextureFragDeferred() : ShaderHelpers.LitTextureFragForward();
            return new Material("LitTextureMaterial", new TextureReference[] { texture }, frag)
            {
                Requirements = deferred ? UniformRequirements.None : UniformRequirements.NeedsLightsAndCamera
            };
        }
        public static Material GetUnlitColorMaterialForward()
            => GetUnlitColorMaterialForward(Color.DarkTurquoise);
        public static Material GetUnlitColorMaterialForward(ColorF4 color)
        {
            ShaderVar[] parameters = new ShaderVar[]
            {
                new ShaderVec4(color, "MatColor"),
            };
            return new Material("UnlitColorMaterial", parameters, ShaderHelpers.UnlitColorFragForward())
            {
                Requirements = UniformRequirements.None
            };
        }
        public static Material GetLitColorMaterial() => GetLitColorMaterial(Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(bool deferred) => GetLitColorMaterial(Color.DarkTurquoise, deferred);
        public static Material GetLitColorMaterial(ColorF4 color) => GetLitColorMaterial(color, Engine.Settings.ShadingStyle == ShadingStyle.Deferred);
        public static Material GetLitColorMaterial(ColorF4 color, bool deferred)
        {
            ShaderVar[] parameters = new ShaderVar[]
            {
                new ShaderVec4(color, "MatColor"),
                new ShaderFloat(20.0f, "MatSpecularIntensity"),
                // ShaderFloat(128.0f, "MatShininess"),
            };
            return new Material("LitColorMaterial", parameters, 
                deferred ? ShaderHelpers.LitColorFragDeferred() : ShaderHelpers.LitColorFragForward())
            {
                Requirements = deferred ? UniformRequirements.None : UniformRequirements.NeedsLightsAndCamera
            };
        }

        public static Material GetBlinnMaterial(
            Vec3? emission,
            Vec3? ambient,
            Vec3? diffuse,
            Vec3? specular,
            float shininess,
            float transparency,
            Vec3 transparent,
            EOpaque transparencyMode,
            float reflectivity,
            Vec3 reflective,
            float indexOfRefraction)
        {
            // color = emission + ambient * al + diffuse * max(N * L, 0) + specular * max(H * N, 0) ^ shininess
            // where:
            // • al – A constant amount of ambient light contribution coming from the scene.In the COMMON
            // profile, this is the sum of all the <light><technique_common><ambient> values in the <visual_scene>.
            // • N – Normal vector (normalized)
            // • L – Light vector (normalized)
            // • I – Eye vector (normalized)
            // • H – Half-angle vector, calculated as halfway between the unit Eye and Light vectors, using the equation H = normalize(I + L)

            int count = 0;
            if (emission.HasValue) ++count;
            if (ambient.HasValue) ++count;
            if (diffuse.HasValue) ++count;
            if (specular.HasValue) ++count;
            ShaderVar[] parameters = new ShaderVar[count + 1];
            count = 0;

            string source = "#version 450\n";

            if (emission.HasValue)
            {
                source += "uniform vec3 Emission;\n";
                parameters[count++] = new ShaderVec3(emission.Value, "Emission");
            }
            else
                source += "uniform sampler2D Emission;\n";

            if (ambient.HasValue)
            {
                source += "uniform vec3 Ambient;\n";
                parameters[count++] = new ShaderVec3(ambient.Value, "Ambient");
            }
            else
                source += "uniform sampler2D Ambient;\n";

            if (diffuse.HasValue)
            {
                source += "uniform vec3 Diffuse;\n";
                parameters[count++] = new ShaderVec3(diffuse.Value, "Diffuse");
            }
            else
                source += "uniform sampler2D Diffuse;\n";

            if (specular.HasValue)
            {
                source += "uniform vec3 Specular;\n";
                parameters[count++] = new ShaderVec3(specular.Value, "Specular");
            }
            else
                source += "uniform sampler2D Specular;\n";

            source += "uniform float Shininess;\n";
            parameters[count++] = new ShaderFloat(shininess, "Shininess");

            if (transparencyMode == EOpaque.RGB_ZERO ||
                transparencyMode == EOpaque.RGB_ONE)
            source += @"
float luminance(in vec3 color)
{
    return (color.r * 0.212671) + (color.g * 0.715160) + (color.b * 0.072169);
}";

            switch (transparencyMode)
            {
                case EOpaque.A_ONE:
                    source += "\nresult = mix(fb, mat, transparent.a * transparency);";
                    break;
                case EOpaque.RGB_ZERO: source += @"
result.rgb = fb.rgb * (transparent.rgb * transparency) + mat.rgb * (1.0f - transparent.rgb * transparency);
result.a = fb.a * (luminance(transparent.rgb) * transparency) + mat.a * (1.0f - luminance(transparent.rgb) * transparency);";
                    break;
                case EOpaque.A_ZERO:
                    source += "\nresult = mix(mat, fb, transparent.a * transparency);";
                    break;
                case EOpaque.RGB_ONE: source += @"
result.rgb = fb.rgb * (1.0f - transparent.rgb * transparency) + mat.rgb * (transparent.rgb * transparency);
result.a = fb.a * (1.0f - luminance(transparent.rgb) * transparency) + mat.a * (luminance(transparent.rgb) * transparency);";
                    break;
            }

           
//#version 450

//layout (location = 0) out vec4 OutColor;

//uniform vec4 MatColor;
//uniform float MatSpecularIntensity;
//uniform float MatShininess;

//uniform vec3 CameraPosition;
//uniform vec3 CameraForward;

//in vec3 FragPos;
//in vec3 FragNorm;

//" + LightingSetupBasic() + @"

//void main()
//{
//    vec3 normal = normalize(FragNorm);

//    " + LightingCalcForward() + @"

//    OutColor = MatColor * vec4(totalLight, 1.0);
//}

            Shader s = new Shader(ShaderMode.Fragment, source);
            return new Material("BlinnMaterial", parameters, s)
            {
                Requirements = UniformRequirements.NeedsLightsAndCamera
            };
        }
    }
}
