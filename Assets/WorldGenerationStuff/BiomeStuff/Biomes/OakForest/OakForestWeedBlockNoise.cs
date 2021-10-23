using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakForestWeedBlockNoise : Noise
{
    private FastNoiseLite value;
    private const float Frequency = 0.4f;
    
    public override float GetNoise(Vector2 vector)
    {
        return value.GetNoise(vector.x, vector.y);
    }


    public OakForestWeedBlockNoise()
    {
        value = new FastNoiseLite();
        value.SetNoiseType(FastNoiseLite.NoiseType.Value);
        value.SetFrequency(Frequency);
    }
}
