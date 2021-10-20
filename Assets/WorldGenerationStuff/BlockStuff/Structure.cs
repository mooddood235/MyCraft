using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Structure
{
    protected static System.Random random = new System.Random();
    protected static List<KeyValuePair<Vector3Int, int>> GenerateStructure()
    {
        throw new System.NotImplementedException();
    }

    protected static void AddBlock(KeyValuePair<Vector3Int, int> positionToBlock, List<KeyValuePair<Vector3Int, int>> blocks,
        Vector2Int chunkPos, bool checkOutOfBounds = false)
    {
        Vector3Int blockPos = positionToBlock.Key;
        
        if (checkOutOfBounds == false ||
            Chunk.horizontalBounds.Within(blockPos.x) && Chunk.horizontalBounds.Within(blockPos.z))
        {
            blocks.Add(positionToBlock);
        }
        else
        {
            blockPos = VMath.PyMod(Chunk.GetArrayCoordsFromCentroidCoords(blockPos), Chunk.dims.x);

            Vector2Int otherChunkPos = chunkPos + GetOffsetFromBlockPos(positionToBlock.Key);

            blockPos = Chunk.GetCentroidCoordsFromArrayCoords(blockPos);
            blockPos.y = positionToBlock.Key.y;

            Chunk.GetChunk(otherChunkPos).SetBlock(positionToBlock.Value, blockPos);
        }
    }

    private static Vector2Int GetOffsetFromBlockPos(Vector3Int blockPos)
    {
        float x = blockPos.x / 16f;
        float z = blockPos.z / 16f;
        
        if (x > 0 && Mathf.FloorToInt(x) == x) x -= 1;
        if (z > 0 && Mathf.FloorToInt(z) == z) z -= 1;

        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(z));
    }
}
