// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Specular("Specular", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		_NormalStrength("Normal Strength", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D NormalMap;
		uniform float _NormalStrength;
		uniform sampler2D _Albedo;
		uniform sampler2D _Specular;
		uniform sampler2D _Occlusion;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 color12 = IsGammaSpace() ? float4(1,1,0,0) : float4(1,1,0,0);
			float3 temp_cast_1 = (color12.b).xxx;
			float4 appendResult16 = (float4(saturate( ( 1.0 - ( ( distance( temp_cast_1 , float3( 0,0,0 ) ) - 0.0 ) / max( 0.0 , 1E-05 ) ) ) ) , _NormalStrength , 0.0 , 0.0));
			o.Normal = ( float4( UnpackNormal( tex2D( NormalMap, i.uv_texcoord ) ) , 0.0 ) * appendResult16 ).xyz;
			o.Albedo = tex2D( _Albedo, i.uv_texcoord ).rgb;
			o.Smoothness = tex2D( _Specular, i.uv_texcoord ).r;
			o.Occlusion = tex2D( _Occlusion, i.uv_texcoord ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
-55;271;2546;988;2484.069;222.311;1;True;False
Node;AmplifyShaderEditor.ColorNode;12;-1977.675,229.1331;Float;False;Constant;_Color0;Color 0;4;0;Create;True;0;0;False;0;1,1,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-1748.179,514.2865;Float;False;Property;_NormalStrength;Normal Strength;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;15;-1684.069,277.689;Float;False;Color Mask;-1;;1;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1580.675,-62.86689;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;16;-1344.069,313.689;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-927.9751,663.9331;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-727.9751,-401.0669;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1210.7,-125.8;Float;True;Global;NormalMap;Normal Map;1;0;Create;True;0;0;False;0;6039db3cd4c0f459c98d4e24c94b21ff;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-957.9751,467.9331;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-511,390;Float;True;Property;_Specular;Specular;2;0;Create;True;0;0;False;0;5a42d080a494e4e7f8a83b62b5f71ef4;5a42d080a494e4e7f8a83b62b5f71ef4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-509,586;Float;True;Property;_Occlusion;Occlusion;3;0;Create;True;0;0;False;0;76ba2bc6689e24fd9a23ce88806f769a;76ba2bc6689e24fd9a23ce88806f769a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-830.7002,11.20002;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;1;-381,-279;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;730f27a115bd7491ca50637b9139cccc;730f27a115bd7491ca50637b9139cccc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;New Amplify Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;3;12;3
WireConnection;16;0;15;0
WireConnection;16;1;13;0
WireConnection;2;1;9;0
WireConnection;3;1;10;0
WireConnection;5;1;11;0
WireConnection;7;0;2;0
WireConnection;7;1;16;0
WireConnection;1;1;8;0
WireConnection;0;0;1;0
WireConnection;0;1;7;0
WireConnection;0;4;3;0
WireConnection;0;5;5;0
ASEEND*/
//CHKSM=8429D0CF7E7C577D4E66185E9F69623030F20B18