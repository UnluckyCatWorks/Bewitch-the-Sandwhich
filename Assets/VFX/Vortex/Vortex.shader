Shader "VFX/Portal"
{
	Properties 
	{
		[Header (Waves)]
		_Alpha ("Transparency", Range(0,1)) = 1.0
		_MainColor ("Main Color", color) = (1,1,1,1)
		_Intensity ("Tile intensity", Float) = 2.0
		_MainTex ("Tile Texture", 2D) = "black" {}

		[Header (Foam)]
		_FColor ("Foam color", Color) = (1,1,1,1)
		_FIntensity ( "Foam intensity", Float ) = 2.0
		_Foam ("Foam thickness", Range (0.1, 5.0)) = 1.0
	}
	
	SubShader  
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		ZWrite Off

		CGPROGRAM
		#define USE_DEPTH float depth = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ (_CameraDepthTexture, UNITY_PROJ_COORD (IN.screenPos)))
		#define SCALE lerp (0.7, 1, pow(abs(sin(_Time.y)), 2))

		#pragma surface surf Standard alpha:fade
		#pragma target 3.0
		struct Input 
		{
			float2 uv_MainTex; 
			float4 screenPos;
			float3 viewDir;
		};
		uniform sampler2D _CameraDepthTexture;

		uniform float _Alpha;
		uniform fixed4 _MainColor;
		uniform float _Intensity;
		uniform sampler2D _MainTex;

		uniform fixed4 _FColor;
		uniform float _FIntensity;
		uniform float _Foam;

		void surf (Input IN, inout SurfaceOutputStandard s) 
		{
			// Foam (depth-based color)
			USE_DEPTH;
			float foamLine = (depth - IN.screenPos.w);
			float foam = 1 - smoothstep (0.0, _Foam, foamLine);
			foam *= _FIntensity;

			// Tile
			float2 uvs = IN.uv_MainTex;
			uvs *= SCALE;
			uvs += 0.5 - (0.5 * SCALE);
			float tile = tex2D (_MainTex, uvs);
			tile *= tex2D (_MainTex, IN.uv_MainTex * -SCALE + float2(0,_Time.x * 2));
			tile *= _Intensity;

			// Feed output
			s.Albedo = _MainColor;
			s.Emission = _FColor * max(foam, tile);
			s.Alpha = _Alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}