using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Rendering.Models.Materials.Functions;
using static TheraEngine.Rendering.Models.Collada.COLLADA.LibraryEffects.Effect.ProfileCommon.Technique;

namespace TheraEngine.Rendering.Models.Materials
{
    [FileExt("mat")]
    [FileDef("Material")]
    public class TMaterial : TMaterialBase
    {
        public static TMaterial InvalidMaterial { get; }
            = CreateUnlitColorMaterialForward(Color.Magenta);

        protected override void OnSetUniforms(int programBindingId)
        {
            //Set engine uniforms
            if (Requirements.HasFlag(UniformRequirements.NeedsCamera))
                AbstractRenderer.CurrentCamera.SetUniforms(programBindingId);
            if (Requirements.HasFlag(UniformRequirements.NeedsLights))
                AbstractRenderer.Current3DScene.Lights.SetUniforms(programBindingId);
        }

#if EDITOR
        [TSerialize]
        public ResultFunc EditorMaterialEnd
        {
            get => _editorMaterialEnd;
            set => _editorMaterialEnd = value;
        }
        private ResultFunc _editorMaterialEnd;
#endif

        private List<GLSLShaderFile> _geometryShaders = new List<GLSLShaderFile>();
        private List<GLSLShaderFile> _tessEvalShaders = new List<GLSLShaderFile>();
        private List<GLSLShaderFile> _tessCtrlShaders = new List<GLSLShaderFile>();
        private List<GLSLShaderFile> _fragmentShaders = new List<GLSLShaderFile>();
        private List<GLSLShaderFile> _vertexShaders = new List<GLSLShaderFile>();
        
        [TSerialize("Shaders")]
        private List<GLSLShaderFile> _shaders = new List<GLSLShaderFile>();
        
        private UniformRequirements _requirements = UniformRequirements.None;

        public List<GLSLShaderFile> FragmentShaders => _fragmentShaders;
        public List<GLSLShaderFile> GeometryShaders => _geometryShaders;
        public List<GLSLShaderFile> TessEvalShaders => _tessEvalShaders;
        public List<GLSLShaderFile> TessCtrlShaders => _tessCtrlShaders;
        public List<GLSLShaderFile> VertexShaders => _vertexShaders;
        
        [Flags]
        public enum UniformRequirements
        {
            None = 0,
            NeedsCamera = 1,
            NeedsLights = 2,
            NeedsLightsAndCamera = 3,
        }
        
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public UniformRequirements Requirements
        {
            get => _requirements;
            set => _requirements = value;
        }

        public TMaterial()
            : this("NewMaterial", new RenderingParameters()) { }

        public TMaterial(string name, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, null, new ShaderVar[0], new BaseTexRef[0], shaders) { }

        public TMaterial(string name, RenderingParameters renderParams, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, renderParams, new ShaderVar[0], new BaseTexRef[0], shaders) { }

        public TMaterial(string name, ShaderVar[] vars, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, null, vars, new BaseTexRef[0], shaders) { }
        
        public TMaterial(string name, RenderingParameters renderParams, ShaderVar[] vars, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, renderParams, vars, new BaseTexRef[0], shaders) { }

        public TMaterial(string name, BaseTexRef[] textures, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, null, new ShaderVar[0], textures, shaders) { }

        public TMaterial(string name, RenderingParameters renderParams, BaseTexRef[] textures, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, renderParams, new ShaderVar[0], textures,  shaders) { }

        public TMaterial(string name, ShaderVar[] vars, BaseTexRef[] textures, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, null, vars, textures,shaders) { }

        public TMaterial(string name, RenderingParameters renderParams, ShaderVar[] vars, BaseTexRef[] textures, params GLSLShaderFile[] shaders)
            : this(name, UniformRequirements.None, renderParams, vars, textures, shaders) { }

        public TMaterial(string name, UniformRequirements requirements, params GLSLShaderFile[] shaders)
            : this(name, requirements, null, new ShaderVar[0], new BaseTexRef[0], shaders) { }

        public TMaterial(string name, UniformRequirements requirements, RenderingParameters renderParams, params GLSLShaderFile[] shaders)
            : this(name, requirements, renderParams, new ShaderVar[0], new BaseTexRef[0], shaders) { }

        public TMaterial(string name, UniformRequirements requirements, ShaderVar[] vars, params GLSLShaderFile[] shaders)
            : this(name, requirements, null, vars, new BaseTexRef[0], shaders) { }

        public TMaterial(string name, UniformRequirements requirements, RenderingParameters renderParams, ShaderVar[] vars, params GLSLShaderFile[] shaders)
            : this(name, requirements, renderParams, vars, new BaseTexRef[0], shaders) { }

        public TMaterial(string name, UniformRequirements requirements, BaseTexRef[] textures, params GLSLShaderFile[] shaders)
            : this(name, requirements, null, new ShaderVar[0], textures, shaders) { }

        public TMaterial(string name, UniformRequirements requirements, RenderingParameters renderParams, BaseTexRef[] textures, params GLSLShaderFile[] shaders)
            : this(name, requirements, renderParams, null, textures, shaders) { }

        public TMaterial(string name, UniformRequirements requirements, ShaderVar[] vars, BaseTexRef[] textures, params GLSLShaderFile[] shaders)
            : this(name, requirements, null, vars, textures, shaders) { }

        public TMaterial(
            string name,
            UniformRequirements requirements,
            RenderingParameters renderParams, 
            ShaderVar[] vars,
            BaseTexRef[] textures,
            params GLSLShaderFile[] shaders)
        {
            _name = name;
            Requirements = requirements;
            RenderParams = renderParams ?? new RenderingParameters();
            _parameters = vars ?? new ShaderVar[0];
            Textures = textures ?? new BaseTexRef[0];
            SetShaders(shaders);
        }

        public void SetShaders(params GLSLShaderFile[] shaders)
        {
            _shaders.Clear();
            _shaders.AddRange(shaders);
            ShadersChanged();
        }
        public void AddShader(GLSLShaderFile shader)
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
            _vertexShaders.Clear();

            if (_program != null)
            {
                _program.Destroy();
                _program = null;
            }

            if (_shaders != null)
                foreach (GLSLShaderFile s in _shaders)
                {
                    switch (s.Type)
                    {
                        case EShaderMode.Vertex:
                            _vertexShaders.Add(s);
                            break;
                        case EShaderMode.Fragment:
                            _fragmentShaders.Add(s);
                            break;
                        case EShaderMode.Geometry:
                            _geometryShaders.Add(s);
                            break;
                        case EShaderMode.TessControl:
                            _tessCtrlShaders.Add(s);
                            break;
                        case EShaderMode.TessEvaluation:
                            _tessEvalShaders.Add(s);
                            break;
                    }
                }

            if (Engine.Settings != null && Engine.Settings.AllowShaderPipelines)
                Program = new RenderProgram(_shaders);
        }
        
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
            GLSLShaderFile frag = deferred ? ShaderHelpers.TextureFragDeferred() : ShaderHelpers.LitTextureFragForward();
            return new TMaterial("LitTextureMaterial", frag)
            {
                Requirements = deferred ? UniformRequirements.None : UniformRequirements.NeedsLightsAndCamera
            };
        }
        public static TMaterial CreateLitTextureMaterial(TexRef2D texture)
            => CreateLitTextureMaterial(texture, Engine.Settings.ShadingStyle3D == ShadingStyle.Deferred);
        public static TMaterial CreateLitTextureMaterial(TexRef2D texture, bool deferred)
        {
            GLSLShaderFile frag = deferred ? ShaderHelpers.TextureFragDeferred() : ShaderHelpers.LitTextureFragForward();
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
            GLSLShaderFile frag;
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
        public static TMaterial CreateBlinnMaterial_Forward(
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

            GLSLShaderFile s = new GLSLShaderFile(EShaderMode.Fragment, source);
            return new TMaterial("BlinnMaterial", parameters, s)
            {
                Requirements = UniformRequirements.NeedsLightsAndCamera
            };
        }
        #endregion
    }
}
