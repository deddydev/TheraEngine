#version 450

layout(location = 0) out vec3 BloomColor;
layout(location = 0) in vec3 FragPos;

uniform sampler2D Texture0; //HDR color from Deferred & Forward passes

void main()
{
    vec3 luminance = vec3(0.299f + 0.587f + 0.114f); //vec3(0.2126f, 0.7152f, 0.0722f)
    vec2 uv = FragPos.xy;
    vec3 hdrSceneColor = texture(Texture0, uv).rgb;
    float brightness = dot(hdrSceneColor, luminance);
    float multiplier = clamp(floor(brightness), 0.0f, 1.0f);
    BloomColor = hdrSceneColor * multiplier;
}