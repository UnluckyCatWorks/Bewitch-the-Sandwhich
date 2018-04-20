Shader "Custom/MedusaStandard"
{
	Properties
	{
		_MainTex  ("Albedo (RGB)", 2D) = "white" {}
		[Normal][NoScaleOffset] _Normal   ("Normal map", 2D) = "bump" {}
		[NoScaleOffset] _Metallic ("Metalness", 2D) = "white" {}
		[NoScaleOffset] _Emission ("Emission", 2D) = "black" {}
		_Intensity ("Intensity", Float) = 1.0

		[Header (Medusa)]
		_StoneTex	("Stone texture", 2D) = "white" {}
		[Normal][NoScaleOffset] _StoneNormal ("Stone normal", 2D) = "bump" {}
		_StoneLevel ("Stone Level", Range (0, 1)) = 0.0
		_StoneExtrude ("Stone extrude amount", Float) = 1.0
		_StoneMin ("Stone Min", Float) = 0.0
		_StoneMax ("Stone Max", Float) = 3.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#define xLerp(param) lerp (n##param, p##param, IN.amount)
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0
		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_StoneTex;
			float amount;
		};

		uniform sampler2D _MainTex;
		uniform sampler2D _Metallic;
		uniform sampler2D _Normal;
		uniform sampler2D _Emission;
		uniform float _Intensity;

		uniform sampler2D _StoneTex;
		uniform sampler2D _StoneNormal;
		uniform float _StoneLevel;
		uniform float _StoneExtrude;
		uniform float _StoneMin;
		uniform float _StoneMax;

		void vert (inout appdata_full v, out Input IN) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, IN);

			float level = 1 - lerp (_StoneMin, (_StoneMin+_StoneMax), _StoneLevel);
			IN.amount = step (level, v.vertex.x);

			// Extrude petrified vertices
			v.vertex.xyz += (v.normal*_StoneExtrude) * IN.amount;
		}

		void surf (Input IN, inout SurfaceOutputStandard o)  
		{
			// Normal material
			fixed3 nColor = tex2D (_MainTex, IN.uv_MainTex).rgb;
			float3 nNormal = UnpackNormal(tex2D (_Normal, IN.uv_MainTex));
			fixed4 nMetal = tex2D (_Metallic, IN.uv_MainTex);
			float3 nEmission = tex2D (_Emission, IN.uv_MainTex) * _Intensity;

			// Petrified material
			fixed3 pColor = tex2D (_StoneTex, IN.uv_StoneTex).rgb;
			float3 pNormal = UnpackNormal(tex2D (_StoneNormal, IN.uv_StoneTex));
			fixed4 pMetal = 0;
			float3 pEmission = 0;

			o.Albedo = xLerp (Color);
			o.Normal = xLerp (Normal);
			o.Metallic = xLerp (Metal).r;
			o.Smoothness = xLerp (Metal).a;
			o.Emission = xLerp (Emission);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
