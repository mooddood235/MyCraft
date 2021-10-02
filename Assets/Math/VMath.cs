using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VMath
{
    public static Vector3 V2ToV3(Vector2 v)
    {
        return new Vector3(v.x, 0f, v.y);
    }

    public static List<Vector3> V2ListToV3List(List<Vector2> vectors)
    {
        List<Vector3> v3List = new List<Vector3>();

        foreach (Vector2 v2 in vectors)
        {
            v3List.Add(V2ToV3(v2));
        }
        return v3List;
    }

    public static Vector2Int SignComponents(Vector2 v)
    {
        return new Vector2Int((int)Mathf.Sign(v.x), (int)Mathf.Sign(v.y));
    }
    
    public static Vector2Int MultiComponents(Vector2Int v1, Vector2Int v2)
    {
        return new Vector2Int(v1.x * v2.x, v1.y * v2.y);
    }

}
