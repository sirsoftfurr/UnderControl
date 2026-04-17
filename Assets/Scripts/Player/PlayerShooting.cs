using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform bodySprite;    // The visible sprite
    public Transform gunPivot;      // Rotates toward aim
    public Transform shootPoint;    // Where bullets come out
    public GameObject projectilePrefab;

    [Header("Settings")]
    public float bulletSpeed = 10f;
    public float fireRate = 5f;

    [Header("AI Target (optional)")]
    public Transform aimTarget;     // If null, use mouse

    private float nextFireTime = 0f;
    private bool canShoot = true;

    private SpriteRenderer sr; // ✅ NEW

    void Start()
    {
        // Get SpriteRenderer from bodySprite
        if (bodySprite != null)
            sr = bodySprite.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!enabled) return;

        Vector2 aimDirection;

        // Determine aiming direction
        if (aimTarget != null)
        {
            aimDirection = ((Vector2)aimTarget.position - (Vector2)gunPivot.position).normalized;
        }
        else
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mousePos - (Vector2)gunPivot.position).normalized;
        }

        // Rotate gun pivot toward target
        gunPivot.up = aimDirection;

        // ✅ FIXED: Flip using SpriteRenderer instead of scale
        if (sr != null && aimDirection.x != 0)
        {
            sr.flipX = aimDirection.x < 0;
        }

        // Shoot input (player or AI target)
        if (canShoot && Time.time >= nextFireTime)
        {
            if (aimTarget != null || Input.GetKey(KeyCode.Mouse0))
            {
                Shoot(aimDirection);
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        if (bulletScript != null)
        {
            bulletScript.SetOwner(gameObject);
        }

        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
    }

    // -------------------------
    // PUBLIC METHODS FOR POSSESSION
    // -------------------------

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    public void ResetCooldown()
    {
        nextFireTime = 0f;
    }
}
