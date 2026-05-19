using Unity.Cinemachine;
using UnityEngine;


public class LatchScript : MonoBehaviour
{
    [Header("Settings")]
    public float latchRange = 1.5f;
    public LayerMask enemyLayer;
    public Vector2 exitOffset = new Vector2(1, 0);

    [Header("Visuals")]
    public SpriteRenderer[] spritesToHide;

    public static Transform ControlledBody;
    public static Transform CameraTarget;

    private GameObject possessedEnemy;
    private Rigidbody2D rb;
    private Collider2D col;

    private int originalLayer;
    private float lastTime;
    public float cooldown = 0.2f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        ControlledBody = transform;
        CameraTarget = transform;

        originalLayer = gameObject.layer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) &&
            Time.time > lastTime + cooldown)
        {
            if (possessedEnemy == null)
                TryPossess();
            else
                Release();

            lastTime = Time.time;
        }
    }

    private void TryPossess()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, latchRange, enemyLayer);
        if (!hit) return;

        GameObject enemy = hit.transform.root.gameObject;
        possessedEnemy = enemy;

        ControlledBody = enemy.transform;
        CameraTarget = enemy.transform;

        // SAFE COMPONENT FETCH (ROOT OR CHILD SAFE)
        PlatformerAI ai = enemy.GetComponentInChildren<PlatformerAI>();
        if (ai) ai.enabled = false;

        EnemyShooter shooter = enemy.GetComponentInChildren<EnemyShooter>();
        if (shooter)
        {
            shooter.SetShootingEnabled(false);
            shooter.enabled = false;
        }

        PlayerMovement pm = enemy.GetComponentInChildren<PlayerMovement>();
        if (pm) pm.enabled = true;

        PlayerShooting ps = enemy.GetComponentInChildren<PlayerShooting>();
        if (ps)
        {
            ps.enabled = true;
            ps.SetShootingEnabled(true);
            ps.ResetCooldown();
        }

        // hide player
        transform.SetParent(enemy.transform);
        transform.localPosition = new Vector3(0, -1000f, 0);

        foreach (var s in spritesToHide)
            if (s) s.enabled = false;

        if (col) col.enabled = false;
        if (rb) rb.simulated = false;

        gameObject.layer = LayerMask.NameToLayer("InvisiblePlayer");
    }

    private void Release()
    {
        if (!possessedEnemy) return;

        ControlledBody = transform;
        CameraTarget = transform;

        PlatformerAI ai = possessedEnemy.GetComponentInChildren<PlatformerAI>();
        if (ai) ai.enabled = true;

        EnemyShooter shooter = possessedEnemy.GetComponentInChildren<EnemyShooter>();
        if (shooter)
        {
            shooter.enabled = true;
            shooter.SetShootingEnabled(true);
        }

        PlayerMovement pm = possessedEnemy.GetComponentInChildren<PlayerMovement>();
        if (pm) pm.enabled = false;

        PlayerShooting ps = possessedEnemy.GetComponentInChildren<PlayerShooting>();
        if (ps)
        {
            ps.enabled = false;
            ps.SetShootingEnabled(false);
        }

        transform.SetParent(null);
        transform.position = possessedEnemy.transform.position + (Vector3)exitOffset;

        foreach (var s in spritesToHide)
            if (s) s.enabled = true;

        if (col) col.enabled = true;
        if (rb) rb.simulated = true;

        gameObject.layer = originalLayer;

        possessedEnemy = null;
    }
}
