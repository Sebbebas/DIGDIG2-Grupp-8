Shader "Custom/PixelateTextSDF_VertexColor_Stencil"
{
    Properties
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 64
        _CullMode ("Cull Mode", Float) = 2

        // Add these for TextMeshPro stencil masking
        _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off ZWrite Off Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull [_CullMode]

        // TMP Stencil support
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

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
                float4 color : COLOR;
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
                float2 pixelUV = floor(i.uv * _PixelSize) / _PixelSize;
                fixed4 texSample = tex2D(_MainTex, pixelUV);
                float alpha = smoothstep(0.5 - 0.1, 0.5 + 0.1, texSample.a);
                float finalAlpha = alpha * saturate(i.color.a + 0.001);
                return fixed4(i.color.rgb, finalAlpha);
            }
            ENDCG
        }
    }
}
