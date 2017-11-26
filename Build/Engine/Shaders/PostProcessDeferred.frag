#version 450

out vec4 OutColor;
in vec3 FragPos;

uniform sampler2D Texture0; //HDR Scene Color
uniform sampler2D Texture1; //Depth

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
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
vec3 HSVtoRGB(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float rand(vec2 co)
{
    return fract(sin(dot(co.xy, vec2(12.9898,78.233))) * 43758.5453);
}

void main()
{
    vec2 uv = FragPos.xy;
    vec3 hdrSceneColor = texture(Texture0, uv).rgb;
    float Depth = texture(Texture1, uv).r;

    //Color grading
    hdrSceneColor *= ColorGrade.Tint;
    vec3 hsv = RGBtoHSV(hdrSceneColor);
    hsv.x *= ColorGrade.Hue;
    hsv.y *= ColorGrade.Saturation;
    hsv.z *= ColorGrade.Brightness;
    hdrSceneColor = HSVtoRGB(hsv);
    hdrSceneColor = (hdrSceneColor - 0.5) * ColorGrade.Contrast + 0.5;

    //Tone mapping
    vec3 ldrSceneColor = vec3(1.0) - exp(-hdrSceneColor * ColorGrade.Exposure);
    
    //Vignette
    uv *= 1.0 - uv.yx;
    float vig = clamp(pow(uv.x * uv.y * Vignette.Intensity, Vignette.Power), 0.0, 1.0);
    ldrSceneColor = mix(Vignette.Color, ldrSceneColor, vig);

    //Gamma-correct
    vec3 gammaCorrected = pow(ldrSceneColor, vec3(1.0 / ColorGrade.Gamma));

    //Fix subtle banding by applying fine noise
    gammaCorrected += mix(-0.5/255.0, 0.5/255.0, rand(uv));

    OutColor = vec4(gammaCorrected, 1.0);
}