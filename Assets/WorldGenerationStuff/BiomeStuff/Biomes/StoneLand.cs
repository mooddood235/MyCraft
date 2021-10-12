using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneLand : Biome
{
    private FastNoiseLite elevationNoise;
    private const FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.Value;
    private const float amplitude = 15f;
    private const float frequency = 0.06f;

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

        return blocks;
    }


    public override int ObjLevelGetBlock(int elevation)
    {
        return Block.blockNameToId["Stone"];
    }

    public StoneLand()
    {
        elevationNoise = new FastNoiseLite();
        elevationNoise.SetNoiseType(noiseType);
        elevationNoise.SetFrequency(frequency);
    }
}
