using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    
    public UnityEvent onDeath = new UnityEvent();

    private bool isDead = false;

    void Start()
    {
        if (healthBar == null)
            healthBar = FindObjectOfType<HealthBar>();
        
        healthBar.SetMaxHealth(maxHealth);
    }
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        onDeath?.Invoke();

        if (CompareTag("Player"))
        {
            // Reload level if player dies
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Enemy just gets destroyed
            Destroy(gameObject);
        }
    }
    
}
