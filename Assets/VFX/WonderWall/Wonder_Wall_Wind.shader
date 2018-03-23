Shader "VFX/Wonder Wall Wind"
{
	Properties 
	{
		_Mask ( "Mask (RGB)", 2D ) = "white" {}
		_SH ("Speed (XYZ), Height (W)", Vector) = (1, 1, 1, 0.5)
	}
	
	SubShader  
	{
		Tags { "Queue"="Transparent+2" "RenderType"="Transparent" }

		Cull Off
		CGPROGRAM
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0
		struct Input 
		{
			float2 uv_Mask;
			float3 worldPos;
		};

		uniform sampler2D _Mask;
		uniform float4 _SH;

		void surf (Input IN, inout SurfaceOutputStandard s) 
		{
			float localHeight = mul ( unity_WorldToObject, float4(IN.worldPos, 1.0) ).y;
			float heightFade = 1 - smoothstep ( _SH.w - 0.5, _SH.w + 0.5, localHeight );

			float2 offset = float2 (1-_SH.w, 0.0);
			float r = tex2D ( _Mask, IN.uv_Mask + float2 (_Time.y * _SH.r, 0.0) ).r;
			float g = tex2D ( _Mask, IN.uv_Mask + float2 (_Time.y * _SH.g, 0.0) ).g;
			float b = tex2D ( _Mask, IN.uv_Mask + float2 (_Time.y * _SH.b, 0.0) ).b;

			s.Emission = float3(0.2, 0.8, 0.3) * 10;
			s.Alpha = clamp ((r+g+b) * 3, 0, 1) * heightFade;
		}
		ENDCG
	}
	FallBack "Diffuse"
}