// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHAD_Dragon_Symbol_Glow"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Cutoff( "Mask Clip Value", Float ) = 1
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Glowiness("Glowiness", Range( 0 , 1)) = 0
		[Enum(Colour,0,Grey,1)]_ColourOrGrey("ColourOrGrey", Int) = 0
		[Enum(Off,0,On,1)]_TextureOffOn("TextureOffOn", Int) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform int _ColourOrGrey;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform float _Glowiness;
		uniform sampler2D _GrabTexture;
		uniform int _TextureOffOn;
		uniform float _Cutoff = 1;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode5 = tex2D( _TextureSample0, uv_TextureSample0 );
			float3 desaturateInitialColor31 = tex2DNode5.rgb;
			float desaturateDot31 = dot( desaturateInitialColor31, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar31 = lerp( desaturateInitialColor31, desaturateDot31.xxx, (float)_ColourOrGrey );
			o.Albedo = desaturateVar31;
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			float lerpResult40 = lerp( 1.0 , 0.2 , (float)_ColourOrGrey);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor11 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
			o.Emission = ( tex2D( _TextureSample1, uv_TextureSample1 ).a * _Glowiness * ( lerpResult40 * screenColor11 ) ).rgb;
			float lerpResult33 = lerp( 1.0 , 0.8 , (float)_ColourOrGrey);
			o.Alpha = lerpResult33;
			clip( ( tex2DNode5.a * _TextureOffOn ) - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
238;91;1325;634;1538.823;372.532;1.281103;True;False
Node;AmplifyShaderEditor.RangedFloatNode;39;-869.5828,-63.3236;Float;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;29;-536.4012,-241.7861;Float;False;Property;_ColourOrGrey;ColourOrGrey;4;1;[Enum];Create;True;2;Colour;0;Grey;1;0;False;0;0;1;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-877.4157,-131.2557;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;-745.4,-122.3274;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;11;-896.9916,174.4492;Float;False;Global;_GrabScreen0;Grab Screen 0;4;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-703.5429,25.89097;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-1179.808,-93.32061;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;5b902e0f7b3b1204f98637396b0915ce;5b902e0f7b3b1204f98637396b0915ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-1165.759,98.67585;Float;True;Property;_Glowiness;Glowiness;3;0;Create;True;0;0;False;0;0;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-919.313,-377.7844;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;5b902e0f7b3b1204f98637396b0915ce;5b902e0f7b3b1204f98637396b0915ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;27;-512.3047,293.476;Float;False;Property;_TextureOffOn;TextureOffOn;5;1;[Enum];Create;True;2;Off;0;On;1;0;False;0;0;1;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-535.6459,-101.0832;Float;False;Constant;_ColourOpactiy;ColourOpactiy;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-527.813,-33.15104;Float;False;Constant;_GreyOpacity;GreyOpacity;6;0;Create;True;0;0;False;0;0.8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-223.1295,243.7901;Float;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;31;-349.2417,-363.3367;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;33;-337.013,-97.27927;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-539.7067,104.3323;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Dragon_Symbol_Glow;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;1;True;True;0;True;Transparent;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;38;0
WireConnection;40;1;39;0
WireConnection;40;2;29;0
WireConnection;37;0;40;0
WireConnection;37;1;11;0
WireConnection;21;0;5;4
WireConnection;21;1;27;0
WireConnection;31;0;5;0
WireConnection;31;1;29;0
WireConnection;33;0;34;0
WireConnection;33;1;35;0
WireConnection;33;2;29;0
WireConnection;10;0;6;4
WireConnection;10;1;9;0
WireConnection;10;2;37;0
WireConnection;0;0;31;0
WireConnection;0;2;10;0
WireConnection;0;9;33;0
WireConnection;0;10;21;0
ASEEND*/
//CHKSM=CEE531755172B437E9993CA30E8B0E26E641296E