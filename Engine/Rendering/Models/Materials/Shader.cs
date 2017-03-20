using CustomEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public enum ShaderMode
    {
        Fragment,           // https://www.opengl.org/wiki/Fragment_Shader
        Vertex,             // https://www.opengl.org/wiki/Vertex_Shader
        Geometry,           // https://www.opengl.org/wiki/Geometry_Shader
        TessEvaluation,     // https://www.opengl.org/wiki/Tessellation_Evaluation_Shader
        TessControl,        // https://www.opengl.org/wiki/Tessellation_Control_Shader
        Compute             // https://www.opengl.org/wiki/Compute_Shader
    }
    public class Shader
    {
        public event EventHandler Compiled;

        public bool NeedsCompile { get { return _sourceChanged; } }
        public ShaderMode ShaderType { get { return _type; } }

        private bool _sourceChanged = false;
        private ShaderMode _type;
        internal string _source;

        public Shader(ShaderMode type)
        {
            _type = type;
        }
        public Shader(ShaderMode type, string source)
        {
            _type = type;
            _source = source;
            _sourceChanged = true;
        }
        public void SetSource(string source)
        {
            _source = source;
            _sourceChanged = true;
        }
        public int Compile()
        {
            _sourceChanged = false;

            Engine.Renderer.SetShaderMode(_type);
            int id = Engine.Renderer.GenerateShader(_source);

            Compiled?.Invoke(this, null);

            return id;
        }
        public static Shader UnlitTextureFragDeferred()
        {
            string source = @"
#version 450

layout (location = 0) out vec4 gAlbedoSpec;
layout (location = 1) out vec3 gPosition;
layout (location = 2) out vec3 gNormal;

uniform sampler2D Texture0;
uniform float MatSpecularIntensity;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

out vec4 OutColor;

void main()
{
    gPosition = InData.Position;
    gNormal = InData.Normal;
    gAlbedoSpec = vec4(texture(Texture0, InData.MultiTexCoord0).rgb, MatSpecularIntensity);
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
        /// <summary>
        /// Provides a fragment shader that outputs the color 
        /// of a single texture with no shading
        /// for a forward shading setup.
        /// </summary>
        public static Shader UnlitTextureFragForward()
        {
            string source = @"
#version 450

uniform sampler2D Texture0;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

out vec4 OutColor;

void main()
{
    OutColor = texture(Texture0, InData.MultiTexCoord0);
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
        public static Shader UnlitColorFragDeferred()
        {
            string source = @"
#version 450

layout (location = 0) out vec4 gAlbedoSpec;
layout (location = 1) out vec3 gPosition;
layout (location = 2) out vec3 gNormal;

uniform vec4 MatColor;
uniform float MatSpecularIntensity;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

void main()
{
    gPosition = InData.Position;
    gNormal = InData.Normal;
    gAlbedoSpec = vec4(texture(Texture0, InData.MultiTexCoord0).rgb, MatSpecularIntensity);
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
        /// <summary>
        /// Provides a fragment shader that outputs a single color
        /// with no shading for a forward shading setup.
        /// </summary>
        public static Shader UnlitColorFragForward()
        {
            string source = @"
#version 450

uniform vec4 MatColor;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

out vec4 OutColor;

void main()
{
    OutColor = MatColor;
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
        public static Shader TestFragDeferred()
        {
            string source = @"
#version 450

layout (location = 0) out vec4 gAlbedoSpec;
layout (location = 1) out vec3 gPosition;
layout (location = 2) out vec3 gNormal;

uniform vec4 MatColor;
uniform float MatSpecularIntensity;
uniform float MatShininess;

uniform sampler2D Texture0;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

void main()
{
    gPosition = InData.Position;
    gNormal = InData.Normal;
    gAlbedoSpec = vec4(texture(Texture0, InData.MultiTexCoord0).rgb, MatSpecularIntensity);
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
        public static Shader TestFragForward()
        {
            string source = @"
#version 450

uniform vec4 MatColor;
uniform float MatSpecularIntensity;
uniform float MatShininess;

uniform vec3 CameraPosition;
uniform vec3 CameraForward;

uniform sampler2D Texture0;

in Data
{
    vec3 Position;
    vec3 Normal;
    vec2 MultiTexCoord0;
} InData;

out vec4 OutColor;

" + LightingSetup() + @"

void main()
{
    vec3 normal = normalize(InData.Normal);

    " + LightingCalcForward() + @"

    vec4 texColor = texture(Texture0, InData.MultiTexCoord0);

    OutColor = MatColor * vec4(totalLight, 1.0);
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
        public static Shader GBufferShader()
        {
            string source = @"

#version 450

out vec4 OutColor;
in vec2 TexCoord0;

uniform sampler2D gAlbedoSpec;
uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gTexCoord;
uniform sampler2D gText;
uniform sampler2D gStencil;

uniform vec3 CameraPosition;

" + LightingSetup() + @"

void main()
{
    vec3 FragPos = texture(gPosition, TexCoord0).rgb;
    vec3 Normal = texture(gNormal, TexCoord0).rgb;
    vec3 Albedo = texture(gAlbedoSpec, TexCoord0).rgb;
    float Specular = texture(gAlbedoSpec, TexCoord0).a;
    vec4 Text = texture(gText, TexCoord0);
    
    " + LightingCalc("totalLight", "Albedo * 0.1", "Normal", "FragPos", "Albedo", "Specular") + @"

    OutColor = mix(vec4(totalLight, 1.0), Text.rgb, Text.a);
}";
            return new Shader(ShaderMode.Fragment, source);
        }
        private static string LightingCalcForward()
            => LightingCalc("totalLight", "vec3(0.0)", "normal", "InData.Position", "MatColor.rgb", "MatSpecularIntensity");
        private static string LightingCalc(
            string lightVarName,
            string baseLightVec3,
            string normalNameVec3,
            string fragPosNameVec3,
            string albedoNameRGB,
            string specNameIntensity)
        {
            return string.Format(@"
    vec3 {0} = {1};

    for (int i = 0; i < DirLightCount; ++i)
        {0} += CalcDirLight(DirectionalLights[i], {2}, {3}, {4}, {5});

    for (int i = 0; i < PointLightCount; ++i)
        {0} += CalcPointLight(PointLights[i], {2}, {3}, {4}, {5});

    for (int i = 0; i < SpotLightCount; ++i)
        {0} += CalcSpotLight(SpotLights[i], {2}, {3}, {4}, {5});", 
        lightVarName, baseLightVec3, normalNameVec3, fragPosNameVec3, albedoNameRGB, specNameIntensity);
        }
        private static string LightingSetup()
        {
            return @"

struct BaseLight
{
    vec3 Color;
    float DiffuseIntensity;
    float AmbientIntensity;
};
struct DirLight
{
    BaseLight Base;
    vec3 Direction;
};
struct PointLight
{
    BaseLight Base;
    vec3 Position;
    vec3 Attenuation;
};
struct SpotLight
{
    PointLight Base;
    vec3 Direction;
    float Cutoff;
    float Exponent;
};

uniform int PointLightCount;
uniform PointLight PointLights[16];

uniform int DirLightCount; 
uniform DirLight DirectionalLights[2];

uniform int SpotLightCount;
uniform SpotLight SpotLights[16];

vec3 CalcColor(BaseLight light, vec3 lightDirection, vec3 normal, vec3 fragPos, vec3 albedo, float spec)
{
    vec3 AmbientColor = vec3(light.Color * light.AmbientIntensity);
    vec3 DiffuseColor = vec3(0.0);
    vec3 SpecularColor = vec3(0.0);

    float DiffuseFactor = dot(normal, -lightDirection);
    if (DiffuseFactor >= 0.0)
    {
        DiffuseColor = light.Color * light.DiffuseIntensity * albedo * DiffuseFactor;

        vec3 posToEye = normalize(CameraPosition - fragPos);
        vec3 reflectDir = reflect(lightDirection, normal);
        float SpecularFactor = dot(posToEye, reflectDir);
        if (SpecularFactor >= 0.0)
        {
            SpecularColor = light.Color * spec * pow(SpecularFactor, 64.0);
        }
    }

    return AmbientColor + DiffuseColor + SpecularColor;
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec)
{
    return CalcColor(light.Base, light.Direction, normal, fragPos, albedo, spec);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec)
{
    vec3 lightToPos = fragPos - light.Position;
    float dist = length(lightToPos);

    //float attn = clamp(1.0 - dist * dist / (radius * radius), 0.0, 1.0); attn *= attn;
    float attn = 1.0f / (light.Attenuation[0] + light.Attenuation[1] * dist + light.Attenuation[2] * dist * dist);

    return attn * CalcColor(light.Base, normalize(lightToPos), normal, fragPos, albedo, spec);
} 

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec)
{
    if (light.Cutoff <= 1.5707) //~90 degrees in radians
    {
        vec3 lightToPos = fragPos - light.Base.Position;
        lightToPos = normalize(lightToPos);
        float clampedCosine = max(0.0, dot(lightToPos, normalize(light.Direction)));
	    if (clampedCosine >= cos(light.Cutoff))
        {
            vec3 lightToPos = fragPos - light.Base.Position;
            float dist = length(lightToPos);

            float spotAttn = pow(clampedCosine, light.Exponent);
            //float attn = clamp(1.0 - dist * dist / (radius * radius), 0.0, 1.0); attn *= attn;
            float attn = 1.0f / (light.Base.Attenuation[0] + light.Base.Attenuation[1] * dist + light.Base.Attenuation[2] * dist * dist);

            return spotAttn * attn * CalcColor(light.Base.Base, normalize(lightToPos), normal, fragPos, albedo, spec);
        }
    }
    return vec3(0.0);
}

";
        }
        public static Shader PBRFragmentShader()
        {
            string source = @"

in vec2 v_texcoord; // texture coords
in vec3 v_normal;   // normal
in vec3 v_binormal; // binormal (for TBN basis calc)
in vec3 v_pos;      // pixel view space position

out vec4 color;

layout(std140) uniform Transforms
{
    mat4x4 world_matrix;  // object's world position
    mat4x4 view_matrix;   // view (camera) transform
    mat4x4 proj_matrix;   // projection matrix
    mat3x3 normal_matrix; // normal transformation matrix ( transpose(inverse(W * V)) )
};

layout(std140) uniform Material
{
    vec4 material; // x - metallic, y - roughness, w - 'rim' lighting
    vec4 albedo;   // constant albedo color, used when textures are off
};

uniform samplerCube envd;  // prefiltered env cubemap
uniform sampler2D tex;     // base texture (albedo)
uniform sampler2D norm;    // normal map
uniform sampler2D spec;    // 'factors' texture (G channel used as roughness)
uniform sampler2D iblbrdf; // IBL BRDF normalization precalculated tex

#define PI 3.1415926

// constant light position, only one light source for testing (treated as point light)
const vec4 light_pos = vec4(-2, 3, -2, 1);

// handy value clamping to 0 - 1 range
float saturate(in float value)
{
    return clamp(value, 0.0, 1.0);
}

// phong (lambertian) diffuse term
float phong_diffuse()
{
    return (1.0 / PI);
}

// compute fresnel specular factor for given base specular and product
// product could be NdV or VdH depending on used technique
vec3 fresnel_factor(in vec3 f0, in float product)
{
    return mix(f0, vec3(1.0), pow(1.01 - product, 5.0));
}

// following functions are copies of UE4
// for computing cook-torrance specular lighting terms

float D_blinn(in float roughness, in float NdH)
{
    float m = roughness * roughness;
    float m2 = m * m;
    float n = 2.0 / m2 - 2.0;
    return (n + 2.0) / (2.0 * PI) * pow(NdH, n);
}

float D_beckmann(in float roughness, in float NdH)
{
    float m = roughness * roughness;
    float m2 = m * m;
    float NdH2 = NdH * NdH;
    return exp((NdH2 - 1.0) / (m2 * NdH2)) / (PI * m2 * NdH2 * NdH2);
}

float D_GGX(in float roughness, in float NdH)
{
    float m = roughness * roughness;
    float m2 = m * m;
    float d = (NdH * m2 - NdH) * NdH + 1.0;
    return m2 / (PI * d * d);
}

float G_schlick(in float roughness, in float NdV, in float NdL)
{
    float k = roughness * roughness * 0.5;
    float V = NdV * (1.0 - k) + k;
    float L = NdL * (1.0 - k) + k;
    return 0.25 / (V * L);
}

// simple phong specular calculation with normalization
vec3 phong_specular(in vec3 V, in vec3 L, in vec3 N, in vec3 specular, in float roughness)
{
    vec3 R = reflect(-L, N);
    float spec = max(0.0, dot(V, R));

    float k = 1.999 / (roughness * roughness);

    return min(1.0, 3.0 * 0.0398 * k) * pow(spec, min(10000.0, k)) * specular;
}

// simple blinn specular calculation with normalization
vec3 blinn_specular(in float NdH, in vec3 specular, in float roughness)
{
    float k = 1.999 / (roughness * roughness);
    return min(1.0, 3.0 * 0.0398 * k) * pow(NdH, min(10000.0, k)) * specular;
}

// cook-torrance specular calculation                      
vec3 cooktorrance_specular(in float NdL, in float NdV, in float NdH, in vec3 specular, in float roughness)
{
#ifdef COOK_BLINN
    float D = D_blinn(roughness, NdH);
#endif

#ifdef COOK_BECKMANN
    float D = D_beckmann(roughness, NdH);
#endif

#ifdef COOK_GGX
    float D = D_GGX(roughness, NdH);
#endif

    float G = G_schlick(roughness, NdV, NdL);
    float rim = mix(1.0 - roughness * material.w * 0.9, 1.0, NdV);
    return (1.0 / rim) * specular * G * D;
}

void main()
{
    // point light direction to point in view space
    vec3 local_light_pos = (view_matrix * (/*world_matrix */ light_pos)).xyz;

    // light attenuation
    float A = 20.0 / dot(local_light_pos - v_pos, local_light_pos - v_pos);

    // L, V, H vectors
    vec3 L = normalize(local_light_pos - v_pos);
    vec3 V = normalize(-v_pos);
    vec3 H = normalize(L + V);
    vec3 nn = normalize(v_normal);

    vec3 nb = normalize(v_binormal);
    mat3x3 tbn = mat3x3(nb, cross(nn, nb), nn);


    vec2 texcoord = v_texcoord;


    // normal map
#if USE_NORMAL_MAP
    // tbn basis
    vec3 N = tbn * (texture2D(norm, texcoord).xyz * 2.0 - 1.0);
#else
    vec3 N = nn;
#endif

    // albedo/specular base
#if USE_ALBEDO_MAP
    vec3 base = texture2D(tex, texcoord).xyz;
#else
    vec3 base = albedo.xyz;
#endif

    // roughness
#if USE_ROUGHNESS_MAP
    float roughness = texture2D(spec, texcoord).y * material.y;
#else
    float roughness = material.y;
#endif

    // material params
    float metallic = material.x;

    // mix between metal and non-metal material, for non-metal
    // constant base specular factor of 0.04 grey is used
    vec3 specular = mix(vec3(0.04), base, metallic);

    // diffuse IBL term
    //    I know that my IBL cubemap has diffuse pre-integrated value in 10th MIP level
    //    actually level selection should be tweakable or from separate diffuse cubemap
    mat3x3 tnrm = transpose(normal_matrix);
    vec3 envdiff = textureCubeLod(envd, tnrm * N, 10).xyz;

    // specular IBL term
    //    11 magic number is total MIP levels in cubemap, this is simplest way for picking
    //    MIP level from roughness value (but it's not correct, however it looks fine)
    vec3 refl = tnrm * reflect(-V, N);
    vec3 envspec = textureCubeLod(envd, refl, max(roughness * 11.0, textureQueryLod(envd, refl).y)).xyz;

    // compute material reflectance
    float NdL = max(0.0, dot(N, L));
    float NdV = max(0.001, dot(N, V));
    float NdH = max(0.001, dot(N, H));
    float HdV = max(0.001, dot(H, V));
    float LdV = max(0.001, dot(L, V));

    // fresnel term is common for any, except phong
    // so it will be calcuated inside ifdefs

# ifdef PHONG
    // specular reflectance with PHONG
    vec3 specfresnel = fresnel_factor(specular, NdV);
    vec3 specref = phong_specular(V, L, N, specfresnel, roughness);
#endif

# ifdef BLINN
    // specular reflectance with BLINN
    vec3 specfresnel = fresnel_factor(specular, HdV);
    vec3 specref = blinn_specular(NdH, specfresnel, roughness);
#endif

# ifdef COOK
    // specular reflectance with COOK-TORRANCE
    vec3 specfresnel = fresnel_factor(specular, HdV);
    vec3 specref = cooktorrance_specular(NdL, NdV, NdH, specfresnel, roughness);
#endif

    specref *= vec3(NdL);

    // diffuse is common for any model
    vec3 diffref = (vec3(1.0) - specfresnel) * phong_diffuse() * NdL;

    // compute lighting
    vec3 reflected_light = vec3(0);
    vec3 diffuse_light = vec3(0); // initial value == constant ambient light

    // point light
    vec3 light_color = vec3(1.0) * A;
    reflected_light += specref * light_color;
    diffuse_light += diffref * light_color;

    // IBL lighting
    vec2 brdf = texture2D(iblbrdf, vec2(roughness, 1.0 - NdV)).xy;
    vec3 iblspec = min(vec3(0.99), fresnel_factor(specular, NdV) * brdf.x + brdf.y);
    reflected_light += iblspec * envspec;
    diffuse_light += envdiff * (1.0 / PI);

    // final result
    vec3 result =
    diffuse_light * mix(base, vec3(0.0), metallic) +
    reflected_light;

    color = vec4(result, 1);
}
";
            return new Shader(ShaderMode.Fragment, source);
        }
    }
}
