//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
	Description = "Shader for Map Geometry";
	DevShader = true;
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
MODES
{
    VrForward();													// Indicates this shader will be used for main rendering
    Depth( "vr_depth_only.vfx" ); 									// Shader that will be used for shadowing and depth prepass
    ToolsVis( S_MODE_TOOLS_VIS ); 									// Ability to see in the editor
    ToolsWireframe( "vr_tools_wireframe.vfx" ); 					// Allows for mat_wireframe to work
	ToolsShadingComplexity( "vr_tools_shading_complexity.vfx" ); 	// Shows how expensive drawing is in debug view
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"

	float3 Tint<Attribute("TintColor");>;
	float TintAmount<Attribute("TintAmount");>;
	
	float AmbientOcclusionAmount<UiType(Slider);Range(0,10);Default(1);>;
	float AmbientOcclusionDistance<UiType(Slider);Range(0,10);Default(1);>;

	float2 AmbientOcclusionSquare<UiType(Slider);Range2(0,0,1,1);Default2(0.1,0.9);>;

	#define CUSTOM_TEXTURE_FILTERING

	SamplerState TextureFiltering < Filter( NEAREST ); MaxAniso( 8 ); >;



	#define PS_INPUT_HAS_TANGENT_BASIS 1
}

//=========================================================================================================================

struct VertexInput
{
	#include "common/vertexinput.hlsl"
	float2 fBlendAmountPerSide : TEXCOORD17 <Semantic( none);>;
	float4 vUVData : TEXCOORD18 <Semantic( none);>;
};

//=========================================================================================================================

struct PixelInput
{
	#include "common/pixelinput.hlsl"

	float2 fBlendAmountPerSide : TEXCOORD17;
	float4 vUVData : TEXCOORD18;
};



//=========================================================================================================================

VS
{
	#include "common/vertex.hlsl"
	//
	// Main
	//
	PixelInput MainVs( INSTANCED_SHADER_PARAMS( VertexInput i ) )
	{
		PixelInput o = ProcessVertex( i );
		// Add your vertex manipulation functions here
		
		o = FinalizeVertex( o );
		o.fBlendAmountPerSide = i.fBlendAmountPerSide;
		o.vUVData = i.vUVData;
		return o;
	}
}


//=========================================================================================================================

PS
{
    #include "common/pixel.hlsl"
	CreateInputTexture2D( SpriteSheet,            Srgb,   8, "",                 "_color",  "Material,10/10", Default3( 1.0, 1.0, 1.0 ) );

	CreateInputTexture2D( SpriteSheetOpacityMask,     Linear, 8, "",                 "_mask",   "Material,10/70", Default( 1.0 ) );
    #define COLOR_TEXTURE_CHANNELSs Channel( RGB,  Box( SpriteSheet ), Srgb ); Channel( A, Box( SpriteSheetOpacityMask ), Linear )

	CreateTexture2DWithoutSampler( g_tSpriteSheet ) < COLOR_TEXTURE_CHANNELSs; OutputFormat( BC7 ); SrgbRead( true ); >;
	CreateTexture2D( g_tSpriteSheetNormalTexCode )< Attribute( "SpriteSheetNormalMap" );SrgbRead( false );>;

	float fRoughness<UiType(Slider);Range(0,1);Default(0.5);>;

	//
	// Main
	//
	float4 MainPs(PixelInput i) : SV_Target0
	{
		float2 spriteStartUV = i.vUVData.xy;
		float2 spriteEndVU = i.vUVData.zw;
		float2 spriteRange = (spriteEndVU - spriteStartUV);
		float2 uv = spriteStartUV + (frac(clamp(i.vTextureCoords.xy,0.001f,0.999f) ) * spriteRange);

		float4 Albedo = Tex2DS(g_tSpriteSheet,TextureFiltering, uv).rgba;
		Albedo.rgb = saturate(Albedo.rgb + (Tint.rgb*TintAmount));
		/* PixelOutput o;
		o.vColor.rgba = 1;
		o.vColor.rgb = 0;
		o.vColor.rg = uv;
		return o; */
		//add ambient occlusion with a 0.2% of TextureCoords if it is a wall in the direction of the wall
		float fBlendAmount = 0;

		if(i.fBlendAmountPerSide.y > 0.5){
			fBlendAmount =(i.vTextureCoords.x);
			Albedo.rgb = Albedo.rgb * saturate(((AmbientOcclusionDistance - fBlendAmount)*AmbientOcclusionAmount*2));
		}
		if(i.fBlendAmountPerSide.x > 0.5){
			fBlendAmount =1-(i.vTextureCoords.x);
			Albedo.rgb = Albedo.rgb * saturate(((AmbientOcclusionDistance - fBlendAmount)*AmbientOcclusionAmount*2));
		}
		
		if(i.vTextureCoords.y < AmbientOcclusionSquare.y && dot( i.vNormalWs,float3(0,0,1)) == 0){
			//blend from y = 0 to y = AmbientOcclusionSquare.y = 1
			fBlendAmount = i.vTextureCoords.y;
			Albedo.rgb = Albedo.rgb * saturate((( (AmbientOcclusionDistance) -fBlendAmount)*AmbientOcclusionAmount));
		}
		
		
		Material b = ToMaterial(i,
			float4(Albedo.rgb,1),
			float4(0.5,0.5,1,0),
			float4(0,0,0,0)
		);

		
		
		float4 po = FinalizePixelMaterial( i, b );
		po.a = Albedo.a;
		return po;
	}
}