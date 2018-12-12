// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHAD_Dragon_WarmthContrast"
{
	Properties
	{
		_BaseTex("BaseTex", 2D) = "white" {}
		_EmissionMap("EmissionMap", 2D) = "white" {}
		_Glowiness("Glowiness", Range( 0 , 10)) = 0
		_LerpTex("LerpTex", 2D) = "white" {}
		_Color("Color", Color) = (0,0.8274511,0.9960785,1)
		_LerpValue("LerpValue", Float) = 0
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

		uniform sampler2D _BaseTex;
		uniform float4 _BaseTex_ST;
		uniform sampler2D _LerpTex;
		uniform float4 _LerpTex_ST;
		uniform float _LerpValue;
		uniform float4 _Color;
		uniform sampler2D _EmissionMap;
		uniform float4 _EmissionMap_ST;
		uniform float _Glowiness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseTex = i.uv_texcoord * _BaseTex_ST.xy + _BaseTex_ST.zw;
			float2 uv_LerpTex = i.uv_texcoord * _LerpTex_ST.xy + _LerpTex_ST.zw;
			float4 lerpResult9 = lerp( tex2D( _BaseTex, uv_BaseTex ) , tex2D( _LerpTex, uv_LerpTex ) , _LerpValue);
			float grayscale14 = Luminance(lerpResult9.rgb);
			o.Albedo = ( grayscale14 * _Color ).rgb;
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			float grayscale16 = Luminance(( tex2D( _EmissionMap, uv_EmissionMap ) * _Glowiness ).rgb);
			o.Emission = ( _Color * grayscale16 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
236;91;1099;492;859.6661;229.7455;1.3;False;False
Node;AmplifyShaderEditor.SamplerNode;11;-1081.35,-249.9739;Float;True;Property;_LerpTex;LerpTex;3;0;Create;True;0;0;False;0;None;6b37264b3c15041408b9d0af234f47de;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-792.9371,-139.1411;Float;False;Property;_LerpValue;LerpValue;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1080.182,-443.1982;Float;True;Property;_BaseTex;BaseTex;0;0;Create;True;0;0;False;0;b417d6514ed452a4dacd9f454a505377;2faf7ec17b5793346a2e818dc0520ad0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1080.513,84.19861;Float;True;Property;_EmissionMap;EmissionMap;1;0;Create;True;0;0;False;0;b417d6514ed452a4dacd9f454a505377;2faf7ec17b5793346a2e818dc0520ad0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-1063.42,298.037;Float;False;Property;_Glowiness;Glowiness;2;0;Create;True;0;0;False;0;0;1.43;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-601.5256,-309.8941;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-756.2714,186.3868;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;16;-620.4134,181.1805;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;18;-408.5658,270.7544;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;14;-454.2791,-315.3347;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-517.7133,-52.94617;Float;False;Property;_Color;Color;4;0;Create;True;0;0;False;0;0,0.8274511,0.9960785,1;0,0.6509804,0.9921569,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;19;-187.5661,252.5545;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-220.9014,38.48274;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-225.2878,-167.1785;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3.284556,-108.3904;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Dragon_WarmthContrast;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;1;0
WireConnection;9;1;11;0
WireConnection;9;2;17;0
WireConnection;5;0;2;0
WireConnection;5;1;4;0
WireConnection;16;0;5;0
WireConnection;14;0;9;0
WireConnection;19;0;18;4
WireConnection;15;0;12;0
WireConnection;15;1;16;0
WireConnection;13;0;14;0
WireConnection;13;1;12;0
WireConnection;0;0;13;0
WireConnection;0;2;15;0
WireConnection;0;14;19;0
ASEEND*/
//CHKSM=58EF8F5D5DEC636327C926C588741F0C35932CB8