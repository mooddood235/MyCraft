using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Structure
{
    protected static System.Random random = new System.Random();
    
    protected List<KeyValuePair<Vector3Int, int>> tempBlocks = new List<KeyValuePair<Vector3Int, int>>();

    public abstract List<KeyValuePair<Vector3Int, int>> GenerateStructure(Vector3Int startPos, Vector2Int chunkPos);

    protected static void AddBlock(KeyValuePair<Vector3Int, int> positionToBlock, List<KeyValuePair<Vector3Int, int>> blocks,
        Vector2Int chunkPos, bool checkOutOfBounds = true)
    {
        Vector3Int blockPos = positionToBlock.Key;
        
        if (checkOutOfBounds == false ||
            Chunk.HorizontalBounds.Within(blockPos.x) && Chunk.HorizontalBounds.Within(blockPos.z))
        {
            blocks.Add(positionToBlock);
        }
        else
        {
            blockPos = Chunk.GetArrayCoordsFromCentroidCoords(blockPos);
            Vector2Int otherChunkPos = chunkPos + Chunk.GetChunkOffsetFromBlockPos(blockPos);
            blockPos = Chunk.GetCentroidCoordsFromArrayCoords(VMath.PyMod(blockPos, Chunk.Dims.x));
            blockPos.y = positionToBlock.Key.y;

            Chunk otherChunk = Chunk.GetChunk(otherChunkPos);
            otherChunk.SetBlock(positionToBlock.Value, blockPos);
            
            if (otherChunk.IsGenerated()) Chunk.AddToRemeshStack(otherChunk);
        }
    }
}
