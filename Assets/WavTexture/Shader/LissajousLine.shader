Shader "Hidden/WavTexture/Lissajous Line"
{
	Properties
	{
        _XWavTex("", 2D) = "gray"{}
        _YWavTex("", 2D) = "gray"{}
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

    sampler2D _XWavTex;
    sampler2D _YWavTex;

    float4 _XWavTex_TexelSize;
    float4 _YWavTex_TexelSize;

    fixed4 _Color;

    // Retrieve a sample from a WavTexture.
    float GetSample(sampler2D tex, float2 texelSize, float position)
    {
        // Calculate the texture coordinate.
        float p4 = position / 4;
        float xy = floor(p4) * texelSize.x;
        float x = frac(xy);
        float y = floor(xy) * texelSize.y;

        x += texelSize.x * 0.5;
        y += texelSize.y * 0.5;

        // Retrieve the quadrupled sample.
        float4 s4 = tex2Dlod(tex, float4(x, y, 0, 0));

        // Extract a single sample from the texture sample.
        float i = frac(p4);
        float w = i < 0.25 ? s4.r : (i < 0.5 ? s4.g : (i < 0.75 ? s4.b : s4.a));

        // Normalize and return.
        return w * 2 - 1;
    }
    
    v2f vert(appdata v)
    {
        float t = _StartTime + v.vertex.x * _Duration;

        float wx1 = GetSample(_XWavTex, _XWavTex_TexelSize.xy, t);
        float wx2 = GetSample(_XWavTex, _XWavTex_TexelSize.xy, t + 1);

        float wy1 = GetSample(_YWavTex, _YWavTex_TexelSize.xy, t);
        float wy2 = GetSample(_YWavTex, _YWavTex_TexelSize.xy, t + 1);

        float wx = lerp(wx1, wx2, frac(t));
        float wy = lerp(wy1, wy2, frac(t));

        v2f o;
        o.vertex = UnityObjectToClipPos(float4(wx, wy, 0, 1));
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
