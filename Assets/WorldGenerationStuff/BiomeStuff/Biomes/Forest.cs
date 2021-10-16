using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : Biome
{
    private FastNoiseLite elevationNoise;
    private const FastNoiseLite.NoiseType NoiseType = FastNoiseLite.NoiseType.Perlin;
    private const float Amplitude = 4f;
    private const float Frequency = 0.02f;

    private int grassBlock = Block.blockNameToId["Grass"];
    private int weedBlock = Block.blockNameToId["Weed"];

    public override float GetElevationNoise(Vector2 vector)
    {
        return elevationNoise.GetNoise(vector.x, vector.y) * Amplitude;
    }

    public override List<KeyValuePair<Vector3Int, int>> ObjLevelGetBlocks(Vector2Int blockPos, Vector2Int chunkPos, Dictionary<Vector2Int, Chunk> chunks)
    {
        List<KeyValuePair<Vector3Int, int>> blocks = new List<KeyValuePair<Vector3Int, int>>();

        Vector3Int grassBlockPos = new Vector3Int(blockPos.x, GetLerpedElevation(blockPos, chunkPos, chunks), blockPos.y);
        blocks.Add(new KeyValuePair<Vector3Int, int>(grassBlockPos, grassBlock));

        System.Random random = new System.Random();

        if (random.Next(0, 10) >= 9)
        {
            Vector3Int weedBlockPos = grassBlockPos + Vector3Int.up;
            blocks.Add(new KeyValuePair<Vector3Int, int>(weedBlockPos, weedBlock));
        }
        
        if (random.Next(0, 40) >= 39)
        {
            blocks.AddRange(OakTree.GenerateStructure(grassBlockPos + Vector3Int.up));
        }
        
        return blocks;
    }

    public Forest()
    {
        elevationNoise = new FastNoiseLite();
        elevationNoise.SetNoiseType(NoiseType);
        elevationNoise.SetFrequency(Frequency);
    }
}
