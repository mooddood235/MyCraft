using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure
{
    protected static System.Random random = new System.Random();
    protected static List<KeyValuePair<Vector3Int, int>> GenerateStructure()
    {
        throw new System.NotImplementedException();
    }

    protected static void AddBlock(KeyValuePair<Vector3Int, int> positionToBlock, List<KeyValuePair<Vector3Int, int>> blocks,
        Chunk chunk)
    {
        Vector3Int pos = positionToBlock.Key;
        
        if (!Chunk.horizontalBounds.Within(pos.x))
        {
            int blocksOver = Mathf.Abs(pos.x) % Chunk.halfExtent;
            int xPosInOtherChunk = blocksOver + (Chunk.halfExtent + 1) * NMath.Sign(pos.x);
        }
        else if (!Chunk.horizontalBounds.Within(pos.z))
        {
            
        }
        else if (!Chunk.horizontalBounds.Within(pos.x) && !Chunk.horizontalBounds.Within(pos.z))
        {
            
        }
        else
        {
            blocks.Add(positionToBlock);
        }
    }
}
