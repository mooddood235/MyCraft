using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class WorldGenerator2 : MonoBehaviour
{
    [SerializeField]
    private int viewRadius = 6;
    [SerializeField]
    private Transform playerTran;
    private Vector2Int previousPlayerPosInChunkSpace = new Vector2Int();
    private Vector2Int playerPosInChunkSpace = new Vector2Int();
    [SerializeField]
    private GameObject chunkObjPrefab;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private List<Chunk> chunksToDespawn = new List<Chunk>();
    private List<Chunk> chunksToSpawn = new List<Chunk>();
    private Queue<GameObject> chunkObjPool = new Queue<GameObject>();
    [SerializeField]
    private float timeBetweenChunkSpawning = 0.05f;
    private bool despawning = false;
    private bool spawning = false;
    Thread generatorThread;
    private bool generating = true;

    private void Start()
    {
        SpawnFirstChunks();    
    }


    private void Update()
    {
        playerPosInChunkSpace = GetPlayerPosInChunkSpace();  
        
        if (!despawning && !spawning && playerPosInChunkSpace != previousPlayerPosInChunkSpace)
        {
            chunksToDespawn = new List<Chunk>();
            chunksToSpawn = new List<Chunk>();

            despawning = true;
            spawning = true;

            GetChunksToDespawn();
            GetChunksToSpawn();

            generatorThread = new Thread(GenerateChunks);
            generatorThread.Start();

            previousPlayerPosInChunkSpace = playerPosInChunkSpace;
        }
        if (!generating)
        {
            generating = true;

            StartCoroutine(DespawnChunks());
            StartCoroutine(SpawnChunks());
        }
    }

    private void SpawnFirstChunks()
    {
        for (int x = -viewRadius; x <= viewRadius; x++)
        {
            for (int y = -viewRadius; y <= viewRadius; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                chunks[chunkPos] = new Chunk(chunkPos);
                chunks[chunkPos].Generate(chunks);
                SpawnChunk(chunks[chunkPos]);
            }
        }
    }

    private void GenerateChunks()
    {
        foreach (Chunk chunk in chunksToSpawn)
        {
            if (!chunk.isGenerated())
            {
                chunk.Generate(chunks);
            }
        }
        generating = false;
    }


    private Vector2Int GetPlayerPosInChunkSpace()
    {
        return Vector2Int.RoundToInt(new Vector2(playerTran.position.x / Chunk.dims.x,
                                                 playerTran.position.z / Chunk.dims.z));
    }

    private IEnumerator SpawnChunks()
    {
        foreach (Chunk chunk in chunksToSpawn)
        {
            SpawnChunk(chunk);
            yield return new WaitForSeconds(timeBetweenChunkSpawning);
        }
        spawning = false;
    }

    private void SpawnChunk(Chunk chunk)
    {
        GameObject chunkObj;
        Vector3 spawnPosInWorldSpace = VMath.V2ToV3(chunk.GetPos()) * Chunk.dims.x - new Vector3(0f, Chunk.dims.y / 2f, 0f);

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

        if (!chunk.meshIsGenerated()) chunk.SetMeshFromMeshData();

        chunkObj.GetComponent<MeshFilter>().mesh = chunk.GetMesh();
        chunkObj.GetComponent<MeshCollider>().sharedMesh = chunk.GetMesh();
        chunk.SetChunkObj(chunkObj);
    }
    
    private IEnumerator DespawnChunks()
    {
        foreach (Chunk chunk in chunksToDespawn)
        {
            DespawnChunk(chunk);
            yield return new WaitForSeconds(timeBetweenChunkSpawning);
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

                    if (!chunks.ContainsKey(chunkPos)) chunks[chunkPos] = new Chunk(chunkPos);

                    chunksToSpawn.Add(chunks[chunkPos]);
                    
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

                    if (!chunks.ContainsKey(chunkPos)) chunks[chunkPos] = new Chunk(chunkPos);

                    chunksToSpawn.Add(chunks[chunkPos]);

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

                    chunksToDespawn.Add(chunks[chunkPos]);
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
                    chunksToDespawn.Add(chunks[chunkPos]);               
                }
            }
        }
    }


}
