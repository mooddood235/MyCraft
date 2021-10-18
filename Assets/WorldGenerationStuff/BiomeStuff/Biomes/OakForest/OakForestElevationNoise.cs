using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakForestElevationNoise : Noise
{
    private FastNoiseLite noise;
    private const FastNoiseLite.NoiseType NoiseType = FastNoiseLite.NoiseType.Perlin;
    private const float Frequency = 0.04f;
    private const float Amplitude = 5;
    
    public override float GetNoise(Vector2 vector)
    {
        return noise.GetNoise(vector.x, vector.y) * Amplitude;
    }

    public OakForestElevationNoise()
    {
        noise = new FastNoiseLite();
        noise.SetNoiseType(NoiseType);
        noise.SetFrequency(Frequency);
    }
}
