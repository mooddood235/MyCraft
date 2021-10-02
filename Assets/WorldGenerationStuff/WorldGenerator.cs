using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform playerTran;
    Vector2Int previousPlayerPosInChunkSpace = new Vector2Int();
    Vector2Int playerPosInChunkSpace = new Vector2Int();
    [SerializeField]
    private int viewRadius = 6;
    [SerializeField]
    private GameObject chunkPrefab;
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private Queue<GameObject> chunkObjPool = new Queue<GameObject>();

    private void Awake()
    {
        chunkPrefab.transform.localScale = Chunk.dims;
    }

    private void Update()
    {
        playerPosInChunkSpace = GetPlayerPosInChunkSpace();

        if (playerPosInChunkSpace != previousPlayerPosInChunkSpace)
        {
            GenerateWorld();
            previousPlayerPosInChunkSpace = playerPosInChunkSpace;
        }  
    }

    void GenerateWorld()
    {
        DespawnChunksOutOfRange();
        SpawnChunksInRange();
    }

    void SpawnChunksInRange()
    {
        Vector2Int movementOffset = playerPosInChunkSpace - previousPlayerPosInChunkSpace;

        if (movementOffset.x != 0)
        {
            int xOffsetSign = (int)Mathf.Sign(movementOffset.x);
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
                    if (chunks[chunkPos].GetChunkObj() == null)
                    {
                        chunks[chunkPos].SetChunkObj(SpawnChunkObj(chunkPos));
                    }
                }
            }
        }
        if (movementOffset.y != 0)
        {
            int yOffsetSign = (int)Mathf.Sign(movementOffset.y);
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

            for (int x = playerPosInChunkSpace.x - viewRadius; x <= playerPosInChunkSpace.x + viewRadius; x++)
            {
                for (int y = yStart; y <= yEnd; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, y);

                    if (!chunks.ContainsKey(chunkPos)) chunks[chunkPos] = new Chunk(chunkPos);
                    if (chunks[chunkPos].GetChunkObj() == null)
                    {
                        chunks[chunkPos].SetChunkObj(SpawnChunkObj(chunkPos));
                    }
                }
            }
        }
    }

    private Vector2Int GetPlayerPosInChunkSpace()
    {
        return Vector2Int.RoundToInt(new Vector2(playerTran.position.x / Chunk.dims.x,
                                                 playerTran.position.z / Chunk.dims.z));
    }

    private bool ChunkWithinViewRadius(Vector2Int viewRadiusVector)
    {
        return Mathf.Sqrt(viewRadiusVector.x * viewRadiusVector.x +
                          viewRadiusVector.y * viewRadiusVector.y) <= viewRadius;
    }
    private GameObject SpawnChunkObj(Vector2Int spawnPos)
    {
        GameObject chunkObj;
        Vector3 spawnPosInWorldSpace = VMath.V2ToV3(spawnPos) * Chunk.dims.x;

        if (chunkObjPool.Count == 0)
        {
            chunkObj = Instantiate(chunkPrefab, spawnPosInWorldSpace, Quaternion.identity);
        }
        else
        {
            chunkObj = chunkObjPool.Dequeue();
            chunkObj.transform.position = spawnPosInWorldSpace;
            chunkObj.SetActive(true);
        }

        return chunkObj;
    }
    private void DespawnChunksOutOfRange()
    {
        Vector2Int movementOffset = playerPosInChunkSpace - previousPlayerPosInChunkSpace;

        if (movementOffset.x != 0)
        {
            int xOffsetSign = (int)Mathf.Sign(movementOffset.x);
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

                    if (!chunks.ContainsKey(chunkPos)) chunks[chunkPos] = new Chunk(chunkPos);
                    else if (chunks[chunkPos].GetChunkObj() != null)
                    {
                        DespawnChunkObj(chunks[chunkPos].GetChunkObj());
                        chunks[chunkPos].SetChunkObj(null);
                    }
                }
            }
        }
        if (movementOffset.y != 0)
        {
            int yOffsetSign = (int)Mathf.Sign(movementOffset.y);
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

            for (int x = previousPlayerPosInChunkSpace.x - viewRadius; x <= previousPlayerPosInChunkSpace.x + viewRadius; x++)
            {
                for (int y = yStart; y <= yEnd; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, y);

                    if (!chunks.ContainsKey(chunkPos)) chunks[chunkPos] = new Chunk(chunkPos);
                    else if (chunks[chunkPos].GetChunkObj() != null)
                    {
                        DespawnChunkObj(chunks[chunkPos].GetChunkObj());
                        chunks[chunkPos].SetChunkObj(null);
                    }
                }
            }
        }
    }
    private void DespawnChunkObj(GameObject chunkObj)
    {
        chunkObj.SetActive(false);
        chunkObj.transform.position = Vector3.zero;
        chunkObjPool.Enqueue(chunkObj);
    }


}
