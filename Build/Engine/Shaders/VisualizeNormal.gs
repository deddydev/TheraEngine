#version 450
layout(triangles) in;
layout(line_strip, max_vertices=6) out;

in vec3 FragPos[];
in vec3 FragNorm[];

uniform mat4 WorldToCameraSpaceMatrix;
uniform mat4 ProjMatrix;

out vec3 FragPos;
out vec3 FragNorm;

in gl_PerVertex
{
    vec4  gl_Position;
    float gl_PointSize;
    float gl_ClipDistance[];
} gl_in[];

in int gl_PrimitiveIDIn;
in int gl_InvocationID;

out gl_PerVertex
{
    vec4  gl_Position;
    float gl_PointSize;
    float gl_ClipDistance[];
};

out int gl_PrimitiveID;
out int gl_Layer;
out int gl_ViewportIndex; 

void main()
{
    int i;
    for (i = 0; i < gl_in.length(); i++)
    {
        vec3 P = FragPos[i];
        vec3 N = FragNorm[i];

        gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * vec4(P, 1.0f);
        FragPos = FragPos[i];
        FragNorm = FragNorm[i];
        EmitVertex();

        gl_Position = ProjMatrix * WorldToCameraSpaceMatrix * vec4(P + N * 10.0f, 1.0f);
        FragPos = FragPos[i];
        FragNorm = FragNorm[i];
        EmitVertex();

        EndPrimitive();
    }
}