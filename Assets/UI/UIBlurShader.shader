Shader "UI/BlurURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            Name "BlurPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float2 pixelSize = _BlurSize / _ScreenParams.xy;

                half4 col = tex2D(_MainTex, uv) * 0.36;
                col += tex2D(_MainTex, uv + pixelSize * float2(1, 0)) * 0.16;
                col += tex2D(_MainTex, uv + pixelSize * float2(-1, 0)) * 0.16;
                col += tex2D(_MainTex, uv + pixelSize * float2(0, 1)) * 0.16;
                col += tex2D(_MainTex, uv + pixelSize * float2(0, -1)) * 0.16;

                return col;
            }
            ENDHLSL
        }
    }
}