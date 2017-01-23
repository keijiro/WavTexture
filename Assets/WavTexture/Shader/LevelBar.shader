Shader "WavTexture/Level Bar"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _LevelTex("", 2D) = "black"{}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    fixed4 _Color;
    sampler2D _LevelTex;

    struct appdata
    {
        float4 vertex : POSITION;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
    };

    v2f vert(appdata v)
    {
        float scale = tex2Dlod(_LevelTex, 0).r;

        float4 p = v.vertex;
        p.y = (p.y + 0.5) * scale - 0.5;

        v2f o;
        o.vertex = UnityObjectToClipPos(p);
        return o;
    }

    fixed4 frag (v2f i) : SV_Target
    {
        return _Color;
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" "DisableBatching"="True" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
