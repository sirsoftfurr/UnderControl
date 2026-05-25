using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewEnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    private int currentHealth;

    [Header("Death")]
    public UnityEvent onDeath;

    private bool isDead = false;

    [Header("Optional")]
    public GameObject deathEffect;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // ==================================================
    // DAMAGE
    // ==================================================

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        // Clamp health
        currentHealth =
            Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(
            gameObject.name +
            " took damage: " +
            damage +
            " | HP: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ==================================================
    // HEAL
    // ==================================================

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        currentHealth =
            Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    // ==================================================
    // DIE
    // ==================================================

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(gameObject.name + " died");

        // Invoke death event
        if (onDeath != null)
        {
            onDeath.Invoke();
        }

        // Optional effect
        if (deathEffect != null)
        {
            Instantiate(
                deathEffect,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }

    // ==================================================
    // GETTERS
    // ==================================================

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}

