using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/PlanesBlockObj")]
public class PlanesBlock : Block
{
    private static Vector3[] verts = new Vector3[]
    {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),

        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f)
    };

    public Vector2 atlasIndex;

    [HideInInspector]
    public List<Vector2> uvs = new List<Vector2>();


    public List<Vector2> GetUvsArray()
    {
        return uvs;
    }

    public static Vector3[] GetVertsArray()
    {
        return verts;
    }
}
