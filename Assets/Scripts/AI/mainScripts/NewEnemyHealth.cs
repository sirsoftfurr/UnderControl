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

    [Header("UI")]
    public EnemySlider healthUI;

    [Header("Optional")]
    public GameObject deathEffect;

    void Start()
    {
        if (healthUI == null)
            healthUI = FindObjectOfType<EnemySlider>();
        
        healthUI.SetMaxHealth(maxHealth);
    }
    private void Awake()
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

        currentHealth =
            Mathf.Clamp(currentHealth, 0, maxHealth);

        // THIS IS THE IMPORTANT PART
        if (healthUI != null)
        {
            healthUI.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
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

        if (onDeath != null)
        {
            onDeath.Invoke();
        }

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

