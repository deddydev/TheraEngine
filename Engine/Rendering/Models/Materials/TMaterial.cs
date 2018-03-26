using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheraEngine.Files;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryEffects.Effect.ProfileCommon.Technique;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Rendering.Models.Materials.Functions;

namespace TheraEngine.Rendering.Models.Materials
{
    public delegate void DelSettingUniforms(int programBindingId);
    [FileExt("mat")]
    [FileDef("Material")]
    public class TMaterial : TFileObject
    {
        public event DelSettingUniforms SettingUniforms;

#if EDITOR
        public ResultFunc EditorMaterialEnd
        {
            get => _editorMaterialEnd;
            set => _editorMaterialEnd = value;
        }
        private ResultFunc _editorMaterialEnd;
#endif

        private List<ShaderFile> _geometryShaders = new List<ShaderFile>();
        private List<ShaderFile> _tessEvalShaders = new List<ShaderFile>();
        private List<ShaderFile> _tessCtrlShaders = new List<ShaderFile>();
        private List<ShaderFile> _fragmentShaders = new List<ShaderFile>();

        [TSerialize("Shaders")]
        private List<ShaderFile> _shaders = new List<ShaderFile>();

        //[TSerialize(nameof(FBODrawAttachments), Condition = "OverrideFBOAttachments")]
        //private EDrawBuffersAttachment[] _fboAttachments;
        //[TSerialize(nameof(OverrideFBOAttachments), XmlNodeType = EXmlNodeType.Attribute)]
        //private bool _overrideAttachments = false;

        private RenderProgram _program;
        //private FrameBuffer _frameBuffer;
        private UniformRequirements _requirements = UniformRequirements.None;

        protected ShaderVar[] _parameters;
        protected BaseTexRef[] _textures;

        private List<PrimitiveManager> _references = new List<PrimitiveManager>();
        private int _uniqueID = -1;

        private LocalFileRef<RenderingParameters> _renderParamsRef = new RenderingParameters();

        [Browsable(false)]
        public RenderingParameters RenderParams
        {
            get => RenderParamsRef.File;
            set => RenderParamsRef.File = value;
        }

        [TSerialize]
        public LocalFileRef<RenderingParameters> RenderParamsRef
        {
            get => _renderParamsRef;
            set => _renderParamsRef = value ?? new LocalFileRef<RenderingParameters>();
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

        [TSerialize]
        public ShaderVar[] Parameters
        {
            get => _parameters;
            set => _parameters = value;
        }

        [TSerialize(Order = 1)]
        public BaseTexRef[] Textures
        {
            get => _textures;
            set
            {
                if (_textures != null)
                    foreach (BaseTexRef t in _textures)
                        t.Material = null;
                _textures = value;
                if (_textures != null)
                    foreach (BaseTexRef t in _textures)
                        t.Material = this;
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

        //public FrameBuffer FrameBuffer
        //{
        //    get => _frameBuffer;
        //    set
        //    {
        //        _frameBuffer = value;
        //        CollectFBOAttachments();
        //    }
        //}

        public List<ShaderFile> FragmentShaders => _fragmentShaders;

        public enum UniformRequirements
        {
            None,
            NeedsCamera,
            NeedsLightsAndCamera,
        }
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public UniformRequirements Requirements
        {
            get => _requirements;
            set => _requirements = value;
        }

        //public EDrawBuffersAttachment[] FBODrawAttachments
        //{
        //    get => _fboAttachments;
        //    set
        //    {
        //        _fboAttachments = value;
        //        _overrideAttachments = true;
        //    }
        //}

        //[TSerialize]
        //public bool OverrideFBOAttachments
        //{
        //    get => _overrideAttachments;
        //    set
        //    {
        //        if (_overrideAttachments == value)
        //            return;
        //        _overrideAttachments = value;
        //        if (!_overrideAttachments)
        //            CollectFBOAttachments();
        //    }
        //}

        public static TMaterial InvalidMaterial { get; }
            = CreateUnlitColorMaterialForward(Color.Magenta);

        public EDrawBuffersAttachment[] CollectFBOAttachments()
        {
            if (_textures != null && _textures.Length > 0)
            {
                List<EDrawBuffersAttachment> fboAttachments = new List<EDrawBuffersAttachment>();
                foreach (BaseTexRef tref in _textures)
                {
                    if (!tref.FrameBufferAttachment.HasValue)
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
                return fboAttachments.ToArray();
            }
            return new EDrawBuffersAttachment[0];
        }
        //internal bool HasAttachment(EFramebufferAttachment value)
        //{
        //    switch (value)
        //    {
        //        case EFramebufferAttachment.Color:
        //        case EFramebufferAttachment.Depth:
        //        case EFramebufferAttachment.DepthAttachment:
        //        case EFramebufferAttachment.DepthStencilAttachment:
        //        case EFramebufferAttachment.Stencil:
        //        case EFramebufferAttachment.StencilAttachment:
        //            return true;
        //    }
        //    return _fboAttachments.Contains((EDrawBuffersAttachment)(int)value);
        //}
        public void GenerateTextures(bool loadSynchronously = false)
        {
            if (_textures != null)
            {
                foreach (var t in _textures)
                //await Task.Run(() => Parallel.ForEach(_textures, t =>
                {
                    t.GetTextureGeneric(loadSynchronously).PushData();
                }
                //));
            }
        }
        internal void AddReference(PrimitiveManager user)
        {
            //if (_references.Count == 0)
            //    _uniqueID = Engine.Scene.AddActiveMaterial(this);
            _references.Add(user);
        }
        internal void RemoveReference(PrimitiveManager user)
        {
            _references.Add(user);
            //if (_references.Count == 0)
            //{
            //    Engine.Scene.RemoveActiveMaterial(this);
            //    _uniqueID = -1;
            //}
        }
        public void SetUniforms(int programBindingId)
        {
            if (programBindingId <= 0)
                programBindingId = Program.BindingId;

            //Set engine uniforms
            if (Requirements == UniformRequirements.NeedsLightsAndCamera)
            {
                AbstractRenderer.CurrentCamera.SetUniforms(programBindingId);
                AbstractRenderer.Current3DScene.Lights.SetUniforms(programBindingId);
            }
            else if (Requirements == UniformRequirements.NeedsCamera)
                AbstractRenderer.CurrentCamera.SetUniforms(programBindingId);

            //Apply special rendering parameters
            if (RenderParams != null)
                Engine.Renderer.ApplyRenderParams(RenderParams);

            //Set variable uniforms
            foreach (ShaderVar v in _parameters)
                v.SetProgramUniform(_program.BindingId);

            //Set texture uniforms
            SetTextureUniforms(programBindingId);

            //Set extra uniforms
            SettingUniforms?.Invoke(programBindingId);
        }
        /// <summary>
        /// Resizes the gbuffer's textures.
        /// Note that they will still fully cover the screen regardless of 
        /// if their dimensions match or not.
        /// </summary>
        public void Resize2DTextures(int width, int height)
        {
            //Update each texture's dimensions
            foreach (BaseTexRef t in Textures)
                if (t is TexRef2D t2d)
                    t2d.Resize(width, height);
        }
        private void SetTextureUniforms(int programBindingId)
        {
            for (int i = 0; i < Textures.Length; ++i)
                SetTextureUniform(i, i, "Texture" + i, programBindingId);
        }
        public void SetTextureUniform(int textureIndex, int textureUnit, string varName, int programBindingId)
        {
            Engine.Renderer.SetActiveTexture(textureUnit);
            Engine.Renderer.Uniform(programBindingId, varName, textureUnit);
            //Engine.PrintLine("Texture unit {0} set: {1}", textureUnit, varName);
            Textures[textureIndex].GetTextureGeneric(true).Bind();
        }

        public TMaterial()
            : this("NewMaterial", new RenderingParameters()) { }

        public TMaterial(string name, params ShaderFile[] shaders)
            : this(name, new RenderingParameters(), new ShaderVar[0], new BaseTexRef[0], shaders) { }

        public TMaterial(string name, RenderingParameters renderParams, params ShaderFile[] shaders)
            : this(name, renderParams, new ShaderVar[0], new BaseTexRef[0], shaders) { }

        public TMaterial(string name, ShaderVar[] vars, params ShaderFile[] shaders)
            : this(name, new RenderingParameters(), vars, new BaseTexRef[0], shaders) { }
        
        public TMaterial(string name, RenderingParameters renderParams, ShaderVar[] vars, params ShaderFile[] shaders)
            : this(name, renderParams, vars, new BaseTexRef[0], shaders) { }

        public TMaterial(string name, BaseTexRef[] textures, params ShaderFile[] shaders)
            : this(name, new RenderingParameters(), new ShaderVar[0], textures, shaders) { }

        public TMaterial(string name, RenderingParameters renderParams, BaseTexRef[] textures, params ShaderFile[] shaders)
            : this(name, renderParams, new ShaderVar[0], textures, shaders) { }

        public TMaterial(string name, ShaderVar[] vars, BaseTexRef[] textures, params ShaderFile[] shaders)
            : this(name, new RenderingParameters(), vars, textures, shaders) { }

        public TMaterial(string name, RenderingParameters renderParams, ShaderVar[] vars, BaseTexRef[] textures, params ShaderFile[] shaders)
        {
            _name = name;
            _parameters = vars ?? new ShaderVar[0];
            Textures = textures ?? new BaseTexRef[0];
            RenderParams = renderParams ?? new RenderingParameters();

            SetShaders(shaders);
        }

        public void SetShaders(ShaderFile[] shaders)
        {
            _shaders.Clear();
            _shaders.AddRange(shaders);
            ShadersChanged();
        }
        public void AddShader(ShaderFile shader)
        {
            _shaders.Add(shader);
            ShadersChanged();
        }

        [PostDeserialize]
        private void ShadersChanged()
        {
            _fragmentShaders.Clear();
            _geometryShaders.Clear();
            _tessCtrlShaders.Clear();
            _tessEvalShaders.Clear();
            if (_program != null)
            {
                _program.Generated -= _program_Generated;
                _program.Destroy();
                _program = null;
            }

            if (_shaders != null)
                foreach (ShaderFile s in _shaders)
                {
                    switch (s.Type)
                    {
                        case ShaderMode.Vertex:
                            throw new InvalidOperationException("Vertex shaders cannot be included in materials.");
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

            if (Engine.Settings != null && Engine.Settings.AllowShaderPipelines)
            {
                _program?.Destroy();
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

        #region Basic Material Generation
        public static TMaterial CreateUnlitTextureMaterialForward(TexRef2D texture, RenderingParameters renderParams)
        {
            return new TMaterial("UnlitTextureMaterial", new BaseTexRef[] { texture },
                ShaderHelpers.UnlitTextureFragForward())
            {
                Requirements = UniformRequirements.None,
                RenderParams = renderParams,
            };
        }
        public static TMaterial CreateUnlitTextureMaterialForward()
        {
            return new TMaterial("UnlitTextureMaterial",
                ShaderHelpers.UnlitTextureFragForward())
            {
                Requirements = UniformRequirements.None,
            };
        }
        public static TMaterial CreateLitTextureMaterial() 
            => CreateLitTextureMaterial(Engine.Settings.ShadingStyle3D == ShadingStyle.Deferred);
        public static TMaterial CreateLitTextureMaterial(bool deferred)
        {
            ShaderFile frag = deferred ? ShaderHelpers.TextureFragDeferred() : ShaderHelpers.LitTextureFragForward();
            return new TMaterial("LitTextureMaterial", frag)
            {
                Requirements = deferred ? UniformRequirements.None : UniformRequirements.NeedsLightsAndCamera
            };
        }
        public static TMaterial CreateLitTextureMaterial(TexRef2D texture)
            => CreateLitTextureMaterial(texture, Engine.Settings.ShadingStyle3D == ShadingStyle.Deferred);
        public static TMaterial CreateLitTextureMaterial(TexRef2D texture, bool deferred)
        {
            ShaderFile frag = deferred ? ShaderHelpers.TextureFragDeferred() : ShaderHelpers.LitTextureFragForward();
            return new TMaterial("LitTextureMaterial", new TexRef2D[] { texture }, frag)
            {
                Requirements = deferred ? UniformRequirements.None : UniformRequirements.NeedsLightsAndCamera
            };
        }
        public static TMaterial CreateUnlitColorMaterialForward()
            => CreateUnlitColorMaterialForward(Color.DarkTurquoise);
        public static TMaterial CreateUnlitColorMaterialForward(ColorF4 color)
        {
            ShaderVar[] parameters = new ShaderVar[]
            {
                new ShaderVec4(color, "MatColor"),
            };
            return new TMaterial("UnlitColorMaterial", parameters, ShaderHelpers.UnlitColorFragForward())
            {
                Requirements = UniformRequirements.None
            };
        }
        public static TMaterial CreateLitColorMaterial() 
            => CreateLitColorMaterial(Engine.Settings.ShadingStyle3D == ShadingStyle.Deferred);
        public static TMaterial CreateLitColorMaterial(bool deferred) 
            => CreateLitColorMaterial(Color.DarkTurquoise, deferred);
        public static TMaterial CreateLitColorMaterial(ColorF4 color)
            => CreateLitColorMaterial(color, Engine.Settings.ShadingStyle3D == ShadingStyle.Deferred);
        public static TMaterial CreateLitColorMaterial(ColorF4 color, bool deferred)
        {
            ShaderVar[] parameters;
            ShaderFile frag;
            if (deferred)
            {
                frag = ShaderHelpers.LitColorFragDeferred();
                parameters = new ShaderVar[]
                {
                    new ShaderVec3((ColorF3)color, "BaseColor"),
                    new ShaderFloat(color.A, "Opacity"),
                    new ShaderFloat(1.0f, "Specular"),
                    new ShaderFloat(0.0f, "Roughness"),
                    new ShaderFloat(0.0f, "Metallic"),
                    new ShaderFloat(1.0f, "IndexOfRefraction"),
                };
            }
            else
            {
                frag = ShaderHelpers.LitColorFragForward();
                parameters = new ShaderVar[]
                {
                    new ShaderVec4(color, "MatColor"),
                    new ShaderFloat(20.0f, "MatSpecularIntensity"),
                    // ShaderFloat(128.0f, "MatShininess"),
                };
            }

            return new TMaterial("LitColorMaterial", parameters, frag)
            {
                Requirements = deferred ? UniformRequirements.None : UniformRequirements.NeedsLightsAndCamera
            };
        }

        /// <summary>
        /// Creates a Blinn lighting model material for a forward renderer.
        /// </summary>
        public static TMaterial CreateBlinnMaterial(
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

            ShaderFile s = new ShaderFile(ShaderMode.Fragment, source);
            return new TMaterial("BlinnMaterial", parameters, s)
            {
                Requirements = UniformRequirements.NeedsLightsAndCamera
            };
        }
        #endregion
    }
}
