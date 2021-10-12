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

    public override List<KeyValuePair<Vector3Int, int>> ObjLevelGetBlocks(Vector2Int blockPos, Vector2Int chunkPos, Dictionary<Vector2Int, Chunk> chunks)
    {
        Vector3Int blockPosIn3D = new Vector3Int(blockPos.x, 0, blockPos.y);
        List<KeyValuePair<Vector3Int, int>> blocks = new List<KeyValuePair<Vector3Int, int>>();

        int lerpedElevation = GetLerpedElevation(blockPos, chunkPos, chunks);

        blocks.Add(new KeyValuePair<Vector3Int, int>(blockPosIn3D + Vector3Int.up * lerpedElevation, ObjLevelGetBlock(lerpedElevation)));

        System.Random random = new System.Random();

        if (random.Next(0, 10) >= 9)
        {
            blocks.Add(new KeyValuePair<Vector3Int, int>(blockPosIn3D + Vector3Int.up * (lerpedElevation + 1), Block.blockNameToId["Weed"]));
        }

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
