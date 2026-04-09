using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;

    public int maxEnemies = 5;
    private int currentEnemies = 0;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies) return;

        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        currentEnemies++;

        HealthEnemy health = enemy.GetComponent<HealthEnemy>();
        if (health != null)
        {
            health.onDeath.AddListener(OnEnemyDeath);
        }
    }

    void OnEnemyDeath()
    {
        currentEnemies--;
    }
}

