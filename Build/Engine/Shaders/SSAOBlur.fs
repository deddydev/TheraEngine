#version 450
layout(location = 0) out float OutIntensity;
layout(location = 6) in vec2 FragUV0;
uniform sampler2D Texture0;

void main()
{
    vec2 texelSize = 1.0f / vec2(textureSize(Texture0, 0));
    float result = 0.0f;
    for (int x = -2; x < 2; ++x) 
    {
        for (int y = -2; y < 2; ++y) 
        {
            vec2 offset = vec2(float(x), float(y)) * texelSize;
            result += texture(Texture0, FragUV0 + offset).r;
        }
    }
    OutIntensity = result / 16.0f;
}