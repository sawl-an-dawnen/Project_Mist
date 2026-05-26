Shader "Custom/Water"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Alpha ("Overall Alpha", Range(0,1)) = 1
        _FadePower ("Fade Power", Range(0.1,8)) = 1.5

        _BoundaryStrength ("Boundary Strength", Range(0,1)) = 0.25
        _BoundaryThickness ("Boundary Thickness", Range(0.001,0.2)) = 0.025

        _RippleStrength ("Ripple Strength", Range(0,0.08)) = 0.01
        _RippleFrequency ("Ripple Frequency", Range(1,80)) = 28
        _RippleSpeed ("Ripple Speed", Range(0,10)) = 1

        _BubbleStrength ("Bubble Strength", Range(0,1)) = 0.25
        _BubbleDensity ("Bubble Density", Range(2,40)) = 5
        _BubbleSpeed ("Bubble Speed", Range(0,5)) = 0.08
        _BubbleMinSize ("Bubble Min Size", Range(0.01,0.4)) = 0.06
        _BubbleMaxSize ("Bubble Max Size", Range(0.01,0.4)) = 0.16
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

            float _Alpha, _FadePower;
            float _BoundaryStrength, _BoundaryThickness;
            float _RippleStrength, _RippleFrequency, _RippleSpeed;
            float _BubbleStrength, _BubbleDensity, _BubbleSpeed;
            float _BubbleMinSize, _BubbleMaxSize;

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

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float2 hash2(float2 p)
            {
                return frac(sin(float2(
                    dot(p, float2(269.5, 183.3)),
                    dot(p, float2(113.5, 271.9))
                )) * 43758.5453);
            }

            float bubbleLayer(float2 uv, float density, float speed, float minSize, float maxSize)
            {
                float2 gridUV = uv * density;
                gridUV.y += _Time.y * speed;

                float2 cell = floor(gridUV);
                float2 local = frac(gridUV);

                float active = step(0.82, hash(cell));
                float2 center = hash2(cell);

                float size = lerp(minSize, maxSize, hash(cell + 12.37));
                float dist = distance(local, center);

                float outer = 1.0 - smoothstep(size, size + 0.015, dist);
                float inner = 1.0 - smoothstep(size * 0.65, size * 0.65 + 0.015, dist);

                float ring = saturate(outer - inner);

                return ring * active;
            }

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
    // Ripple strongest near top
    float topMask = pow(i.uv.y, 5.0);

    float ripple =
        sin(
            (i.uv.x * _RippleFrequency)
            + (_Time.y * _RippleSpeed)
        )
        * _RippleStrength
        * topMask;

    float2 warpedUV = i.uv;

    // Vertical ripple motion
    warpedUV.y += ripple;

    fixed4 tex = tex2D(_MainTex, warpedUV);

    // Preserve sprite color
    fixed4 col = tex * i.color;

   float waterLine = (1.0 - i.uv.y) + ripple;

// 0 at top/ripple line, 1 deeper into water
float depth = saturate(waterLine);

// Soft transition at the water surface
float edgeFade = smoothstep(
    0.0,
    _BoundaryThickness,
    waterLine
);

// Gradient downward into the water
float gradient = pow(depth, _FadePower);

// Top slightly transparent, bottom solid
float alphaFade = lerp(
    0.15,
    1.0,
    gradient
);

// Hide above waterline
alphaFade *= edgeFade;

// Visible ripple boundary
float boundary =
    1.0 -
    smoothstep(
        0.0,
        _BoundaryThickness * 0.5,
        abs(waterLine)
    );

alphaFade += boundary * _BoundaryStrength;

    // Bubble rings
    float bubbles = 0;

    bubbles += bubbleLayer(
        i.uv,
        _BubbleDensity,
        _BubbleSpeed,
        _BubbleMinSize,
        _BubbleMaxSize
    );

    bubbles += bubbleLayer(
        i.uv + float2(0.37, 0.19),
        _BubbleDensity * 0.45,
        _BubbleSpeed * 0.6,
        _BubbleMinSize * 1.4,
        _BubbleMaxSize * 1.8
    );

    // Keep bubbles away from very top/bottom
    bubbles *= smoothstep(0.05, 0.2, i.uv.y);
    bubbles *= smoothstep(1.0, 0.75, i.uv.y);

    // Bright bubble outlines
    col.rgb = lerp(
        col.rgb,
        1.0,
        bubbles * _BubbleStrength
    );

    col.a *= saturate(alphaFade) * _Alpha;

    return col;
}
            ENDCG
        }
    }
}