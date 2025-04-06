Shader "Custom/PixelateTextSDF_VertexColor"
{
    Properties
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 64
        _CullMode ("Cull Mode", Float) = 2 // Needed for Unity/TMP builds
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off ZWrite Off Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull [_CullMode] // Make Unity happy

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // TMP vertex color (faceColor)
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Pixelation: snap UVs to pixel grid
                float2 pixelUV = floor(i.uv * _PixelSize) / _PixelSize;

                // Sample SDF alpha from font atlas
                fixed4 texSample = tex2D(_MainTex, pixelUV);

                // Smooth edge based on SDF
                float alpha = smoothstep(0.5 - 0.1, 0.5 + 0.1, texSample.a);

                // Fix: If vertex alpha is too low, bump it slightly (or force to 1)
                float finalAlpha = alpha * saturate(i.color.a + 0.001);

                // Apply TMP face color
                return fixed4(i.color.rgb, finalAlpha);
            }
            ENDCG
        }
    }
}
