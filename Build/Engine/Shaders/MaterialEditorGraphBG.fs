#version 450

out vec4 OutColor;

layout (location = 0) in vec3 FragPos;
layout (location = 6) in vec2 FragUV0;

uniform vec3 LineColor;
uniform vec3 BGColor;
uniform float Scale;
uniform float LineWidth;
uniform vec2 Translation;

void main()
{
    
    float realScale = fract(Scale / 30.0f / 10.0f) * 10.0f + 1.0f;
    
    vec2 scaledUV = (FragPos.xy - Translation) / Scale / 30.0f;
    vec2 fractUV = fract(scaledUV);
    fractUV = (fractUV - 0.5f) * 2.0f;
    vec2 lines = vec2(floor(abs(fractUV) + LineWidth));
    float lerp = clamp(lines.x + lines.y, 0.0f, 1.0f);
    vec3 col = mix(BGColor, LineColor, lerp);
    OutColor = vec4(col, 1.0f);
}