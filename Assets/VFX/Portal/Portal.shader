Shader "VFX/Portal"
{
	Properties 
	{
		[Header (Waves)]
		_MainColor ("Main Color", color) = (1,1,1,1)
		_Intensity ("Waving intensity", Float) = 2.0
		[Normal] _MainTex ("Waves Texture", 2D) = "bump" {}

		[Header (Foam)]
		_FColor ("Foam color", Color) = (1,1,1,1)
		_Foam ("Foam thickness", Range (0.1, 5.0)) = 1.0
	}
	
	SubShader  
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		ZWrite Off

		CGPROGRAM
		#define USE_DEPTH float depth = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ (_CameraDepthTexture, UNITY_PROJ_COORD (IN.screenPos)))
		#pragma surface surf Standard
		#pragma target 3.0
		struct Input 
		{
			float2 uv_MainTex; 
			float4 screenPos;
		};
		uniform sampler2D _CameraDepthTexture;

		uniform fixed4 _MainColor;
		uniform float _Intensity;
		uniform sampler2D _MainTex;

		uniform fixed4 _FColor;
		uniform float _Foam;

		void surf (Input IN, inout SurfaceOutputStandard s) 
		{
			// Expanding UVs 1
			float sn1 = lerp (0.7, 1, abs( pow(cos(_Time.y * 0.3), 1)));
			float2 uvs1 = IN.uv_MainTex * sn1;
			uvs1 += 0.5 - 0.5*sn1;
			// Expanding UVs 2
			float sn2 = lerp (0.7, 1, abs( pow(1 - cos(_Time.y * 0.5), 2)));
			float2 uvs2 = IN.uv_MainTex * sn2;
			uvs2 += 0.5 - 0.5*sn2;
			// Moving waves
			float3 waves1 = UnpackNormal (tex2D (_MainTex, uvs1));
			float3 waves2 = UnpackNormal (tex2D (_MainTex, uvs2));
			float waving = 1 - dot (abs(waves1 * waves2), float3 (0,0,1));

			// Foam (depth-based color)
			USE_DEPTH;
			float foamLine = (depth - IN.screenPos.w);
			float foam = 1 - smoothstep (0.0, _Foam, foamLine);

			// Emission fonts
			float3 finalColor = lerp (waving*_FColor, _FColor, foam);
			float3 


			// Feed output
			s.Albedo = finalColor;
			s.Emission = ;
			/*
			s.Emission = (_FColor * foam)
						+ (_FColor * abs(dot(waves1, waves2)))
						+ lerp(0, _FColor, pow(foam, 0.1));*/
			s.Smoothness = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}