using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakForestOakTreeNoise : Noise
{
    private FastNoiseLite perlinNoise;
    private FastNoiseLite valueNoise;
    private const float ValueFrequency = 0.05f;
    
    public override float GetNoise(Vector2 vector)
    {
        return (valueNoise.GetNoise(vector.x, vector.y) + perlinNoise.GetNoise(vector.x, vector.y)) / 2f;
    }

    public OakForestOakTreeNoise()
    {
        perlinNoise = new FastNoiseLite();
        perlinNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        perlinNoise.SetFrequency(0.02f);

        valueNoise = new FastNoiseLite();
        valueNoise.SetNoiseType(FastNoiseLite.NoiseType.Value);
        valueNoise.SetFrequency(ValueFrequency);
    }
}
