#version 330 core
layout (triangles) in;
layout (triangle_strip, max_vertices=18) out;

uniform mat4 ShadowMatrices[6];

out vec3 FragPos;

void main()
{
	vec4 pos;
    for (int face = 0; face < 6; ++face)
    {
        gl_Layer = face;
        for (int i = 0; i < 3; ++i)
        {
            pos = gl_in[i].gl_Position;
			FragPos = pos.xyz;
            gl_Position = ShadowMatrices[face] * pos;
            EmitVertex();
        }    
        EndPrimitive();
    }
} 