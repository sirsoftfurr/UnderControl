using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;
    public EnemyBrain brain;
    public Raycaster bloodManager;

    public void TakeHit(Vector2 hitPoint, int damage)
    {
        health -= damage;

        if (bloodManager != null)
            bloodManager.SpawnBlood(hitPoint, false);

        if (brain != null)
            brain.OnHit();

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}


