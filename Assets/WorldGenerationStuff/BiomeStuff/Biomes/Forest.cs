using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : Biome
{
    private FastNoiseLite elevationNoise;
    private const FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.Perlin;
    private const float amplitude = 4f;
    private const float frequency = 0.02f;

    public override float GetElevationNoise(Vector2 vector)
    {
        return elevationNoise.GetNoise(vector.x, vector.y) * amplitude;
    }

    public override List<KeyValuePair<int, int>> ObjLevelGetBlocks(Vector2Int blockPos, Vector2Int chunkPos, Dictionary<Vector2Int, Chunk> chunks)
    {
        List<KeyValuePair<int, int>> blocks = new List<KeyValuePair<int, int>>();

        int lerpedElevation = GetLerpedElevation(blockPos, chunkPos, chunks);

        blocks.Add(new KeyValuePair<int, int>(lerpedElevation, ObjLevelGetBlock(lerpedElevation)));

        return blocks;
    }


    public override int ObjLevelGetBlock(int elevation)
    {
        return Block.blockNameToId["Grass"];
    }

    public Forest()
    {
        elevationNoise = new FastNoiseLite();
        elevationNoise.SetNoiseType(noiseType);
        elevationNoise.SetFrequency(frequency);
    }
}
