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

    public void GetUvs(List<Vector2> blockUvs)
    {
        blockUvs.AddRange(uvs);
    }

    public static void GetVerts(Vector3Int blockPos, List<Vector3> blockVerts)
    {
        int startingIndex = blockVerts.Count;
        blockVerts.AddRange(verts);
        AMath.MutatorAddVector(blockVerts, blockPos, startingIndex);
    }
}
