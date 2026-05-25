using UnityEngine;

public class LatchScript : MonoBehaviour
{
    [Header("Possession Settings")]
    public float latchRange = 1.5f;
    public LayerMask enemyLayer;
    public Vector2 exitOffset = new Vector2(1f, 0f);

    // Camera + AI target
    public static Transform currentTarget;

    [Header("Player Visuals")]
    // Drag ONLY the player graphics object here
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

    private Vector3 originalScale;
    private Vector3 storedPlayerPosition;

    void Start()
    {
        originalLayer = gameObject.layer;

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        originalScale = transform.localScale;

        // Default camera/AI target = player
        currentTarget = transform;
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

        // Get root enemy object
        GameObject enemy =
            enemyCollider.transform.root.gameObject;

        possessedEnemy = enemy;

        // Camera + AI now follow possessed enemy
        currentTarget = enemy.transform;

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
        // ENABLE PLAYER CONTROL ON ENEMY
        // =========================================

        PlayerMovement playerMovement =
            enemy.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.enabled = true;

            // Possessed enemy faces mouse
            playerMovement.faceMouse = true;
        }

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

        storedPlayerPosition = transform.position;

        transform.SetParent(enemy.transform);

        // Keep hidden player centered
        transform.localPosition = Vector3.zero;

        // Prevent inheriting enemy scale
        transform.localScale = originalScale;

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
        // SUBSCRIBE TO ENEMY DEATH
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
        // REDIRECT AI TARGETS
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
        {
            playerMovement.faceMouse = false;
            playerMovement.enabled = false;
        }

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

        transform.localScale = originalScale;

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

        // Camera returns to player
        currentTarget = transform;

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

        // =========================================
        // DETACH PLAYER
        // =========================================

        transform.SetParent(null);

        transform.localScale = originalScale;

        Vector3 exitPosition =
            possessedEnemy.transform.position +
            (Vector3)exitOffset;

        exitPosition.z = 0f;

        transform.position = exitPosition;

        // =========================================
        // RESTORE VISUALS
        // =========================================

        SetVisualsVisible(true);

        // =========================================
        // RESTORE PHYSICS
        // =========================================

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        // =========================================
        // RESTORE LAYER
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

        // Camera returns to player
        currentTarget = transform;

        Debug.Log("Possessed enemy died");

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
