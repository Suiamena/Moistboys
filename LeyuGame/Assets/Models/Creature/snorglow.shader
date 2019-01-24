// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHAD_Creature_Glow"
{
	Properties
	{
		_TEX_Creature("TEX_Creature", 2D) = "white" {}
		_TEX_CreatureSymbol("TEX_CreatureSymbol", 2D) = "white" {}
		_TEX_GlowMask("TEX_GlowMask", 2D) = "white" {}
		_PulseSpeed("PulseSpeed", Range( 0 , 10)) = 1
		_GlowColour("GlowColour", Color) = (0,1,0.990005,0)
		_GlowStrength("GlowStrength", Range( 0 , 1)) = 1
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
		uniform float4 _GlowColour;
		uniform sampler2D _TEX_GlowMask;
		uniform float4 _TEX_GlowMask_ST;
		uniform float _PulseSpeed;
		uniform float _GlowStrength;
		uniform sampler2D _TEX_CreatureSymbol;
		uniform float4 _TEX_CreatureSymbol_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TEX_Creature = i.uv_texcoord * _TEX_Creature_ST.xy + _TEX_Creature_ST.zw;
			o.Albedo = tex2D( _TEX_Creature, uv_TEX_Creature ).rgb;
			float2 uv_TEX_GlowMask = i.uv_texcoord * _TEX_GlowMask_ST.xy + _TEX_GlowMask_ST.zw;
			float mulTime17 = _Time.y * ( _PulseSpeed * _PulseSpeed );
			float2 uv_TEX_CreatureSymbol = i.uv_texcoord * _TEX_CreatureSymbol_ST.xy + _TEX_CreatureSymbol_ST.zw;
			o.Emission = ( ( ( _GlowColour * tex2D( _TEX_GlowMask, uv_TEX_GlowMask ) ) * (0.7 + (sin( mulTime17 ) - 0.0) * (1.0 - 0.7) / (1.0 - 0.0)) * _GlowStrength ) + tex2D( _TEX_CreatureSymbol, uv_TEX_CreatureSymbol ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
238;91;1325;634;1917.982;480.8123;1.6;True;False
Node;AmplifyShaderEditor.RangedFloatNode;18;-1557.757,118.8864;Float;False;Property;_PulseSpeed;PulseSpeed;3;0;Create;True;0;0;False;0;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1256.737,115.3044;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;17;-1117.738,118.9591;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;20;-943.2109,118.4709;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;16;-1066.278,-97.19307;Float;True;Property;_TEX_GlowMask;TEX_GlowMask;2;0;Create;True;0;0;False;0;400a34856b35e3441a75ef0aca7fb442;400a34856b35e3441a75ef0aca7fb442;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;22;-1055.386,-265.2454;Float;False;Property;_GlowColour;GlowColour;4;0;Create;True;0;0;False;0;0,1,0.990005,0;0,1,0.990005,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-689.958,-81.15662;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-867.0637,338.4144;Float;False;Property;_GlowStrength;GlowStrength;5;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;21;-773.9114,122.5981;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.7;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-522.3904,-10.4614;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;28;-527.5467,137.8255;Float;True;Property;_TEX_CreatureSymbol;TEX_CreatureSymbol;1;0;Create;True;0;0;False;0;400a34856b35e3441a75ef0aca7fb442;ee46616aff9c94b4695a93becddad62b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-210.4878,-2.874801;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;15;-414.7794,-262.368;Float;True;Property;_TEX_Creature;TEX_Creature;0;0;Create;True;0;0;False;0;4c9c0ee684418494f9cfffbfee8e623c;4c9c0ee684418494f9cfffbfee8e623c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-43.88642,-254.5412;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Creature_Glow;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;18;0
WireConnection;30;1;18;0
WireConnection;17;0;30;0
WireConnection;20;0;17;0
WireConnection;23;0;22;0
WireConnection;23;1;16;0
WireConnection;21;0;20;0
WireConnection;25;0;23;0
WireConnection;25;1;21;0
WireConnection;25;2;24;0
WireConnection;29;0;25;0
WireConnection;29;1;28;0
WireConnection;0;0;15;0
WireConnection;0;2;29;0
ASEEND*/
//CHKSM=389B88B6CEA2E5C19807729393EFC8F88781DBB9