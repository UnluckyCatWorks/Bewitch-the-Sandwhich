Shader "Custom/Outline Unlit"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
//		_Thickness ("Thickness", Range(0.0001, 0.2)) = 0.5
//		_MaxThickness ("Max thickness allowed", Float) = 0.20
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
//		float _Thickness;
//		float _MaxThickness;
		ENDCG

		Pass
		{
			Cull Back
			ZWrite Off
			Blend SrcAlpha One
			CGPROGRAM
			v2f vert (float3 vertex : POSITION)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos (vertex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}

		/*
		Pass
		{
			Cull Front
			ZWrite Off
			CGPROGRAM
			v2f vert (float3 vertex : POSITION, float3 outDir : COLOR)
			{
				v2f o;
				// Make inmutable to distance from Camera
				float3 objPos = float3 (unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3]);
				float distance = length(_WorldSpaceCameraPos.xyz - objPos);
				float scale = clamp (_Thickness * distance, 0.0, _MaxThickness);

				// Expand vertex 
				outDir = outDir * 2.0 - 1.0;
				o.vertex = UnityObjectToClipPos (vertex + outDir*scale);

				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
		*/
	}
}
