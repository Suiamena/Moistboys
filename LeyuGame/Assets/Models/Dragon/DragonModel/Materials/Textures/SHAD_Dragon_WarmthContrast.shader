// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SHAD_Dragon_WarmthContrast"
{
	Properties
	{
		_Color("Color", 2D) = "white" {}
		_EmissionMap("EmissionMap", 2D) = "white" {}
		_Glowiness("Glowiness", Range( 0 , 10)) = 0
<<<<<<< HEAD
		_Color("Color", Color) = (1,1,1,1)
		_Alpha("Alpha", Range( 0 , 1)) = 0
=======
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
>>>>>>> origin/master
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

<<<<<<< HEAD
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float4 _Color;
		uniform float _Alpha;
=======
		uniform sampler2D _Color;
		uniform float4 _Color_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
>>>>>>> origin/master
		uniform sampler2D _EmissionMap;
		uniform float4 _EmissionMap_ST;
		uniform float _Glowiness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
<<<<<<< HEAD
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float grayscale10 = Luminance(tex2D( _Diffuse, uv_Diffuse ).rgb);
			o.Albedo = ( ( grayscale10 * _Color ) * _Alpha ).rgb;
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			o.Emission = ( _Alpha * ( _Color * ( tex2D( _EmissionMap, uv_EmissionMap ) * _Glowiness ) ) ).rgb;
=======
			float2 uv_Color = i.uv_texcoord * _Color_ST.xy + _Color_ST.zw;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 lerpResult9 = lerp( tex2D( _Color, uv_Color ) , tex2D( _TextureSample0, uv_TextureSample0 ) , float4( 0,0,0,0 ));
			o.Albedo = lerpResult9.rgb;
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			o.Emission = ( tex2D( _EmissionMap, uv_EmissionMap ) * _Glowiness ).rgb;
>>>>>>> origin/master
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
<<<<<<< HEAD
236;91;1099;624;1129.477;860.6178;1.450901;False;False
Node;AmplifyShaderEditor.SamplerNode;1;-999.7079,-690.3963;Float;True;Property;_Diffuse;Diffuse;1;0;Create;True;0;0;False;0;b417d6514ed452a4dacd9f454a505377;2faf7ec17b5793346a2e818dc0520ad0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1094.93,-186.84;Float;True;Property;_EmissionMap;EmissionMap;2;0;Create;True;0;0;False;0;b417d6514ed452a4dacd9f454a505377;2faf7ec17b5793346a2e818dc0520ad0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-1084,30.30001;Float;False;Property;_Glowiness;Glowiness;4;0;Create;True;0;0;False;0;0;0.5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;10;-670.9418,-746.2853;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-718.2003,-56.70001;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;7;-809.8705,-401.2737;Float;False;Property;_Color;Color;5;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-472.0102,-394.4903;Float;False;Property;_Alpha;Alpha;6;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-428.9426,-680.4462;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-448.9401,-165.6798;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-180.5887,-548.6741;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-172.0222,-270.7523;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-3300,542.5;Float;True;Property;_Float0;Float 0;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;8.772774,-498.3014;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Dragon_WarmthContrast;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;False;Transparent;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;0;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;1;0
WireConnection;5;0;2;0
WireConnection;5;1;4;0
WireConnection;8;0;10;0
WireConnection;8;1;7;0
WireConnection;9;0;7;0
WireConnection;9;1;5;0
WireConnection;28;0;8;0
WireConnection;28;1;14;0
WireConnection;21;0;14;0
WireConnection;21;1;9;0
WireConnection;0;0;28;0
WireConnection;0;2;21;0
ASEEND*/
//CHKSM=6C353D9DB5F88D0CC32251FA6E7EE52B45A78728
=======
2564;1247;2546;976;2112.473;1043.001;1.355;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-876,113.5;Float;False;Property;_Glowiness;Glowiness;3;0;Create;True;0;0;False;0;0;0.81;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1094.93,-186.84;Float;True;Property;_EmissionMap;EmissionMap;1;0;Create;True;0;0;False;0;b417d6514ed452a4dacd9f454a505377;2faf7ec17b5793346a2e818dc0520ad0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-993.6805,-763.2543;Float;True;Property;_Color;Color;0;0;Create;True;0;0;False;0;b417d6514ed452a4dacd9f454a505377;2faf7ec17b5793346a2e818dc0520ad0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-1008.148,-499.6459;Float;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;None;6b37264b3c15041408b9d0af234f47de;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-3300,542.5;Float;True;Property;_Float0;Float 0;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-491,10.5;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;9;-445.8226,-468.4808;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SHAD_Dragon_WarmthContrast;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;2;0
WireConnection;5;1;4;0
WireConnection;9;0;1;0
WireConnection;9;1;11;0
WireConnection;0;0;9;0
WireConnection;0;2;5;0
ASEEND*/
//CHKSM=7C9BB4424D0ECAC51D5D0CAF23F0C177165474E9
>>>>>>> origin/master
