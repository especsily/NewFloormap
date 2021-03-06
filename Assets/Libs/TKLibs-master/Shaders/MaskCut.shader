﻿Shader "Hidden/MaskCut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Opacity ("Opacity", Float) = 0.5
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        ColorMask A

        Tags{
            "Queue" = "Background-10"
        }

        Pass
        {
            Blend SrcAlpha Zero

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            float _Opacity;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return fixed4(col.r, col.g, col.b, col.a*_Opacity);
            }
            ENDCG
        }

        GrabPass { "_M" }
    }
}
