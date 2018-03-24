Shader "Hidden/Marker"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
//		#pragma multi_compile_fog
		#include "UnityCG.cginc"
		struct v2f 
		{
			float4 vertex : SV_POSITION;
//			UNITY_FOG_COORDS(1)
		};

		fixed4 _Color;

		v2f vert(float3 vertex : POSITION) 
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(vertex);
			//UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}
		ENDCG

		Pass
		{
			Cull Back
			ZWrite Off
			Blend SrcAlpha One

			CGPROGRAM
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
