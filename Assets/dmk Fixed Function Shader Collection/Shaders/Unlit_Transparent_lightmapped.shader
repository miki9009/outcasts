Shader "Unlit/Transparent (supports lightmap)" {

	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}

	}

		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }


		ZWrite off
		Blend SrcAlpha OneMinusSrcAlpha


		// Lightmapped, encoded as RGBM
	//	Pass{
	//	Tags{ "LightMode" = "VertexLM" }
	//	BindChannels{
	//	//Bind "Vertex", vertex
	//	Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
	//	//Bind "texcoord", texcoord1 // main uses 1st uv
	//}

	//	SetTexture[unity_Lightmap]{
	//	matrix[unity_LightmapMatrix]
	//	combine texture * texture alpha DOUBLE
	//}

	//	SetTexture[_MainTex]{
	//	constantColor[_Color]
	//	combine constant * previous
	//}

	//	SetTexture[_MainTex]{
	//	combine texture * previous QUAD, texture * primary
	//}
	//}
		Pass{
		Tags{ "LightMode" = "VertexLM" }
		Lighting Off
		BindChannels{
		Bind "Vertex", vertex
		Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
		Bind "texcoord", texcoord1 // main uses 1st uv
	}
		SetTexture[unity_Lightmap]{
		matrix[unity_LightmapMatrix]
	
	}
		SetTexture[_MainTex]{

		combine texture * previous QUAD
	}
	}


	}
}