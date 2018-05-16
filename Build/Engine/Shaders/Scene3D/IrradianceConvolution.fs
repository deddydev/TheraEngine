#version 450

layout (location = 0) out vec3 FragColor;
layout (location = 0) in vec3 FragPos;

layout (binding = 0) uniform samplerCube Texture0; //Environment map

const float PI = 3.14159265359f;

void main()
{
    vec3 N = normalize(FragPos);

    vec3 irradiance = vec3(0.0f);

    // tangent space calculation from origin point
    vec3 up    = vec3(0.0f, 1.0f, 0.0f);
    vec3 right = cross(up, N);
    up         = cross(N, right);

    float sampleDelta = 0.025f;
    float numSamples = 0.0f;
    for (float phi = 0.0f; phi < 2.0f * PI; phi += sampleDelta)
    {
        for (float theta = 0.0f; theta < 0.5f * PI; theta += sampleDelta)
        {
            // spherical to cartesian (in tangent space)
            float tanX = sin(theta) * cos(phi);
            float tanY = sin(theta) * sin(phi);
            float tanZ = cos(theta);

            // tangent space to world
            vec3 sampleVec = tanX * right + tanY * up + tanZ * N;

            irradiance += texture(Texture0, sampleVec).rgb * cos(theta) * sin(theta);
            numSamples += 1.0f;
        }
    }

    FragColor = PI * irradiance * (1.0f / numSamples);
}
