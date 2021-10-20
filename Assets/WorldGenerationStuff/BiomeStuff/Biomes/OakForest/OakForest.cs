using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakForest : Biome
{
    private OakForestElevationNoise elevationNoise;
    private OakForestOakTreeNoise oakTreeNoise;

    private int grassBlock = Block.blockNameToId["Grass"];

    private Range oakTreeRange = new Range(0, 0.01f);

    public override float GetElevationNoise(Vector2 vector)
    {
        return elevationNoise.GetNoise(vector);
    }

    public override List<KeyValuePair<Vector3Int, int>> ObjLevelGetBlocks(Vector2Int surfaceBlockPos, Vector2Int chunkPos)
    {
        List<KeyValuePair<Vector3Int, int>> blocks = new List<KeyValuePair<Vector3Int, int>>();

        Vector2Int surfaceBlockPosInWorldSpace = surfaceBlockPos + chunkPos;
        
        int lerpedElevation = GetLerpedElevation(surfaceBlockPos, chunkPos);
        
        // Add grass block.
        Vector3Int grassPos = new Vector3Int(surfaceBlockPos.x, lerpedElevation, surfaceBlockPos.y);
        blocks.Add(new KeyValuePair<Vector3Int, int>(grassPos, grassBlock));

        bool shouldSpawnOakTree = oakTreeRange.Within(oakTreeNoise.GetNoise(surfaceBlockPosInWorldSpace));
        // Add oak tree.
        if (shouldSpawnOakTree && random.Next(0, 2) == 1)
        {
            blocks.AddRange(OakTree.GenerateStructure(grassPos + Vector3Int.up, chunkPos));
        }

        return blocks;
    }

    public OakForest()
    {
        elevationNoise = new OakForestElevationNoise();
        oakTreeNoise = new OakForestOakTreeNoise();
    }
}
