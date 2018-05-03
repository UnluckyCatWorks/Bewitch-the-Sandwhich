Shader "Custom/Medusa Standard"
{
	Properties
	{
		_MainTex  ("Albedo (RGB)", 2D) = "white" {}
		[Normal][NoScaleOffset] _Normal   ("Normal map", 2D) = "bump" {}
		[NoScaleOffset] _Metallic ("Metalness", 2D) = "white" {}
		[NoScaleOffset] _Emission ("Emission", 2D) = "black" {}
		[HDR] _EmissionColor ("Emission color", Color) = (0,0,0,0)

		[Header (Medusa)]
		_StoneScale ("Triplanar scale (XYZ) Blend shaperness (W)", Vector) = (1,1,1,1)
		[NoScaleOffset] _StoneTex	("Stone texture", 2D) = "white" {}
		_StoneNormalScale ("Stone normal scale", Float) = 1.0
		[Normal][NoScaleOffset] _StoneNormal ("Stone normal", 2D) = "bump" {}
		[NoScaleOffset] _StoneEmission ("Stone emissive", 2D) = "black" {}
		[HDR] _StoneIntensity ("Stone emission Intensity", Color) = (0,0,0,0)

		_StoneLevel ("Stone Level", Range (0, 1)) = 0.0
		_StoneExtrude ("Stone extrude amount", Float) = 1.0
		_StoneMin ("Stone Min", Float) = 0.0
		_StoneMax ("Stone Max", Float) = 3.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#define TRI(s) TriplanarMap (s, IN.normal, IN.position)
		#define xLerp(param) lerp (n##param, p##param, IN.amount)
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0
		struct Input 
		{
			float2 uv_MainTex;
			float amount;
			// Triplanar proj
			float3 position;
			float3 normal;
		};

		uniform sampler2D _MainTex;
		uniform sampler2D _Metallic;
		uniform sampler2D _Normal;
		uniform sampler2D _Emission;
		uniform float3 _EmissionColor;

		uniform float4 _StoneScale;
		uniform sampler2D _StoneTex;
		uniform float _StoneNormalScale;
		uniform sampler2D _StoneNormal;
		uniform sampler2D _StoneEmission;
		uniform float _StoneIntensity;

		uniform float _StoneLevel;
		uniform float _StoneExtrude;
		uniform float _StoneMin;
		uniform float _StoneMax;

		void vert (inout appdata_full v, out Input IN) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, IN);
			IN.position = v.vertex.xyz;
			IN.normal = v.normal;

			float level = 1 - lerp (_StoneMin, (_StoneMin+_StoneMax), _StoneLevel);
			IN.amount = step (level, v.vertex.x);

			// Extrude petrified vertices
			v.vertex.xyz += (v.normal*_StoneExtrude) * IN.amount;
		}

		float4 TriplanarMap (sampler2D s, float3 normal, float3 coords) 
		{
			// Get the UVs
			coords /= _StoneScale.xyz;
			float2 xUV = coords.zy;
			float2 yUV = coords.xz;
			float2 zUV = coords.xy;
			// Read textures
			fixed4 yDiff = tex2D (s, yUV);
			fixed4 xDiff = tex2D (s, xUV);
			fixed4 zDiff = tex2D (s, zUV);

			// Get the absolute value of the world normal.
			// Put the blend weights to the power of BlendSharpness, the higher the value, 
            // the sharper the transition between the planar maps will be.
			float3 blend = pow (abs(normal), _StoneScale.w);
			// Divide our blend mask by the sum of it's components, this will make x+y+z=1
			blend /= (blend.x + blend.y + blend.z);

			// Finally, blend together all three samples based on the blend mask.
			return
				xDiff * blend.x + 
				yDiff * blend.y + 
				zDiff * blend.z;
		}

		void surf (Input IN, inout SurfaceOutputStandard o)  
		{
			// Normal material
			fixed3 nColor = tex2D (_MainTex, IN.uv_MainTex).rgb;
			float3 nNormal = UnpackNormal(tex2D (_Normal, IN.uv_MainTex));
			float3 nEmission = tex2D (_Emission, IN.uv_MainTex) * _EmissionColor;
			fixed4 nMetal = tex2D (_Metallic, IN.uv_MainTex);

			// Petrified material
			fixed3 pColor = TRI (_StoneTex).rgb;
			float3 pNormal = UnpackScaleNormal(TRI (_StoneNormal), _StoneNormalScale);
			float3 pEmission = TRI (_StoneEmission).rgb * _StoneIntensity;
			fixed4 pMetal = 0;

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
