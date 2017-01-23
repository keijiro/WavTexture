// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

Shader "Hidden/WavTexture/Waveform Line"
{
    Properties
    {
        _WavTex("", 2D) = "gray"{}
        _Color("", Color) = (1, 1, 1, 0.5)
    }

    CGINCLUDE

    #include "Common.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
    };

    sampler2D _WavTex;
    float4 _WavTex_TexelSize;

    fixed4 _Color;

    v2f vert(appdata v)
    {
        float t = _StartTime + v.vertex.x * _Duration;
        float s1 = GetSample(_WavTex, _WavTex_TexelSize.xy, t);
        float s2 = GetSample(_WavTex, _WavTex_TexelSize.xy, t + 1);
        float w = lerp(s1, s2, frac(t));

        v2f o;
        o.vertex = UnityObjectToClipPos(float4(v.vertex.x - 0.5, w, 0, 1));
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        return _Color;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
