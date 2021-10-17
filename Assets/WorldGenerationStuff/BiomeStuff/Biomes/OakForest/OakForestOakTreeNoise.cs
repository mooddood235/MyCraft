using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakForestOakTreeNoise : Noise
{
    private FastNoiseLite cubic;
    private FastNoiseLite simplex;
    private FastNoiseLite perlin;
    private FastNoiseLite fractal;
    
    private const float Frequency = 0.07f;
    private const float CubicAmplitude = 2f;
        
        
    public float GetNoise(Vector2 vector)
    {
        return cubic.GetNoise(vector.x, vector.y) * CubicAmplitude * simplex.GetNoise(vector.x, vector.y) *
                fractal.GetNoise(vector.x, vector.y) +
                perlin.GetNoise(vector.x, vector.y);
    }

    public OakForestOakTreeNoise()
    {
        cubic = new FastNoiseLite();
        cubic.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
        cubic.SetFrequency(Frequency);
        
        simplex = new FastNoiseLite();
        simplex.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        simplex.SetFrequency(Frequency);
        
        perlin = new FastNoiseLite();
        perlin.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        fractal = new FastNoiseLite();
        fractal.SetNoiseType(FastNoiseLite.NoiseType.Value);
        fractal.SetFractalType(FastNoiseLite.FractalType.Ridged);
    }
}
