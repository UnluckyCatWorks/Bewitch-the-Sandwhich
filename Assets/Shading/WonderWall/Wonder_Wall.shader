/// Adaptation from:
/// https://gist.github.com/marsh12th/36e039102ef4eb7eb6d11e0a21fd1225

Shader "VFX/Wonder Wall"
{
	Properties 
	{
		[Header (Wall Settings)]
		_Height ("Current height",	Range(0.0, 8.0)) = 0.5
		_Alpha	("Transparency",	Range(0.0, 1.0)) = 1.0


		[Header (Color and disortion)]
		_Dark	 ("Dark water color",  Color)		    = (0,0,0,1)
		_Lit	 ("Lit water color",   Color)		    = (1,1,1,1)

		[Header (Fresnel)]
		_R0		("Fresnel R0",	   Range(0.00, 0.1))   = 0.05
		_FPower ("Fresnel power",  Range(0.001, 10.0)) = 5.0
		_Lum	("Emisison power", Float)			   = 1.0



		[Header (Disortion settings)] [Normal]
		_Waves		("Waves (normal map)", 2D)    = "bump" {}
		_Waves2		("Waves (normal map)", 2D)    = "gray" {}
		_WavesFres	("Fresnel strenght",   Float) = 1.0

		[Space(10)]
		_SpeedX ("Waves speed (X)", Float) = 0.5
		_SpeedY ("Waves speed (Y)", Float) = 0.5

		[Space(15)] [IntRange]
		_BumpAmt ("Distortion Amount", Range (0,128))	= 10
	}
	
	SubShader  
	{
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as "_GrabTex"
		GrabPass 
		{
			Tags { "LightMode" = "Always" }
			"_GrabTex"
		}

		CGINCLUDE
		ENDCG

		Cull Off
		CGPROGRAM
		// Some utilities
		#define SPEED(c) _Time.c * float2(_SpeedX, _SpeedY)
		#define UTEX(name) tex2D(name, IN.uv##name)

		#pragma surface surf WonderWall alpha:fade
		#pragma target 3.0
		struct Input 
		{
			float2 uv_Waves;
			float3 worldPos;
			float4 screenPos;
		};
		sampler2D _GrabTex;
		float4 _GrabTex_TexelSize;

		uniform float _Height;
		uniform float _Alpha;

		uniform float4 _Dark;
		uniform float4 _Lit;

		uniform float _R0;
		uniform float _FPower;
		uniform float _Lum;

		uniform sampler2D _Waves;
		uniform sampler2D _Waves2;
		uniform float _WavesFres;

		uniform float _SpeedX;
		uniform float _SpeedY;

		uniform float _BumpAmt;

		void surf (Input IN, inout SurfaceOutput s) 
		{
			// Calculate normal bump
			IN.uv_Waves += SPEED(xy);
			float t = tex2D ( _Waves2, IN.uv_Waves ).x;

			// Calculate bumped surface
			float3 waves = t * (_WavesFres*2-1) * (_BumpAmt*_BumpAmt);
			float2 offset = waves * _GrabTex_TexelSize.xy;

			// Calculate dissorted UVs
			IN.screenPos.xy += offset * IN.screenPos.z;
			#if UNITY_UV_STARTS_AT_TOP
				fixed2 sm2Adjust = IN.screenPos.xy / IN.screenPos.w;
				sm2Adjust.y = 1 - sm2Adjust.y; 
			#endif
			s.Albedo = tex2Dproj( _GrabTex, IN.screenPos);

			// Calculate fresnel amount
			float fresnel;
			fresnel = saturate ( 1.0 + (t*2-1) );
			fresnel = pow (fresnel, _FPower);
			fresnel = _R0 + (1. - _R0) * fresnel;

			// Lerp color and iluminate fresnel
			s.Albedo += _Dark;
			s.Emission +=  _Lit * fresnel * _Lum;

			float localHeight = mul ( unity_WorldToObject, float4(IN.worldPos, 1.0) ).y;
			float heightFade = 1 - smoothstep ( _Height - 0.5, _Height + 0.5, localHeight );

			s.Alpha = _Alpha * heightFade;
//			s.Normal = n;
		}

		float4 LightingWonderWall ( SurfaceOutput s, float3 lightDir, float atten )
		{
			return float4 (s.Albedo.rgb * s.Emission, s.Alpha);
		}
		ENDCG
	}
	FallBack "Diffuse"
}