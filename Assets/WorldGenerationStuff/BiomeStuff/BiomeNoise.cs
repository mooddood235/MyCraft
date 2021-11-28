using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeNoise : Noise
{
    private FastNoiseLite perlinNoise;
    private FastNoiseLite cellularNoise;
    private const float Frequency = 0.001f;
    
    public override float GetNoise(Vector2 vector)
    {
        return (perlinNoise.GetNoise(vector.x, vector.y) + cellularNoise.GetNoise(vector.x, vector.y)) / 2 + 0.4f;
    }

    public BiomeNoise()
    {
        perlinNoise = new FastNoiseLite();
        perlinNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        perlinNoise.SetFrequency(Frequency);
        cellularNoise = new FastNoiseLite();
        cellularNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        cellularNoise.SetFrequency(Frequency);
    }
}
