Shader "Custom/VolumetricFogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _FogDensity ("Fog Density", Range(0.01, 1)) = 0.1
        _ParticleSize ("Particle Size", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _FogColor;
            float _FogDensity;
            float _ParticleSize;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = _FogColor;

                // Adjust size based on _ParticleSize
                o.pos.xy *= _ParticleSize;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distance = length(i.uv - float2(0.5, 0.5));
                float fogFactor = saturate(distance * _FogDensity);
                fixed4 texColor = tex2D(_MainTex, i.uv);
                return lerp(texColor, i.color, fogFactor);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
