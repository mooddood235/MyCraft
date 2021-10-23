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
            blockPos = Chunk.GetArrayCoordsFromCentroidCoords(blockPos);
            Vector2Int otherChunkPos = chunkPos + Chunk.GetChunkOffsetFromBlockPos(blockPos);
            blockPos = Chunk.GetCentroidCoordsFromArrayCoords(VMath.PyMod(blockPos, Chunk.dims.x));
            blockPos.y = positionToBlock.Key.y;

            Chunk otherChunk = Chunk.GetChunk(otherChunkPos);
            otherChunk.SetBlock(positionToBlock.Value, blockPos);
            Chunk.AddToRemeshStack(otherChunk);
        }
    }
}
