﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D


// Unlit shader.
// - no lighting
// - lightmap_kj support
// - vertex color

Shader "Mobile/SimpleDiffuseTransparent" {

	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
#pragma surface surf StandardDefaultGI

#include "UnityPBSLighting.cginc"

		sampler2D _MainTex;

	inline half4 LightingStandardDefaultGI(SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
	{
		return LightingStandard(s, viewDir, gi);
	}

	inline void LightingStandardDefaultGI_GI(
		SurfaceOutputStandard s,
		UnityGIInput data,
		inout UnityGI gi)
	{
		LightingStandard_GI(s, data, gi);
	}

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
	}
	ENDCG
	}
		FallBack "Diffuse"
}