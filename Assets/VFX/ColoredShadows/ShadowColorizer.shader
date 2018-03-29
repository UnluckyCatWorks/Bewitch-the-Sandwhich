Shader "Hidden/Shadow Colorizer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Header (Settings)]
		_Color ("Shadow Color", Color) = (1,1,1,1)
		_Max ("Max value", Range(0.0, 1)) = 0.05
	}
	SubShader
	{
		Cull Off 
		ZWrite Off
		ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float _Max;

			float4 frag (v2f_img i) : SV_Target
			{
				float4 screen = tex2D (_MainTex, i.uv);

				float darkness = length (screen.rbg);
				float amount = 1.0 -  (_Max * darkness);
				float3 color = lerp (screen, _Color, (1-screen) * amount * _Color.a * screen.a);

				return float4 (color, 1);
			}
			ENDCG
		}
	}
}
