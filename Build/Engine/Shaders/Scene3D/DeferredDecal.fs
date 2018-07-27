#version 450

const float PI = 3.14159265359f;
const float InvPI = 0.31831f;

layout (location = 0) out vec4 AlbedoOpacity;
layout (location = 1) out vec3 Normal;
layout (location = 2) out vec4 RMSI;

layout(location = 0) in vec3 FragPos;

uniform sampler2D Texture0; //Screen AlbedoOpacity
uniform sampler2D Texture1; //Screen Normal
uniform sampler2D Texture2; //Screen PBR: Roughness, Metallic, Specular, Index of refraction
uniform sampler2D Texture3; //Screen Depth
uniform sampler2D Texture4; //Decal AlbedoOpacity
uniform sampler2D Texture5; //Decal Normal
uniform sampler2D Texture6; //Decal PBR: Roughness, Metallic, Specular, Index of refraction

uniform mat4 InvProjMatrix;
uniform mat4 CameraToWorldSpaceMatrix;
uniform mat4 InvMeshWorldMatrix;
uniform mat4 MeshWorldMatrix;
uniform vec2 MeshScale;

vec3 WorldPosFromDepth(in float depth, in vec2 uv)
{
	vec4 clipSpacePosition = vec4(vec3(uv, depth) * 2.0f - 1.0f, 1.0f);
	vec4 viewSpacePosition = InvProjMatrix * clipSpacePosition;
	viewSpacePosition /= viewSpacePosition.w;
	return (CameraToWorldSpaceMatrix * viewSpacePosition).xyz;
}
void main()
{
  vec2 uv = gl_FragCoord.xy / vec2(ScreenWidth, ScreenHeight);

	//Retrieve shading information from GBuffer textures
	vec3 albedo = texture(Texture0, uv).rgb;
	vec3 normal = texture(Texture1, uv).rgb;
	vec3 rms = texture(Texture2, uv).rgb;
	float depth = texture(Texture3, uv).r;

	//Resolve world fragment position using depth and screen UV
	vec3 fragPosWS = WorldPosFromDepth(depth, uv);
  vec3 fragPosOS = (InvMeshWorldMatrix * fragPosWS);
  fragPosOS.xy /= MeshScale;
  if (abs(fragPosOS.x) > 0.5f || abs(fragPosOS.y) > 0.5f)
    discard;
  vec2 decalUV = fragPosOS.xy + vec2(0.5f);

	AlbedoOpacity = texture(Texture4, decalUV);
  Normal = normalize(mat3(MeshWorldMatrix) * texture(Texture5, decalUV));
	RMSI = texture(Texture6, decalUV);
}
