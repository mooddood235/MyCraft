using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class OakForest : Biome
{
    private OakForestElevationNoise elevationNoise;
    private OakForestOakTreeNoise oakTreeNoise;
    private OakForestWeedBlockNoise weedBlockNoise;

    private int grassBlock = Block.blockNameToId["Grass"];
    private int weedBlock = Block.blockNameToId["Weed"];
    private OakTree oakTree = new OakTree();

    private Range oakTreeRange = new Range(0f, 0.01f);
    private Range weedBlockRange = new Range(0, 0.2f);

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
        
        // Add weed block.
        bool shouldSpawnWeedBlock =
            weedBlockRange.Within(weedBlockNoise.GetNoise(surfaceBlockPos)) && random.Next(0, 2) == 1;

        if (shouldSpawnWeedBlock)
        {
            Vector3Int weedPos = grassPos + Vector3Int.up;
            blocks.Add(new KeyValuePair<Vector3Int, int>(weedPos, weedBlock));
        }

        // Add oak tree.
        bool shouldSpawnOakTree = oakTreeRange.Within(oakTreeNoise.GetNoise(surfaceBlockPosInWorldSpace)) &&
                                  random.Next(0, 2) == 1;
        if (shouldSpawnOakTree)
        {
            blocks.AddRange(oakTree.GenerateStructure(grassPos + Vector3Int.up, chunkPos));
        }

        return blocks;
    }
    
    
    

    public OakForest()
    {
        elevationNoise = new OakForestElevationNoise();
        oakTreeNoise = new OakForestOakTreeNoise();
        weedBlockNoise = new OakForestWeedBlockNoise();
    }
}
