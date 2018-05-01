#version 410

layout (location = 0) out vec3 FragColor;
layout (location = 0) in vec3 FragPos;

uniform samplerCube Texture0; //Environment map

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
    float nrSamples = 0.0f;
    for (float phi = 0.0f; phi < 2.0f * PI; phi += sampleDelta)
    {
        for (float theta = 0.0f; theta < 0.5f * PI; theta += sampleDelta)
        {
            // spherical to cartesian (in tangent space)
            vec3 tangentSample = vec3(sin(theta) * cos(phi),  sin(theta) * sin(phi), cos(theta));

            // tangent space to world
            vec3 sampleVec = tangentSample.x * right + tangentSample.y * up + tangentSample.z * N; 

            irradiance += texture(Texture0, sampleVec).rgb * cos(theta) * sin(theta);
            nrSamples++;
        }
    }

    FragColor = PI * irradiance * (1.0f / float(nrSamples));
}
