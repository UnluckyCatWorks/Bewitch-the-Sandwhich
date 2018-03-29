/// Adaptation from:
/// https://gist.github.com/marsh12th/36e039102ef4eb7eb6d11e0a21fd1225

Shader "VFX/Wonder Wall"
{
	Properties 
	{
		[Header (Wall Settings)]
		_Height ("Current height", Range(0.0, 8.0))   = 1.5
		_HWin	("Height window",  Range (0.01, 1.0)) = 0.5
		_Speed	("Texture speed",  Vector)			  = (0,0,0,0)

		[Header (Color)]
		_Waves ("Waves texture",	2D )   = "gray" {}
		_Dark  ("Dark water color", Color) = (0,0,0,1)
		_Lit   ("Lit water color",  Color) = (1,1,1,1)

		[Header (Foam)]
		_Foam ("Foam thickness", Range (0.1, 5.0)) = 1.0
	}
	
	SubShader  
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		ZWrite Off
		CGPROGRAM
		#define USE_DEPTH float depth = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ (_CameraDepthTexture, UNITY_PROJ_COORD (IN.screenPos)))
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0
		struct Input 
		{
			float2 uv_Waves;
			float3 viewDir;
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};
		uniform sampler2D _CameraDepthTexture;

		uniform float _Height;
		uniform float _HWin;
		uniform float2 _Speed;

		uniform sampler2D _Waves;
		uniform fixed4 _Dark;
		uniform fixed4 _Lit;

		uniform float _R0;
		uniform float _FPower;

		uniform float _Foam;

		void surf (Input IN, inout SurfaceOutputStandard s) 
		{
			// Calculate fresnel amount
			float fresnel;
			fresnel = 1.0 + dot ( IN.viewDir, IN.worldNormal);
			fresnel = pow ( fresnel, _FPower );
			fresnel = saturate ( fresnel * _R0 );

			// Lerp color and iluminate fresnel
			fixed t = tex2D (_Waves, IN.uv_Waves + _Time.y * _Speed.xy);
			float4 color = _Lit * t.x;

			// Up & Down fades
			float h = mul ( unity_WorldToObject, float4(IN.worldPos, 1.0) ).y;
			float fade = smoothstep (_Height+_HWin, _Height-_HWin, h);
			fade *= smoothstep ( -0.3, 0.3, h );

			// Foam (depth-based color)
			USE_DEPTH;
			float foamLine = (depth - IN.screenPos.w);
			float foam = 1 - smoothstep (0.0, _Foam, foamLine);

			s.Albedo = _Dark.rgb;
			s.Emission = color.rgb + foam * _Lit.rgb;
			s.Alpha = saturate ((_Dark.a + color.a) * fade);
		}
		ENDCG
	}
	FallBack "Diffuse"
}