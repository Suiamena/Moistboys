Shader "Ciconia Studio/Effects/Surface/World blend" {
    Properties {
        [Space(15)][Header(Main Properties)]
        [Space(10)]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
        _Desaturate ("Desaturate", Range(0, 1)) = 0
        
		[Space(35)]_SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecGlossMap ("Specular map(Gloss A)", 2D) = "white" {}
        [Space(10)]_SpecularIntensity ("Specular Intensity", Range(0, 8)) = 0.5
        _Glossiness ("Glossiness", Range(0, 2)) = 0.3
        [Space(10)]_FresnelStrength ("Fresnel Strength", Range(0, 8)) = 0
        _Ambientlight ("Ambient light", Range(0, 2)) = 0
        
		[Space(35)]_BumpMap ("Normal map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 2)) = 1
        
		[Space(35)]_OcclusionMap ("Ambient Occlusion map", 2D) = "white" {}
        _AoIntensity ("Ao Intensity", Range(0, 2)) = 1
        
		[Space(35)]_EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionMap ("Emission map", 2D) = "white" {}
        _EmissiveIntensity ("Emissive Intensity", Range(0, 2)) = 1
        
		
        [Space(45)][Header(Surface Maps)]
        [Space(10)][MaterialToggle] _BlendmodeScreenLinearDodge ("Blend mode Screen/Linear Dodge", Float ) = 0
        [Space(10)]_SurfaceColor ("Surface Color", Color) = (1,1,1,1)
        _SurfaceTexture ("Surface Texture", 2D) = "white" {}
        
		[Space(35)]_s_SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _s_SpecularmapGlossA ("Specular map(Gloss A)", 2D) = "white" {}
        [Space(10)]_s_SpecularIntensity ("Specular Intensity", Range(0, 8)) = 0.5
        _s_Glossiness ("Glossiness", Range(0, 2)) = 0.3
        [Space(10)]_s_Ambientlight ("Ambient light", Range(0, 2)) = 0
        
		[Space(35)]_s_Normalmap ("Normal map", 2D) = "bump" {}
        _s_NormalIntensity ("Normal Intensity", Range(0, 2)) = 1
        
		[Space(35)]_s_AmbientOcclusion ("Ambient Occlusion map", 2D) = "white" {}
        _s_AoIntensity ("Ao Intensity", Range(0, 2)) = 1
        
		[Space(35)]_s_EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _s_EmissionMap ("Emission map", 2D) = "white" {}
        _s_EmissiveIntensity ("Emissive Intensity", Range(0, 2)) = 1
        
		
        [Space(45)][Header(Surface Properties)]
        [Space(10)]_SurfaceSpreadTop ("Surface Spread Top", Range(0, 1)) = 0.5
        _TopMultiplier ("Multiplier", Float ) = 0
        _SharpenEdgeblendTop ("Sharpen Edge", Range(0, 5)) = 3
        
		[Space(25)]_SurfaceSpreadBottom ("Surface Spread Bottom", Range(0, 1)) = 0.5
        _BottomMultiplier ("Multiplier", Float ) = 0
        _SharpenEdgeblendBottom ("Sharpen Edge", Range(0, 5)) = 1
        [Space(15)]_SurfaceBlur ("Surface Blur ", Range(0, 10)) = 3.5
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float _SharpenEdgeblendTop;
            uniform float _SurfaceSpreadTop;
            uniform float _SharpenEdgeblendBottom;
            uniform float _SurfaceSpreadBottom;
            uniform float _BottomMultiplier;
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _SurfaceBlur;
            uniform float _TopMultiplier;
            uniform sampler2D _SurfaceTexture; uniform float4 _SurfaceTexture_ST;
            uniform float4 _SurfaceColor;
            uniform sampler2D _s_SpecularmapGlossA; uniform float4 _s_SpecularmapGlossA_ST;
            uniform float4 _s_SpecularColor;
            uniform float _s_SpecularIntensity;
            uniform float _s_Glossiness;
            uniform sampler2D _s_Normalmap; uniform float4 _s_Normalmap_ST;
            uniform float _s_NormalIntensity;
            uniform sampler2D _s_AmbientOcclusion; uniform float4 _s_AmbientOcclusion_ST;
            uniform float _s_AoIntensity;
            uniform float _FresnelStrength;
            uniform float _s_Ambientlight;
            uniform sampler2D _SpecGlossMap; uniform float4 _SpecGlossMap_ST;
            uniform float _Glossiness;
            uniform float _SpecularIntensity;
            uniform float4 _SpecularColor;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _NormalIntensity;
            uniform float _AoIntensity;
            uniform sampler2D _OcclusionMap; uniform float4 _OcclusionMap_ST;
            uniform float _Ambientlight;
            uniform float _Desaturate;
            uniform fixed _BlendmodeScreenLinearDodge;
            uniform sampler2D _s_EmissionMap; uniform float4 _s_EmissionMap_ST;
            uniform float4 _s_EmissionColor;
            uniform float _s_EmissiveIntensity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissiveIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_3611 = float3(0,0,1);
                float3 _s_Normalmap_var = UnpackNormal(tex2D(_s_Normalmap,TRANSFORM_TEX(i.uv0, _s_Normalmap)));
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float4 _Diffuse2 = tex2Dlod(_MainTex,float4(TRANSFORM_TEX(i.uv0, _MainTex),0.0,_SurfaceBlur));
                float RedChanel = _Diffuse2.r;
                float node_7636 = RedChanel;
                float node_1172 = ((i.vertexColor.r+node_7636)*0.6+-0.5);
                float SurfaceTop = saturate(((-1*_TopMultiplier)+((((node_1172-lerp(node_1172,node_7636,_SharpenEdgeblendTop))-i.normalDir.g)*15.0)+lerp(30,0,_SurfaceSpreadTop))));
                float node_8364 = RedChanel;
                float node_7006 = ((i.vertexColor.r+node_8364)*0.6+-0.5);
                float SurfaceBottom = saturate(((-1*_BottomMultiplier)+((((node_7006-lerp(node_7006,node_8364,_SharpenEdgeblendBottom))-(1.0 - i.posWorld.rgb.g))*15.0)+lerp(30,0,_SurfaceSpreadBottom))));
                float3 Normalmap = lerp(lerp(node_3611,_s_Normalmap_var.rgb,_s_NormalIntensity),lerp(node_3611,_BumpMap_var.rgb,_NormalIntensity),(SurfaceTop*SurfaceBottom));
                float3 normalLocal = Normalmap;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _s_SpecularmapGlossA_var = tex2D(_s_SpecularmapGlossA,TRANSFORM_TEX(i.uv0, _s_SpecularmapGlossA));
                float4 _SpecGlossMap_var = tex2D(_SpecGlossMap,TRANSFORM_TEX(i.uv0, _SpecGlossMap));
                float Glossiness = lerp((_s_SpecularmapGlossA_var.a*_s_Glossiness),(_SpecGlossMap_var.a*_Glossiness),(SurfaceTop*SurfaceBottom));
                float gloss = Glossiness;
                float perceptualRoughness = 1.0 - Glossiness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float Fresnel = (((0.95*pow(1.0-max(0,dot(normalDirection, viewDirection)),3.0))+0.05)*_FresnelStrength);
                float node_9179 = Fresnel;
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 Specularmap = lerp((_s_SpecularColor.rgb*_s_SpecularmapGlossA_var.rgb*_s_SpecularIntensity),(_SpecularColor.rgb*_SpecGlossMap_var.rgb*_SpecularIntensity),(SurfaceTop*SurfaceBottom));
                float3 specularColor = Specularmap;
                float specularMonochrome;
                float4 _SurfaceTexture_var = tex2D(_SurfaceTexture,TRANSFORM_TEX(i.uv0, _SurfaceTexture));
                float node_6668 = (SurfaceTop*SurfaceBottom);
                float3 node_5596 = (_SurfaceColor.rgb*_SurfaceTexture_var.rgb*(1.0 - node_6668));
                float4 node_5742 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_5917 = (node_6668*(_Color.rgb*lerp(node_5742.rgb,dot(node_5742.rgb,float3(0.3,0.59,0.11)),_Desaturate)));
                float3 Diffusemap = lerp( saturate((1.0-(1.0-node_5596)*(1.0-node_5917))), saturate((node_5596+node_5917)), _BlendmodeScreenLinearDodge );
                float3 diffuseColor = Diffusemap; // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular + float3(node_9179,node_9179,node_9179));
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                float Ambientlight = lerp(_s_Ambientlight,_Ambientlight,(SurfaceTop*SurfaceBottom));
                float node_5434 = Ambientlight;
                indirectDiffuse += float3(node_5434,node_5434,node_5434); // Diffuse Ambient Light
                indirectDiffuse += gi.indirect.diffuse;
                float4 _s_AmbientOcclusion_var = tex2D(_s_AmbientOcclusion,TRANSFORM_TEX(i.uv0, _s_AmbientOcclusion));
                float4 _OcclusionMap_var = tex2D(_OcclusionMap,TRANSFORM_TEX(i.uv0, _OcclusionMap));
                float Aomap = lerp(saturate((1.0-(1.0-_s_AmbientOcclusion_var.r)*(1.0-lerp(1,0,_s_AoIntensity)))),saturate((1.0-(1.0-_OcclusionMap_var.r)*(1.0-lerp(1,0,_AoIntensity)))),(SurfaceTop*SurfaceBottom));
                indirectDiffuse *= Aomap; // Diffuse AO
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _s_EmissionMap_var = tex2D(_s_EmissionMap,TRANSFORM_TEX(i.uv0, _s_EmissionMap));
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float3 EmissiveMap = lerp(((_s_EmissionColor.rgb*_s_EmissionMap_var.rgb)*_s_EmissiveIntensity),((_EmissionColor.rgb*_EmissionMap_var.rgb)*_EmissiveIntensity),(SurfaceTop*SurfaceBottom));
                float3 emissive = EmissiveMap;
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float _SharpenEdgeblendTop;
            uniform float _SurfaceSpreadTop;
            uniform float _SharpenEdgeblendBottom;
            uniform float _SurfaceSpreadBottom;
            uniform float _BottomMultiplier;
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _SurfaceBlur;
            uniform float _TopMultiplier;
            uniform sampler2D _SurfaceTexture; uniform float4 _SurfaceTexture_ST;
            uniform float4 _SurfaceColor;
            uniform sampler2D _s_SpecularmapGlossA; uniform float4 _s_SpecularmapGlossA_ST;
            uniform float4 _s_SpecularColor;
            uniform float _s_SpecularIntensity;
            uniform float _s_Glossiness;
            uniform sampler2D _s_Normalmap; uniform float4 _s_Normalmap_ST;
            uniform float _s_NormalIntensity;
            uniform sampler2D _SpecGlossMap; uniform float4 _SpecGlossMap_ST;
            uniform float _Glossiness;
            uniform float _SpecularIntensity;
            uniform float4 _SpecularColor;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float _NormalIntensity;
            uniform float _Desaturate;
            uniform fixed _BlendmodeScreenLinearDodge;
            uniform sampler2D _s_EmissionMap; uniform float4 _s_EmissionMap_ST;
            uniform float4 _s_EmissionColor;
            uniform float _s_EmissiveIntensity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissiveIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 node_3611 = float3(0,0,1);
                float3 _s_Normalmap_var = UnpackNormal(tex2D(_s_Normalmap,TRANSFORM_TEX(i.uv0, _s_Normalmap)));
                float3 _BumpMap_var = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(i.uv0, _BumpMap)));
                float4 _Diffuse2 = tex2Dlod(_MainTex,float4(TRANSFORM_TEX(i.uv0, _MainTex),0.0,_SurfaceBlur));
                float RedChanel = _Diffuse2.r;
                float node_7636 = RedChanel;
                float node_1172 = ((i.vertexColor.r+node_7636)*0.6+-0.5);
                float SurfaceTop = saturate(((-1*_TopMultiplier)+((((node_1172-lerp(node_1172,node_7636,_SharpenEdgeblendTop))-i.normalDir.g)*15.0)+lerp(30,0,_SurfaceSpreadTop))));
                float node_8364 = RedChanel;
                float node_7006 = ((i.vertexColor.r+node_8364)*0.6+-0.5);
                float SurfaceBottom = saturate(((-1*_BottomMultiplier)+((((node_7006-lerp(node_7006,node_8364,_SharpenEdgeblendBottom))-(1.0 - i.posWorld.rgb.g))*15.0)+lerp(30,0,_SurfaceSpreadBottom))));
                float3 Normalmap = lerp(lerp(node_3611,_s_Normalmap_var.rgb,_s_NormalIntensity),lerp(node_3611,_BumpMap_var.rgb,_NormalIntensity),(SurfaceTop*SurfaceBottom));
                float3 normalLocal = Normalmap;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _s_SpecularmapGlossA_var = tex2D(_s_SpecularmapGlossA,TRANSFORM_TEX(i.uv0, _s_SpecularmapGlossA));
                float4 _SpecGlossMap_var = tex2D(_SpecGlossMap,TRANSFORM_TEX(i.uv0, _SpecGlossMap));
                float Glossiness = lerp((_s_SpecularmapGlossA_var.a*_s_Glossiness),(_SpecGlossMap_var.a*_Glossiness),(SurfaceTop*SurfaceBottom));
                float gloss = Glossiness;
                float perceptualRoughness = 1.0 - Glossiness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 Specularmap = lerp((_s_SpecularColor.rgb*_s_SpecularmapGlossA_var.rgb*_s_SpecularIntensity),(_SpecularColor.rgb*_SpecGlossMap_var.rgb*_SpecularIntensity),(SurfaceTop*SurfaceBottom));
                float3 specularColor = Specularmap;
                float specularMonochrome;
                float4 _SurfaceTexture_var = tex2D(_SurfaceTexture,TRANSFORM_TEX(i.uv0, _SurfaceTexture));
                float node_6668 = (SurfaceTop*SurfaceBottom);
                float3 node_5596 = (_SurfaceColor.rgb*_SurfaceTexture_var.rgb*(1.0 - node_6668));
                float4 node_5742 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_5917 = (node_6668*(_Color.rgb*lerp(node_5742.rgb,dot(node_5742.rgb,float3(0.3,0.59,0.11)),_Desaturate)));
                float3 Diffusemap = lerp( saturate((1.0-(1.0-node_5596)*(1.0-node_5917))), saturate((node_5596+node_5917)), _BlendmodeScreenLinearDodge );
                float3 diffuseColor = Diffusemap; // Need this for specular when using metallic
                diffuseColor = EnergyConservationBetweenDiffuseAndSpecular(diffuseColor, specularColor, specularMonochrome);
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                diffuseColor *= 1-specularMonochrome;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float _SharpenEdgeblendTop;
            uniform float _SurfaceSpreadTop;
            uniform float _SharpenEdgeblendBottom;
            uniform float _SurfaceSpreadBottom;
            uniform float _BottomMultiplier;
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _SurfaceBlur;
            uniform float _TopMultiplier;
            uniform sampler2D _SurfaceTexture; uniform float4 _SurfaceTexture_ST;
            uniform float4 _SurfaceColor;
            uniform sampler2D _s_SpecularmapGlossA; uniform float4 _s_SpecularmapGlossA_ST;
            uniform float4 _s_SpecularColor;
            uniform float _s_SpecularIntensity;
            uniform float _s_Glossiness;
            uniform sampler2D _SpecGlossMap; uniform float4 _SpecGlossMap_ST;
            uniform float _Glossiness;
            uniform float _SpecularIntensity;
            uniform float4 _SpecularColor;
            uniform float _Desaturate;
            uniform fixed _BlendmodeScreenLinearDodge;
            uniform sampler2D _s_EmissionMap; uniform float4 _s_EmissionMap_ST;
            uniform float4 _s_EmissionColor;
            uniform float _s_EmissiveIntensity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _EmissionColor;
            uniform float _EmissiveIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 _s_EmissionMap_var = tex2D(_s_EmissionMap,TRANSFORM_TEX(i.uv0, _s_EmissionMap));
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float4 _Diffuse2 = tex2Dlod(_MainTex,float4(TRANSFORM_TEX(i.uv0, _MainTex),0.0,_SurfaceBlur));
                float RedChanel = _Diffuse2.r;
                float node_7636 = RedChanel;
                float node_1172 = ((i.vertexColor.r+node_7636)*0.6+-0.5);
                float SurfaceTop = saturate(((-1*_TopMultiplier)+((((node_1172-lerp(node_1172,node_7636,_SharpenEdgeblendTop))-i.normalDir.g)*15.0)+lerp(30,0,_SurfaceSpreadTop))));
                float node_8364 = RedChanel;
                float node_7006 = ((i.vertexColor.r+node_8364)*0.6+-0.5);
                float SurfaceBottom = saturate(((-1*_BottomMultiplier)+((((node_7006-lerp(node_7006,node_8364,_SharpenEdgeblendBottom))-(1.0 - i.posWorld.rgb.g))*15.0)+lerp(30,0,_SurfaceSpreadBottom))));
                float3 EmissiveMap = lerp(((_s_EmissionColor.rgb*_s_EmissionMap_var.rgb)*_s_EmissiveIntensity),((_EmissionColor.rgb*_EmissionMap_var.rgb)*_EmissiveIntensity),(SurfaceTop*SurfaceBottom));
                o.Emission = EmissiveMap;
                
                float4 _SurfaceTexture_var = tex2D(_SurfaceTexture,TRANSFORM_TEX(i.uv0, _SurfaceTexture));
                float node_6668 = (SurfaceTop*SurfaceBottom);
                float3 node_5596 = (_SurfaceColor.rgb*_SurfaceTexture_var.rgb*(1.0 - node_6668));
                float4 node_5742 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_5917 = (node_6668*(_Color.rgb*lerp(node_5742.rgb,dot(node_5742.rgb,float3(0.3,0.59,0.11)),_Desaturate)));
                float3 Diffusemap = lerp( saturate((1.0-(1.0-node_5596)*(1.0-node_5917))), saturate((node_5596+node_5917)), _BlendmodeScreenLinearDodge );
                float3 diffColor = Diffusemap;
                float4 _s_SpecularmapGlossA_var = tex2D(_s_SpecularmapGlossA,TRANSFORM_TEX(i.uv0, _s_SpecularmapGlossA));
                float4 _SpecGlossMap_var = tex2D(_SpecGlossMap,TRANSFORM_TEX(i.uv0, _SpecGlossMap));
                float3 Specularmap = lerp((_s_SpecularColor.rgb*_s_SpecularmapGlossA_var.rgb*_s_SpecularIntensity),(_SpecularColor.rgb*_SpecGlossMap_var.rgb*_SpecularIntensity),(SurfaceTop*SurfaceBottom));
                float3 specColor = Specularmap;
                float specularMonochrome = max(max(specColor.r, specColor.g),specColor.b);
                diffColor *= (1.0-specularMonochrome);
                float Glossiness = lerp((_s_SpecularmapGlossA_var.a*_s_Glossiness),(_SpecGlossMap_var.a*_Glossiness),(SurfaceTop*SurfaceBottom));
                float roughness = 1.0 - Glossiness;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
