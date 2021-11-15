using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


public class WorldGenerator2 : MonoBehaviour
{
    [SerializeField]
    private int viewRadius = 6;
    [SerializeField]
    private Transform playerTran;
    private Vector2Int previousPlayerPosInChunkSpace;
    private Vector2Int playerPosInChunkSpace;
    [SerializeField]
    private GameObject chunkObjPrefab;
    private List<Chunk> chunksToDespawn = new List<Chunk>();
    private List<Chunk> chunksToSpawn = new List<Chunk>();
    private HashSet<Chunk> chunksToRemesh = new HashSet<Chunk>();
    private Queue<GameObject> chunkObjPool = new Queue<GameObject>();
    [SerializeField]
    private float timeBetweenChunkSpawning = 1f;
    private WaitForSeconds coroutineWaitTime;
    private bool despawning;
    private bool spawning;
    private bool remeshing;
    private Thread generatorThread;
    private bool generating = true;
    
    private void Awake()
    {
        Chunk.chunksToRemesh = chunksToRemesh;
        coroutineWaitTime = new WaitForSeconds(timeBetweenChunkSpawning);
    }

    private void Start()
    {
        SpawnFirstChunks();    
    }


    private void Update()
    {
        playerPosInChunkSpace = GetPlayerPosInChunkSpace();  
        
        if (!despawning && !spawning && !remeshing && playerPosInChunkSpace != previousPlayerPosInChunkSpace)
        {
            chunksToDespawn = new List<Chunk>();
            chunksToSpawn = new List<Chunk>();

            despawning = true;
            spawning = true;
            remeshing = true;

            GetChunksToDespawn();
            GetChunksToSpawn();

            generatorThread = new Thread(GenerateChunks);
            generatorThread.Start();
            // GenerateChunks();

            previousPlayerPosInChunkSpace = playerPosInChunkSpace;
        }
        if (!generating)
        {
            generating = true;
            
            StartCoroutine(DespawnChunks());
            StartCoroutine(SpawnChunks());
            StartCoroutine(CoroutineApplyMeshesToChunksInRemeshStack());
        }
    }


    private void SpawnFirstChunks()
    {
        for (int x = -viewRadius; x <= viewRadius; x++)
        {
            for (int y = -viewRadius; y <= viewRadius; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                Chunk.GetChunk(chunkPos).Generate();
                SpawnChunk(Chunk.GetChunk(chunkPos));
            }
        }
        RegenerateMeshesInRemeshStack();
        ApplyMeshesToChunksInRemeshStack();
    }
    private void RegenerateMeshesInRemeshStack()
    {
        foreach (Chunk chunk in chunksToRemesh)
        {
            chunk.GenerateMesh();
        }
    }
    private void ApplyMeshesToChunksInRemeshStack()
    {
        foreach (Chunk chunk in chunksToRemesh)
        {
            ApplyMeshToChunk(chunk);
        }
        chunksToRemesh.Clear();
    }

    private IEnumerator CoroutineApplyMeshesToChunksInRemeshStack()
    {
        foreach (Chunk chunk in chunksToRemesh)
        {
            ApplyMeshToChunk(chunk);
            yield return coroutineWaitTime;
        }

        remeshing = false;
        chunksToRemesh.Clear();
    }
    private void ApplyMeshToChunk(Chunk chunk)
    {
        chunk.SetMeshFromMeshData();
        chunk.SetColliderMeshFromColliderMeshData();

        chunk.GetChunkObj().GetComponent<MeshFilter>().mesh = chunk.GetMesh();
        chunk.GetChunkObj().GetComponent<MeshCollider>().sharedMesh = chunk.GetColliderMesh();
    }
    
    private void GenerateChunks()
    {
        foreach (Chunk chunk in chunksToSpawn)
        {
            if (!chunk.IsGenerated())
            {
                chunk.Generate();
            }
        }
        RegenerateMeshesInRemeshStack();
        generating = false;
    }

    private Vector2Int GetPlayerPosInChunkSpace()
    {
        return Vector2Int.RoundToInt(new Vector2(playerTran.position.x / Chunk.Dims.x,
                                                 playerTran.position.z / Chunk.Dims.z));
    }

    private IEnumerator SpawnChunks()
    {
        foreach (Chunk chunk in chunksToSpawn)
        {
            SpawnChunk(chunk);
            yield return coroutineWaitTime;
        }
        spawning = false;
    }

    private void SpawnChunk(Chunk chunk)
    {
        GameObject chunkObj;
        Vector3 spawnPosInWorldSpace = VMath.V2ToV3(chunk.GetPos()) * Chunk.Dims.x - new Vector3(0f, Chunk.Dims.y / 2f, 0f);

        if (chunkObjPool.Count == 0)
        {
            chunkObj = Instantiate(chunkObjPrefab, spawnPosInWorldSpace, Quaternion.identity);
        }
        else
        {
            chunkObj = chunkObjPool.Dequeue();
            chunkObj.transform.position = spawnPosInWorldSpace;
            chunkObj.SetActive(true);
        }

        if (!chunk.MeshIsGenerated())
        {
            chunk.SetMeshFromMeshData();
            chunk.SetColliderMeshFromColliderMeshData();
        }

        chunkObj.GetComponent<MeshFilter>().mesh = chunk.GetMesh();
        chunkObj.GetComponent<MeshCollider>().sharedMesh = chunk.GetColliderMesh();
        chunk.SetChunkObj(chunkObj);
    }
    
    private IEnumerator DespawnChunks()
    {
        foreach (Chunk chunk in chunksToDespawn)
        {
            DespawnChunk(chunk);
            yield return coroutineWaitTime;
        }
        despawning = false;
    }

    private void DespawnChunk(Chunk chunk)
    {   
        GameObject chunkObj = chunk.GetChunkObj();
        chunkObj.SetActive(false);
        chunkObj.transform.position = new Vector3();
        chunkObj.GetComponent<MeshFilter>().mesh = new Mesh();
        chunkObj.GetComponent<MeshCollider>().sharedMesh = null;
        chunkObjPool.Enqueue(chunkObj);
        chunk.SetChunkObj(null);     
    }
    
    private void GetChunksToSpawn()
    {
        Vector2Int movementOffset = playerPosInChunkSpace - previousPlayerPosInChunkSpace;

        int xOffsetSign = (int)Mathf.Sign(movementOffset.x);
        int yOffsetSign = (int)Mathf.Sign(movementOffset.y);

        if (movementOffset.x != 0)
        {
            Vector2Int corner1 = previousPlayerPosInChunkSpace + new Vector2Int(xOffsetSign * viewRadius, -viewRadius) + new Vector2Int(xOffsetSign, 0); ;
            Vector2Int corner2 = playerPosInChunkSpace + new Vector2Int(xOffsetSign * viewRadius, viewRadius);

            int xStart;
            int xEnd;

            if (movementOffset.x < 0)
            {
                if (corner1.x > playerPosInChunkSpace.x + viewRadius)
                {
                    corner1.x = playerPosInChunkSpace.x + viewRadius;
                }
                xStart = corner2.x;
                xEnd = corner1.x;
            }
            else
            {
                if (corner1.x < playerPosInChunkSpace.x - viewRadius)
                {
                    corner1.x = playerPosInChunkSpace.x - viewRadius;
                }
                xStart = corner1.x;
                xEnd = corner2.x;
            }

            for (int x = xStart; x <= xEnd; x++)
            {
                for (int y = playerPosInChunkSpace.y - viewRadius; y <= playerPosInChunkSpace.y + viewRadius; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, y);
                    
                    chunksToSpawn.Add(Chunk.GetChunk(chunkPos));
                }
            }
        }
        if (movementOffset.y != 0)
        {
            Vector2Int corner1 = previousPlayerPosInChunkSpace + new Vector2Int(-viewRadius, yOffsetSign * viewRadius) + new Vector2Int(0, yOffsetSign);
            Vector2Int corner2 = playerPosInChunkSpace + new Vector2Int(viewRadius, yOffsetSign * viewRadius);

            int yStart;
            int yEnd;

            if (movementOffset.y < 0)
            {
                if (corner1.y > playerPosInChunkSpace.y + viewRadius)
                {
                    corner1.y = playerPosInChunkSpace.y + viewRadius;
                }
                yStart = corner2.y;
                yEnd = corner1.y;
            }
            else
            {
                if (corner1.y < playerPosInChunkSpace.y - viewRadius)
                {
                    corner1.y = playerPosInChunkSpace.y - viewRadius;
                }
                yStart = corner1.y;
                yEnd = corner2.y;
            }


            int xStart = playerPosInChunkSpace.x - viewRadius;
            int xEnd = playerPosInChunkSpace.x + viewRadius;

            if (movementOffset.x != 0)
            {
                if (xOffsetSign < 0) xStart -= movementOffset.x;
                else xEnd -= movementOffset.x;
            }

            for (int x = xStart; x <= xEnd; x++)
            {
                for (int y = yStart; y <= yEnd; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, y);

                    chunksToSpawn.Add(Chunk.GetChunk(chunkPos));
                }
            }
        }
    }

    private void GetChunksToDespawn()
    {
        Vector2Int movementOffset = playerPosInChunkSpace - previousPlayerPosInChunkSpace;

        int xOffsetSign = (int)Mathf.Sign(movementOffset.x);
        int yOffsetSign = (int)Mathf.Sign(movementOffset.y);

        if (movementOffset.x != 0)
        {
            Vector2Int corner1 = previousPlayerPosInChunkSpace + new Vector2Int(-xOffsetSign * viewRadius, -viewRadius);
            Vector2Int corner2 = playerPosInChunkSpace + new Vector2Int(-xOffsetSign * viewRadius, viewRadius) + new Vector2Int(-xOffsetSign, 0);

            int xStart;
            int xEnd;

            if (movementOffset.x < 0)
            {
                if (corner2.x < previousPlayerPosInChunkSpace.x - viewRadius)
                {
                    corner2.x = previousPlayerPosInChunkSpace.x - viewRadius;
                }
                xStart = corner2.x;
                xEnd = corner1.x;
            }
            else
            {
                if (corner2.x > previousPlayerPosInChunkSpace.x + viewRadius)
                {
                    corner2.x = previousPlayerPosInChunkSpace.x + viewRadius;
                }
                xStart = corner1.x;
                xEnd = corner2.x;
            }

            for (int x = xStart; x <= xEnd; x++)
            {
                for (int y = previousPlayerPosInChunkSpace.y - viewRadius; y <= previousPlayerPosInChunkSpace.y + viewRadius; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, y);

                    chunksToDespawn.Add(Chunk.GetChunk(chunkPos));
                }
            }
        }
        if (movementOffset.y != 0)
        {
            Vector2Int corner1 = previousPlayerPosInChunkSpace + new Vector2Int(-viewRadius, -yOffsetSign * viewRadius);
            Vector2Int corner2 = playerPosInChunkSpace + new Vector2Int(viewRadius, -yOffsetSign * viewRadius) + new Vector2Int(0, -yOffsetSign);

            int yStart;
            int yEnd;

            if (movementOffset.y < 0)
            {
                if (corner2.y < previousPlayerPosInChunkSpace.y - viewRadius)
                {
                    corner2.y = previousPlayerPosInChunkSpace.y - viewRadius;
                }
                yStart = corner2.y;
                yEnd = corner1.y;
            }
            else
            {
                if (corner2.y > previousPlayerPosInChunkSpace.y + viewRadius)
                {
                    corner2.y = previousPlayerPosInChunkSpace.y + viewRadius;
                }
                yStart = corner1.y;
                yEnd = corner2.y;
            }

            int xStart = previousPlayerPosInChunkSpace.x - viewRadius;
            int xEnd = previousPlayerPosInChunkSpace.x + viewRadius;

            if (movementOffset.x != 0)
            {
                if (xOffsetSign < 0) xEnd += movementOffset.x;
                else xStart += movementOffset.x;
            }

            for (int x = xStart; x <= xEnd; x++)
            {
                for (int y = yStart; y <= yEnd; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, y);
                    chunksToDespawn.Add(Chunk.GetChunk(chunkPos));               
                }
            }
        }
    }


}
