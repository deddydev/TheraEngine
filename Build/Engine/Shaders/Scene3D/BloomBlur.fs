#version 450

layout(location = 0) out vec3 OutColor;
layout(location = 0) in vec3 FragPos;

uniform sampler2D Texture0;

uniform float Ping;
uniform int Iteration;
uniform float Weight[5] = float[] (0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162);

void main()
{
     vec2 uv = FragPos.xy;
     vec2 uvOffset = 1.0f / textureSize(Texture0, 0);
     uvOffset.x *= 1.0f - Ping;
     uvOffset.y *= Ping;
     vec3 result = texture(Texture0, uv).rgb * Weight[0];
     for (int i = 1; i <= 4; ++i)
     {
         result += texture(Texture0, uv + (uvOffset * i)).rgb * Weight[i];
         result += texture(Texture0, uv - (uvOffset * i)).rgb * Weight[i];
     }
     OutColor = result;
}