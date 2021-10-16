using System.Collections.Generic;
using UnityEngine;

public class OakTree : Structure
{
    private const int MinHeight = 6;
    private const int MaxHeight = 7;
    private static int oakLog = Block.blockNameToId["OakLog"];
    
    
    public static List<KeyValuePair<Vector3Int, int>> GenerateStructure(Vector3Int startPos)
    {
        List<KeyValuePair<Vector3Int, int>> blocks = new List<KeyValuePair<Vector3Int, int>>();
        
        // Generate trunk.
        System.Random random = new System.Random();

        for (int i = 0; i < random.Next(MinHeight, MaxHeight); i++)
        {
            Vector3Int oakLogPos = startPos + Vector3Int.up * i;
            
            blocks.Add(new KeyValuePair<Vector3Int, int>(oakLogPos, oakLog));
        }
        
        return blocks;
    }
}
