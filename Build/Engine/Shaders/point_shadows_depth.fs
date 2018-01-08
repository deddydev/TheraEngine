#version 330 core
in vec4 FragPos;

uniform vec3 LightPos;
uniform float FarPlane;

void main()
{
    // write modified depth
	// map to [0;1] range by dividing by far_plane
    gl_FragDepth = length(FragPos.xyz - LightPos) / FarPlane;
}