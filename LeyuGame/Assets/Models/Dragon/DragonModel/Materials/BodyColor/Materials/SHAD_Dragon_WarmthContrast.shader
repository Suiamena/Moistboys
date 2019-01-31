// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHAD_Dragon_WarmthContrast"
{
	Properties
	{
		_BaseTex("BaseTex", 2D) = "white" {}
		_Glowiness("Glowiness", Range( 0 , 10)) = 0
		_baseColor("baseColor", Color) = (0,0.8274511,0.9960785,1)
		_PulseSpeed("PulseSpeed", Float) = 1
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
		uniform float4 _baseColor;
		uniform float _Glowiness;
		uniform float _PulseSpeed;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseTex = i.uv_texcoord * _BaseTex_ST.xy + _BaseTex_ST.zw;
			float4 tex2DNode2 = tex2D( _BaseTex, uv_BaseTex );
			float grayscale14 = Luminance(tex2DNode2.rgb);
			o.Albedo = ( grayscale14 * _baseColor ).rgb;
			float grayscale16 = Luminance(( tex2DNode2 * _Glowiness ).rgb);
			float mulTime31 = _Time.y * _PulseSpeed;
			float4 _Vector0 = float4(-1,1,0.4,0.9);
			o.Emission = ( _baseColor * grayscale16 * (_Vector0.z + (( cos( ( ( 2.0 * UNITY_PI ) * mulTime31 ) ) * -1.0 ) - _Vector0.x) * (_Vector0.w - _Vector0.z) / (_Vector0.y - _Vector0.x)) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
290;91;1256;634;1943.347;51.53463;1.504352;True;False
Node;AmplifyShaderEditor.RangedFloatNode;29;-1494.13,405.7885;Float;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1497.138,505.0753;Float;False;Property;_PulseSpeed;PulseSpeed;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;27;-1340.684,411.8057;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;-1328.65,505.0756;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1154.145,438.8843;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1147.029,-51.60543;Float;True;Property;_BaseTex;BaseTex;0;0;Create;True;0;0;False;0;b417d6514ed452a4dacd9f454a505377;6b37264b3c15041408b9d0af234f47de;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosOpNode;36;-1008.223,426.8493;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1020.258,499.0579;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1063.42,298.037;Float;False;Property;_Glowiness;Glowiness;1;0;Create;True;0;0;False;0;0;1.43;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-881.8576,435.8753;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-756.2714,186.3868;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;35;-1054.858,577.2845;Float;False;Constant;_Vector0;Vector 0;4;0;Create;True;0;0;False;0;-1,1,0.4,0.9;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;16;-596.3437,181.1805;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;14;-550.2791,-307.3347;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-661.7133,-52.94617;Float;False;Property;_baseColor;baseColor;2;0;Create;True;0;0;False;0;0,0.8274511,0.9960785,1;0.3170612,0.7075472,0.6331359,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;33;-642.665,423.8406;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-371.057,161.8601;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-329.2878,-171.1785;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3.284556,-108.3904;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Dragon_WarmthContrast;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;29;0
WireConnection;31;0;32;0
WireConnection;30;0;27;0
WireConnection;30;1;31;0
WireConnection;36;0;30;0
WireConnection;37;0;36;0
WireConnection;37;1;38;0
WireConnection;5;0;2;0
WireConnection;5;1;4;0
WireConnection;16;0;5;0
WireConnection;14;0;2;0
WireConnection;33;0;37;0
WireConnection;33;1;35;1
WireConnection;33;2;35;2
WireConnection;33;3;35;3
WireConnection;33;4;35;4
WireConnection;15;0;12;0
WireConnection;15;1;16;0
WireConnection;15;2;33;0
WireConnection;13;0;14;0
WireConnection;13;1;12;0
WireConnection;0;0;13;0
WireConnection;0;2;15;0
ASEEND*/
//CHKSM=CF2C971C27E4DBEFAF6FAD5690E3AEF995D34FE5