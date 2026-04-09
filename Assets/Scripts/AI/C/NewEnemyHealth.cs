using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewEnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public EnemySlider enemyBar;

    public UnityEvent onDeath;

    void Start()
    {
        currentHealth = maxHealth;
        enemyBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        enemyBar.SetHealth(currentHealth);
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        foreach (var ai in FindObjectsOfType<PlatformerAI>())
        {
            if (ai.target == transform)
            {
                ai.target = null;
            }
        }

        onDeath?.Invoke();
        Destroy(gameObject);
    }
}

