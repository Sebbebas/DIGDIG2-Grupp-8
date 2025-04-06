Shader "Custom/PixelateTextSDF_VertexColor"
{
    Properties
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 64
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off ZWrite Off Cull Off Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR; // <- This is the TMP face color
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
                // Pixelation logic
                float2 pixelUV = floor(i.uv * _PixelSize) / _PixelSize;

                // Sample the font texture (SDF alpha)
                fixed4 texSample = tex2D(_MainTex, pixelUV);

                // Smoothstep for signed distance field sharpness
                float alpha = smoothstep(0.5 - 0.1, 0.5 + 0.1, texSample.a);

                // Apply TMP face color (with alpha)
                return fixed4(i.color.rgb, alpha * i.color.a);
            }
            ENDCG
        }
    }
}
