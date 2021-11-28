using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertElevationNoise : Noise
{
    private FastNoiseLite noise;
    private const FastNoiseLite.NoiseType NoiseType = FastNoiseLite.NoiseType.Perlin;
    private const float Frequency = 0.01f;

    public override float GetNoise(Vector2 vector)
    {
        return noise.GetNoise(vector.x, vector.y);
    }

    public DesertElevationNoise()
    {
        noise = new FastNoiseLite();
        noise.SetNoiseType(NoiseType);
        noise.SetFrequency(Frequency);
    }
}
