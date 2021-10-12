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

    public static Vector3[] GetVertsArray(faces face)
    {
        if (face == faces.negXFace) return negXFaceVerts;
        else if (face == faces.xFace) return xFaceVerts;
        else if (face == faces.negYFace) return negYFaceVerts;
        else if (face == faces.yFace) return yFaceVerts;
        else if (face == faces.negZFace) return negZFaceVerts;
        else if (face == faces.zFace) return zFaceVerts;
        else return allFaceVerts;
    }

    public List<Vector2> GetUvsArray(faces face)
    {
        if (face == faces.negXFace) return negXFaceUvs;
        else if (face == faces.xFace) return xFaceUvs;
        else if (face == faces.negYFace) return negYFaceUvs;
        else if (face == faces.yFace) return yFaceUvs;
        else if (face == faces.negZFace) return negZFaceUvs;
        else return zFaceUvs;
    }
}
