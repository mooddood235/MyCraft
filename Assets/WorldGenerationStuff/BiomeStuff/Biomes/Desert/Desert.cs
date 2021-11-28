using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desert : Biome
{
    private DesertElevationNoise desertElevationNoise;
    private int sandBlock = Block.blockNameToId["Sand"];

    public override float GetElevationNoise(Vector2 vector)
    {
        return desertElevationNoise.GetNoise(vector);
    }

    public override List<KeyValuePair<Vector3Int, int>> ObjLevelGetBlocks(Vector2Int surfaceBlockPos, Vector2Int chunkPos)
    {
        int lerpedElevation = GetLerpedElevation(surfaceBlockPos, chunkPos);
        Vector3Int blockPos = new Vector3Int(surfaceBlockPos.x, lerpedElevation, surfaceBlockPos.y);
        
        return new List<KeyValuePair<Vector3Int, int>>() {new KeyValuePair<Vector3Int, int>(blockPos, sandBlock) };
    }

    public Desert()
    {
        desertElevationNoise = new DesertElevationNoise();
    }
}
