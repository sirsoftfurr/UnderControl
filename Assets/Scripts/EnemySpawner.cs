using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        [Header("Enemy")]
        public GameObject enemyPrefab;

        [Header("Spawn Chance")]
        [Range(1, 100)]
        public int spawnWeight = 10;
    }

    [Header("Spawn Settings")]
    public List<EnemySpawnData> enemies =
        new List<EnemySpawnData>();

    [Header("Population Control")]
    public int maxEnemies = 10;

    public float checkInterval = 2f;

    [Header("Spawn Area")]
    public Transform[] spawnPoints;

    [Header("Enemy Tag")]
    public string enemyTag = "Enemy";

    private float nextCheckTime;

    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            CheckEnemyCount();

            nextCheckTime =
                Time.time + checkInterval;
        }
    }

    // ==================================================
    // CHECK ENEMY COUNT
    // ==================================================

    void CheckEnemyCount()
    {
        GameObject[] currentEnemies =
            GameObject.FindGameObjectsWithTag(
                enemyTag
            );

        int aliveEnemies =
            currentEnemies.Length;

        // =========================================
        // SPAWN MISSING ENEMIES
        // =========================================

        while (aliveEnemies < maxEnemies)
        {
            SpawnEnemy();

            aliveEnemies++;
        }
    }

    // ==================================================
    // SPAWN ENEMY
    // ==================================================

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
            return;

        GameObject enemyPrefab =
            GetRandomEnemy();

        if (enemyPrefab == null)
            return;

        Transform spawnPoint =
            spawnPoints[
                Random.Range(
                    0,
                    spawnPoints.Length
                )
            ];

        Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }

    // ==================================================
    // WEIGHTED RANDOM ENEMY
    // ==================================================

    GameObject GetRandomEnemy()
    {
        if (enemies.Count == 0)
            return null;

        int totalWeight = 0;

        foreach (EnemySpawnData enemy in enemies)
        {
            totalWeight += enemy.spawnWeight;
        }

        int randomValue =
            Random.Range(0, totalWeight);

        int currentWeight = 0;

        foreach (EnemySpawnData enemy in enemies)
        {
            currentWeight += enemy.spawnWeight;

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
        if (spawnPoints == null)
            return;

        Gizmos.color = Color.green;

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

