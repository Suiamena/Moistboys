// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHAD_Dragon_Symbol_Glow"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Glowiness("Glowiness", Range( 0 , 10)) = 0
		[Enum(Colour,0,Grey,1)]_ColourOrGrey("ColourOrGrey", Int) = 0
		[Enum(Off,0,On,1)]_TextureOffOn("TextureOffOn", Int) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
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
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor11 = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD( ase_grabScreenPos ) );
			o.Emission = ( ( tex2D( _TextureSample1, uv_TextureSample1 ).a * _Glowiness ) * screenColor11 ).rgb;
			o.Alpha = 1;
			clip( ( tex2DNode5.a * _TextureOffOn ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
407;92;1441;863;1659.662;542.764;1.689243;True;True
Node;AmplifyShaderEditor.RangedFloatNode;9;-1165.759,98.67585;Float;True;Property;_Glowiness;Glowiness;3;0;Create;True;0;0;False;0;0;0.25;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-1183.187,-140.6194;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;5b902e0f7b3b1204f98637396b0915ce;5b902e0f7b3b1204f98637396b0915ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-854.3088,-270.9918;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;5b902e0f7b3b1204f98637396b0915ce;5b902e0f7b3b1204f98637396b0915ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-820.0952,157.2374;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;27;-478.1684,291.1212;Float;False;Property;_TextureOffOn;TextureOffOn;5;1;[Enum];Create;True;2;Off;0;On;1;0;False;0;0;1;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;29;-744.2907,-66.69348;Float;False;Property;_ColourOrGrey;ColourOrGrey;4;1;[Enum];Create;True;2;Colour;0;Grey;1;0;False;0;0;1;0;1;INT;0
Node;AmplifyShaderEditor.ScreenColorNode;11;-842.9359,262.2897;Float;False;Global;_GrabScreen0;Grab Screen 0;4;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-636.6301,166.4903;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-259.6364,221.4286;Float;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;31;-341.8201,-178.5983;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Dragon_Symbol_Glow;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;1;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;6;4
WireConnection;10;1;9;0
WireConnection;12;0;10;0
WireConnection;12;1;11;0
WireConnection;21;0;5;4
WireConnection;21;1;27;0
WireConnection;31;0;5;0
WireConnection;31;1;29;0
WireConnection;0;0;31;0
WireConnection;0;2;12;0
WireConnection;0;10;21;0
ASEEND*/
//CHKSM=E895CF6310776FDD98457306F93AADF235AE9B16