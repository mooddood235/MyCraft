using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/PlanesBlockObj")]
public class PlanesBlock : Block
{
    private static List<Vector3> verts = new List<Vector3>
    {
        // Front face.
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        
        // Back face.
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        
        // Front face.
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),

        // Back face.
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f)
    };

    public Vector2 atlasIndex;

    [HideInInspector]
    public List<Vector2> uvs = new List<Vector2>();

    public List<Vector2> GetUvs()
    {
        return uvs;
    }

    public static List<Vector3> GetVerts(Vector3Int blockPos)
    {
        return AMath.AddVector(verts, blockPos);
    }
}
