Shader "Custom/WonderWall"
{
	Properties
	{
		_Height ("Wall height", Range(0.0, 3.0)) = 1.0
		_Emission ("Emission power", Float) = 1.0
		_Color ("Color", Color) = (1,1,1,1)
		_Masks ("Mask (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }

		GrabPass { "_GrabTex" }

		Cull Off
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows //alpha:fade
		#pragma target 3.0
		struct Input
		{
			float2 uv_Masks;
			float3 worldPos;
			float4 screenPos;
		};
		sampler2D _GrabTex;

		float _Height;
		float _Emission;
		fixed4 _Color;
		sampler2D _Masks;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float4 grab = tex2Dproj (_GrabTex, UNITY_PROJ_COORD(IN.screenPos));

			fixed2 noise = tex2D (_Masks, IN.uv_Masks + -_Time.x).r;
			fixed2 masks = tex2D (_Masks, IN.uv_Masks).gb;

			float height = mul ( unity_WorldToObject, float4(IN.worldPos, 1.0) ).y;
			float heightFade = 1 - (height / _Height);

			o.Emission = grab + _Color*_Emission * clamp (heightFade*masks.x, 0.0, 1.0);
//			o.Alpha = clamp (heightFade * masks.x, 0.0, 1.0);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
