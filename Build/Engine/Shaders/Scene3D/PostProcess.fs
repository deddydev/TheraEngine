#version 450

layout(location = 0) out vec4 OutColor;
layout(location = 0) in vec3 FragPos;

uniform sampler2D Texture0; //HDR scene color
uniform sampler2D Texture1; //Bloom
uniform sampler2D Texture2; //Depth
uniform usampler2D Texture3; //Stencil

uniform vec3 HighlightColor = vec3(0.92f, 1.0f, 0.086f);

struct VignetteStruct
{
    vec3 Color;
    float Intensity;
    float Power;
};
uniform VignetteStruct Vignette;

struct ColorGradeStruct
{
    vec3 Tint;

    float Exposure;
    float Contrast;
    float Gamma;

    float Hue;
    float Saturation;
    float Brightness;
};
uniform ColorGradeStruct ColorGrade;

vec3 RGBtoHSV(vec3 c)
{
    vec4 K = vec4(0.0f, -1.0f / 3.0f, 2.0f / 3.0f, -1.0f);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10f;
    return vec3(abs(q.z + (q.w - q.y) / (6.0f * d + e)), d / (q.x + e), q.x);
}
vec3 HSVtoRGB(vec3 c)
{
    vec4 K = vec4(1.0f, 2.0f / 3.0f, 1.0f / 3.0f, 3.0f);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0f - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0f, 1.0f), c.y);
}
float rand(vec2 coord)
{
    return fract(sin(dot(coord, vec2(12.9898f, 78.233f))) * 43758.5453f);
}
void main()
{
	vec2 uv = FragPos.xy;
	if (uv.x > 1.0f || uv.y > 1.0f)
		discard;

	vec3 hdrSceneColor = texture(Texture0, uv).rgb;
	vec3 bloom = texture(Texture1, uv).rgb;
	hdrSceneColor += bloom;

	//Color grading
	hdrSceneColor *= ColorGrade.Tint;
	vec3 hsv = RGBtoHSV(hdrSceneColor);
	hsv.x *= ColorGrade.Hue;
	hsv.y *= ColorGrade.Saturation;
	hsv.z *= ColorGrade.Brightness;
	hdrSceneColor = HSVtoRGB(hsv);
	hdrSceneColor = (hdrSceneColor - 0.5f) * ColorGrade.Contrast + 0.5f;

	//Tone mapping
	vec3 ldrSceneColor = vec3(1.0f) - exp(-hdrSceneColor * ColorGrade.Exposure);
    
	int outlineSize = 1;
    
	ivec2 texSize = textureSize(Texture0, 0);
	vec2 texelSize = 1.0f / texSize;
	vec2 texelX = vec2(texelSize.x, 0.0f);
	vec2 texelY = vec2(0.0f, texelSize.y);
    	uint stencilCurrent = texture(Texture3, uv).r;
	uint selectionBits = stencilCurrent & 3;
	uint diff = 0;
	vec2 zero = vec2(0.0f);
	//Check neighboring stencil texels that indicate highlighted/selected
	for (int i = 1; i <= outlineSize; ++i)
	{
    		vec2 yPos = clamp(uv + texelY * i, zero, uv);
    		vec2 yNeg = clamp(uv - texelY * i, zero, uv);
    		vec2 xPos = clamp(uv + texelX * i, zero, uv);
    		vec2 xNeg = clamp(uv - texelX * i, zero, uv);
		diff |= (texture(Texture3, yPos).r & 3) - selectionBits;
		diff |= (texture(Texture3, yNeg).r & 3) - selectionBits;
		diff |= (texture(Texture3, xPos).r & 3) - selectionBits;
		diff |= (texture(Texture3, xNeg).r & 3) - selectionBits;

		//diagonal is not necessary, just x and y is sufficient
		//vec2 diagTR = uv + texelSize * i;
    		//vec2 diagBL = uv - texelSize * i;
    		//vec2 diagTL = uv + vec2(-texelSize.x, texelSize.y) * i;
    		//vec2 diagBR = uv + vec2(texelSize.x, -texelSize.y) * i;
		//diff += texture(Texture3, diagTR).r - stencilCurrent;
		//diff += texture(Texture3, diagBL).r - stencilCurrent;
		//diff += texture(Texture3, diagTL).r - stencilCurrent;
		//diff += texture(Texture3, diagBR).r - stencilCurrent;
	}
	float diff2 = clamp(float(diff), 0.0f, 1.0f);
	ldrSceneColor = mix(ldrSceneColor, HighlightColor, diff2);

    //Vignette
    vec2 vigUV = uv * (1.0f - uv.yx);
    float vig = clamp(pow(vigUV.x * vigUV.y * Vignette.Intensity, Vignette.Power), 0.0f, 1.0f);
    ldrSceneColor = mix(Vignette.Color, ldrSceneColor, vig);

    //Gamma-correct
    vec3 gammaCorrected = pow(ldrSceneColor, vec3(1.0f / ColorGrade.Gamma));

    //Fix subtle banding by applying fine noise
    gammaCorrected += mix(-0.5f / 255.0f, 0.5f / 255.0f, rand(uv));

    OutColor = vec4(gammaCorrected, 1.0f);
}