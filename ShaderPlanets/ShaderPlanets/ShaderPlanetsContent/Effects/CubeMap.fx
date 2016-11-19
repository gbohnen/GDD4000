////////////////////////////////////////////////////////////////////
// Effect Parameters
////////////////////////////////////////////////////////////////////
float4x4 WorldViewProjection;
float4x4 WorldView;
float4x4 Normal;

float Mix;                  // Mix of refraction and reflection
float RefractIndex;         // index of refraction
Texture ReflectionCubeMap;
Texture RefractionCubeMap;
float4 LightPosition;
float4 CameraPosition;

////////////////////////////////////////////////////////////////////
// Shader Structs
////////////////////////////////////////////////////////////////////
samplerCUBE ReflectionCubeMapSampler = sampler_state
{
    texture = <ReflectionCubeMap>;    
};

samplerCUBE RefractionCubeMapSampler = sampler_state
{
    texture = <RefractionCubeMap>;    
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 TexCoords : TEXCOORD0;
    float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float LightIntensity : COLOR0; 
    float3 ReflectVector : TEXCOORD0;
    float3 RefractVector : TEXCOORD1;
};

////////////////////////////////////////////////////////////////////
// Shaders
////////////////////////////////////////////////////////////////////
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    // Compute the position in eye coordinates
    float3 ECposition = (mul(input.Position, WorldView));

    // Compute the eye vector
	float3 eyeDir = normalize(ECposition.xyz);			// vector from eye to pt

    // Compute the vertex normal
    float3 normal = normalize(mul(input.Normal, Normal).xyz);

    // Compute the refraction and reflection vectors
	output.RefractVector = refract( eyeDir, normal, RefractIndex );
	output.ReflectVector = reflect( eyeDir, normal );

    // Compute ambient & diffuse lighting contribution
    output.LightIntensity  = 1.5 * abs( dot( normalize(LightPosition.xyz - ECposition.xyz), normal ) );
	if( output.LightIntensity < 0.2 )
		output.LightIntensity = 0.2;
		
    // Update the position into projection coordinates
	output.Position = mul(input.Position, WorldViewProjection);
    
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 WHITE = float4( 1.,1.,1.,1. );

    // Use the refraction vector to pull a color from the cube map
	float4 refractcolor = texCUBE( RefractionCubeMapSampler, 
                                   input.RefractVector );

    // Use the reflection vector to pull a color from the cube map
	float4 reflectcolor = texCUBE( ReflectionCubeMapSampler, 
                                   input.ReflectVector );

    // Combine the refraction color with WHITE (make a ghostly appearance)
	refractcolor = lerp( refractcolor, WHITE, .3 );

    // Combine the reflect & refract colors according to Mix
    // (0 is refract, 1 is reflect)
	return lerp( refractcolor, reflectcolor, Mix );
}

////////////////////////////////////////////////////////////////////
// Techniques
////////////////////////////////////////////////////////////////////
technique CubeMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
