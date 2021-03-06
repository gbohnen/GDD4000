//////////////////////////////////////////////////////////////////////////
// Effect Parameters
//////////////////////////////////////////////////////////////////////////
float4x4 World;
float4x4 View;
float4x4 Projection;

//////////////////////////////////////////////////////////////////////////
// Constants
//////////////////////////////////////////////////////////////////////////
static const float PI = 3.14159265f;

#define PALE_BLUE float4(0.35, 0.35, 0.55, 1.0)
#define MEDIUM_BLUE float4(0.10, 0.10, 0.30, 1.0)
#define DARK_BLUE float4(0.05, 0.05, 0.26, 1.0)
#define DARKER_BLUE float4(0.03, 0.03, 0.20, 1.0)
#define WHITE float4(0.8, 0.8, 1.0, 1.0)

//////////////////////////////////////////////////////////////////////////
// Shader Data Structures
//////////////////////////////////////////////////////////////////////////
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
	float4 wPosition: TEXCOORD1;
};

//////////////////////////////////////////////////////////////////////////
// Noise permutation texture
//////////////////////////////////////////////////////////////////////////
texture permTexture2d;
texture permGradTexture;

sampler permSampler2d = sampler_state
{
	texture = <permTexture2d>;
	AddressU = Wrap;
	AddressV = Wrap;
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = NONE;
};

sampler permGradSampler = sampler_state
{
	texture = <permGradTexture>;
	AddressU = Wrap;
	AddressV = Clamp;
	MAGFILTER = POINT;
	MINFILTER = POINT;
	MIPFILTER = NONE;
};

//////////////////////////////////////////////////////////////////////////
// Parameters and Spline Values for Marble
//////////////////////////////////////////////////////////////////////////

// Define arbitrary weights for marble splines
#define CR00 (-0.5)
#define CR01 (1.5)
#define CR02 (-1.5)
#define CR03 (0.5)
#define CR10 (1.0)
#define CR11 (-2.5)
#define CR12 (1.0)
#define CR13 (-0.5)
#define CR20 (-0.5)
#define CR21 (0.5)
#define CR22 (0.5)
#define CR23 (0.0)
#define CR30 (0.0)
#define CR31 (1.0)
#define CR32 (0.0)
#define CR33 (0.0)

// Calculate the splines to be used to drive the marble shape
float4 spline(float x, int nknots, float4 knots[25]) {
	int nspans = nknots - 3;
	if (nspans < 1) {
		//there must be at least one span
		return float4(0.0, 0.0, 0.0, 0.0);
	}
	else if (x < 0.0) {
		return knots[1];
	}
	else if (x >= 1.0) {
		return knots[nknots - 2];
	}
	else {
		float4 val0, val1, val2, val3;
		if (x < 1.0 / float(nspans)) {
			val0 = knots[0];
			val1 = knots[1];
			val2 = knots[2];
			val3 = knots[3];
		}
		else if (x < 2.0 / float(nspans)) {
			val0 = knots[1];
			val1 = knots[2];
			val2 = knots[3];
			val3 = knots[4];
		}
		else if (x < 3.0 / float(nspans)) {
			val0 = knots[2];
			val1 = knots[3];
			val2 = knots[4];
			val3 = knots[5];
		}
		else if (x < 4.0 / float(nspans)) {
			val0 = knots[3];
			val1 = knots[4];
			val2 = knots[5];
			val3 = knots[6];
		}
		else if (x < 5.0 / float(nspans)) {
			val0 = knots[4];
			val1 = knots[5];
			val2 = knots[6];
			val3 = knots[7];
		}
		else if (x < 6.0 / float(nspans)) {
			val0 = knots[5];
			val1 = knots[6];
			val2 = knots[7];
			val3 = knots[8];
		}
		else if (x < 7.0 / float(nspans)) {
			val0 = knots[6];
			val1 = knots[7];
			val2 = knots[8];
			val3 = knots[9];
		}
		else if (x < 8.0 / float(nspans)) {
			val0 = knots[7];
			val1 = knots[8];
			val2 = knots[9];
			val3 = knots[10];
		}
		else if (x < 9.0 / float(nspans)) {
			val0 = knots[8];
			val1 = knots[9];
			val2 = knots[10];
			val3 = knots[11];
		}
		else if (x < 10.0 / float(nspans)) {
			val0 = knots[9];
			val1 = knots[10];
			val2 = knots[11];
			val3 = knots[12];
		}
		else if (x < 11.0 / float(nspans)) {
			val0 = knots[10];
			val1 = knots[11];
			val2 = knots[12];
			val3 = knots[13];
		}
		else if (x < 12.0 / float(nspans)) {
			val0 = knots[11];
			val1 = knots[12];
			val2 = knots[13];
			val3 = knots[14];
		}
		else if (x < 13.0 / float(nspans)) {
			val0 = knots[12];
			val1 = knots[13];
			val2 = knots[14];
			val3 = knots[15];
		}
		else if (x < 14.0 / float(nspans)) {
			val0 = knots[13];
			val1 = knots[14];
			val2 = knots[15];
			val3 = knots[16];
		}
		else if (x < 15.0 / float(nspans)) {
			val0 = knots[14];
			val1 = knots[15];
			val2 = knots[16];
			val3 = knots[17];
		}
		else if (x < 16.0 / float(nspans)) {
			val0 = knots[15];
			val1 = knots[16];
			val2 = knots[17];
			val3 = knots[18];
		}
		else if (x < 17.0 / float(nspans)) {
			val0 = knots[16];
			val1 = knots[17];
			val2 = knots[18];
			val3 = knots[19];
		}
		else if (x < 18.0 / float(nspans)) {
			val0 = knots[17];
			val1 = knots[18];
			val2 = knots[19];
			val3 = knots[20];
		}
		else if (x < 19.0 / float(nspans)) {
			val0 = knots[18];
			val1 = knots[19];
			val2 = knots[20];
			val3 = knots[21];
		}
		else if (x < 20.0 / float(nspans)) {
			val0 = knots[19];
			val1 = knots[20];
			val2 = knots[21];
			val3 = knots[22];
		}
		else if (x < 21.0 / float(nspans)) {
			val0 = knots[20];
			val1 = knots[21];
			val2 = knots[22];
			val3 = knots[23];
		}
		else {
			val0 = knots[21];
			val1 = knots[22];
			val2 = knots[23];
			val3 = knots[24];
		}

		float y = frac(clamp(x, 0.0, 1.0) * float(nspans));

		float4 c3 = CR00*val0 + CR01*val1 + CR02*val2 + CR03*val3;
		float4 c2 = CR10*val0 + CR11*val1 + CR12*val2 + CR13*val3;
		float4 c1 = CR20*val0 + CR21*val1 + CR22*val2 + CR23*val3;
		float4 c0 = CR30*val0 + CR31*val1 + CR32*val2 + CR33*val3;

		return ((c3*y + c2)*y + c1)*y + c0;
	}
}

// Define the colors to use for the marble
float4 marble_color(float m) {
	float4 c[25];

	c[0] = PALE_BLUE;
	c[1] = MEDIUM_BLUE;
	c[2] = WHITE;
	c[3] = DARKER_BLUE;
	c[4] = PALE_BLUE;
	c[5] = MEDIUM_BLUE;
	c[6] = WHITE;
	c[7] = DARK_BLUE; 
	c[8] = DARKER_BLUE;       // Inner fills, PALE_BLUE
	c[9] = PALE_BLUE;     // Middle ring, DARKER_BLUE
	c[10] = MEDIUM_BLUE;    // Outer ring, DARKER_BLUE
	c[11] = WHITE;      // Outer fills, PALE_BLUE
	c[12] = DARKER_BLUE;

	float4 res = spline(clamp(2.0*m + 0.75, 0.0, 1.0), 13, c);

	return res;
}

//////////////////////////////////////////////////////////////////////////
// Noise Functions
//////////////////////////////////////////////////////////////////////////
float3 fade(float3 t)
{
	return t * t * t * (t * (t * 6 - 15) + 10); // new curve
}

float4 perm2d(float2 p)
{
	return tex2D(permSampler2d, p);
}

float gradperm(float x, float3 p)
{
	return dot(tex1D(permGradSampler, x), p);
}

float inoise(float3 p)
{
	float divisor = 256.0;

	float3 P = fmod(floor(p), divisor);	// FIND UNIT CUBE THAT CONTAINS POINT
	p -= floor(p);                      // FIND RELATIVE X,Y,Z OF POINT IN CUBE.
	float3 f = fade(p);                 // COMPUTE FADE CURVES FOR EACH OF X,Y,Z.

	P = P / divisor;
	const float one = 1.0 / divisor;

	// HASH COORDINATES OF THE 8 CUBE CORNERS
	float4 AA = perm2d(P.xy) + P.z;

	// AND ADD BLENDED RESULTS FROM 8 CORNERS OF CUBE
	return lerp(lerp(lerp(gradperm(AA.x, p),
		gradperm(AA.z, p + float3(-1, 0, 0)), f.x),
		lerp(gradperm(AA.y, p + float3(0, -1, 0)),
			gradperm(AA.w, p + float3(-1, -1, 0)), f.x), f.y),

		lerp(lerp(gradperm(AA.x + one, p + float3(0, 0, -1)),
			gradperm(AA.z + one, p + float3(-1, 0, -1)), f.x),
			lerp(gradperm(AA.y + one, p + float3(0, -1, -1)),
				gradperm(AA.w + one, p + float3(-1, -1, -1)), f.x), f.y), f.z);
}

float4 noiseVector(float3 p)
{
	float freq = 1.0;
	float4 output = float4(0.0, 0.0, 0.0, 0.0);

	for (int i = 0; i < 4; ++i)
	{
		output[i] = inoise(p * freq) / freq;
		freq *= 2.0;
	}

	return output;
}

//////////////////////////////////////////////////////////////////////////
// Vertex Shader
//////////////////////////////////////////////////////////////////////////
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.wPosition = input.Position;
	output.texCoord = input.texCoord;

	return output;
}

//////////////////////////////////////////////////////////////////////////
// Pixel Shaders
//////////////////////////////////////////////////////////////////////////
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 p = input.wPosition;
	float scale = 1;
	float3 inz = inoise(p * scale)*0.5 + 0.5;
	return float4(inz.xyz, 1.0);
}

float4 PixelShaderFunction_Scribbles(VertexShaderOutput input) : COLOR0
{
	float NoiseAmp = 10;
	float NoiseScale = .5;
	float LightIntensity = .9;
	float A = .2;
	float P = .45;
	float Tol = .5;

	float4 BLUE = float4(0., 0., 1., 1.);

	float4  noisevec = float4(0.0, 0.0, 0.0, 0.0);
	for (int i = 0; i < 4; ++i)
	{
		noisevec[i] = inoise(input.wPosition * NoiseScale) / NoiseScale;
		NoiseScale *= 2.0;
	}

	float size = noisevec[0] + noisevec[1] + noisevec[2] + noisevec[3];
	size = .5 * (size - 1.);
	float deltax = NoiseAmp * size;

	float f = frac(A * (input.wPosition.x + deltax));

	float t = smoothstep(0.5 - P - Tol, 0.5 - P + Tol, f)
				-smoothstep(0.5 + P - Tol, 0.5 + P + Tol, f);

	float4 output = lerp(BLUE, WHITE, t);
	output.rgb *= LightIntensity;

	return output;
}

float4 PixelShaderFunction_Banding(VertexShaderOutput input) : COLOR0
{
	float NoiseAmp = 5;
	float NoiseScale = 1;
	float LightIntensity = .9;
	float A = .2;
	float P = .45;
	float Tol = .5;

	float4 BLUE = float4(0., 0., 1., 1.);

	float4 noisevec = float4(0.0, 0.0, 0.0, 0.0);
	for (int i = 0; i < 4; ++i)
	{
		noisevec[i] = inoise(input.wPosition * NoiseScale) / NoiseScale;
		NoiseScale *= 2.0;
	}

	float size = noisevec[0] + noisevec[1] + noisevec[2] + noisevec[3];
	size = .5 * (size - 1.);
	float deltax = NoiseAmp * size;

	float f = frac(A * (input.wPosition.x + deltax));

	float t = smoothstep(0.5 - P - Tol, 0.5 - P + Tol, f)
				-smoothstep(0.5 + P - Tol, 0.5 + P + Tol, f);

	float4 output = lerp(BLUE, WHITE, t);
	output.rgb *= LightIntensity;

	return output;
}

float4 PixelShaderFunction_Contours(VertexShaderOutput input) : COLOR0
{
	float NoiseAmp = 1;
	float NoiseScale = .1;
	float LightIntensity = .9;
	float A = .2;
	float P = .45;
	float Tol = .5;

	float4 BLUE = float4(0., 0., 1., 1.);

	float4  noisevec = float4(0.0, 0.0, 0.0, 0.0);
	for (int i = 0; i < 4; ++i)
	{
		noisevec[i] = inoise(input.wPosition * NoiseScale) / NoiseScale;
		NoiseScale *= 2.0;
	}

	float size = noisevec[0] + noisevec[1] + noisevec[2] + noisevec[3];
	size = .5 * (size - 1.);
	float deltax = NoiseAmp * size;

	float f = frac(A * (input.wPosition.x + deltax));

	float t = smoothstep(0.5 - P - Tol, 0.5 - P + Tol, f)
				-smoothstep(0.5 + P - Tol, 0.5 + P + Tol, f);

	float4 output = lerp(BLUE, WHITE, t);
	output.rgb *= LightIntensity;

	return output;
}

float4 PixelShaderFunction_Turbulence(VertexShaderOutput input) : COLOR0
{
	float LightIntensity = .8;
	float Amplify = .4;
	float NoiseScale = .25;
	float4 Color1 = float4(1.0, 1.0, 0.0, 1.0);
	float4 Color2 = float4(1.0, 0.0, 0.0, 1.0);

	float4  noisevec = float4(0.0, 0.0, 0.0, 0.0);
	for (int i = 0; i < 4; ++i)
	{
		noisevec[i] = inoise(input.wPosition * NoiseScale) / NoiseScale;
		NoiseScale *= 2.0;
	}
	//return noisevec;

	float sum = (abs(noisevec[0] - .5) + abs(noisevec[1] - .5)
		+ abs(noisevec[2] - .5) + abs(noisevec[3] - .5)) / 2.0;

	sum = clamp(sum * Amplify, 0.0, 1.0);

	float4 output = lerp(Color1, Color2, sum) * LightIntensity;

	return output;
}

float4 PixelShaderFunction_Discard(VertexShaderOutput input) : COLOR0
{
	float LightIntensity = .9;
	float Amplify = .7;
	float NoiseScale = .2;
	float4 Color1 = float4(1.0, 0.0, 0.0, 1.0);
	float4 Color2 = float4(0.0, 1.0, 0.0, 1.0);

	float4  noisevec = float4(0.0, 0.0, 0.0, 0.0);
	for (int i = 0; i < 4; ++i)
	{
		// 2.17 is an arbitrary value chosen at >= 2.0
		noisevec[i] = inoise(input.wPosition * NoiseScale) / NoiseScale;
		NoiseScale *= 2.0;
	}

	float sum = (abs(noisevec[0] - .5) + abs(noisevec[1] - .5)
		+ abs(noisevec[2] - .5) + abs(noisevec[3] - .5)) / 2.0;

	sum = clamp(sum * Amplify, 0.0, 1.0);

	float4 output = lerp(Color1, Color2, sum) * LightIntensity;

	if (output.r < .10)
		discard;

	return output;
}

float4 PixelShaderFunction_Marble(VertexShaderOutput input) : COLOR0
{
	float LightIntensity = .9;
	float Amplify = .5;
	float NoiseScale = .1;
	float NoiseOctaves = 4;

	float4  noisevec = float4(0.0, 0.0, 0.0, 0.0);
	for (int i = 0; i < 4; ++i)
	{
		noisevec[i] = ((inoise(input.wPosition * NoiseScale) / NoiseScale) + 1) / 2;
		NoiseScale *= 2.0;
	}

	// size has range 1 to 4
	float intensity = noisevec[0] + noisevec[1] + noisevec[2] + noisevec[3];
	intensity = (intensity - 1.) / 4;  // size has range 0 to 1

	intensity = clamp(intensity * Amplify, 0.0, 1.0);

	float4 output = marble_color(intensity);
	return output;
}

float4 PixelShaderFunction_Cloud(VertexShaderOutput input) : COLOR0
{
	float LightIntensity = 1;
	float Amplify = .05;
	float NoiseScale = .1;   //  0 to 1
	float Bias = .9;         // -1 to 1
	float4 Color1 = float4(0.0, 0.0, 0.8, 1.0);
	float4 Color2 = float4(0.9, 0.9, 0.9, 1.0);

	float4  noisevec = float4(0.0, 0.0, 0.0, 0.0);
	//for (int i = 0; i < 4; ++i)
	//{
	//	noisevec[i] = ((inoise( input.wPosition * NoiseScale ) / NoiseScale) + 1) / 2;
	//	NoiseScale *= 2;
	//}

	//NoiseScale *= 8;
	noisevec = noiseVector(input.wPosition * NoiseScale);
	float intensity = (noisevec[0] + noisevec[1] + noisevec[2] + noisevec[3] - 1) / 2.;

	intensity = clamp(Bias + intensity, 0, 1);

	float4 output = float4(0.0, 0.0, 0.0, 1.0);
	output.rgb = lerp(Color1.rgb, Color2.rgb, intensity) * LightIntensity;

	return output;
}

float4 PixelShaderFunction_Wood(VertexShaderOutput input) : COLOR0
{
	float LightIntensity = .9;
	float NoiseScale = .1;
	float NoiseFreq = 1;
	float RingFreq = 1;
	float4 Color1 = float4(0.6, 0.47, 0.33, 1.0); // light wood
	float4 Color2 = float4(0.4, 0.3, 0.13, 1.0); // dark wood

	float4 noisevec = noiseVector(input.wPosition * NoiseScale);

	float3 location = input.wPosition.xyz + noisevec.xyz;
	float dist = sqrt(location.x * location.x + location.z * location.z)
				+ sqrt(100 + (location.y))
				+ sqrt(100 + (location.x));
	dist *= RingFreq;

	float intensity = frac(dist + noisevec[0] / 256.0 + noisevec[1] / 32.0
		+ noisevec[2] / 16.0) * 2.0;
	if (intensity > 1.0)
	intensity = 2 - intensity;

	float4 output = float4(0.0, 0.0, 0.0, 1.0);
	output.rgb = lerp(Color1.rgb, Color2.rgb, intensity) * LightIntensity;

	return output;
}

//////////////////////////////////////////////////////////////////////////
// Techniques
//////////////////////////////////////////////////////////////////////////
technique PerlinNoise_Scribbles
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Scribbles();
	}
}

technique PerlinNoise_Banding
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Banding();
	}
}

technique PerlinNoise_Contours
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Contours();
	}
}

technique PerlinNoise_Turbulence
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Turbulence();
	}
}

technique PerlinNoise_Discard
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Discard();
	}
}

technique PerlinNoise_Marble
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Marble();
	}
}

technique PerlinNoise_Cloud
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Cloud();
	}
}

technique PerlinNoise_Wood
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction_Wood();
	}
}

technique PerlinNoise
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
