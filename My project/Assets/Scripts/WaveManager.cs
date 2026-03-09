using UnityEngine;
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
    [SerializeField] float zombieSpeed = 3f;
    [SerializeField] int zombieHealth = 3;

    [Header("References")]
    [SerializeField] GameObject zombiePrefab;

    int currentWave = 0;
    int zombiesAlive = 0;
    int zombiesToSpawn = 0;
    bool isSpawning = false;

    void Start()
    {
        // small delay before wave 1 starts
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;
        int zombieCount = baseZombieCount + (currentWave - 1) * zombiesPerWaveIncrease;
        zombiesToSpawn = zombieCount;
        zombiesAlive = 0;

        // update wave UI
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

            // random delay between spawns for variety
            float delay = spawnInterval + Random.Range(-0.5f, 0.5f);
            yield return new WaitForSeconds(Mathf.Max(delay, 0.3f));
        }

        isSpawning = false;
    }

    void SpawnZombie()
    {
        if (zombiePrefab == null) return;

        // spawn at random point on edge of circle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 spawnPos = new Vector3(
            Mathf.Cos(angle) * spawnRadius,
            1f, // slight offset so they dont clip into ground
            Mathf.Sin(angle) * spawnRadius
        );

        GameObject z = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);
        Zombie zombie = z.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.SetStats(zombieHealth, zombieSpeed);
        }
    }

    public void ZombieKilled()
    {
        zombiesAlive--;

        // all zombies dead and done spawning = next wave
        if (zombiesAlive <= 0 && !isSpawning)
        {
            if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
                StartCoroutine(StartNextWave());
        }
    }
}
