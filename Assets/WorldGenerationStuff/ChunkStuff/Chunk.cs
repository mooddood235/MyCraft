using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Chunk
{
    /*
     * Representation Invariants:
     *      dims.x == dims.z && all dimensions must be odd.
     */
    private static Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    public static readonly Vector3Int Dims = new Vector3Int(17, 61, 17);
    private static readonly int HalfExtent = (Dims.x - 1) / 2;
    public static readonly Range HorizontalBounds = new Range(-(Dims.x - 1) / 2f, (Dims.x - 1) / 2f);
    private Vector2Int pos;
    private GameObject chunkObj;
    private int[,,] blocks = new int[Dims.x, Dims.y, Dims.z];
    public static HashSet<Chunk> chunksToRemesh;
    private MeshData meshData = new MeshData();
    private MeshData colliderMeshData = new MeshData();
    private static List<Vector3> tempMeshVerts = new List<Vector3>();
    private static List<int> tempMeshTris = new List<int>();
    private static List<Vector2> tempMeshUvs = new List<Vector2>();
    private static List<Vector3> tempColliderMeshVerts = new List<Vector3>();
    private static List<int> tempColliderMeshTris = new List<int>();
    private Mesh mesh;
    private Mesh colliderMesh;
    private static List<Vector3> tempCubeBlockVerts = new List<Vector3>();
    private static List<CubeBlock.faces> tempCubeBlockFaces = new List<CubeBlock.faces>();
    
    private bool meshGenerated;
    private bool generated;
    private Biome targetBiome;

    public static readonly Vector2 offsetToSouthWestCorner =
        new Vector2((Dims.x - 1f) / 2f + 0.5f, (Dims.z - 1f) / 2f + 0.5f);
    
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
        int x = centroidCoords.x + HalfExtent;
        int y = centroidCoords.y + (Dims.y - 1) / 2;
        int z = centroidCoords.z + HalfExtent;

        return new Vector3Int(x, y, z);
    }

    public static Vector3Int GetCentroidCoordsFromArrayCoords(Vector3Int arrayCoords)
    {
        int x = arrayCoords.x - HalfExtent;
        int y = arrayCoords.y - (Dims.y - 1) / 2;
        int z = arrayCoords.z - HalfExtent;

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
        int xRadius = HalfExtent;
        int zRadius = HalfExtent;

        for (int x = -xRadius; x <= xRadius; x++)
        {
            for (int z = -zRadius; z <= zRadius; z++)
            {
                List<KeyValuePair<Vector3Int, int>> posToBlock = Biome.GetBlocks(new Vector2Int(x, z), pos);

                foreach (KeyValuePair<Vector3Int, int> elevationToBlock in posToBlock)
                {
                    SetBlock(elevationToBlock.Value, elevationToBlock.Key);
                }
            }
        }
    }  
    public static void AddToRemeshStack(Chunk chunk)
    {
        chunksToRemesh.Add(chunk);
    }
    public void GenerateMesh()
    {
        tempMeshVerts.Clear();
        tempMeshTris.Clear();
        tempMeshUvs.Clear();
        tempColliderMeshVerts.Clear();
        tempColliderMeshTris.Clear();
        
        int xRadius = HalfExtent;
        int yRadius = (Dims.y - 1) / 2;
        int zRadius = HalfExtent;

        for (int x = -xRadius; x <= xRadius; x++)
        {
            for (int y = -yRadius; y <= yRadius; y++)
            {
               for (int z = -zRadius; z <= zRadius; z++)
               {
                    Vector3Int blockPos = new Vector3Int(x, y, z);
                    if (GetBlock(blockPos) != Block.blockNameToId["Air"])
                    {
                        int meshTrisStart = tempMeshVerts.Count;
                        
                        if (Block.idToBlock[GetBlock(blockPos)] is CubeBlock)
                        {
                            tempCubeBlockFaces.Clear();
                            tempCubeBlockVerts.Clear();
                            
                            int colliderMeshTrisStart = tempColliderMeshVerts.Count;

                            GetCubeBlockFaces(blockPos, tempCubeBlockFaces);
                            CubeBlock.GetVerts(blockPos, tempCubeBlockFaces, tempCubeBlockVerts);
                            tempMeshVerts.AddRange(tempCubeBlockVerts);
                            tempColliderMeshVerts.AddRange(tempCubeBlockVerts);

                            CubeBlock block_ = (CubeBlock)Block.idToBlock[GetBlock(blockPos)];
                            block_.GetUvs(tempCubeBlockFaces, tempMeshUvs);
                            
                            MeshData.AddTris(meshTrisStart, tempMeshVerts.Count, tempMeshTris);
                            MeshData.AddTris(colliderMeshTrisStart, tempColliderMeshVerts.Count, tempColliderMeshTris);
                        }
                        else
                        {
                            PlanesBlock.GetVerts(blockPos, tempMeshVerts);
                            PlanesBlock block_ = (PlanesBlock)Block.idToBlock[GetBlock(blockPos)];
                            block_.GetUvs(tempMeshUvs);
                            
                            MeshData.AddDoubleSidedTris(meshTrisStart, tempMeshVerts.Count, tempMeshTris);
                        }
                    }
               }
            }
        }
        meshData.vertices = tempMeshVerts.ToArray();
        meshData.triangles = tempMeshTris.ToArray();
        meshData.uv = tempMeshUvs.ToArray();

        colliderMeshData.vertices = tempColliderMeshVerts.ToArray();
        colliderMeshData.triangles = tempColliderMeshTris.ToArray();
    }

    private void SetTargetBiome()
    {
        Vector3 southWestCorner = new Vector3(-(Dims.x - 1f) / 2f - 0.5f, 0f, -(Dims.z - 1f) / 2f - 0.5f);
        targetBiome = Biome.GetBiome(pos, southWestCorner);
    }

    public Biome GetTargetBiome()
    {
        return targetBiome;
    }

    private void GetCubeBlockFaces(Vector3Int blockPos, List<CubeBlock.faces> cubeBlockFaces)
    {
        // Boolean expression logic for all if statements is the following:
        // If (the block is NOT at the side of the chunk AND (the block next to it is air or is a PlanesBlock)) OR (The block is at the side of the chunk)

        Vector3Int negXblockPos = blockPos + Vector3Int.left;
        Vector3Int xBlockPos = blockPos + Vector3Int.right;

        Vector3Int negYBlockPos = blockPos + Vector3Int.down;
        Vector3Int yBlockPos = blockPos + Vector3Int.up;

        Vector3Int negZBlockPos = blockPos + Vector3Int.back;
        Vector3Int zBlockPos = blockPos + Vector3Int.forward;

        if (blockPos.x > -HalfExtent && (GetBlock(negXblockPos) == 0 || Block.idToBlock[GetBlock(negXblockPos)] is PlanesBlock) || blockPos.x == -HalfExtent)
        {
            cubeBlockFaces.Add(CubeBlock.faces.negXFace);
        }
        if (blockPos.x < HalfExtent && (GetBlock(xBlockPos) == 0 || Block.idToBlock[GetBlock(xBlockPos)] is PlanesBlock) || blockPos.x == HalfExtent)
        {
            cubeBlockFaces.Add(CubeBlock.faces.xFace);
        }
        if (blockPos.y > -(Dims.y - 1) / 2 && (GetBlock(negYBlockPos) == 0 || Block.idToBlock[GetBlock(negYBlockPos)] is PlanesBlock) || blockPos.y == -(Dims.y - 1) / 2)
        {
            cubeBlockFaces.Add(CubeBlock.faces.negYFace);
        }
        if (blockPos.y < (Dims.y - 1) / 2 && (GetBlock(yBlockPos) == 0 || Block.idToBlock[GetBlock(yBlockPos)] is PlanesBlock) || blockPos.y == (Dims.y - 1) / 2)
        {
            cubeBlockFaces.Add(CubeBlock.faces.yFace);
        }
        if (blockPos.z > -HalfExtent && (GetBlock(negZBlockPos) == 0 || Block.idToBlock[GetBlock(negZBlockPos)] is PlanesBlock) || blockPos.z == -HalfExtent)
        {
            cubeBlockFaces.Add(CubeBlock.faces.negZFace);
        }
        if (blockPos.z < HalfExtent && (GetBlock(zBlockPos) == 0 || Block.idToBlock[GetBlock(zBlockPos)] is PlanesBlock) || blockPos.z == HalfExtent)
        {
            cubeBlockFaces.Add(CubeBlock.faces.zFace);
        }
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

    public bool IsGenerated()
    {
        return generated;
    }
    
    public static Vector2Int GetChunkOffsetFromBlockPos(Vector3Int blockPos)
    {
        // Precondition: blockPos is in array coordinates.

        return new Vector2Int(
            Mathf.FloorToInt(blockPos.x / (float) Dims.x),
            Mathf.FloorToInt(blockPos.z / (float) Dims.x));
    }

    public Chunk(Vector2Int pos)
    {
        this.pos = pos;
        SetTargetBiome();
    }
}
