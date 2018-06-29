using System.IO;

namespace TheraEngine.Rendering.Models.Materials
{
    public static class ShaderHelpers
    {
        public const string LightFalloff = "pow(clamp(1.0 - pow({1} / {0}, 4), 0.0, 1.0), 2.0) / ({1} * {1} + 1.0);";
        //public const string LightFallof = "clamp(1.0 - {1} * {1} / ({0} * {0}), 0.0, 1.0); attn *= attn;";
        //public const string LightFallof = "1.0f / (light.Attenuation[0] + light.Attenuation[1] * dist + light.Attenuation[2] * dist* dist)";
        public static string GetLightFalloff(string radiusName, string distanceName)
            => string.Format(LightFalloff, radiusName, distanceName);

        public static readonly string Frag_Nothing = @"
#version 100
void main() { }";
        public static readonly string Frag_DepthOutput = @"
#version 450
layout(location = 0) out float Depth;
void main()
{
    Depth = gl_FragCoord.z;
}";
        public static readonly string Func_WorldPosFromDepth = @"
vec3 WorldPosFromDepth(float depth, vec2 uv)
{
    float z = depth * 2.0 - 1.0;
    vec4 clipSpacePosition = vec4(uv * 2.0 - 1.0, z, 1.0);
    vec4 viewSpacePosition = InvProjMatrix * clipSpacePosition;
    viewSpacePosition /= viewSpacePosition.w;
    vec4 worldSpacePosition = CameraToWorldSpaceMatrix * viewSpacePosition;
    return worldSpacePosition.xyz;
}";
        public static readonly string Func_ViewPosFromDepth = @"
vec3 ViewPosFromDepth(float depth, vec2 uv)
{
    float z = depth * 2.0 - 1.0;
    vec4 clipSpacePosition = vec4(uv * 2.0 - 1.0, z, 1.0);
    vec4 viewSpacePosition = InvProjMatrix * clipSpacePosition;
    return viewSpacePosition.xyz / viewSpacePosition.w;
}";
        public static readonly string Func_GetDistanceFromDepth = @"
float GetDistanceFromDepth(float depth)
{
    float depthSample = 2.0 * depth - 1.0;
    float zLinear = 2.0 * CameraNearZ * CameraFarZ / (CameraFarZ + CameraNearZ - depthSample * (CameraFarZ - CameraNearZ));
    return zLinear;
}";
        public static readonly string Func_GetDepthFromDistance = @"
float GetDepthFromDistance(float z)
{
    float nonLinearDepth = (CameraFarZ + CameraNearZ - 2.0 * CameraNearZ * CameraFarZ / z) / (CameraFarZ - CameraNearZ);
    nonLinearDepth = (nonLinearDepth + 1.0) / 2.0;
    return nonLinearDepth;
}";
        public static readonly string Func_RGBtoHSV = @"
vec3 RGBtoHSV(vec3 c)
{
    vec4 K = vec4(0.0, -1.0f / 3.0f, 2.0f / 3.0f, -1.0f);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10f;
    return vec3(abs(q.z + (q.w - q.y) / (6.0f * d + e)), d / (q.x + e), q.x);
}";
        public static readonly string Func_HSVtoRGB = @"
vec3 HSVtoRGB(vec3 c)
{
    vec4 K = vec4(1.0f, 2.0f / 3.0f, 1.0f / 3.0f, 3.0f);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0f - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0f, 1.0f), c.y);
}";
        
        public static GLSLShaderFile LitTextureFragForward()
        {
            string source = @"
#version 450

layout (location = 0) out vec4 OutColor;

uniform float MatSpecularIntensity;
uniform float MatShininess;

uniform vec3 CameraPosition;

uniform sampler2D Texture0;

layout (location = 0) in vec3 FragPos;
layout (location = 1) in vec3 FragNorm;
layout (location = 6) in vec2 FragUV0;

" + LightingDeclBasic() + @"

void main()
{
    vec3 normal = normalize(FragNorm);
    vec4 texColor = texture(Texture0, FragUV0);
    float AmbientOcclusion = 1.0;

    " + LightingCalcBasic("totalLight", "vec3(0.0f)", "normal", "FragPos", "texColor.rgb", "MatSpecularIntensity", "AmbientOcclusion") + @"

    OutColor = texColor * vec4(totalLight, 1.0);
}
";
            return new GLSLShaderFile(EShaderMode.Fragment, source);
        }
        public static GLSLShaderFile TextureFragDeferred()
            => Engine.LoadEngineShader(Path.Combine("Common", "TexturedDeferred.fs"), EShaderMode.Fragment);
        public static GLSLShaderFile LitColorFragDeferred()
            => Engine.LoadEngineShader(Path.Combine("Common", "ColoredDeferred.fs"), EShaderMode.Fragment);
        public static GLSLShaderFile UnlitTextureFragForward()
             => Engine.LoadEngineShader(Path.Combine("Common", "UnlitTexturedForward.fs"), EShaderMode.Fragment);
        public static GLSLShaderFile UnlitColorFragForward()
             => Engine.LoadEngineShader(Path.Combine("Common", "UnlitColoredForward.fs"), EShaderMode.Fragment);
        public static GLSLShaderFile LitColorFragForward()
        {
            string source = @"
#version 450

layout (location = 0) out vec4 OutColor;

uniform vec4 MatColor;
uniform float MatSpecularIntensity;
uniform float MatShininess;

uniform vec3 CameraPosition;
uniform vec3 CameraForward;

layout (location = 0) in vec3 FragPos;
layout (location = 1) in vec3 FragNorm;

" + LightingDeclBasic() + @"

void main()
{
    vec3 normal = normalize(FragNorm);

    " + LightingCalcForward() + @"

    OutColor = MatColor * vec4(totalLight, 1.0);
}
";
            return new GLSLShaderFile(EShaderMode.Fragment, source);
        }
        public static string LightingCalcForward()
            => LightingCalcBasic("totalLight", "GlobalAmbient", "normal", "FragPos", "MatColor.rgb", "MatSpecularIntensity", "1.0");
        public static string LightingCalcBasic(
            string lightVarName,
            string baseLightVec3,
            string normalNameVec3,
            string fragPosNameVec3,
            string albedoNameRGB,
            string specNameIntensity,
            string ambientOcclusionFloat)
        {
            return string.Format(@"
    vec3 {0} = {1};

    for (int i = 0; i < DirLightCount; ++i)
        {0} += CalcDirLight(DirectionalLights[i], {2}, {3}, {4}, {5}, {6});

    for (int i = 0; i < PointLightCount; ++i)
        {0} += CalcPointLight(PointLights[i], {2}, {3}, {4}, {5}, {6});

    for (int i = 0; i < SpotLightCount; ++i)
        {0} += CalcSpotLight(SpotLights[i], {2}, {3}, {4}, {5}, {6});",
        lightVarName, baseLightVec3, normalNameVec3, fragPosNameVec3, albedoNameRGB, specNameIntensity, ambientOcclusionFloat);
        }
        public static string LightingDeclBasic()
        {
            return @"

struct BaseLight
{
    vec3 Color;
    float DiffuseIntensity;
    float AmbientIntensity;
    mat4 WorldToLightSpaceProjMatrix;
    sampler2D ShadowMap;
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
    float Radius;
    float Brightness;
};
struct SpotLight
{
    PointLight Base;
    vec3 Direction;
    float InnerCutoff;
    float OuterCutoff;
    float Exponent;
};

uniform vec3 GlobalAmbient;

uniform int DirLightCount; 
uniform DirLight DirectionalLights[2];

uniform int SpotLightCount;
uniform SpotLight SpotLights[16];

uniform int PointLightCount;
uniform PointLight PointLights[16];

//0 is fully in shadow, 1 is fully lit
float ReadShadowMap(in vec3 fragPos, in vec3 normal, in float diffuseFactor, in BaseLight light)
{
    float maxBias = 0.04;
    float minBias = 0.001;

    vec4 fragPosLightSpace = light.WorldToLightSpaceProjMatrix * vec4(fragPos, 1.0);
    vec3 fragCoord = fragPosLightSpace.xyz / fragPosLightSpace.w;
    fragCoord = fragCoord * vec3(0.5) + vec3(0.5);
    float bias = max(maxBias * -diffuseFactor, minBias);

    float depth = texture(light.ShadowMap, fragCoord.xy).r;
    float shadow = (fragCoord.z - bias) > depth ? 0.0 : 1.0;        

    //float shadow = 0.0;
    //vec2 texelSize = 1.0 / textureSize(light.ShadowMap, 0);
    //for (int x = -1; x <= 1; ++x)
    //{
    //    for (int y = -1; y <= 1; ++y)
    //    {
    //        float pcfDepth = texture(light.ShadowMap, fragCoord.xy + vec2(x, y) * texelSize).r;
    //        shadow += fragCoord.z - bias > pcfDepth ? 0.0 : 1.0;        
    //    }    
    //}
    //shadow *= 0.111111111; //divided by 9

    return shadow;
}

float Attenuate(in float dist, in float radius)
{
    return " + GetLightFalloff("radius", "dist") + @"
}

vec3 CalcColor(BaseLight light, vec3 lightDirection, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    vec3 AmbientColor = vec3(light.Color * light.AmbientIntensity);
    vec3 DiffuseColor = vec3(0.0);
    vec3 SpecularColor = vec3(0.0);

    float DiffuseFactor = dot(normal, -lightDirection);
    if (DiffuseFactor > 0.0)
    {
        DiffuseColor = light.Color * light.DiffuseIntensity * albedo * DiffuseFactor;

        vec3 posToEye = normalize(CameraPosition - fragPos);
        vec3 reflectDir = reflect(lightDirection, normal);
        float SpecularFactor = dot(posToEye, reflectDir);
        if (SpecularFactor > 0.0)
        {
            SpecularColor = light.Color * spec * pow(SpecularFactor, 64.0);
        }
    }

    float shadow = ReadShadowMap(fragPos, normal, DiffuseFactor, light);
    return (AmbientColor + (DiffuseColor + SpecularColor) * shadow) * ambientOcclusion;
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    return CalcColor(light.Base, light.Direction, normal, fragPos, albedo, spec, ambientOcclusion);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    vec3 lightToPos = fragPos - light.Position;
    return Attenuate(length(lightToPos), light.Radius) * CalcColor(light.Base, normalize(lightToPos), normal, fragPos, albedo, spec, ambientOcclusion);
} 

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    //if (light.OuterCutoff <= 1.5707) //~90 degrees in radians
    {
        vec3 lightToPos = normalize(fragPos - light.Base.Position);
        float clampedCosine = max(0.0, dot(lightToPos, normalize(light.Direction)));
        float spotEffect = smoothstep(light.OuterCutoff, light.InnerCutoff, clampedCosine);
	    //if (clampedCosine >= light.OuterCutoff)
        {
            vec3 lightToPos = fragPos - light.Base.Position;
            float spotAttn = pow(clampedCosine, light.Exponent);
            float distAttn = Attenuate(length(lightToPos) / light.Base.Brightness, light.Base.Radius);
            vec3 color = CalcColor(light.Base.Base, normalize(lightToPos), normal, fragPos, albedo, spec, ambientOcclusion);
            return spotEffect * spotAttn * distAttn * color;
        }
    }
    return vec3(0.0);
}
";
        }
        public static string LightingDeclPhysicallyBased()
        {
            return @"

struct BaseLight
{
    vec3 Color;
    float DiffuseIntensity;
    float AmbientIntensity;
    mat4 WorldToLightSpaceProjMatrix;
    sampler2D ShadowMap;
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
    float Radius;
    float Brightness;
};
struct SpotLight
{
    PointLight Base;
    vec3 Direction;
    float InnerCutoff;
    float OuterCutoff;
    float Exponent;
};

uniform vec3 GlobalAmbient;

uniform int DirLightCount; 
uniform DirLight DirectionalLights[2];

uniform int SpotLightCount;
uniform SpotLight SpotLights[16];

uniform int PointLightCount;
uniform PointLight PointLights[16];

//0 is fully in shadow, 1 is fully lit
float ReadShadowMap(in vec3 fragPos, in vec3 normal, in float diffuseFactor, in BaseLight light)
{
    float maxBias = 0.04;
    float minBias = 0.001;

    vec4 fragPosLightSpace = light.WorldToLightSpaceProjMatrix * vec4(fragPos, 1.0);
    vec3 fragCoord = fragPosLightSpace.xyz / fragPosLightSpace.w;
    fragCoord = fragCoord * vec3(0.5) + vec3(0.5);
    float bias = max(maxBias * -diffuseFactor, minBias);

    float depth = texture(light.ShadowMap, fragCoord.xy).r;
    float shadow = fragCoord.z - bias > depth ? 0.0 : 1.0;        

    //float shadow = 0.0;
    //vec2 texelSize = 1.0 / textureSize(light.ShadowMap, 0);
    //for (int x = -1; x <= 1; ++x)
    //{
    //    for (int y = -1; y <= 1; ++y)
    //    {
    //        float pcfDepth = texture(light.ShadowMap, fragCoord.xy + vec2(x, y) * texelSize).r;
    //        shadow += fragCoord.z - bias > pcfDepth ? 0.0 : 1.0;        
    //    }    
    //}
    //shadow *= 0.111111111; //divided by 9

    return shadow;
}

float Attenuate(in float dist, in float radius)
{
    return " + GetLightFalloff("radius", "dist") + @"
}

vec3 CalcColor(BaseLight light, vec3 lightDirection, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    vec3 AmbientColor = vec3(light.Color * light.AmbientIntensity);
    vec3 DiffuseColor = vec3(0.0);
    vec3 SpecularColor = vec3(0.0);

    float DiffuseFactor = dot(normal, -lightDirection);
    if (DiffuseFactor > 0.0)
    {
        DiffuseColor = light.Color * light.DiffuseIntensity * albedo * DiffuseFactor;

        vec3 posToEye = normalize(CameraPosition - fragPos);
        vec3 reflectDir = reflect(lightDirection, normal);
        float SpecularFactor = dot(posToEye, reflectDir);
        if (SpecularFactor > 0.0)
        {
            SpecularColor = light.Color * spec * pow(SpecularFactor, 64.0);
        }
    }

    float shadow = ReadShadowMap(fragPos, normal, DiffuseFactor, light);
    return (AmbientColor + (DiffuseColor + SpecularColor) * shadow) * ambientOcclusion;
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    return CalcColor(light.Base, light.Direction, normal, fragPos, albedo, spec, ambientOcclusion);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    vec3 lightToPos = fragPos - light.Position;
    return Attenuate(length(lightToPos), light.Radius) * CalcColor(light.Base, normalize(lightToPos), normal, fragPos, albedo, spec, ambientOcclusion);
} 

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 albedo, float spec, float ambientOcclusion)
{
    //if (light.OuterCutoff <= 1.5707) //~90 degrees in radians
    {
        vec3 lightToPos = normalize(fragPos - light.Base.Position);
        float clampedCosine = max(0.0, dot(lightToPos, normalize(light.Direction)));
        float spotEffect = smoothstep(light.OuterCutoff, light.InnerCutoff, clampedCosine);
	    //if (clampedCosine >= light.OuterCutoff)
        {
            vec3 lightToPos = fragPos - light.Base.Position;
            float spotAttn = pow(clampedCosine, light.Exponent);
            float distAttn = Attenuate(length(lightToPos) / light.Base.Brightness, light.Base.Radius);
            vec3 color = CalcColor(light.Base.Base, normalize(lightToPos), normal, fragPos, albedo, spec, ambientOcclusion);
            return spotEffect * spotAttn * distAttn * color;
        }
    }
    return vec3(0.0);
}
";
        }
        public static GLSLShaderFile LightingSetupPhysicallyBased()
        {
            string source = @"
const float PI = 3.14159265359;
const float InvPI = 0.31831;

//Trowbridge-Reitz GGX
float SpecD_TRGGX(float NoH2, float a2)
{
    float num    = a2;
    float denom  = (NoH2 * (a2 - 1.0) + 1.0);
    denom        = PI * denom * denom;

    return num / denom;
}
float SpecG_SchlickGGX(float NdotV, float k)
{
    float num   = NdotV;
   	float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}
float SpecG_Smith(float NoV, float NoL, float k)
{
    float ggx1 = SpecG_SchlickGGX(NoV, k);
    float ggx2 = SpecG_SchlickGGX(NoL, k);
    return ggx1 * ggx2;
}
vec3 SpecF_Schlick(float VoH, vec3 F0)
{
	//Regular implementation
	//float pow = pow(1.0 - VoH, 5.0);

	//Spherical Gaussian Approximation
	//https://seblagarde.wordpress.com/2012/06/03/spherical-gaussien-approximation-for-blinn-phong-phong-and-fresnel/
	float pow = exp2((-5.55473 * VoH - 6.98316) * VoH);

    return F0 + (1.0 - F0) * pow;
}
vec3 Spec_CookTorrance(float D, float G, vec3 F, float NoV, float NoL)
{
	vec3 num = D * G * F;
	float denom = 4.0 * NoV * NoL + 0.001; 
	return num / denom;
}
vec3 CalcLighting(vec3 N, vec3 L, vec3 color, float roughness, float metallic, float ior)
{
	float NoV = max(dot(N, V), 0.0);
	float NoL = max(dot(N, L), 0.0);
	float NoH = max(dot(N, H), 0.0);
	float NoH2 = NoH * NoH;

	float a = roughness * roughness;
	float a2 = a * a;

	float k = pow(roughness + 1.0, 2.0) * 0.125; //divide by 8

	float D = SpecD_TRGGX(NoH2, a2);
	float G = SpecG_Smith(NoV, NoL, k);
	float F = SpecF_Schlick(VoH, F0);

	float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

	vec3 F0 = abs((1.0 - ior) / (1.0 + ior));
	F0      = mix(F0, color, metallic);
 
	vec3 kS = F;
	vec3 kD = (vec3(1.0) - kS) * (1.0 - metallic);

	vec3 specular = Spec_CookTorrance(D, G, F, NoV, NoL);
	return kd * color * InvPI + ks * specular;
      
    Lo += CalcLighting() * radiance * NoL;
}

float RiemannSum()
{
    int steps = 100;
    float sum = 0.0f;
    vec3 P    = ...;
    vec3 Wo   = ...;
    vec3 N    = ...;
    float dW  = 1.0f / steps;
    for(int i = 0; i < steps; ++i) 
    {
        vec3 Wi = getNextIncomingLightDir(i);
        sum += Fr(P, Wi, Wo) * L(P, Wi) * dot(N, Wi) * dW;
    }
}

vec3 ImportanceSampleGGX(vec2 Xi, float a, vec3 N)
{
	float Phi = 2.0 * PI * Xi.x;

	float CosTheta = sqrt((1.0 - Xi.y) / (1.0 + (a * a - 1.0) * Xi.y));
	float SinTheta = sqrt(1.0 - CosTheta * CosTheta);

	vec3 H = vec3(SinTheta * cos(Phi), SinTheta * sin(Phi), CosTheta);
	vec3 UpVector = abs(N.z) < 0.999 ? vec3(0.0, 0.0, 1.0) : vec3(1.0, 0.0, 0.0);
	vec3 TangentX = normalize(cross(UpVector, N));
	vec3 TangentY = cross(N, TangentX);

	// Tangent to world space
	return TangentX * H.x + TangentY * H.y + N * H.z;
}
vec3 SpecularIBL(vec3 SpecularColor, float Roughness, vec3 N, vec3 V)
{
	const uint NumSamples = 1024;

	vec3 radiance = vec3(0.0);
	float a = Roughness * Roughness;
	for (uint i = 0; i < NumSamples; i++)
	{
		vec2 Xi = Hammersley(i, NumSamples);
		vec3 H = ImportanceSampleGGX(Xi, a, N);
		float VoH = dot(V, H);
		vec3 L = vec3(2.0 * VoH) * H - V;
		float NoV = saturate(dot(N, V));
		float NoL = saturate(dot(N, L));
		float NoH = saturate(dot(N, H));
		VoH = saturate(VoH);
		if(NoL > 0)
		{
			vec3 SampleColor = EnvMap.SampleLevel(EnvMapSampler, L, 0).rgb;
			float G = G_Smith(Roughness, NoV, NoL);
			float Fc = pow( 1 - VoH, 5 );
			float3 F = (1 - Fc) * SpecularColor + Fc;
			// Incident light = SampleColor * NoL
			// Microfacet specular = D*G*F / (4*NoL*NoV)
			// pdf = D * NoH / (4 * VoH)
			radiance += SampleColor * F * G * VoH / (NoH * NoV);
		}
	}
	return radiance / NumSamples;
}

in vec2 v_texcoord; // texture coords
in vec3 v_normal;   // normal
in vec3 v_binormal; // binormal (for TBN basis calc)
in vec3 v_pos;      // pixel view space position

out vec4 color;

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
            return new GLSLShaderFile(EShaderMode.Fragment, source);
        }
    }
}
