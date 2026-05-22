using UnityEngine;

public class LatchScript : MonoBehaviour
{
    [Header("Possession Settings")]
    public float latchRange = 1.5f;
    public LayerMask enemyLayer;
    public Vector2 exitOffset = new Vector2(1f, 0f);

    [Header("Player Visuals")]
    // Drag ONLY the graphics object here
    public GameObject visualsToHide;

    [Header("Possession Cooldown")]
    public float possessCooldown = 0.2f;

    [Header("Invisible Layer")]
    public string invisibleLayerName = "InvisiblePlayer";

    private int originalLayer;

    private GameObject possessedEnemy = null;
    private float lastPossessTime = -10f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    void Start()
    {
        originalLayer = gameObject.layer;

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) &&
            Time.time > lastPossessTime + possessCooldown)
        {
            if (possessedEnemy == null)
            {
                TryPossess();
            }
            else
            {
                ReleasePossession();
            }

            lastPossessTime = Time.time;
        }
    }

    // ==================================================
    // POSSESS
    // ==================================================

    void TryPossess()
    {
        Collider2D enemyCollider =
            Physics2D.OverlapCircle(
                transform.position,
                latchRange,
                enemyLayer
            );

        if (enemyCollider == null)
            return;

        // ✅ Gets root enemy object
        GameObject enemy =
            enemyCollider.transform.root.gameObject;

        possessedEnemy = enemy;

        // =========================================
        // DISABLE ENEMY AI
        // =========================================

        PlatformerEnemyAI ai =
            enemy.GetComponent<PlatformerEnemyAI>();

        if (ai != null)
            ai.enabled = false;

        EnemyShooter enemyShooter =
            enemy.GetComponent<EnemyShooter>();

        if (enemyShooter != null)
        {
            enemyShooter.SetShootingEnabled(false);
            enemyShooter.enabled = false;
        }

        // =========================================
        // ENABLE PLAYER CONTROL
        // =========================================

        PlayerMovement playerMovement =
            enemy.GetComponent<PlayerMovement>();

        if (playerMovement != null)
            playerMovement.enabled = true;

        PlayerShooting playerShooting =
            enemy.GetComponent<PlayerShooting>();

        if (playerShooting != null)
        {
            playerShooting.enabled = true;
            playerShooting.SetShootingEnabled(true);
            playerShooting.ResetCooldown();
        }

        // =========================================
        // MOVE PLAYER INTO ENEMY
        // =========================================

        transform.position = enemy.transform.position;

        transform.SetParent(enemy.transform);

        // =========================================
        // HIDE PLAYER VISUALS
        // =========================================

        SetVisualsVisible(false);

        // =========================================
        // DISABLE PLAYER PHYSICS
        // =========================================

        if (playerCollider != null)
            playerCollider.enabled = false;

        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
        }

        // =========================================
        // CHANGE PLAYER LAYER
        // =========================================

        int invisibleLayer =
            LayerMask.NameToLayer(invisibleLayerName);

        if (invisibleLayer != -1)
            gameObject.layer = invisibleLayer;

        // =========================================
        // SUBSCRIBE TO DEATH EVENT
        // =========================================

        NewEnemyHealth enemyHealth =
            enemy.GetComponent<NewEnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.onDeath.AddListener(
                OnPossessedEnemyDeath
            );
        }

        // =========================================
        // REDIRECT ENEMY AI TARGETS
        // =========================================

        foreach (PlatformerEnemyAI otherAI in
                 FindObjectsOfType<PlatformerEnemyAI>())
        {
            if (otherAI.player == transform)
            {
                otherAI.player = enemy.transform;
            }
        }

        // =========================================
        // SHOW POSSESSED ENEMY UI
        // =========================================

        EnemyUI ui =
            enemy.GetComponentInChildren<EnemyUI>(true);

        if (ui != null)
            ui.SetHealthBarVisible(true);

        Debug.Log("Possessed: " + enemy.name);
    }

    // ==================================================
    // RELEASE
    // ==================================================

    void ReleasePossession()
    {
        if (possessedEnemy == null)
            return;

        EnemyUI ui =
            possessedEnemy.GetComponentInChildren<EnemyUI>(true);

        if (ui != null)
            ui.SetHealthBarVisible(false);

        // =========================================
        // REMOVE DEATH EVENT
        // =========================================

        NewEnemyHealth enemyHealth =
            possessedEnemy.GetComponent<NewEnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.onDeath.RemoveListener(
                OnPossessedEnemyDeath
            );
        }

        // =========================================
        // RE-ENABLE AI
        // =========================================

        PlatformerEnemyAI ai =
            possessedEnemy.GetComponent<PlatformerEnemyAI>();

        if (ai != null)
            ai.enabled = true;

        EnemyShooter enemyShooter =
            possessedEnemy.GetComponent<EnemyShooter>();

        if (enemyShooter != null)
        {
            enemyShooter.enabled = true;
            enemyShooter.SetShootingEnabled(true);
        }

        // =========================================
        // DISABLE PLAYER CONTROL
        // =========================================

        PlayerMovement playerMovement =
            possessedEnemy.GetComponent<PlayerMovement>();

        if (playerMovement != null)
            playerMovement.enabled = false;

        PlayerShooting playerShooting =
            possessedEnemy.GetComponent<PlayerShooting>();

        if (playerShooting != null)
        {
            playerShooting.SetShootingEnabled(false);
            playerShooting.enabled = false;
        }

        // =========================================
        // DETACH PLAYER
        // =========================================

        transform.SetParent(null);

        Vector3 exitPosition =
            possessedEnemy.transform.position +
            (Vector3)exitOffset;

        exitPosition.z = 0f;

        transform.position = exitPosition;

        // =========================================
        // RESTORE PLAYER VISUALS
        // =========================================

        SetVisualsVisible(true);

        // =========================================
        // RESTORE PLAYER PHYSICS
        // =========================================

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        // =========================================
        // RESTORE PLAYER LAYER
        // =========================================

        gameObject.layer = originalLayer;

        // =========================================
        // RESTORE AI TARGETS
        // =========================================

        foreach (PlatformerEnemyAI otherAI in
                 FindObjectsOfType<PlatformerEnemyAI>())
        {
            if (otherAI.player == possessedEnemy.transform)
            {
                otherAI.player = transform;
            }
        }

        Debug.Log("Released: " + possessedEnemy.name);

        possessedEnemy = null;
    }

    // ==================================================
    // POSSESSED ENEMY DIED
    // ==================================================

    void OnPossessedEnemyDeath()
    {
        if (possessedEnemy == null)
            return;

        EnemyUI ui =
            possessedEnemy.GetComponentInChildren<EnemyUI>(true);

        if (ui != null)
            ui.SetHealthBarVisible(false);

        transform.SetParent(null);

        transform.position =
            possessedEnemy.transform.position;

        // Restore visuals
        SetVisualsVisible(true);

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

        // Restore AI targets
        foreach (PlatformerEnemyAI otherAI in
                 FindObjectsOfType<PlatformerEnemyAI>())
        {
            if (otherAI.player == possessedEnemy.transform)
            {
                otherAI.player = transform;
            }
        }

        Debug.Log("Possessed enemy died.");

        possessedEnemy = null;
    }

    // ==================================================
    // VISUALS
    // ==================================================

    void SetVisualsVisible(bool visible)
    {
        if (visualsToHide == null)
            return;

        Renderer[] renderers =
            visualsToHide.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
    }

    // ==================================================
    // GIZMOS
    // ==================================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            latchRange
        );
    }
}
