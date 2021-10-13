using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    /*
     * Representation Invariants:
     *      dims.x == dims.y && all dimensions must be odd.
     */
    public static Vector3Int dims = new Vector3Int(17, 61, 17);
    private Vector2Int pos;
    private GameObject chunkObj = null;
    private int[,,] blocks = new int[dims.x, dims.y, dims.z];
    private MeshData meshData = new MeshData();
    private Mesh mesh;
    private bool meshGenerated = false;
    private bool generated = false;
    private Biome targetBiome;
    public static readonly Vector2 offsetToSouthWestCorner = new Vector2((dims.x - 1f) / 2f + 0.5f, (dims.z - 1f) / 2f + 0.5f);

    public void SetChunkObj(GameObject chunkObj)
    {
        this.chunkObj = chunkObj;
    }
    public GameObject GetChunkObj()
    {
        return chunkObj;
    }

    public Vector2Int GetPos()
    {
        return pos;
    }

    private void SetBlock(int block, Vector3Int blockPos)
    {
        Vector3Int arrayCoords = GetArrayCoordsFromCentroidCoords(blockPos);
        blocks[arrayCoords.x, arrayCoords.y, arrayCoords.z] = block;
    }

    private int GetBlock(Vector3Int blockPos)
    {
        Vector3Int arrayCoords = GetArrayCoordsFromCentroidCoords(blockPos);
        return blocks[arrayCoords.x, arrayCoords.y, arrayCoords.z];
    }

    private Vector3Int GetArrayCoordsFromCentroidCoords(Vector3Int centroidCoords)
    {
        int x = centroidCoords.x + (dims.x - 1) / 2;
        int y = centroidCoords.y + (dims.y - 1) / 2;
        int z = centroidCoords.z + (dims.z - 1) / 2;

        return new Vector3Int(x, y, z);
    }

    public void Generate(Dictionary<Vector2Int, Chunk> chunks)
    {
        GenerateBlocks(chunks);
        GenerateMesh();

        generated = true;
    }
    public void GenerateBlocks(Dictionary<Vector2Int, Chunk> chunks)
    {
        int xRadius = (dims.x - 1) / 2;
        int zRadius = (dims.z - 1) / 2;

        for (int x = -xRadius; x <= xRadius; x++)
        {
            for (int z = -zRadius; z <= zRadius; z++)
            {
                foreach (KeyValuePair<Vector3Int, int> elevationToBlock in Biome.GetBlocks(new Vector2Int(x, z), pos, chunks))
                {
                    SetBlock(elevationToBlock.Value, elevationToBlock.Key);
                }
            }
        }
    }
    public void GenerateMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int xRadius = (dims.x - 1) / 2;
        int yRadius = (dims.y - 1) / 2;
        int zRadius = (dims.z - 1) / 2;

        for (int x = -xRadius; x <= xRadius; x++)
        {
            for (int y = -yRadius; y <= yRadius; y++)
            {
               for (int z = -zRadius; z <= zRadius; z++)
               {
                    Vector3Int blockPos = new Vector3Int(x, y, z);
                    if (GetBlock(blockPos) != Block.blockNameToId["Air"])
                    {
                        int trisStart = verts.Count;

                        if (Block.idToBlock[GetBlock(blockPos)] is CubeBlock)
                        {
                            List<CubeBlock.faces> blockFaces = GetCubeBlockFaces(blockPos);
                            verts.AddRange(GetCubeBlockVerts(blockFaces, blockPos));
                            uvs.AddRange(GetCubeBlockUvs((CubeBlock)Block.idToBlock[GetBlock(blockPos)], blockFaces));
                        }
                        else
                        {
                            verts.AddRange(AMath.AddVector(PlanesBlock.GetVertsArray(), blockPos));
                            PlanesBlock block = (PlanesBlock)Block.idToBlock[GetBlock(blockPos)];
                            uvs.AddRange(block.GetUvsArray());
                        }

                        
                        for (int i = trisStart; i < verts.Count; i += 4)
                        {
                            int[] firstTri = new int[] { i, i + 1, i + 2 };
                            int[] secondTri = new int[] { i + 2, i + 3, i };
                            tris.AddRange(firstTri);
                            tris.AddRange(secondTri);
                        }
                    }
               }
            }
        }
        meshData.vertices = verts.ToArray();
        meshData.triangles = tris.ToArray();
        meshData.uv = uvs.ToArray();
    }
    
    private void SetTargetBiome()
    {
        Vector3 southWestCorner = new Vector3(-(dims.x - 1f) / 2f - 0.5f, 0f, -(dims.z - 1f) / 2f - 0.5f);
        targetBiome = Biome.GetBiome(pos, southWestCorner);
    }

    public Biome GetTargetBiome()
    {
        return targetBiome;
    }

    private Vector3[] GetCubeBlockVerts(List<CubeBlock.faces> blockFaces, Vector3Int blockPos)
    {
        List<Vector3> blockVerts = new List<Vector3>();
        
        foreach (CubeBlock.faces face in blockFaces)
        {
            blockVerts.AddRange(CubeBlock.GetVertsArray(face));
        }

        return AMath.AddVector(blockVerts.ToArray(), blockPos);
    }

    private Vector2[] GetCubeBlockUvs(CubeBlock block, List<CubeBlock.faces> blockFaces)
    {
        List<Vector2> blockUvs = new List<Vector2>();
        
        foreach (CubeBlock.faces face in blockFaces)
        {
            blockUvs.AddRange(block.GetUvsArray(face));
        }
        return blockUvs.ToArray();
    }

    private List<CubeBlock.faces> GetCubeBlockFaces(Vector3Int blockPos)
    {
        List<CubeBlock.faces> faces = new List<CubeBlock.faces>();

        // Boolean expression logic for all if statements is the following:
        // If (the block is NOT at the side of the chunk AND (the block next to it is air or is a PlanesBlock)) OR (The block is at the side of the chunk)

        Vector3Int negXblockPos = blockPos + Vector3Int.left;
        Vector3Int xBlockPos = blockPos + Vector3Int.right;

        Vector3Int negYBlockPos = blockPos + Vector3Int.down;
        Vector3Int yBlockPos = blockPos + Vector3Int.up;

        Vector3Int negZBlockPos = blockPos + Vector3Int.back;
        Vector3Int zBlockPos = blockPos + Vector3Int.forward;

        if (blockPos.x > -(dims.x - 1) / 2 && (GetBlock(negXblockPos) == 0 || Block.idToBlock[GetBlock(negXblockPos)] is PlanesBlock) || blockPos.x == -(dims.x - 1) / 2)
        {
            faces.Add(CubeBlock.faces.negXFace);
        }
        if (blockPos.x < (dims.x - 1) / 2 && (GetBlock(xBlockPos) == 0 || Block.idToBlock[GetBlock(xBlockPos)] is PlanesBlock) || blockPos.x == (dims.x - 1) / 2)
        {
            faces.Add(CubeBlock.faces.xFace);
        }
        if (blockPos.y > -(dims.y - 1) / 2 && (GetBlock(negYBlockPos) == 0 || Block.idToBlock[GetBlock(negYBlockPos)] is PlanesBlock) || blockPos.y == -(dims.y - 1) / 2)
        {
            faces.Add(CubeBlock.faces.negYFace);
        }
        if (blockPos.y < (dims.y - 1) / 2 && (GetBlock(yBlockPos) == 0 || Block.idToBlock[GetBlock(yBlockPos)] is PlanesBlock) || blockPos.y == (dims.y - 1) / 2)
        {
            faces.Add(CubeBlock.faces.yFace);
        }
        if (blockPos.z > -(dims.z - 1) / 2 && (GetBlock(negZBlockPos) == 0 || Block.idToBlock[GetBlock(negZBlockPos)] is PlanesBlock) || blockPos.z == -(dims.z - 1) / 2)
        {
            faces.Add(CubeBlock.faces.negZFace);
        }
        if (blockPos.z < (dims.z - 1) / 2 && (GetBlock(zBlockPos) == 0 || Block.idToBlock[GetBlock(zBlockPos)] is PlanesBlock) || blockPos.z == (dims.z - 1) / 2)
        {
            faces.Add(CubeBlock.faces.zFace);
        }

        return faces; 
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    public void SetMeshFromMeshData()
    {
        meshGenerated = true;
        mesh = meshData.GetMesh();
    }

    public bool meshIsGenerated()
    {
        return meshGenerated;
    }

    public bool isGenerated()
    {
        return generated;
    }

    public Chunk(Vector2Int pos)
    {
        this.pos = pos;
        SetTargetBiome();
    }

}
