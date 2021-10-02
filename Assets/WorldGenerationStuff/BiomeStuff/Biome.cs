using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="ScriptableObjects", menuName="ScriptableObjects/BiomeObj")]
public class Biome : ScriptableObject
{
    #region staticAttributes
    public static FastNoiseLite biomeNoise = new FastNoiseLite();
    public static Dictionary<Vector2, Biome> boundToBiome = new Dictionary<Vector2, Biome>();
    public static Vector2 boundWithSmallestMin = new Vector2(Mathf.Infinity, 0);
    public static Vector2 boundWithGreatestMax = new Vector2(0, Mathf.NegativeInfinity);
    #endregion

    public Vector2 biomeNoiseBounds;
    public FastNoiseLite.NoiseType elevationNoiseType;
    public float elevationNoiseFrequency;
    public float elevationNoiseAmplitude;
    public FastNoiseLite elevationNoise = new FastNoiseLite();
    public string block;
    private static int lerpRange = 4;

    public static Biome GetBiome(Vector3 blockPos, Vector2Int chunkPos)
    {
        Vector2Int chunkPosInWorldSpace = chunkPos * Chunk.dims.x;
        Vector2 blockPosInWorldSpace = chunkPosInWorldSpace + new Vector2(blockPos.x, blockPos.z);

        float biomeNoiseValue = biomeNoise.GetNoise(blockPosInWorldSpace.x, blockPosInWorldSpace.y);
 
        foreach (Vector2 bound in boundToBiome.Keys)
        {
            if (bound.x <= biomeNoiseValue && biomeNoiseValue <= bound.y)
            {
                return boundToBiome[bound];
            }
        }
        if (biomeNoiseValue >= boundWithGreatestMax.y) return boundToBiome[boundWithGreatestMax];
        else return boundToBiome[boundWithSmallestMin];   
    }
    public static int GetElevation(Vector2Int blockPos, Chunk chunk, Dictionary<Vector2Int, Chunk> chunks)
    {
        Vector2Int blockPosInWorldSpace = chunk.GetPos() * Chunk.dims.x + blockPos;

        List<Chunk> adjacentChunks = GetAdjacentChunks(chunk, chunks);

        float maxDist = Mathf.Sqrt(2f * Mathf.Pow((lerpRange + 1f) * Chunk.dims.x - 0.5f , 2));
        float sumOfWeights = 0f;
        float weightedAverage = 0f;

        foreach (Chunk chunk_ in adjacentChunks)
        {
            float distFromChunk = Vector2.Distance(chunk_.GetPos() * Chunk.dims.x - Chunk.offsetToSouthWestCorner, blockPosInWorldSpace);
            float weight = 1f - distFromChunk / maxDist;
            float elevation = chunk_.GetTargetBiome().GetElevationNoise(blockPosInWorldSpace);

            sumOfWeights += weight;
            weightedAverage += elevation * weight;
        }
        weightedAverage /= sumOfWeights;
        return Mathf.RoundToInt(weightedAverage);
    }

    public static List<Chunk> GetAdjacentChunks(Chunk centreChunk, Dictionary<Vector2Int, Chunk> chunks)
    {
        List<Chunk> adjacentChunks = new List<Chunk>();

        for (int x  = -lerpRange; x <= lerpRange; x++)
        {
            for (int y = -lerpRange; y <= lerpRange; y++)
            {
                Vector2Int chunkPos = centreChunk.GetPos() + new Vector2Int(x, y);
                if (!chunks.ContainsKey(chunkPos)) chunks[chunkPos] = new Chunk(chunkPos);
                adjacentChunks.Add(chunks[chunkPos]);
            }
        }
        return adjacentChunks;
    }

    public float GetElevationNoise(Vector2Int vector)
    {
        return elevationNoise.GetNoise(vector.x, vector.y) * elevationNoiseAmplitude;
    }

    public int GetBlock(Vector3Int blockPos, Vector2Int chunkPos)
    {
        Vector3Int chunkPosInWorldSpace = new Vector3Int(chunkPos.x, 0, chunkPos.y) * Chunk.dims.x;
        Vector3Int blockPosInWorldSpace = chunkPosInWorldSpace + blockPos;

        return Block.blockNameToId[block];
    }
}
