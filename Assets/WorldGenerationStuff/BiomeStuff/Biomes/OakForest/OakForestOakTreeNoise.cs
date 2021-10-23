using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakForestOakTreeNoise : Noise
{
    private FastNoiseLite cellular;
    private const float Frequency = 0.3f;

    public override float GetNoise(Vector2 vector)
    {
        return cellular.GetNoise(vector.x, vector.y) + 1f;
    }

    public OakForestOakTreeNoise()
    {
        cellular = new FastNoiseLite();
        cellular.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        cellular.SetFrequency(Frequency);
    }
}
