using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public float GetNoise(Vector2 vector)
    {
        throw new NotImplementedException();
    }

    public static bool Within(float noiseValue, float min, float max)
    {
        return noiseValue >= min && noiseValue <= max;
    }
}
