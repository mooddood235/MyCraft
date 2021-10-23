using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Chunk
{
    /*
     * Representation Invariants:
     *      dims.x == dims.z && all dimensions must be odd.
     */
    private static Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private static Stack<Chunk> chunksToRemesh = new Stack<Chunk>();
    public static Vector3Int dims = new Vector3Int(17, 61, 17);
    private static readonly int halfExtent = (dims.x - 1) / 2;
    public static readonly Range horizontalBounds = new Range(-(dims.x - 1) / 2f, (dims.x - 1) / 2f);
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

    public static readonly Vector2 offsetToSouthWestCorner =
        new Vector2((dims.x - 1f) / 2f + 0.5f, (dims.z - 1f) / 2f + 0.5f);

    public static void SetChunk(Vector2Int chunkPos)
    {
        chunks[chunkPos] = new Chunk(chunkPos);
    }

    public static Chunk GetChunk(Vector2Int chunkPos)
    {
        if (!ChunkExists(chunkPos)) SetChunk(chunkPos);
        return chunks[chunkPos];
    }

    private static bool ChunkExists(Vector2Int chunkPos)
    {
        return chunks.ContainsKey(chunkPos);
    }
    
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
    
    public void SetBlock(int block, Vector3Int blockPos)
    {
        Vector3Int arrayCoords = GetArrayCoordsFromCentroidCoords(blockPos);
        try
        {
            blocks[arrayCoords.x, arrayCoords.y, arrayCoords.z] = block;
        }
        catch
        {
            Debug.LogWarning("Coordinate out of bounds: " + arrayCoords);
        }
    }

    public int GetBlock(Vector3Int blockPos)
    {
        Vector3Int arrayCoords = GetArrayCoordsFromCentroidCoords(blockPos);
        return blocks[arrayCoords.x, arrayCoords.y, arrayCoords.z];
    }

    public static Vector3Int GetArrayCoordsFromCentroidCoords(Vector3Int centroidCoords)
    {
        int x = centroidCoords.x + halfExtent;
        int y = centroidCoords.y + (dims.y - 1) / 2;
        int z = centroidCoords.z + halfExtent;

        return new Vector3Int(x, y, z);
    }

    public static Vector3Int GetCentroidCoordsFromArrayCoords(Vector3Int arrayCoords)
    {
        int x = arrayCoords.x - halfExtent;
        int y = arrayCoords.y - (dims.y - 1) / 2;
        int z = arrayCoords.z - halfExtent;

        return new Vector3Int(x, y, z);
    }

    public void Generate()
    {
        GenerateBlocks();
        GenerateMesh();

        generated = true;
    }
    public void GenerateBlocks()
    {
        int xRadius = halfExtent;
        int zRadius = halfExtent;

        for (int x = -xRadius; x <= xRadius; x++)
        {
            for (int z = -zRadius; z <= zRadius; z++)
            {
                foreach (KeyValuePair<Vector3Int, int> elevationToBlock in Biome.GetBlocks(new Vector2Int(x, z), pos))
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

        int xRadius = halfExtent;
        int yRadius = (dims.y - 1) / 2;
        int zRadius = halfExtent;

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

        if (blockPos.x > -halfExtent && (GetBlock(negXblockPos) == 0 || Block.idToBlock[GetBlock(negXblockPos)] is PlanesBlock) || blockPos.x == -halfExtent)
        {
            faces.Add(CubeBlock.faces.negXFace);
        }
        if (blockPos.x < halfExtent && (GetBlock(xBlockPos) == 0 || Block.idToBlock[GetBlock(xBlockPos)] is PlanesBlock) || blockPos.x == halfExtent)
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
        if (blockPos.z > -halfExtent && (GetBlock(negZBlockPos) == 0 || Block.idToBlock[GetBlock(negZBlockPos)] is PlanesBlock) || blockPos.z == -halfExtent)
        {
            faces.Add(CubeBlock.faces.negZFace);
        }
        if (blockPos.z < halfExtent && (GetBlock(zBlockPos) == 0 || Block.idToBlock[GetBlock(zBlockPos)] is PlanesBlock) || blockPos.z == halfExtent)
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

    public static void AddToRemeshStack(Chunk chunk)
    {
        chunksToRemesh.Push(chunk);
    }

    public static void RemeshChunksInRemeshStack()
    {
        List<Chunk> chunksRemeshed = new List<Chunk>();
        
        foreach (Chunk chunk in chunksToRemesh)
        {
            if (chunk.GetChunkObj() != null && !chunksRemeshed.Contains(chunk))
            {
                chunk.GenerateMesh();

                chunk.SetMeshFromMeshData();
                chunk.SetColliderMeshFromColliderMeshData();

                chunk.chunkObj.GetComponent<MeshFilter>().mesh = chunk.GetMesh();
                chunk.chunkObj.GetComponent<MeshCollider>().sharedMesh = chunk.GetColliderMesh();

                chunksRemeshed.Add(chunk);
            }
        }
        chunksToRemesh.Clear();
    }
    
    public static Vector2Int GetChunkOffsetFromBlockPos(Vector3Int blockPos)
    {
        // Precondition: blockPos is in array coordinates.

        return new Vector2Int(
            Mathf.FloorToInt(blockPos.x / (float) dims.x),
            Mathf.FloorToInt(blockPos.z / (float) dims.x));
    }

    public Chunk(Vector2Int pos)
    {
        this.pos = pos;
        SetTargetBiome();
    }
}
