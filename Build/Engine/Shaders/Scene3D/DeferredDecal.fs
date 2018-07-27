#version 450

const float PI = 3.14159265359f;
const float InvPI = 0.31831f;

layout (location = 0) out vec4 AlbedoOpacity;
layout (location = 1) out vec3 Normal;
layout (location = 2) out vec4 RMSI;

layout(location = 0) in vec3 FragPos;

uniform sampler2D Texture0; //Screen AlbedoOpacity
uniform sampler2D Texture1; //Decal AlbedoOpacity
uniform sampler2D Texture2; //Decal Normal
uniform sampler2D Texture3; //Decal PBR: Roughness, Metallic, Specular, Index of refraction

uniform float ScreenWidth;
uniform float ScreenHeight;
uniform mat4 InvProjMatrix;
uniform mat4 CameraToWorldSpaceMatrix;
uniform mat4 InvBoxWorldMatrix;
uniform mat4 BoxWorldMatrix;
uniform vec3 BoxHalfScale;

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
	float depth = texture(Texture0, uv).r;

	//Resolve world fragment position using depth and screen UV
	vec3 fragPosWS = WorldPosFromDepth(depth, uv);
  vec4 fragPosOS = (InvBoxWorldMatrix * vec4(fragPosWS, 1.0f));
	fragPosOS.xyz /= BoxHalfScale;
  if (abs(fragPosOS.x) > 1.0f || abs(fragPosOS.z) > 1.0f)
    discard;
  vec2 decalUV = fragPosOS.xz * vec2(0.5f) + vec2(0.5f);

	AlbedoOpacity = texture(Texture1, decalUV);
  Normal = normalize(mat3(BoxWorldMatrix) * texture(Texture2, decalUV).rgb);
	RMSI = texture(Texture3, decalUV);
}
