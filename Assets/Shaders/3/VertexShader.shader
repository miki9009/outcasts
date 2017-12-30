// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable

Shader "MyShader/VertexShader" {

	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color" , Color) = (1,1,1,1)
		_vertStrength("Vert" , Range(-1,1)) = 0.4
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		float4 _Color;
		float _vertStrength;
	
		sampler2D _NormalMap;
		
		struct Input {
		
			float4 vertex;
			float2 uv_MainTex;
			float3 vertColors;
			float4 color : COLOR;
		};

		void vert(inout appdata_full v, out Input o){
			
			 // Use Color when cietwi ciewti 
		//	 o.vertColors = v.vertex;  
			  o.vertColors = v.color;
	    }
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			// Use assigned var for surface
			//o.Albedo = (IN.vertColors) * _vertStrength;
			// Convert to gray;
			o.Albedo = ((IN.vertColors.r + IN.vertColors.g, IN.vertColors.b)/3) * _Color;
			
            o.Alpha = c.a; // vertex Alpha
          
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
