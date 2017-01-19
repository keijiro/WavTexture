Shader "Hidden/WavTexture/Waveform Line"
{
    Properties
    {
        _WavTex("", 2D) = "gray"{}
        _Color("", Color) = (1, 1, 1, 0.5)
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    float _StartTime;
    float _Duration;

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

    // Retrieve a sample from a WavTexture.
    float GetSample(float position)
    {
        // Calculate the texture coordinate.
        float p4 = position / 4;
        float xy = floor(p4) * _WavTex_TexelSize.x;
        float x = frac(xy);
        float y = floor(xy) * _WavTex_TexelSize.y;

        x += _WavTex_TexelSize.x * 0.5;
        y += _WavTex_TexelSize.y * 0.5;

        // Retrieve the quadrupled sample.
        float4 s4 = tex2Dlod(_WavTex, float4(x, y, 0, 0));

        // Extract a single sample from the texture sample.
        float i = frac(p4);
        float w = i < 0.25 ? s4.r : (i < 0.5 ? s4.g : (i < 0.75 ? s4.b : s4.a));

        // Normalize and return.
        return w * 2 - 1;
    }
    
    v2f vert(appdata v)
    {
        float t = _StartTime + v.vertex.x * _Duration;
        float w = lerp(GetSample(t), GetSample(t + 1), frac(t));

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
