#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0 : register(s0);

float4 MainPS(float2 coords : TEXCOORD0) : COLOR
{
	float3 colour = tex2D(s0, coords).rgb;
    return float4(colour.r, colour.g, colour.b, 1.0);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
    }
};