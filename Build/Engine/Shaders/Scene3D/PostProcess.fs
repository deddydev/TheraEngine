#version 450

layout(location = 0) out vec4 OutColor;
layout(location = 0) in vec3 FragPos;

uniform sampler2D Texture0; //HDR scene color
uniform sampler2D Texture1; //Bloom
uniform sampler2D Texture2; //Depth
uniform usampler2D Texture3; //Stencil

uniform float NearZ;
uniform float FarZ;

struct VignetteStruct
{
    vec3 Color;
    float Intensity;
    float Power;
};
uniform VignetteStruct Vignette;

struct ColorGradeStruct
{
    vec3 Tint;

    float Exposure;
    float Contrast;
    float Gamma;

    float Hue;
    float Saturation;
    float Brightness;
};
uniform ColorGradeStruct ColorGrade;

vec3 RGBtoHSV(vec3 c)
{
    vec4 K = vec4(0.0f, -1.0f / 3.0f, 2.0f / 3.0f, -1.0f);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10f;
    return vec3(abs(q.z + (q.w - q.y) / (6.0f * d + e)), d / (q.x + e), q.x);
}
vec3 HSVtoRGB(vec3 c)
{
    vec4 K = vec4(1.0f, 2.0f / 3.0f, 1.0f / 3.0f, 3.0f);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0f - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0f, 1.0f), c.y);
}
float rand(vec2 coord)
{
    return fract(sin(dot(coord, vec2(12.9898f, 78.233f))) * 43758.5453f);
}
float LinearizeDepth(float depth) 
{
    float z = depth * 2.0f - 1.0f; // back to NDC 
    return (2.0f * NearZ * FarZ) / (FarZ + NearZ - z * (FarZ - NearZ));	
}
void main()
{
    vec2 uv = FragPos.xy;
    vec3 hdrSceneColor = texture(Texture0, uv).rgb;
    vec3 bloom = texture(Texture1, uv).rgb;
    hdrSceneColor += bloom;

    //Color grading
    hdrSceneColor *= ColorGrade.Tint;
    vec3 hsv = RGBtoHSV(hdrSceneColor);
    hsv.x *= ColorGrade.Hue;
    hsv.y *= ColorGrade.Saturation;
    hsv.z *= ColorGrade.Brightness;
    hdrSceneColor = HSVtoRGB(hsv);
    hdrSceneColor = (hdrSceneColor - 0.5f) * ColorGrade.Contrast + 0.5f;

    //Tone mapping
    vec3 ldrSceneColor = vec3(1.0f) - exp(-hdrSceneColor * ColorGrade.Exposure);
    
    //Vignette
    uv *= 1.0f - uv.yx;
    float vig = clamp(pow(uv.x * uv.y * Vignette.Intensity, Vignette.Power), 0.0f, 1.0f);
    ldrSceneColor = mix(Vignette.Color, ldrSceneColor, vig);

    //Gamma-correct
    vec3 gammaCorrected = pow(ldrSceneColor, vec3(1.0f / ColorGrade.Gamma));

    //Fix subtle banding by applying fine noise
    gammaCorrected += mix(-0.5f / 255.0f, 0.5f / 255.0f, rand(uv));

    uint stencil = texture(Texture3, uv).r;
    OutColor = vec4(vec3(float(stencil)), 1.0f);//vec4(gammaCorrected, 1.0f);
}