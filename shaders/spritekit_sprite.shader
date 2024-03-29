//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
	Description = "Shader for 2D Sprites";
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

	#define PS_INPUT_HAS_TANGENT_BASIS 1
	#define DEPTH_STATE_ALREADY_SET
}

//=========================================================================================================================

struct VertexInput
{
	#include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
	#include "common/pixelinput.hlsl"

	float3 wBitangent : TEXCOORD13;
};



//=========================================================================================================================

VS
{
	#include "common/vertex.hlsl"

	
	//
	// Main
	//
	PixelInput MainVs( INSTANCED_SHADER_PARAMS( VS_INPUT i ) )
	{
		PixelInput o = ProcessVertex( i );
		// Add your vertex manipulation functions here
		
		o = FinalizeVertex( o );
		o.wBitangent = cross(-o.vTangentUWs,o.vNormalWs) *o.vTangentVWs.y;
		float3 normal = normalize(o.vNormalWs.xyz);
    	float3 tangent = normalize(o.vTangentUWs.xyz);

		o.wBitangent = cross(normal, tangent.xyz);
		return o;
	}
}


//=========================================================================================================================

PS
{
    #include "common/pixel.hlsl"

	DynamicCombo( D_HAS_NORMALS, 0..1, Sys( ALL ) );

	RenderState( DepthEnable, false );

    

	float2 spriteStartUV <Attribute("StartUV");>;     // corner uv coord for sprite in atlas
	float2 spriteEndVU <Attribute("EndUV");>;       // opposite corner uv coord for sprite in atlas
	float FacingDirection<Attribute("Facing");Default(1);>;

	float3 vSpritePivot <Attribute("SpritePivot");>;
	
    CreateTexture2D( g_tDepthBufferCopyTexture ) <AsSceneDepth; SrgbRead( false );	AddressU( CLAMP ); AddressV( CLAMP ); Filter( POINT ); >;

	
	CreateTexture2D( g_tSpriteSheetTex )< Attribute( "SpriteSheet" );SrgbRead( true );Filter(NEAREST);AddressU( WRAP );AddressV( WRAP );>;
	CreateTexture2D( g_tSpriteSheetNormalTex )< Attribute( "SpriteSheetNormalMap" );SrgbRead( false );Filter(NEAREST);AddressU( WRAP );AddressV( WRAP );>;


	float FetchDepth( float2 vTexCoord )
	{
		float flProjectedDepth = Tex2D(g_tDepthBufferCopyTexture, vTexCoord.xy);
		// Remap depth to viewport depth range
		flProjectedDepth = RemapValClamped( flProjectedDepth, g_flViewportMinZ, g_flViewportMaxZ, 0.0, 1.0 );

		return flProjectedDepth;
	}

	bool DepthTest( PixelInput i )
	{
		float fDepth = FetchDepth(CalculateViewportUv( i.vPositionSs ) );
		float4 vPosPs = Position3WsToPs( vSpritePivot );
		float fDepthObj = vPosPs.z / vPosPs.w;

		return fDepth <= fDepthObj;
	}
	//
	// Main
	//
	float4 MainPs(PixelInput i) : SV_Target0
	{
		float4 o;
		if(FacingDirection == 1)
		i.vTextureCoords.x = 1 - i.vTextureCoords.x;
		float2 spriteRange = (spriteEndVU - spriteStartUV);
		float2 uv = spriteStartUV + (frac(clamp(i.vTextureCoords.xy,0.001f,0.999f) ) * spriteRange);
		float4 Color = Tex2D(g_tSpriteSheetTex, uv).rgba;
		if(Color.a <0.01f || DepthTest(i)){ 
			discard;
		}
		o.a = 1;


		/* #if D_HAS_NORMALS
			float3 norms = normalize(Tex2D(g_tSpriteSheetNormalTex, uv).rgb *2 -1);

			norms.x *= FacingDirection;
			float3 N = mul(norms.xyz,g_matWorldToView);
            half3 toonLight = saturate(dot(N , g_vSunLightDir)) > 0.3 ?  g_vSunLightColor : g_vUnlitShadowColor;
			o.rgb = Color * toonLight;

		#else */
        	o.rgb = Color;
		/* #endif */
		o.rgb = saturate(o.rgb + (Tint.rgb*TintAmount));
		
		
		return o;
	}
}