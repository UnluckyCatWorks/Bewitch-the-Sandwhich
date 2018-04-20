Shader "VFX/Wonder Wall Wind"
{
	Properties 
	{
		[Header (Wall Settings)]
		_Height ("Current height", Range(0.0, 8.0))   = 1.5
		_HWin	("Height window",  Range (0.01, 1.0)) = 0.5

		[Header (Color and Fresnel)]
		_Color	("Color",		  Color)			  = (1,1,1,0.5)
		_R0		("Fresnel R0",	  Range(0.00, 0.1))   = 0.05
		_FPower ("Fresnel power", Range(0.001, 10.0)) = 5.0
	}
	
	SubShader  
	{
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
		ZWrite Off
		CGPROGRAM
		#pragma surface surf Standard alpha:fade
		struct Input 
		{
			float3 viewDir;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform fixed4 _Color;
		uniform float _R0;
		uniform float _FPower;

		uniform float _Height;
		uniform float _HWin;

		void surf (Input IN, inout SurfaceOutputStandard s) 
		{
			float fresnel;
			fresnel = 1.0 + dot (IN.viewDir, IN.worldNormal);
			fresnel = pow (fresnel, _FPower);
			fresnel *= _R0;

			// Up&Down fades
			float h = mul ( unity_WorldToObject, float4(IN.worldPos, 1.0) ).y;
			float fade = smoothstep ( _Height+_HWin, _Height-_HWin, h);
			fade *= smoothstep ( -0.1, 0.5, h );

			s.Emission = _Color.rgb;
			s.Alpha = saturate(fresnel * fade);
		}
		ENDCG
	}
	FallBack "Diffuse"
}