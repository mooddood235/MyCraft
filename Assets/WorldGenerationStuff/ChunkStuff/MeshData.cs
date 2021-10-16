using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;

    public static void AddTris(int startIndex, int endIndex, List<int> tris)
    {
        for (int i = startIndex; i < endIndex; i += 4)
        {
            int[] firstTri = { i, i + 1, i + 2 };
            int[] secondTri = { i + 2, i + 3, i };
            tris.AddRange(firstTri);
            tris.AddRange(secondTri);
        }
    }
    
    public static void AddDoubleSidedTris(int startIndex, int endIndex, List<int> tris)
    {
        // Precondition: The vertices for the mesh must be organised in alternating order of front facing triangle
        // then back facing triangle.
        bool front = true;

        for (int i = startIndex; i < endIndex; i += 4)
        {
            int[] firstTri = { i, i + 1, i + 2 };
            int[] secondTri = { i + 2, i + 3, i };

            if (front)
            {
                tris.AddRange(firstTri);
                tris.AddRange(secondTri);
            }
            else
            {
                tris.AddRange(firstTri.Reverse());
                tris.AddRange(secondTri.Reverse()); 
            }
            front = !front;
        }
    }
    
    public Mesh GetMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        return mesh;
    }
}
