using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
     [Header("Health")]
    public int maxHealth = 100;

    public int currentHealth;

    public HealthBar healthBar;

    [Header("Events")]
    public UnityEvent onDeath = new UnityEvent();

    private bool isDead = false;

    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        if (healthBar == null)
        {
            healthBar =
                FindObjectOfType<HealthBar>();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(
                maxHealth
            );
        }
    }

    // ==================================================
    // DAMAGE
    // ==================================================

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth
            );

        // Update UI
        if (healthBar != null)
        {
            healthBar.SetHealth(
                currentHealth
            );
        }

        // Die
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ==================================================
    // DIE
    // ==================================================

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        onDeath?.Invoke();

        // =========================================
        // PLAYER DEATH
        // =========================================

        if (CompareTag("Player"))
        {
            DeathScreen deathScreen =
                FindObjectOfType<DeathScreen>();

            RoundManager roundManager =
                FindObjectOfType<RoundManager>();

            int round =
                roundManager != null
                    ? roundManager.currentRound
                    : 1;

            if (deathScreen != null)
            {
                deathScreen.ShowDeathScreen(
                    round
                );
            }

            // Disable player object
            gameObject.SetActive(false);
        }

        // =========================================
        // ENEMY DEATH
        // =========================================

        else
        {
            Destroy(gameObject);
        }
    }
}
