Shader "Unlit/Marker"
{
	Properties
	{
		[PerRendererData]
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Pass
		{
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			// #pragma multi_compile_fog
			#include "UnityCG.cginc"
			struct appdata 
			{
				float4 vertex : POSITION;
			};
			struct v2f 
			{
				float4 vertex : SV_POSITION;
				float height : TEXCOORD1;
				// UNITY_FOG_COORDS(1)
			};
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.height = v.vertex.y;

				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				// apply fog
				// UNITY_APPLY_FOG(i.fogCoord, col);

				float3 color = _Color * 5;
				float alpha = _Color.a * smoothstep ( 3, 0, i.height ) * 0.5;
				return float4 (color, alpha);
			}
			ENDCG
		}
	}
}
