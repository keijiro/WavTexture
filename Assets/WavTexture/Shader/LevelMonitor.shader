Shader "Hidden/WavTexture/Level Monitor"
{
    Properties
    {
        _MainTex("", 2D) = "gray" {}
    }

    CGINCLUDE

    #include "Common.cginc"

    #define kReduction (1.0 / 32)

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half4 frag_reduction(v2f_img i) : SV_Target
    {
        float2 uv = i.uv.xy - 0.5 * kReduction;
        float t = _StartTime + (uv.x * kReduction + uv.y) * _Duration;
        float s1 = GetSample(_MainTex, _MainTex_TexelSize.xy, t);
        float s2 = GetSample(_MainTex, _MainTex_TexelSize.xy, t + 1);
        return lerp(s1, s2, frac(t));
    }

    half4 frag_rms(v2f_img i) : SV_Target
    {
        float acc = 0;

        for (float y = 0.5 * kReduction; y < 1; y += kReduction)
        {
            for (float x = 0.5 * kReduction; x < 1; x += kReduction)
            {
                float s = tex2D(_MainTex, float2(x, y)).r;
                acc += s * s;
            }
        }

        return sqrt(acc * kReduction * kReduction);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_reduction
            #pragma target 3.0
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_rms
            #pragma target 3.0
            ENDCG
        }
    }
}
