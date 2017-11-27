#version 450

out vec4 OutColor;
in vec3 FragPos;

uniform sampler2D Texture0; //HDR HUD Color
uniform sampler2D Texture1; //Depth

uniform float Exposure;
uniform float InvGamma;

float rand(vec2 co)
{
    return fract(sin(dot(co.xy, vec2(12.9898,78.233))) * 43758.5453);
}

void main()
{
    vec2 uv = FragPos.xy;
    vec3 hdrSceneColor = texture(Texture0, uv).rgb;
    float Depth = texture(Texture1, uv).r;

    //Tone mapping
    vec3 ldrSceneColor = vec3(1.0) - exp(-hdrSceneColor * Exposure);
    
    //Gamma-correct
    vec3 gammaCorrected = pow(ldrSceneColor, vec3(InvGamma));

    //Fix subtle banding by applying fine noise
    gammaCorrected += mix(-0.5/255.0, 0.5/255.0, rand(uv));

    OutColor = vec4(gammaCorrected, 1.0);
}