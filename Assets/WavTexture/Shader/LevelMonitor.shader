// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

Shader "Hidden/WavTexture/Level Monitor"
{
    Properties
    {
        _MainTex("", 2D) = "gray" {}
    }

    CGINCLUDE

    #include "Common.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    float _CropRes; // 1 / CropBuffer.width

    // Crop the current frame from the soruce waveform into a 2D texture.
    half4 frag_crop(v2f_img i) : SV_Target
    {
        float2 uv = i.uv.xy - 0.5 * _MainTex_TexelSize.xy; // Cancel half texel offset.
        float t = _StartTime + (uv.x * _CropRes + uv.y) * _Duration;
        float s1 = GetSample(_MainTex, _MainTex_TexelSize.xy, t);
        float s2 = GetSample(_MainTex, _MainTex_TexelSize.xy, t + 1);
        return lerp(s1, s2, frac(t));
    }

    // Calculate the RMS of the samples in the crop buffer.
    half4 frag_rms(v2f_img i) : SV_Target
    {
        float acc = 0;

        for (float y = 0.5 * _CropRes; y < 1; y += _CropRes)
        {
            for (float x = 0.5 * _CropRes; x < 1; x += _CropRes)
            {
                float s = tex2D(_MainTex, float2(x, y)).r;
                acc += s * s;
            }
        }

        return sqrt(acc * _CropRes * _CropRes);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_crop
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
