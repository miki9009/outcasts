// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable

Shader "MyShader/VertexShader" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color" , Color) = (1,1,1,1)
		
		_vertStrength("Vert" , Range(-1,1)) = 0.4
	
	}
	
	SubShader {
		
		//Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		float4 _Color;
		float _vertStrength;
	
		
		// float4x4 _Object2World;
		// float4x4 _World2Object; 
		//float4 _WorldSpaceLightPos0; 
		sampler2D _NormalMap;
		
		struct Input {
		
			float4 vertex;
			float2 uv_MainTex;
			float3 vertColors;
			float4 color : COLOR;
		};

		void vert(inout appdata_full v, out Input o){
			
			
			float3 normalDirection = mul(float4(v.normal, 0.0), unity_WorldToObject).xyz;
			//float3 lightDirection;
			//float atten = 1.0;
			//lightDirection = normalize( _WorldSpaceLightPos0.xyz);
			//float3 diffuseReflection = atten * _LightColor0.xyz * max(0.0, dot(normalDirection, lightDirection));
			//o.color = float4(diffuseReflection,1.0);//float4(v.normal, 1.0);
			
			//o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			//o.vertColors = float4(float3(normalize(v.vertex)), 1.0);
			
			 o.color = v.color;
			 //o.vertex = v.vertex;
			 o.vertColors = v.vertex;
		     //o.normal = float3(v.vertex);//v.normal * -1;		
				//o.vertColors = v.texcoord1;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = (IN.vertColors) * _vertStrength;
			//o.Albedo = ((IN.vertColors.r + IN.vertColors.g, IN.vertColors.b)/3) * _vertStrength;
			//c.rgb = lerp( (IN.vertColors.r + IN.vertColors.g, IN.vertColors.b)/3, IN.vertColors.rgb,  _vertStrength) ;
			
			//c.r = lerp( c.r,(c.r/2) * (IN.vertColors.r / 2),  _vertStrength);
			//c.g = lerp( c.g,(c.g/2) * (IN.vertColors.g / 2),  _vertStrength);
			//c.b = lerp( c.b,(c.b/2) * (IN.vertColors.b / 2),  _vertStrength);
			 //o.Albedo = (IN.vertColors.r + IN.vertColors.g, IN.vertColors.b)/3;
			// c.rgb = ((c.r+c.g + c.b)/3);// _vertStrength;
			 //o.Albedo = c.rgb;
			// Transform to gray scale
		    // o.Albedo =  (c.r + c.g + c.b)/3;
		   	  //o.Albedo.r = (o.Albedo.r + o.Albedo.g + o.Albedo.b)/3;
		   // o.Albedo = c.rgb;
		    //o.Albedo = c.rgb;
            o.Alpha = c.a; // vertex Alpha
            //o.Normal = IN.normal;
			//o.Gloss = c.a;
			//o.Specular = _Specular;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
