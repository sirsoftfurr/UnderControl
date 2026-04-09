using UnityEngine;
using UnityEngine.Events;

public class HealthEnemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    // Add this event so other scripts can subscribe to death
    public UnityEvent onDeath;

    private void Awake() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        // Invoke death event
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}
