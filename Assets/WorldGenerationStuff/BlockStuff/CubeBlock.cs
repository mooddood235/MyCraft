using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/CubeBlockObj")]
public class CubeBlock : Block
{
    # region vertArrays
    private static Vector3[] zFaceVerts = new Vector3[]
    {
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
    };
    private static Vector3[] negZFaceVerts = new Vector3[]
    {
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
    };
    private static Vector3[] xFaceVerts = new Vector3[]
    {
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
    };
    private static Vector3[] negXFaceVerts = new Vector3[]
    {
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
    };
    private static Vector3[] yFaceVerts = new Vector3[]
    {
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
    };
    private static Vector3[] negYFaceVerts = new Vector3[]
    {
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
    };
    private static Vector3[] allFaceVerts = new Vector3[]
    {
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),

        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),

        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),

        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),

        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),

        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
    };
    #endregion

    #region uvLists
    [HideInInspector]
    public List<Vector2> negXFaceUvs = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> xFaceUvs = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> negYFaceUvs = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> yFaceUvs = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> negZFaceUvs = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> zFaceUvs = new List<Vector2>();

    #endregion

    #region AtlasIndices

    [Header("Atlas Indices")]
    public Vector2 negXFaceAtlasIndex;
    public Vector2 xFaceAtlasIndex;
    public Vector2 negYFaceAtlasIndex;
    public Vector2 yFaceAtlasIndex;
    public Vector2 negZFaceAtlasIndex;
    public Vector2 zFaceAtlasIndex;

    #endregion

    public enum faces
    {
        negXFace,
        xFace,
        negYFace,
        yFace,
        negZFace,
        zFace,
        all
    }

    private static Vector3[] FaceToVerts(faces face)
    {
        if (face == faces.negXFace) return negXFaceVerts;
        else if (face == faces.xFace) return xFaceVerts;
        else if (face == faces.negYFace) return negYFaceVerts;
        else if (face == faces.yFace) return yFaceVerts;
        else if (face == faces.negZFace) return negZFaceVerts;
        else if (face == faces.zFace) return zFaceVerts;
        else return allFaceVerts;
    }

    public static List<Vector3> GetVerts(Vector3Int blockPos, List<faces> blockFaces)
    {
        List<Vector3> blockVerts = new List<Vector3>();

        foreach (faces face in blockFaces)
        {
            blockVerts.AddRange(FaceToVerts(face));
        }

        return AMath.AddVector(blockVerts, blockPos);
    }

    public List<Vector2> GetUvs(List<faces> blockFaces)
    {
        List<Vector2> blockUvs = new List<Vector2>();

        foreach (faces face in blockFaces)
        {
            blockUvs.AddRange(FaceToUv(face));
        }
        return blockUvs;
    }

    private List<Vector2> FaceToUv(faces face)
    {
        if (face == faces.negXFace) return negXFaceUvs;
        else if (face == faces.xFace) return xFaceUvs;
        else if (face == faces.negYFace) return negYFaceUvs;
        else if (face == faces.yFace) return yFaceUvs;
        else if (face == faces.negZFace) return negZFaceUvs;
        else return zFaceUvs;
    }
}
