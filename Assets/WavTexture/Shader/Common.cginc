// WavTexture - Audio waveform to texture converter
// https://github.com/keijiro/WavTexture

#include "UnityCG.cginc"

float _StartTime;
float _Duration;

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
