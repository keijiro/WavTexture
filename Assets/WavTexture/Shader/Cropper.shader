// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

Shader "Hidden/WavTexture/Cropper"
{
    Properties
    {
        _MainTex("", 2D) = "gray" {}
    }

    CGINCLUDE

    #include "Common.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    half4 frag(v2f_img i) : SV_Target
    {
        float t = _StartTime + (i.uv.x - 0.5 * _MainTex_TexelSize.x) * _Duration;
        float s1 = GetSample(_MainTex, _MainTex_TexelSize.xy, t);
        float s2 = GetSample(_MainTex, _MainTex_TexelSize.xy, t + 1);
        return lerp(s1, s2, frac(t));
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
