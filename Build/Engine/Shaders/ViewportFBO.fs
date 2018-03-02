#version 450

out vec4 OutColor;
in vec3 FragPos;
in vec2 FragUV0;

uniform sampler2D Texture0;

void main()
{
    OutColor = texture(Texture0, FragUV0);
}