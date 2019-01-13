#version 450

out vec4 OutColor;

layout (location = 0) in vec3 FragPos;
layout (location = 6) in vec2 FragUV0;

uniform vec3 LineColor;
uniform vec3 BGColor;
uniform float Scale;
uniform float LineWidth;
uniform vec2 Translation;
uniform vec2 XYIncrements;

void main()
{
    vec2 scaledUV = (FragPos.xy - Translation) / vec2(Scale) / XYIncrements;
    vec2 fractUV = fract(scaledUV);
    fractUV = (fractUV - 0.5f) * 2.0f;
    vec2 lines = vec2(floor(abs(fractUV) + (LineWidth / Scale)));
    float lerp = clamp(lines.x + lines.y, 0.0f, 1.0f);
    vec3 col = mix(BGColor, LineColor, lerp);
    OutColor = vec4(col, 1.0f);
}
