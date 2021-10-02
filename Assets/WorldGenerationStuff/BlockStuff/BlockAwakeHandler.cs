using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The purpose of this class is to fill block scriptable objects with their data.
public class BlockAwakeHandler : MonoBehaviour
{
    [SerializeField]
    private List<Block> blocks;
    [SerializeField]
    private bool refillUvs = false;

    private void Awake()
    {
        foreach (Block block in blocks)
        {
            FillIdToBlockAndBlockNameToIdDictionaries(block);

            if (refillUvs)
            {
                RefillUvs(block);
            }      
        } 
    }

    private void FillIdToBlockAndBlockNameToIdDictionaries(Block block)
    {
        Block.idToBlock[block.GetId()] = block;
        Block.blockNameToId[block.GetName()] = block.GetId();
    }
    private void RefillUvs(Block block)
    {
        block.negXFaceUvs = new List<Vector2>();
        block.xFaceUvs = new List<Vector2>();
        block.negYFaceUvs = new List<Vector2>();
        block.yFaceUvs = new List<Vector2>();
        block.negZFaceUvs = new List<Vector2>();
        block.zFaceUvs = new List<Vector2>();

        foreach (Vector2 uv in Block.normalUvs)
        {
            // Logic behind algorithm:
            // uvPos = offsetFromPadding + offsetFromIndex + offsetFromUv

            block.negXFaceUvs.Add((new Vector2(1, 1) + block.negXFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
            block.xFaceUvs.Add((new Vector2(1, 1) + block.xFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
            block.negYFaceUvs.Add((new Vector2(1, 1) + block.negYFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
            block.yFaceUvs.Add((new Vector2(1, 1) + block.yFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
            block.negZFaceUvs.Add((new Vector2(1, 1) + block.negZFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
            block.zFaceUvs.Add((new Vector2(1, 1) + block.zFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
        }
    }


}
