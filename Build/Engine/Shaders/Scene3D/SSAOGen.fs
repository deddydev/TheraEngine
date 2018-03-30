#version 450

const float PI = 3.14159265359f;
const float InvPI = 0.31831f;

layout(location = 0) out float OutIntensity;
layout(location = 0) in vec3 FragPos;

uniform sampler2D Texture0; //Normal
uniform sampler2D Texture1; //SSAO Noise
uniform sampler2D Texture2; //Depth

uniform vec3 SSAOSamples[64];
uniform float SSAORadius = 0.75f;
uniform float SSAOPower = 4.0f;

uniform mat4 WorldToCameraSpaceMatrix;
uniform mat4 CameraToWorldSpaceMatrix;
uniform mat4 ProjMatrix;
uniform mat4 InvProjMatrix;

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

    vec3 Normal = texture(Texture0, uv).rgb;
    float Depth = texture(Texture2, uv).r;

    vec3 FragPosVS = ViewPosFromDepth(Depth, uv);
    vec3 FragPosWS = (CameraToWorldSpaceMatrix * vec4(FragPosVS, 1.0f)).xyz;

    ivec2 res = textureSize(Texture0, 0);
    vec2 noiseScale = vec2(res.x * 0.25f, res.y * 0.25f);

    vec3 randomVec = vec3(texture(Texture1, uv * noiseScale).rg * 2.0f - 1.0f, 0.0f);
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

        float sampleDepth = ViewPosFromDepth(texture(Texture2, offset.xy).r, offset.xy).z;

        float rangeCheck = smoothstep(0.0f, 1.0f, SSAORadius / abs(FragPosVS.z - sampleDepth));
        occlusion += (sampleDepth >= noiseSample.z + bias ? 1.0f : 0.0f) * rangeCheck;  
    }

    OutIntensity = pow(1.0f - (occlusion / kernelSize), SSAOPower);
}