using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{

    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    Vector3 noTerrainPosition;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    PlayerMovement playerMovement;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist; // Debe ser superior que la longitud y ancho del tilemap.
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        // RIGHT
        if (playerMovement.moveDir.x > 0
            && playerMovement.moveDir.y == 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right").position;
                SpawnChunk();
            }
        }

        // LEFT
        else if (playerMovement.moveDir.x < 0
            && playerMovement.moveDir.y == 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left").position;
                SpawnChunk();
            }
        }

        // UP
        else if (playerMovement.moveDir.x == 0
            && playerMovement.moveDir.y > 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Up").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Up").position;
                SpawnChunk();
            }
        }

        // DOWN
        else if (playerMovement.moveDir.x == 0
            && playerMovement.moveDir.y < 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Down").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Down").position;
                SpawnChunk();
            }
        }

        // RIGHT UP
        else if (playerMovement.moveDir.x > 0
            && playerMovement.moveDir.y > 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Up").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right Up").position;
                SpawnChunk();
            }
        }

        // RIGHT DOWN
        else if (playerMovement.moveDir.x > 0
            && playerMovement.moveDir.y < 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Right Down").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Right Down").position;
                SpawnChunk();
            }
        }

        // LEFT UP
        else if (playerMovement.moveDir.x < 0
            && playerMovement.moveDir.y > 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Up").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left Up").position;
                SpawnChunk();
            }
        }

        // LEFT DOWN
        else if (playerMovement.moveDir.x < 0
            && playerMovement.moveDir.y < 0)
        {
            if (!Physics2D.OverlapCircle(currentChunk.transform.Find("Left Down").position,
                checkerRadius, terrainMask))
            {
                noTerrainPosition = currentChunk.transform.Find("Left Down").position;
                SpawnChunk();
            }
        }
    }

    void SpawnChunk()
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], noTerrainPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimizer()
    {

        optimizerCooldown -= Time.deltaTime;
        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
