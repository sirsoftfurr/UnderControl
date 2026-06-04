using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
   [System.Serializable]
    public class EnemySpawnData
    {
        [Header("Enemy")]
        public GameObject enemyPrefab;

        [Header("Spawn Weight")]
        [Range(1, 100)]
        public int spawnWeight = 10;
    }

    [Header("Enemies")]
    public List<EnemySpawnData> enemies =
        new List<EnemySpawnData>();

    [Header("Population")]
    public int maxEnemies = 10;

    public float checkInterval = 2f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Enemy Tag")]
    public string enemyTag = "Enemy";
    
    [HideInInspector]
    public int currentRound = 1;

    [HideInInspector]
    public float spawnRateMultiplier = 1f;

    private float nextCheckTime;

    private bool isSpawning = false;

    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime =
                Time.time + checkInterval;

            CheckEnemies();
        }
    }

    // ==================================================
    // CHECK ENEMIES
    // ==================================================

    void CheckEnemies()
    {
        if (isSpawning)
            return;

        GameObject[] enemiesInScene =
            GameObject.FindGameObjectsWithTag(
                enemyTag
            );

        int currentEnemyCount =
            enemiesInScene.Length;

        // Already enough enemies
        if (currentEnemyCount >= maxEnemies)
            return;

        // Spawn only ONE at a time
        StartCoroutine(SpawnEnemyRoutine());
    }

    // ==================================================
    // SPAWN ROUTINE
    // ==================================================

    IEnumerator SpawnEnemyRoutine()
    {
        isSpawning = true;

        // =====================================
        // PICK RANDOM SPAWN POINT
        // =====================================

        if (spawnPoints.Length == 0)
        {
            isSpawning = false;
            yield break;
        }

        Transform spawnPoint =
            spawnPoints[
                Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];

        // =====================================
        // OPEN DOOR
        // =====================================

        SpawnDoor door =
            spawnPoint.GetComponentInChildren<SpawnDoor>();

        if (door != null)
        {
            yield return StartCoroutine(
                door.OpenDoor()
            );
        }

        // =====================================
        // PICK RANDOM ENEMY
        // =====================================

        GameObject enemyPrefab =
            GetRandomEnemy();

        if (enemyPrefab != null)
        {
            Instantiate(
                enemyPrefab,
                spawnPoint.position,
                Quaternion.identity
            );
        }

        // Small delay
        yield return new WaitForSeconds(0.1f);

        isSpawning = false;
    }

    // ==================================================
    // WEIGHTED RANDOM
    // ==================================================

    GameObject GetRandomEnemy()
    {
        if (enemies.Count == 0)
            return null;

        int totalWeight = 0;

        // =====================================
        // CALCULATE ROUND-BASED WEIGHTS
        // =====================================

        foreach (EnemySpawnData enemy in enemies)
        {
            int modifiedWeight =
                Mathf.RoundToInt(
                    enemy.spawnWeight *
                    spawnRateMultiplier
                );

            totalWeight += modifiedWeight;
        }

        int randomValue =
            Random.Range(0, totalWeight);

        int currentWeight = 0;

        foreach (EnemySpawnData enemy in enemies)
        {
            int modifiedWeight =
                Mathf.RoundToInt(
                    enemy.spawnWeight *
                    spawnRateMultiplier
                );

            currentWeight += modifiedWeight;

            if (randomValue < currentWeight)
            {
                return enemy.enemyPrefab;
            }
        }

        return enemies[0].enemyPrefab;
    }

    // ==================================================
    // GIZMOS
    // ==================================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        if (spawnPoints == null)
            return;

        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(
                    point.position,
                    0.5f
                );
            }
        }
    }
}

