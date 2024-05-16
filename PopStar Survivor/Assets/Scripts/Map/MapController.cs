using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{

    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPosition;

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
        playerLastPosition = player.transform.position;
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

        // Posición del jugador.
        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        // Obtiene el nombre de la dirección para generar el chunk.
        string directionName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(directionName);

        // Comprueba direcciones adyacentes adicionales a la dirección del jugador
        // para generar terreno, evitando problemas con las diagonales.
        if (directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }

        if (directionName.Contains("Dpwn"))
        {
            CheckAndSpawnChunk("Down");
        }

        if (directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }

        if (directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Si el movimiento horizontal es mayor que el vertical.
            if (direction.y > 0.5f)
            {
                // Horizontal + Arriba
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.y < -0.5f)
            {
                // Horizontal + Abajo
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                // Recto horizontal
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            // Si el movimiento vertical es mayor que el horizontal.
            if (direction.x > 0.5f)
            {
                // Vertical + Derecha
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if (direction.x < -0.5f)
            {
                // Vertical + Izquierda
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                // Recto vertical
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
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
