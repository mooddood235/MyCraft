using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The purpose of this class is to set static Biome attributes to their correct settings.
public class BiomeAwakeHandler : MonoBehaviour
{
    [SerializeField]
    private List<Biome> biomes;
    [SerializeField]
    private FastNoiseLite.NoiseType biomeNoiseType;
    [SerializeField]
    private float biomeNoiseFrequency;

    private void Awake()
    {
        FillBoundToBiomeDictionary();
        SetMaxAndMinBounds();
        SetBiomeNoiseSettings();
        SetElevationAndGetBlockNoiseSettings();
    }

    private void FillBoundToBiomeDictionary()
    {
        foreach (Biome biome in biomes)
        {
            Biome.boundToBiome[biome.biomeNoiseBounds] = biome;
        }
    }

    private void SetMaxAndMinBounds()
    {
        foreach (Biome biome in biomes)
        {
            if (biome.biomeNoiseBounds.y >= Biome.boundWithGreatestMax.y) Biome.boundWithGreatestMax = biome.biomeNoiseBounds;
            if (biome.biomeNoiseBounds.x <= Biome.boundWithSmallestMin.x) Biome.boundWithSmallestMin = biome.biomeNoiseBounds;
        }
    }

    private void SetBiomeNoiseSettings()
    {
        Biome.biomeNoise.SetNoiseType(biomeNoiseType);
        Biome.biomeNoise.SetFrequency(biomeNoiseFrequency);
    }
    private void SetElevationAndGetBlockNoiseSettings()
    {
        foreach (Biome biome in biomes)
        {
            biome.elevationNoise.SetNoiseType(biome.elevationNoiseType);
            biome.elevationNoise.SetFrequency(biome.elevationNoiseFrequency);
        }
    }
}
