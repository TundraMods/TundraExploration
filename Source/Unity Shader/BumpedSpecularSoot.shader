Shader "KSP/Bumped Specular Soot"
{
    Properties
    {
        [Header(Texture Maps)]
        _MainTex("_MainTex (RGB spec(A))", 2D) = "gray" {}
        _BumpMap("_BumpMap", 2D) = "bump" {}
        _Color ("_Color", Color) = (1,1,1,1)
        _SpecColor ("_SpecColor", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("_Shininess", Range (0.03, 1)) = 0.4

    	
    	[Header(Soot 1)]
        _Soot1("Soot 1", 2D) = "gray" {}
		_Soot1Alpha ("Soot 1 Alpha", Range (0, 1)) = 0
		_Soot1Spec ("Soot 1 Specular", Range (0, 1)) = 0

		[Header(Soot 2)]
        _Soot2("Soot 2", 2D) = "gray" {}
        _Soot2Alpha ("Soot 2 Alpha", Range (0, 1)) = 0
        _Soot2Spec ("Soot 2 Specular", Range (0, 1)) = 0
    	
		[Header(Flag 1)]
        _Flag1("Flag 1", 2D) = "gray" {}
        _Flag1Alpha ("Flag 1 Alpha", Range (0, 1)) = 0
        _Flag1Spec ("Flag 1 Specular", Range (0, 1)) = 0

		[Header(Flag 2)]
        _Flag2("Flag 2", 2D) = "gray" {}
        _Flag2Alpha ("Flag 2 Alpha", Range (0, 1)) = 0
        _Flag2Spec ("Flag 2 Specular", Range (0, 1)) = 0

		[Header(Flag 3)]
        _Flag3("Flag 3", 2D) = "gray" {}
        _Flag3Alpha ("Flag 3 Alpha", Range (0, 1)) = 0
        _Flag3Spec ("Flag 3 Specular", Range (0, 1)) = 0

        [Header(Effects)]
        [PerRendererData]_Opacity("_Opacity", Range(0,1) ) = 1
        [PerRendererData]_RimFalloff("_RimFalloff", Range(0.01,5) ) = 0.1
        [PerRendererData]_RimColor("_RimColor", Color) = (0,0,0,0)
        [PerRendererData]_TemperatureColor("_TemperatureColor", Color) = (0,0,0,0)
        [PerRendererData]_BurnColor ("Burn Color", Color) = (1,1,1,1)
        [PerRendererData]_UnderwaterFogFactor ("Underwater Fog Factor", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM

        #include "../LightingKSP.cginc"
        #pragma surface surf BlinnPhongSmooth keepalpha

		#pragma target 3.5

		#define FLAGS 3

		half _Shininess;

		sampler2D _MainTex;

        float4 _Flag1_ST;
        float4 _Flag2_ST;
        float4 _Flag3_ST;

        #define DEFGRP(NAME, N) \
			sampler2D _##NAME##N;\
			float _##NAME##N##Alpha;\
			float _##NAME##N##Spec;

        #define UVGRP(NAME, N, COORD1, TEXCOORD)\
			float2 uv##COORD1##_##NAME##N: TEXCOORD;

        #define SAMPLE_FLAG(N)\
			if(_Flag##N##Alpha > 0.01) {\
				float2 uv_Flag##N = TRANSFORM_TEX(IN.color.rg, _Flag##N); \
				float4 flag##N = tex2D(_Flag##N,(uv_Flag##N)); \
				_Flag##N##Alpha *= flag##N.a * SDFAA(sdBox(uv_Flag##N - float2(0.5, 0.5), float2(0.5, 0.5)));\
				flag##N.a = _Flag##N##Spec;\
				color = lerp(color, flag##N, _Flag##N##Alpha);\
			}

        #define SAMPLE_SOOT(N)\
        	if(_Soot##N##Alpha > 0.01) {\
				float4 soot##N = tex2D(_Soot##N,(IN.uv2_Soot##N));\
				_Soot##N##Alpha *= soot##N.a;\
				soot##N.a = _Soot##N##Spec;\
				color = lerp(color, soot##N, _Soot##N##Alpha);\
			}


		DEFGRP(Soot, 1)
		DEFGRP(Soot, 2)
        
		DEFGRP(Flag, 1)
		DEFGRP(Flag, 2)
		DEFGRP(Flag, 3)

		sampler2D _BumpMap;

		float _Opacity;
		float _RimFalloff;
		float4 _RimColor;
		float4 _TemperatureColor;
		float4 _BurnColor;

		// based on functions by Inigo Quilez
		// https://iquilezles.org/www/articles/distfunctions2d/distfunctions2d.htm

		// SDF of a box
		float sdBox( in float2 p, in float2 b ) {
		    float2 d = abs(p)-b;
		    return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
		}

		inline float SDFdDist(float dist) {
		    return length(float2(ddx(dist), ddy(dist)));
		}

		inline float SDFAA(float dist, float ddist) {
		    float pixelDist = dist / ddist;
		    return saturate(0.5-pixelDist);
		}

		inline float SDFAA(float dist) {
		    return SDFAA(dist, SDFdDist(dist));
		}

		
		struct Input
		{
			float2 uv_MainTex : TEXCOORD0;

			UVGRP(Soot, 1, 2, TEXCOORD1)
			UVGRP(Soot, 2, 2, TEXCOORD1)
			
			UVGRP(Flag, 1, 3, TEXCOORD2)
			UVGRP(Flag, 2, 3, TEXCOORD2)
			UVGRP(Flag, 3, 3, TEXCOORD2)
			
			float2 uv_BumpMap;
			float3 viewDir;
			float3 worldPos;
			float4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{

			float4 color = tex2D(_MainTex,(IN.uv_MainTex));
			float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			SAMPLE_FLAG(1)
			SAMPLE_FLAG(2)
			SAMPLE_FLAG(3)
			
			SAMPLE_SOOT(1)
			SAMPLE_SOOT(2)

			color *= _BurnColor * _Color;

			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), normal));

			float3 emission = (_RimColor.rgb * pow(rim, _RimFalloff)) * _RimColor.a;
			emission += _TemperatureColor.rgb * _TemperatureColor.a;

			float4 fog = UnderwaterFog(IN.worldPos, color);

			o.Albedo = fog.rgb;
			o.Emission = emission;
		    o.Gloss = color.a;
			o.Specular = _Shininess;
			o.Normal = normal;
			o.Emission *= _Opacity * fog.a;
			o.Alpha = _Opacity * fog.a;
		}
		ENDCG
    }
    Fallback "Standard"
}