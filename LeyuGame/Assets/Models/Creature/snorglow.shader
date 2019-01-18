// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHAD_Creature_Glow"
{
	Properties
	{
		_TEX_Creature("TEX_Creature", 2D) = "white" {}
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_PulseSpeed("PulseSpeed", Float) = 1
		_Color0("Color 0", Color) = (0,1,0.990005,0)
		_GlowStrength("GlowStrength", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TEX_Creature;
		uniform float4 _TEX_Creature_ST;
		uniform float4 _Color0;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform float _PulseSpeed;
		uniform float _GlowStrength;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TEX_Creature = i.uv_texcoord * _TEX_Creature_ST.xy + _TEX_Creature_ST.zw;
			o.Albedo = tex2D( _TEX_Creature, uv_TEX_Creature ).rgb;
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			o.Emission = ( ( _Color0 * tex2D( _TextureSample1, uv_TextureSample1 ) ) * (0.7 + (sin( ( _Time.y * _PulseSpeed ) ) - 0.0) * (1.0 - 0.7) / (1.0 - 0.0)) * _GlowStrength ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
1999;24;1294;728;1900.354;377.767;1.762169;True;True
Node;AmplifyShaderEditor.RangedFloatNode;18;-1462.842,385.4417;Float;False;Property;_PulseSpeed;PulseSpeed;2;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;17;-1471.621,280.1144;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1197.33,264.754;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;22;-779.6809,-277.4409;Float;False;Property;_Color0;Color 0;3;0;Create;True;0;0;False;0;0,1,0.990005,0;0,1,0.990005,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-811.1503,-28.25554;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;400a34856b35e3441a75ef0aca7fb442;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;20;-892.3188,227.4508;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;21;-612.4454,240.6167;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.7;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-497.8794,550.4375;Float;False;Property;_GlowStrength;GlowStrength;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-379.9509,-135.6012;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;15;-795.3035,-525.4965;Float;True;Property;_TEX_Creature;TEX_Creature;0;0;Create;True;0;0;False;0;4c9c0ee684418494f9cfffbfee8e623c;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-229.739,-62.16808;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-43.88642,-254.5412;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Creature_Glow;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;17;0
WireConnection;19;1;18;0
WireConnection;20;0;19;0
WireConnection;21;0;20;0
WireConnection;23;0;22;0
WireConnection;23;1;16;0
WireConnection;25;0;23;0
WireConnection;25;1;21;0
WireConnection;25;2;24;0
WireConnection;0;0;15;0
WireConnection;0;2;25;0
ASEEND*/
//CHKSM=E627AB3A882D97F434EBDC596C5030B3D92D1847