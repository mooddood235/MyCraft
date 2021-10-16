using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLand : Biome
{
    private FastNoiseLite elevationNoise;
    private const FastNoiseLite.NoiseType NoiseType = FastNoiseLite.NoiseType.Value;
    private const float Amplitude = 15f;
    private const float Frequency = 0.06f;

    private int stoneBlock = Block.blockNameToId["Stone"];

    public override float GetElevationNoise(Vector2 vector)
    {
        return elevationNoise.GetNoise(vector.x, vector.y) * Amplitude;
    }

    public override List<KeyValuePair<Vector3Int, int>> ObjLevelGetBlocks(Vector2Int blockPos, Vector2Int chunkPos, Dictionary<Vector2Int, Chunk> chunks)
    {
        List<KeyValuePair<Vector3Int, int>> blocks = new List<KeyValuePair<Vector3Int, int>>();

        Vector3Int stoneBlockPos = new Vector3Int(blockPos.x, GetLerpedElevation(blockPos, chunkPos, chunks), blockPos.y);
        
        blocks.Add(new KeyValuePair<Vector3Int, int>(stoneBlockPos, stoneBlock));

        return blocks;
    }

    public StoneLand()
    {
        elevationNoise = new FastNoiseLite();
        elevationNoise.SetNoiseType(NoiseType);
        elevationNoise.SetFrequency(Frequency);
    }
}
