using UnityEngine;

public class LatchScript : MonoBehaviour
{
   [Header("Possession Settings")]
    public float latchRange = 1.5f;
    public LayerMask enemyLayer;
    public Vector2 exitOffset = new Vector2(1f, 0f);

    [Header("Player Visuals")]
    public SpriteRenderer[] spritesToHide;

    [Header("Possession Cooldown")]
    public float possessCooldown = 0.2f;

    [Header("Invisible Layer")]
    public string invisibleLayerName = "InvisiblePlayer";

    private int originalLayer;
    private GameObject possessedEnemy = null;
    private float lastPossessTime = -10f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private void Start()
    {
        originalLayer = gameObject.layer;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.time > lastPossessTime + possessCooldown)
        {
            if (possessedEnemy == null)
                TryPossess();
            else
                ReleasePossession();

            lastPossessTime = Time.time;
        }
    }

    private void TryPossess()
    {
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, latchRange, enemyLayer);
        if (enemyCollider == null) return;

        GameObject enemy = enemyCollider.gameObject;
        possessedEnemy = enemy;

        // 🔴 Disable AI movement
        PlatformerAI ai = enemy.GetComponent<PlatformerAI>();
        if (ai != null) ai.enabled = false;

        // 🔴 HARD disable AI shooting
        EnemyShooter aiShoot = enemy.GetComponent<EnemyShooter>();
        if (aiShoot != null)
        {
            aiShoot.SetShootingEnabled(false);
            aiShoot.enabled = false;
        }

        // 🟢 Enable player control
        PlayerMovement pm = enemy.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;

        PlayerShooting ps = enemy.GetComponent<PlayerShooting>();
        if (ps != null)
        {
            ps.enabled = true;
            ps.SetShootingEnabled(true);
            ps.ResetCooldown();
        }

        // Move player into enemy
        transform.position = enemy.transform.position;
        transform.SetParent(enemy.transform);

        // Hide player visuals
        foreach (var sr in spritesToHide)
            sr.enabled = false;

        // Disable player physics
        if (playerCollider != null)
            playerCollider.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Change player layer
        int layer = LayerMask.NameToLayer(invisibleLayerName);
        if (layer != -1)
            gameObject.layer = layer;

        // Subscribe to death
        NewEnemyHealth enemyHealth = enemy.GetComponent<NewEnemyHealth>();
        if (enemyHealth != null)
            enemyHealth.onDeath.AddListener(OnPossessedEnemyDeath);

        // Redirect all AI to possessed enemy
        foreach (var otherAI in FindObjectsOfType<PlatformerAI>())
        {
            if (otherAI.target == transform)
                otherAI.target = enemy.transform;
        }
        
        EnemyUI ui = enemy.GetComponent<EnemyUI>();
        if (ui != null)
            ui.SetHealthBarVisible(true);
    }

    private void ReleasePossession()
    {
        EnemyUI ui = possessedEnemy.GetComponent<EnemyUI>();
        if (ui != null)
            
            ui.SetHealthBarVisible(false);
        if (possessedEnemy == null) return;

        // Unsubscribe death event
        NewEnemyHealth enemyHealth = possessedEnemy.GetComponent<NewEnemyHealth>();
        if (enemyHealth != null)
            enemyHealth.onDeath.RemoveListener(OnPossessedEnemyDeath);

        // Restore AI movement
        PlatformerAI ai = possessedEnemy.GetComponent<PlatformerAI>();
        if (ai != null) ai.enabled = true;

        // 🔴 Restore AI shooting
        EnemyShooter aiShoot = possessedEnemy.GetComponent<EnemyShooter>();
        if (aiShoot != null)
        {
            aiShoot.enabled = true;
            aiShoot.SetShootingEnabled(true); // ✅ FIXED (was false before)
        }

        // Disable player control
        PlayerMovement pm = possessedEnemy.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        PlayerShooting ps = possessedEnemy.GetComponent<PlayerShooting>();
        if (ps != null)
        {
            ps.SetShootingEnabled(false);
            ps.enabled = false;
        }

        // Unparent player
        transform.SetParent(null);

        // Move player next to enemy
        Vector3 exitPos = possessedEnemy.transform.position + (Vector3)exitOffset;
        exitPos.z = 0f;
        transform.position = exitPos;

        // Restore visuals
        foreach (var sr in spritesToHide)
            sr.enabled = true;

        // Restore physics
        if (playerCollider != null)
            playerCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        // Restore layer
        gameObject.layer = originalLayer;

        // Redirect AI back to player
        foreach (var otherAI in FindObjectsOfType<PlatformerAI>())
        {
            if (otherAI.target == possessedEnemy.transform)
                otherAI.target = transform;
        }

        possessedEnemy = null;
    }

    private void OnPossessedEnemyDeath()
    {
        EnemyUI ui = possessedEnemy.GetComponent<EnemyUI>();
        if (ui != null)
            ui.SetHealthBarVisible(false);
        
        if (possessedEnemy == null) return;

        transform.SetParent(null);
        transform.position = possessedEnemy.transform.position;

        foreach (var sr in spritesToHide)
            sr.enabled = true;

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        gameObject.layer = originalLayer;

        foreach (var otherAI in FindObjectsOfType<PlatformerAI>())
        {
            if (otherAI.target == possessedEnemy.transform)
                otherAI.target = transform;
        }

        possessedEnemy = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, latchRange);
    }
}
