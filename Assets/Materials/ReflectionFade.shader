Shader "Custom/ReflectionFade"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0,1)) = 0.2
        _FadePower ("Fade Power", Range(0.1, 8)) = 2.5

        _RippleStrength ("Ripple Strength", Range(0,0.1)) = 0.015
        _RippleFrequency ("Ripple Frequency", Range(1,80)) = 24
        _RippleSpeed ("Ripple Speed", Range(0,10)) = 1.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Alpha;
            float _FadePower;
            float _RippleStrength;
            float _RippleFrequency;
            float _RippleSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float distanceFromSource = i.uv.y;

                float rippleMask = pow(distanceFromSource, 2.0);

                float ripple =
                    sin((i.uv.y * _RippleFrequency) + (_Time.y * _RippleSpeed))
                    * _RippleStrength
                    * rippleMask;

                float2 warpedUV = i.uv;
                warpedUV.x += ripple;

                fixed4 tex = tex2D(_MainTex, warpedUV);
                fixed4 col = tex * i.color;

                float fade = pow(1.0 - i.uv.y, _FadePower);

                col.a *= fade * _Alpha;

                return col;
            }
            ENDCG
        }
    }
}