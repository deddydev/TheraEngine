#version 450

const float PI = 3.14159265359f;
const float InvPI = 0.31831f;

layout (location = 0) out vec4 OutColor;
in vec3 FragPos;

uniform sampler2D Texture0; //AlbedoOpacity
uniform sampler2D Texture1; //Normal
uniform sampler2D Texture2; //PBR: Roughness, Metallic, Specular, Index of refraction
uniform sampler2D Texture3; //SSAO Noise
uniform sampler2D Texture4; //Depth

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
uniform samplerCube PointShadowMap;

struct BaseLight
{
	vec3 Color;
	float DiffuseIntensity;
	float AmbientIntensity;
};
struct DirLight
{
	BaseLight Base;
	sampler2D ShadowMap;
	mat4 WorldToLightSpaceProjMatrix;

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
	BaseLight Base;
	sampler2D ShadowMap;
	mat4 WorldToLightSpaceProjMatrix;

	vec3 Position;
	vec3 Direction;
	float Radius;
	float Brightness;
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

//Trowbridge-Reitz GGX
float SpecD_TRGGX(float NoH2, float a2)
{
	float num    = a2;
	float denom  = (NoH2 * (a2 - 1.0f) + 1.0f);
	denom        = PI * denom * denom;

	return num / denom;
}
float SpecG_SchlickGGX(float NoV, float k)
{
	float num   = NoV;
	float denom = NoV * (1.0f - k) + k;

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
	//float pow = pow(1.0f - VoH, 5.0f);

	//Spherical Gaussian Approximation
	//https://seblagarde.wordpress.com/2012/06/03/spherical-gaussien-approximation-for-blinn-phong-phong-and-fresnel/
	float pow = exp2((-5.55473f * VoH - 6.98316f) * VoH);

	return F0 + (1.0f - F0) * pow;
}
vec3 Spec_CookTorrance(float D, float G, vec3 F, float NoV, float NoL)
{
	vec3 num = D * G * F;
	float denom = 4.0f * NoV * NoL + 0.0001f;
	return num / denom;
}

//0 is fully in shadow, 1 is fully lit
float ReadShadowMap2D(in vec3 fragPosWS, in vec3 N, in float NoL, in sampler2D shadowMap, mat4 lightMatrix)
{
	//Move the fragment position into light space
	vec4 fragPosLightSpace = lightMatrix * vec4(fragPosWS, 1.0f);

	//Perspective divide
	vec3 fragCoord = fragPosLightSpace.xyz / fragPosLightSpace.w;

	//Scale and bias
	fragCoord = fragCoord * 0.5f + 0.5f;

	//Create bias depending on angle of normal to the light
	float maxBias = 0.04f;
	float minBias = 0.001f;
	float bias = 0.001f;//mix(maxBias, minBias, NoL);

	//Read depth value from shadow map rendered in light space
	float depth = texture(shadowMap, fragCoord.xy).r;

	//Hard shadow
	float shadow = (fragCoord.z - bias) > depth ? 0.0f : 1.0f;

	//PCF shadow
	//float shadow = 0.0;
	//vec2 texelSize = 1.0f / textureSize(light.ShadowMap, 0);
	//for (int x = -1; x <= 1; ++x)
	//{
	//    for (int y = -1; y <= 1; ++y)
	//    {
	//        float pcfDepth = texture(light.ShadowMap, fragCoord.xy + vec2(x, y) * texelSize).r;
	//        shadow += fragCoord.z - bias > pcfDepth ? 0.0f : 1.0f;        
	//    }    
	//}
	//shadow *= 0.111111111f; //divided by 9

	return shadow;
}
float ReadShadowMapCube(in vec3 fragToLightWS, in float lightDist, in float NoL)
{
	float mapped = pow(10.0f, -NoL * 2.5f);
    float bias = mix(0.05f, 4.0f, mapped);
	float closestDepth = texture(PointShadowMap, fragToLightWS).r * 1000.0f;
	return lightDist - bias > closestDepth ? 0.0f : 1.0f;
}

float Attenuate(in float dist, in float radius)
{
	//return 1.0f / (dist * dist);
	return pow(clamp(1.0f - pow(dist / radius, 4.0f), 0.0f, 1.0f), 2.0f) / (dist * dist + 1.0f);
}

void CalcColor(BaseLight light, float NoL, float NoH, float NoV, float HoV, float attn, vec4 albedoOpacity, vec4 rmsi, out vec3 color, out vec3 ambient)
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
vec3 CalcDirLight(DirLight light, vec3 N, vec3 V, vec3 fragPosWS, vec4 albedoOpacity, vec4 rmsi, float ambOcc)
{
	vec3 L = -light.Direction;
	vec3 H = normalize(V + L);
	float NoL = max(dot(N, L), 0.0f);
	float NoH = max(dot(N, H), 0.0f);
	float NoV = max(dot(N, V), 0.0f);
	float HoV = max(dot(H, V), 0.0f);
	
	vec3 color;
	vec3 ambient;
   	CalcColor(light.Base, NoL, NoH, NoV, HoV, 1.0f, albedoOpacity, rmsi, color, ambient);

	float shadow = ReadShadowMap2D(fragPosWS, N, NoL, light.ShadowMap, light.WorldToLightSpaceProjMatrix);
	return ColorCombine(color, ambient, shadow, ambOcc);
}
vec3 CalcPointLight(PointLight light, vec3 N, vec3 V, vec3 fragPosWS, vec4 albedoOpacity, vec4 rmsi, float ambOcc)
{
    vec3 L = light.Position - fragPosWS;
	float lightDist = length(L);
	float attn = Attenuate(lightDist / light.Brightness, light.Radius);
	L = normalize(L);
	vec3 H = normalize(V + L);
	float NoL = max(dot(N, L), 0.0f);
	float NoH = max(dot(N, H), 0.0f);
	float NoV = max(dot(N, V), 0.0f);
	float HoV = max(dot(H, V), 0.0f);

	vec3 color;
	vec3 ambient;
   	CalcColor(light.Base, NoL, NoH, NoV, HoV, attn, albedoOpacity, rmsi, color, ambient);

	float shadow = ReadShadowMapCube(-L, lightDist, NoL);
	return ColorCombine(color, ambient, shadow, ambOcc);
} 
vec3 CalcSpotLight(SpotLight light, vec3 N, vec3 V, vec3 fragPosWS, vec4 albedoOpacity, vec4 rmsi, float ambOcc)
{
    	vec3 L = light.Position - fragPosWS;
	float distance = length(L);
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

    	float spotAttn = pow(clampedCosine, light.Exponent);
    	float distAttn = Attenuate(distance / light.Brightness, light.Radius);
	float attn = spotAmt * spotAttn * distAttn;

    	vec3 H = normalize(V + L);
	float NoL = max(dot(N, L), 0.0f);
	float NoH = max(dot(N, H), 0.0f);
	float NoV = max(dot(N, V), 0.0f);
	float HoV = max(dot(H, V), 0.0f);
	
	vec3 color;
	vec3 ambient;
   	CalcColor(light.Base, NoL, NoH, NoV, HoV, attn, albedoOpacity, rmsi, color, ambient);

	float shadow = ReadShadowMap2D(fragPosWS, N, NoL, light.ShadowMap, light.WorldToLightSpaceProjMatrix);
	return ColorCombine(color, ambient, shadow, ambOcc);
}
vec3 CalcTotalLight(vec3 N, vec3 fragPosWS, vec4 albedoOpacity, vec4 rmsi, float ambOcc)
{
    vec3 totalLight = GlobalAmbient;
	vec3 V = normalize(CameraPosition - fragPosWS);

    for (int i = 0; i < DirLightCount; ++i)
       	totalLight += CalcDirLight(DirectionalLights[i], N, V, fragPosWS, albedoOpacity, rmsi, ambOcc);

    for (int i = 0; i < PointLightCount; ++i)
       	totalLight += CalcPointLight(PointLights[i], N, V, fragPosWS, albedoOpacity, rmsi, ambOcc);

    for (int i = 0; i < SpotLightCount; ++i)
       	totalLight += CalcSpotLight(SpotLights[i], N, V, fragPosWS, albedoOpacity, rmsi, ambOcc);

    return totalLight;
}
vec3 ViewPosFromDepth(float depth, vec2 uv)
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
    OutColor = vec4(AlbedoOpacity.rgb * totalLight, 1.0f);//vec4(0.0f, 1.0f, 0.0f, 1.0f);//
}