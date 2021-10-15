using System;
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
    private GameObject chunkObj;
    private int[,,] blocks = new int[dims.x, dims.y, dims.z];
    private MeshData meshData = new MeshData();
    private MeshData colliderMeshData = new MeshData();
    private Mesh mesh;
    private Mesh colliderMesh;
    private bool meshGenerated;
    private bool generated;
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
        List<Vector3> meshVerts = new List<Vector3>();
        List<int> meshTris = new List<int>();
        List<Vector2> mesUvs = new List<Vector2>();
        
        List<Vector3> colliderMeshVerts = new List<Vector3>();
        List<int> colliderMeshTris = new List<int>();

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
                        int meshTrisStart = meshVerts.Count;
                        
                        if (Block.idToBlock[GetBlock(blockPos)] is CubeBlock)
                        {
                            int colliderMeshTrisStart = colliderMeshVerts.Count;

                            List<CubeBlock.faces> blockFaces = GetCubeBlockFaces(blockPos);
                            List<Vector3> verts = CubeBlock.GetVerts(blockPos, blockFaces);
                            meshVerts.AddRange(verts);
                            colliderMeshVerts.AddRange(verts);

                            CubeBlock block_ = (CubeBlock)Block.idToBlock[GetBlock(blockPos)];
                            mesUvs.AddRange(block_.GetUvs(blockFaces));
                            
                            MeshData.AddTris(meshTrisStart, meshVerts.Count, meshTris);
                            MeshData.AddTris(colliderMeshTrisStart, colliderMeshVerts.Count, colliderMeshTris);
                        }
                        else
                        {
                            meshVerts.AddRange(PlanesBlock.GetVerts(blockPos));
                            PlanesBlock block_ = (PlanesBlock)Block.idToBlock[GetBlock(blockPos)];
                            mesUvs.AddRange(block_.GetUvs());
                            
                            MeshData.AddDoubleSidedTris(meshTrisStart, meshVerts.Count, meshTris);
                        }
                    }
               }
            }
        }
        meshData.vertices = meshVerts.ToArray();
        meshData.triangles = meshTris.ToArray();
        meshData.uv = mesUvs.ToArray();

        colliderMeshData.vertices = colliderMeshVerts.ToArray();
        colliderMeshData.triangles = colliderMeshTris.ToArray();
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

    public Mesh GetColliderMesh()
    {
        return colliderMesh;
    }
    
    public void SetMeshFromMeshData()
    {
        meshGenerated = true;
        mesh = meshData.GetMesh();
    }

    public void SetColliderMeshFromColliderMeshData()
    {
        colliderMesh = colliderMeshData.GetMesh();
    }

    public bool MeshIsGenerated()
    {
        return meshGenerated;
    }

    public bool IsGenerated()
    {
        return generated;
    }

    public Chunk(Vector2Int pos)
    {
        this.pos = pos;
        SetTargetBiome();
    }

}
