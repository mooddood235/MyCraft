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

    public static List<KeyValuePair<Vector3Int, int>> GenerateStructure(Vector3Int startPos)
    {
        List<KeyValuePair<Vector3Int, int>> blocks = new List<KeyValuePair<Vector3Int, int>>();

        int treeHeight = random.Next(TreeMinHeight, TreeMaxHeight);
        int leavesHeight = random.Next(LeavesMinHeight, LeavesMaxHeight);

        GenerateTrunk(startPos, treeHeight, blocks);
        GenerateLeaves(startPos, treeHeight, leavesHeight, blocks);
        
        return blocks;
    }

    private static void GenerateLeaves(Vector3Int startPos, int treeHeight, int leavesHeight, List<KeyValuePair<Vector3Int, int>> blocks)
    {
        // Top collection
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 || z == 0)
                {
                    Vector3Int oakLeavesBlockPos = startPos + new Vector3Int(x, treeHeight - 1, z);
                    blocks.Add(new KeyValuePair<Vector3Int, int>(oakLeavesBlockPos, oakLeaves));
                }
            }
        }
        // Second top collection
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if ((x == 0 || z == 0 || random.Next(0, 2) == 1) && (x != 0 || z != 0))
                {
                    Vector3Int oakLeavesBlockPos = startPos + new Vector3Int(x, treeHeight - 2, z);
                    blocks.Add(new KeyValuePair<Vector3Int, int>(oakLeavesBlockPos, oakLeaves));
                }
            }
        }
        // The bottom collections
        for (int i = 0; i < leavesHeight - 2; i++)
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int z = -2; z <= 2; z++)
                {
                    if (x != 0 || z != 0 && (Mathf.Abs(x) == 2 || Mathf.Abs(z) == 2) && random.Next(0, 2) == 1)
                    {
                        Vector3Int oakLeavesBlockPos = startPos + new Vector3Int(x, treeHeight - 1 - i, z);
                        blocks.Add(new KeyValuePair<Vector3Int, int>(oakLeavesBlockPos, oakLeaves));
                    }
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
