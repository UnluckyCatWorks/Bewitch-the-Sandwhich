Shader "VFX/CaveWater"
{
	Properties
	{
		[Header (Coloring)]
		_WTint   ("Water tint (RGBA)",  Color) = (1,1,1,1)
		_FTint	 ("Foam tint  (RGBA)",  Color) = (1,1,1,1)
		_TileTex ("Tile (R+G+B, A)",    2D)	   = "white" {}
//		_TileST	 ("Tile scale",		 Vector) = (1,1,1,1)
//		_TileSp	 ("Tile speed",		 Float ) = (1,1,1,1)

		[Header (Wave settings)]
		_Speed	("Wave speed",	   Range(0,1)) = 0.5
		_Amount ("Wave amount",	   Range(0,1)) = 0.5
		_Height ("Wave height",	   Range(0,1)) = 0.5
		[Space]
		_Foam	("Foam thickness", Range(0.1, 5.0)) = 1.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }

		ZWrite Off
		CGPROGRAM
		// Utils
		#define SPEED (_Time.z * _Speed)
		#define AMOUNT (v.vertex.x * v.vertex.z * _Amount)
		#define USE_DEPTH float depth = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ (_CameraDepthTexture, UNITY_PROJ_COORD (IN.screenPos)))

		#pragma surface surf Cave fullforwardshadows alpha:fade vertex:vert
		#pragma target 3.0
		struct Input
		{
			float2 uv_TileTex;
			float4 screenPos;
			float3 viewDir;
		};
		uniform sampler2D _CameraDepthTexture;

		uniform fixed4 _WTint;
		uniform fixed4 _FTint;
		uniform sampler2D _TileTex;
//		uniform float4 _TileST;
//		uniform float4 _TileSp;

		uniform float _Speed;
		uniform float _Amount;
		uniform float _Height;
		uniform float _Foam;

		void vert ( inout appdata_tan v, out Input IN )
		{
			UNITY_INITIALIZE_OUTPUT ( Input, IN );
			v.vertex.y += sin ( SPEED + AMOUNT ) * _Height;
//			UNITY_TRANSFER_DEPTH ( IN.depth );
		}

		void surf (Input IN, inout SurfaceOutput s)
		{
			USE_DEPTH;
			fixed4 tile = tex2D ( _TileTex, IN.uv_TileTex );

			float foamLine = _Foam * (depth - IN.screenPos.w);
			float foamFactor = saturate (1 - abs(foamLine / 3.0));

			s.Albedo = lerp (tile, _FTint, foamFactor).rgb;
			s.Alpha = _WTint.a;
		}

		half4 LightingCave (SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			return half4(s.Albedo * _LightColor0.rgb * atten*2, s.Alpha);
		}
		ENDCG
	}
}
