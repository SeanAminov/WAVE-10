using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] int baseZombieCount = 5;
    [SerializeField] int zombiesPerWaveIncrease = 3;
    [SerializeField] float timeBetweenWaves = 3f;

    [Header("Spawn Settings")]
    [SerializeField] float spawnInterval = 1.5f;
    [SerializeField] float spawnRadius = 30f;

    [Header("Zombie Defaults")]
    [SerializeField] float baseZombieSpeed = 1.2f;
    [SerializeField] float speedIncreasePerWave = 0.3f;
    [SerializeField] float maxZombieSpeed = 4f;
    [SerializeField] int zombieHealth = 3;

    [Header("References")]
    [SerializeField] GameObject[] zombiePrefabs;

    int currentWave = 0;
    int zombiesAlive = 0;
    int zombiesToSpawn = 0;
    bool isSpawning = false;

    void Start()
    {
        if (zombiePrefabs == null || zombiePrefabs.Length == 0)
        {
            Debug.LogError("WaveManager: No zombie prefabs assigned!");
            return;
        }

        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        int zombieCount = baseZombieCount + (currentWave - 1) * zombiesPerWaveIncrease;
        zombiesToSpawn = zombieCount;
        zombiesAlive = 0;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateWave(currentWave);

        if (GameManager.Instance != null)
            GameManager.Instance.currentWave = currentWave;

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        while (zombiesToSpawn > 0)
        {
            SpawnZombie();
            zombiesToSpawn--;
            zombiesAlive++;

            float delay = spawnInterval + Random.Range(-0.5f, 0.5f);
            yield return new WaitForSeconds(Mathf.Max(delay, 0.3f));
        }

        isSpawning = false;
    }

    void SpawnZombie()
    {
        if (zombiePrefabs == null || zombiePrefabs.Length == 0) return;

        GameObject prefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];
        if (prefab == null) return;

        Vector3 spawnPos = GetSpawnPoint();

        GameObject z = Instantiate(prefab, spawnPos, Quaternion.identity);
        Zombie zombie = z.GetComponent<Zombie>();
        if (zombie != null)
        {
            float waveSpeed = Mathf.Min(baseZombieSpeed + (currentWave - 1) * speedIncreasePerWave, maxZombieSpeed);
            zombie.SetStats(zombieHealth, waveSpeed);
        }
    }

    Vector3 GetSpawnPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 candidate = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                1f,
                Mathf.Sin(angle) * spawnRadius
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 5f, NavMesh.AllAreas))
                return hit.position;
        }

        // if no valid spot found, try center area
        NavMeshHit centerHit;
        if (NavMesh.SamplePosition(Vector3.zero, out centerHit, spawnRadius, NavMesh.AllAreas))
            return centerHit.position;

        return new Vector3(spawnRadius, 0f, 0f);
    }

    public void ZombieKilled()
    {
        zombiesAlive--;

        if (zombiesAlive <= 0 && !isSpawning)
        {
            if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
                StartCoroutine(StartNextWave());
        }
    }
}
