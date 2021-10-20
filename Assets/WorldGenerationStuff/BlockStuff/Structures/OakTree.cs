using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OakTree : Structure
{
    private const int TreeMinHeight = 6;
    private const int TreeMaxHeight = 7;
    private const int LeavesMinHeight = 3;
    private const int LeavesMaxHeight = 5;
    private static int oakLog = Block.blockNameToId["OakLog"];
    private static int oakLeaves = Block.blockNameToId["OakLeaves"];

    public static List<KeyValuePair<Vector3Int, int>> GenerateStructure(Vector3Int startPos, Vector2Int chunkPos)
    {
        List<KeyValuePair<Vector3Int, int>> blocks = new List<KeyValuePair<Vector3Int, int>>();

        int treeHeight = random.Next(TreeMinHeight, TreeMaxHeight);
        int leavesHeight = random.Next(LeavesMinHeight, LeavesMaxHeight);

        GenerateTrunk(startPos, treeHeight, blocks);
        GenerateLeaves(startPos, chunkPos, treeHeight, leavesHeight, blocks);
        
        return blocks;
    }

    private static void GenerateLeaves(Vector3Int startPos, Vector2Int chunkPos, int treeHeight, int leavesHeight, List<KeyValuePair<Vector3Int, int>> blocks)
    {
        // Top collection
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 || z == 0)
                {
                    Vector3Int oakLeavesBlockPos = startPos + new Vector3Int(x, treeHeight - 1, z);
                    AddBlock(new KeyValuePair<Vector3Int, int>(oakLeavesBlockPos, oakLeaves), blocks, chunkPos, true);
                }
            }
        }
    }
    
    private static void GenerateTrunk(Vector3Int startPos, int treeHeight, List<KeyValuePair<Vector3Int, int>> blocks)
    {
        for (int i = 0; i < treeHeight - 1; i++)
        {
            Vector3Int oakLogPos = startPos + Vector3Int.up * i;
            
            blocks.Add(new KeyValuePair<Vector3Int, int>(oakLogPos, oakLog));
        }
    }
}
