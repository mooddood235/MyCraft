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
        if (block is CubeBlock)
        {
            CubeBlock block_ = (CubeBlock)block;

            block_.negXFaceUvs = new List<Vector2>();
            block_.xFaceUvs = new List<Vector2>();
            block_.negYFaceUvs = new List<Vector2>();
            block_.yFaceUvs = new List<Vector2>();
            block_.negZFaceUvs = new List<Vector2>();
            block_.zFaceUvs = new List<Vector2>();

            foreach (Vector2 uv in Block.normalUvs)
            {
                // Logic behind algorithm:
                // uvPos = offsetFromPadding + offsetFromIndex + offsetFromUv

                block_.negXFaceUvs.Add((new Vector2(1, 1) + block_.negXFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
                block_.xFaceUvs.Add((new Vector2(1, 1) + block_.xFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
                block_.negYFaceUvs.Add((new Vector2(1, 1) + block_.negYFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
                block_.yFaceUvs.Add((new Vector2(1, 1) + block_.yFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
                block_.negZFaceUvs.Add((new Vector2(1, 1) + block_.negZFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
                block_.zFaceUvs.Add((new Vector2(1, 1) + block_.zFaceAtlasIndex * 18f + uv * 16f) / Block.AtlasRes);
            }
        }
        else
        {
            PlanesBlock block_ = (PlanesBlock)block;
            block_.uvs = new List<Vector2>();

            List<Vector2> uvsToAdd = new List<Vector2>();

            foreach (Vector2 uv in Block.normalUvs)
            {
                uvsToAdd.Add((new Vector2(1, 1) + block_.atlasIndex * 18f + uv * 16f) / Block.AtlasRes);
            }

            block_.uvs.AddRange(uvsToAdd);
            block_.uvs.AddRange(uvsToAdd);
            block_.uvs.AddRange(uvsToAdd);
            block_.uvs.AddRange(uvsToAdd);
        }       
    }
}
