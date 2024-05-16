using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;

        public List<EnemyGroup> enemyGroups; // La lista de grupos de enemigos que aparecer�n en esta ronda.

        public int waveQuota; // N�mero total de enemigos para la ronda.
        public float spawnInterval; // Intervalo de aparici�n de enemigos.
        public float spawnCount; // N�mero de enemigos que ya existen en esta ronda.
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; // N�mero de enemigos que spawnean en esta ronda de este grupo.
        public int spawnCount; // N�mero de enemigos de este grupo que ya existen en esta ronda.
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; // Lista de rondas en la partida.
    public int currentWaveCount; // �ndice de la ronda actual (0 ~ N).

    [Header("Spawner Attributes")]
    float spawnTimer; // Temporizador para determinar cu�ndo spawnear el siguiente enemigo
    public int enemiesAlive;
    public int maxEnemiesAllowed; // M�ximo de enemigos permitidos al mismo tiempo.
    public bool maxEnemiesReached = false;
    public float waveInterval; //Intervalo entre rondas.
    bool isWaveActive = false;


    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; // Lista de puntos de aparici�n de enemigos relativos al jugador.

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        // Comprueba si se ha terminado la ronda y deber�a comenzar la siguiente.
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        // La ronda durar� su intervalo antes de comenzar la siguiente.
        yield return new WaitForSeconds(waveInterval);

        // Si existen m�s rondas tras la actual, comenzar la siguiente.
        if (currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;

        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;

        // Debug.LogWarning(currentWaveQuota);
    }

    /// <summary>
    /// Este m�todo para de generar enemigos al alcanzar el m�ximo posible.
    /// Solo generar� nuevos enemigos cuando llegue la siguiente ronda.
    /// </summary>

    void SpawnEnemies()
    {

        // Comprueba si se ha generado el n�mero m�nimo de enemigos de la ronda.
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {

            // Genera enemigos de cada tipo hasta llenar el m�ximo.
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {

                // Comprueba si han aparecido el m�nimo de enemigos de este tipo.
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {

                    // Genera enemigos en posiciones aleatorias relativas al jugador
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    // L�mite a los enemigos que pueden aparecer.
                    if (enemiesAlive >= enemyGroup.enemyCount)
                    {

                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }

    }
}
