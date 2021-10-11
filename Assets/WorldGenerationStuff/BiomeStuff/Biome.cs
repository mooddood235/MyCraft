using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Biome
{
    public static FastNoiseLite biomeNoise = new FastNoiseLite();
    public static Dictionary<Vector2, Biome> boundToBiome = new Dictionary<Vector2, Biome>()
    {
        { new Vector2(-1f, 0f), new Forest() },
        { new Vector2 (0f, 1f), new StoneLand() }
    };
    public static Vector2 boundWithSmallestMin = new Vector2(Mathf.Infinity, 0);
    public static Vector2 boundWithGreatestMax = new Vector2(0, Mathf.NegativeInfinity);
    private static int lerpRange = 4;

    public static Biome GetBiome(Vector2Int chunkPos, Vector3 blockPos)
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

    protected static int GetLerpedElevation(Vector2Int blockPos, Vector2Int chunkPos, Dictionary<Vector2Int, Chunk> chunks)
    {
        Vector2Int blockPosInWorldSpace = chunkPos * Chunk.dims.x + blockPos;

        List<Chunk> adjacentChunks = GetAdjacentChunks(chunkPos, chunks);

        float maxDist = Mathf.Sqrt(2f * Mathf.Pow((lerpRange + 1f) * Chunk.dims.x - 0.5f, 2));
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

    private static List<Chunk> GetAdjacentChunks(Vector2Int centreChunkPos, Dictionary<Vector2Int, Chunk> chunks)
    {
        List<Chunk> adjacentChunks = new List<Chunk>();

        for (int x = -lerpRange; x <= lerpRange; x++)
        {
            for (int y = -lerpRange; y <= lerpRange; y++)
            {
                Vector2Int chunkPos = centreChunkPos + new Vector2Int(x, y);
                if (!chunks.ContainsKey(chunkPos)) chunks[chunkPos] = new Chunk(chunkPos);
                adjacentChunks.Add(chunks[chunkPos]);
            }
        }
        return adjacentChunks;
    }

    public static List<KeyValuePair<int, int>> GetBlocks(Vector2Int blockPos, Vector2Int chunkPos, Dictionary<Vector2Int, Chunk> chunks)
    {
        Biome biome = GetBiome(chunkPos, new Vector3Int(blockPos.x, 0, blockPos.y));
        return biome.ObjLevelGetBlocks(blockPos, chunkPos, chunks);
    }

    abstract public float GetElevationNoise(Vector2 vector);

    abstract public List<KeyValuePair<int, int>> ObjLevelGetBlocks(Vector2Int blockPos, Vector2Int chunkPos, Dictionary<Vector2Int, Chunk> chunks);

    abstract public int ObjLevelGetBlock(int elevation);
}