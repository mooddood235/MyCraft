using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Block : ScriptableObject
{
    public static int AtlasRes = 128;
    public static Vector2[] normalUvs = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    [Header("Identification")]
    [SerializeField]
    protected int id;
    [SerializeField]
    protected string name;

    public static Dictionary<int, Block> idToBlock = new Dictionary<int, Block>();
    public static Dictionary<string, int> blockNameToId = new Dictionary<string, int>();

    public List<Vector3> GetVerts()
    {
        throw new System.NotImplementedException();
    }
    public List<Vector2> GetUvs()
    {
        throw new System.NotImplementedException();
    }

    public int GetId()
    {
        return id;
    }
    public string GetName()
    {
        return name;
    }

}
