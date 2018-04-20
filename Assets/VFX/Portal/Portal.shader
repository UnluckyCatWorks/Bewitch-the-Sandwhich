Shader "VFX/Portal"
{
	Properties 
	{
		_MainTex ("Main Texture", 2D) = "white" {}

		[Header (Foam)]
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

		uniform sampler2D _MainTex;
		uniform float _Foam;

		void surf (Input IN, inout SurfaceOutputStandard s) 
		{
			USE_DEPTH;



			// Foam (depth-based color)
			float foamLine = (depth - IN.screenPos.w);
			float foam = 1 - smoothstep (0.0, _Foam, foamLine);

			s.Albedo = 0;
			s.Emission = foam;
		}
		ENDCG
	}
	FallBack "Diffuse"
}