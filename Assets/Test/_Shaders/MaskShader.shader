// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MaskShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex("Mask Texture",2D) = "white"{}
		_Mask("Mask",2D) = "white"{}

	}
	SubShader {
		Tags{"RenderType" = "Transparent" "Queue" = "Transparent"}
		pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "unitycg.cginc"

			struct v2f 
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD1;
			};

			//sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _Mask;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			float4 frag(v2f i):COLOR
			{
				//float4 mainColor = tex2D(_MainTex,i.uv);
				float4 maskTexColor = tex2D(_MaskTex,i.uv);
				float4 maskColor = tex2D(_Mask,i.uv);
				maskTexColor.a = 1 - maskColor.a;
				return maskTexColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
