#version 450

#define MAX_DIR_LIGHTS 1
#define MAX_SPOT_LIGHTS 3
#define MAX_POINT_LIGHTS 1

const float PI = 3.14159265359f;
const float InvPI = 0.31831f;

layout (location = 0) out vec4 OutColor; //Final Deferred Pass Color. Used later by the Post Process fragment shader.
layout (location = 0) in vec3 FragPos;

uniform sampler2D Texture0; //AlbedoOpacity
uniform sampler2D Texture1; //Normal
uniform sampler2D Texture2; //PBR: Roughness, Metallic, Specular, Index of refraction
uniform sampler2D Texture3; //SSAO Noise
uniform sampler2D Texture4; //Depth
uniform sampler2D DirShadowMaps[MAX_DIR_LIGHTS];
uniform sampler2D SpotShadowMaps[MAX_SPOT_LIGHTS];
uniform samplerCube PointShadowMaps[MAX_POINT_LIGHTS];

uniform vec3 SSAOSamples[64];
uniform float SSAORadius = 0.75f;
uniform float SSAOPower = 4.0f;

uniform vec3 CameraPosition;
uniform vec3 CameraForward;
uniform float CameraNearZ;
uniform float CameraFarZ;
uniform float ScreenWidth;
uniform float ScreenHeight;
uniform float ScreenOrigin;
uniform float ProjOrigin;
uniform float ProjRange;
uniform mat4 WorldToCameraSpaceMatrix;
uniform mat4 CameraToWorldSpaceMatrix;
uniform mat4 ProjMatrix;
uniform mat4 InvProjMatrix;

uniform vec3 GlobalAmbient;
uniform int DirLightCount; 
uniform int SpotLightCount;
uniform int PointLightCount;

struct BaseLight
{
	vec3 Color;
	float DiffuseIntensity;
	float AmbientIntensity;
    //vec3 Padding;
};
struct DirLight
{
	BaseLight Base;
	mat4 WorldToLightSpaceProjMatrix;

	vec3 Direction;
    //float Padding;
};
struct PointLight
{
	BaseLight Base;

	vec3 Position;
	float Radius;
	float Brightness;
    //vec3 Padding;
};
struct SpotLight
{
	BaseLight Base;
	mat4 WorldToLightSpaceProjMatrix;

	vec3 Position;
	vec3 Direction;
	float Radius;
	float Brightness;
	float Exponent;

	float InnerCutoff;
	float OuterCutoff;
    //vec2 Padding;
};

uniform DirLight DirLights[MAX_DIR_LIGHTS];
uniform SpotLight SpotLights[MAX_SPOT_LIGHTS];
uniform PointLight PointLights[MAX_POINT_LIGHTS];

//Trowbridge-Reitz GGX
float SpecD_TRGGX(in float NoH2, in float a2)
{
	float num    = a2;
	float denom  = (NoH2 * (a2 - 1.0f) + 1.0f);
	denom        = PI * denom * denom;

	return num / denom;
}
float SpecG_SchlickGGX(in float NoV, in float k)
{
	float num   = NoV;
	float denom = NoV * (1.0f - k) + k;

	return num / denom;
}
float SpecG_Smith(in float NoV, in float NoL, in float k)
{
	float ggx1 = SpecG_SchlickGGX(NoV, k);
	float ggx2 = SpecG_SchlickGGX(NoL, k);
	return ggx1 * ggx2;
}
vec3 SpecF_Schlick(in float VoH, in vec3 F0)
{
	//Regular implementation
	//float pow = pow(1.0f - VoH, 5.0f);

	//Spherical Gaussian Approximation
	//https://seblagarde.wordpress.com/2012/06/03/spherical-gaussien-approximation-for-blinn-phong-phong-and-fresnel/
	float pow = exp2((-5.55473f * VoH - 6.98316f) * VoH);

	return F0 + (1.0f - F0) * pow;
}
vec3 Spec_CookTorrance(in float D, in float G, in vec3 F, in float NoV, in float NoL)
{
	vec3 num = D * G * F;
	float denom = 4.0f * NoV * NoL + 0.0001f;
	return num / denom;
}

float GetShadowBias(in float NoL, in float power, in float minBias, in float maxBias)
{
    float mapped = pow(10.0f, -NoL * power);
    return mix(minBias, maxBias, mapped);
}
//0 is fully in shadow, 1 is fully lit
float ReadShadowMap2D(in vec3 fragPosWS, in vec3 N, in float NoL, in sampler2D shadowMap, in mat4 lightMatrix)
{
	//Move the fragment position into light space
	vec4 fragPosLightSpace = lightMatrix * vec4(fragPosWS, 1.0f);
	vec3 fragCoord = fragPosLightSpace.xyz / fragPosLightSpace.w;
	fragCoord = fragCoord * 0.5f + 0.5f;

	//Create bias depending on angle of normal to the light
	float bias = GetShadowBias(NoL, 20.0f, 0.001f, 0.004f);

	//Hard shadow
	float depth = texture(shadowMap, fragCoord.xy).r;
	float shadow = (fragCoord.z - bias) > depth ? 0.0f : 1.0f;

	//PCF shadow
	//float shadow = 0.0;
	//vec2 texelSize = 1.0f / textureSize(shadowMap, 0);
	//for (int x = -1; x <= 1; ++x)
	//{
	//    for (int y = -1; y <= 1; ++y)
	//    {
	//        float pcfDepth = texture(shadowMap, fragCoord.xy + vec2(x, y) * texelSize).r;
	//        shadow += fragCoord.z - bias > pcfDepth ? 0.0f : 1.0f;        
	//    }    
	//}
	//shadow *= 0.111111111f; //divided by 9

	return shadow;
}
//0 is fully in shadow, 1 is fully lit
float ReadPointShadowMap(in int lightIndex, in float farPlaneDist, in vec3 fragToLightWS, in float lightDist, in float NoL)
{
    float bias = GetShadowBias(NoL, 2.5f, 0.15f, 5.0f);

    //Hard shadow
	float closestDepth = texture(PointShadowMaps[lightIndex], fragToLightWS).r * farPlaneDist;
	float shadow = lightDist - bias > closestDepth ? 0.0f : 1.0f;

    //PCF shadow
	//float shadow = 0.0;
	//vec2 texelSize = 1.0f / textureSize(map, 0);
	//for (int x = -1; x <= 1; ++x)
	//{
	//    for (int y = -1; y <= 1; ++y)
	//    {
 //           for (int z = -1; z <= 1; ++z)
	//        {
	//            float pcfDepth = texture(map, fragToLightWS + vec3(x, y, z) * texelSize).r * farPlaneDist;
	//            shadow += fragCoord.z - bias > pcfDepth ? 0.0f : 1.0f;    
 //           }
	//    }    
	//}
	//shadow *= 0.111111111f; //divided by 9

	return shadow;
}

float Attenuate(in float dist, in float radius)
{
	return pow(clamp(1.0f - pow(dist / radius, 4.0f), 0.0f, 1.0f), 2.0f) / (dist * dist + 1.0f);
}

void CalcColor(in BaseLight light, in float NoL, in float NoH, in float NoV, in float HoV, in float attn, in vec4 albedoOpacity, in vec4 rmsi, out vec3 color, out vec3 ambient)
{
	vec3 radiance = attn * light.Color * light.DiffuseIntensity;
	
	//Cook-torrance BRDF

	vec3 F0 = vec3(0.04f); //abs((1.0f - rmsi.a) / (1.0f + rmsi.a))
	F0 = mix(F0, albedoOpacity.rgb, rmsi.g);
	float a = rmsi.r * rmsi.r;
	float k = rmsi.r + 1.0f;
	k = k * k * 0.125f; //divide by 8

	float D = SpecD_TRGGX(NoH * NoH, a * a);
	float G = SpecG_Smith(NoV, NoL, k);
	vec3  F = SpecF_Schlick(HoV, F0);
	vec3 spec = rmsi.b * Spec_CookTorrance(D, G, F, NoV, NoL);
    
	vec3 kS = F;
	vec3 kD = vec3(1.0f) - kS;
	kD *= 1.0f - rmsi.g;

	color = (kD * albedoOpacity.rgb / PI + spec) * radiance * NoL;
	ambient = light.Color * light.AmbientIntensity;
}
vec3 ColorCombine(vec3 color, vec3 ambient, float shadow, float ambOcc)
{
	return (ambient + color * shadow) * ambOcc;
}
vec3 CalcDirLight(in int lightIndex, in vec3 N, in vec3 V, in vec3 fragPosWS, in vec4 albedoOpacity, in vec4 rmsi, in float ambOcc)
{
	DirLight light = DirLights[lightIndex];

	vec3 L = -light.Direction;
	vec3 H = normalize(V + L);
	float NoL = max(dot(N, L), 0.0f);
	float NoH = max(dot(N, H), 0.0f);
	float NoV = max(dot(N, V), 0.0f);
	float HoV = max(dot(H, V), 0.0f);
	
	vec3 color;
	vec3 ambient;
   	CalcColor(light.Base, NoL, NoH, NoV, HoV, 1.0f, albedoOpacity, rmsi, color, ambient);

	float shadow = ReadShadowMap2D(fragPosWS, N, NoL, DirShadowMaps[lightIndex], light.WorldToLightSpaceProjMatrix);
	return ColorCombine(color, ambient, shadow, ambOcc);
}
vec3 CalcPointLight(in int lightIndex, in vec3 N, in vec3 V, in vec3 fragPosWS, in vec4 albedoOpacity, in vec4 rmsi, in float ambOcc)
{
	PointLight light = PointLights[lightIndex];

    vec3 L = light.Position - fragPosWS;
	float lightDist = length(L);
	float attn = Attenuate(lightDist / light.Brightness, light.Radius / light.Brightness);
	L = normalize(L);
	vec3 H = normalize(V + L);
	float NoL = max(dot(N, L), 0.0f);
	float NoH = max(dot(N, H), 0.0f);
	float NoV = max(dot(N, V), 0.0f);
	float HoV = max(dot(H, V), 0.0f);

	vec3 color;
	vec3 ambient;
   	CalcColor(light.Base, NoL, NoH, NoV, HoV, attn, albedoOpacity, rmsi, color, ambient);

	float shadow = ReadPointShadowMap(lightIndex, light.Radius, -L, lightDist, NoL);
	return ColorCombine(color, ambient, shadow, ambOcc);
} 
vec3 CalcSpotLight(in int lightIndex, in vec3 N, in vec3 V, in vec3 fragPosWS, in vec4 albedoOpacity, in vec4 rmsi, in float ambOcc)
{
	SpotLight light = SpotLights[lightIndex];

    vec3 L = light.Position - fragPosWS;
	float lightDist = length(L);
	L = normalize(L);

	//OuterCutoff is the smaller value, despite being a larger angle
	//cos(90) == 0
	//cos(0) == 1

	float cosine = dot(L, -normalize(light.Direction));

	if (cosine <= light.OuterCutoff)
		return vec3(light.Base.Color * light.Base.AmbientIntensity) * ambOcc;

    float clampedCosine = clamp(cosine, light.OuterCutoff, light.InnerCutoff);

	//Subtract smaller value and divide by range to normalize value
    float time = (clampedCosine - light.OuterCutoff) / (light.InnerCutoff - light.OuterCutoff);

	//Make transition smooth rather than linear
	float spotAmt = smoothstep(0.0f, 1.0f, time);
    float distAttn = Attenuate(lightDist / light.Brightness, light.Radius / light.Brightness);
	float attn = spotAmt * distAttn * pow(clampedCosine, light.Exponent);

    vec3 H = normalize(V + L);
	float NoL = max(dot(N, L), 0.0f);
	float NoH = max(dot(N, H), 0.0f);
	float NoV = max(dot(N, V), 0.0f);
	float HoV = max(dot(H, V), 0.0f);
	
	vec3 color;
	vec3 ambient;
   	CalcColor(light.Base, NoL, NoH, NoV, HoV, attn, albedoOpacity, rmsi, color, ambient);

	float shadow = ReadShadowMap2D(fragPosWS, N, NoL, SpotShadowMaps[lightIndex], light.WorldToLightSpaceProjMatrix);
	return ColorCombine(color, ambient, shadow, ambOcc);
}
vec3 CalcTotalLight(in vec3 N, in vec3 fragPosWS, in vec4 albedoOpacity, in vec4 rmsi, in float ambOcc)
{
    vec3 totalLight = GlobalAmbient;
	vec3 V = normalize(CameraPosition - fragPosWS);

    int i;
    for (i = 0; i < DirLightCount; ++i)
       	totalLight += CalcDirLight(i, N, V, fragPosWS, albedoOpacity, rmsi, ambOcc);

    for (i = 0; i < PointLightCount; ++i)
       	totalLight += CalcPointLight(i, N, V, fragPosWS, albedoOpacity, rmsi, ambOcc);

    for (i = 0; i < SpotLightCount; ++i)
       	totalLight += CalcSpotLight(i, N, V, fragPosWS, albedoOpacity, rmsi, ambOcc);

    return totalLight;
}
vec3 ViewPosFromDepth(in float depth, in vec2 uv)
{
    float z = depth * 2.0f - 1.0f;
    vec4 clipSpacePosition = vec4(uv * 2.0f - 1.0f, z, 1.0f);
    vec4 viewSpacePosition = InvProjMatrix * clipSpacePosition;
    return viewSpacePosition.xyz / viewSpacePosition.w;
}
void main()
{
    vec2 uv = FragPos.xy;
    if (uv.x > 1.0f || uv.y > 1.0f)
        discard;

    vec4 AlbedoOpacity = texture(Texture0, uv);
    vec3 Normal = texture(Texture1, uv).rgb;
    vec4 RMSI = vec4(texture(Texture2, uv).rgb, 1.0f);
    float Depth = texture(Texture4, uv).r;

    vec3 FragPosVS = ViewPosFromDepth(Depth, uv);
    vec3 FragPosWS = (CameraToWorldSpaceMatrix * vec4(FragPosVS, 1.0f)).xyz;

    ivec2 res = textureSize(Texture0, 0);
    vec2 noiseScale = vec2(res.x * 0.25f, res.y * 0.25f);

    vec3 randomVec = vec3(texture(Texture3, uv * noiseScale).rg * 2.0f - 1.0f, 0.0f);
    vec3 n = normalize(vec3(WorldToCameraSpaceMatrix * vec4(Normal, 0.0f)));
    vec3 tangent = normalize(randomVec - n * dot(randomVec, n));
    vec3 bitangent = cross(n, tangent);
    mat3 TBN = mat3(tangent, bitangent, n); 

    int kernelSize = 64;
    float bias = 0.025f;

    float occlusion = 0.0f;
    for (int i = 0; i < kernelSize; ++i)
    {
        vec3 noiseSample = TBN * SSAOSamples[i];
        noiseSample = FragPosVS + noiseSample * SSAORadius;

        vec4 offset = ProjMatrix * vec4(noiseSample, 1.0f);
        offset.xyz /= offset.w;
        offset.xyz = offset.xyz * 0.5f + 0.5f;

        float sampleDepth = ViewPosFromDepth(texture(Texture4, offset.xy).r, offset.xy).z;

        float rangeCheck = smoothstep(0.0f, 1.0f, SSAORadius / abs(FragPosVS.z - sampleDepth));
        occlusion += (sampleDepth >= noiseSample.z + bias ? 1.0f : 0.0f) * rangeCheck;  
    }

    occlusion = pow(1.0f - (occlusion / kernelSize), SSAOPower);

    vec3 totalLight = CalcTotalLight(Normal, FragPosWS, AlbedoOpacity, RMSI, occlusion);
    OutColor = vec4(AlbedoOpacity.rgb * totalLight, 1.0f);
}