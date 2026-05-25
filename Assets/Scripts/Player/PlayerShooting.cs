using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform bodySprite;        // Visual body
    public Transform gunPivot;          // Rotates toward aim
    public Transform shootPoint;        // Bullet spawn point
    public GameObject projectilePrefab;

    [Header("Settings")]
    public float bulletSpeed = 10f;
    public float fireRate = 5f;

    [Header("AI Target (optional)")]
    // If assigned, AI shoots this target
    // Otherwise player uses mouse
    public Transform aimTarget;

    private float nextFireTime = 0f;
    private bool canShoot = true;

    private void Update()
    {
        if (!enabled || !canShoot)
            return;

        if (gunPivot == null || shootPoint == null)
            return;

        Vector2 aimDirection;

        // =====================================
        // AI AIMING
        // =====================================

        if (aimTarget != null)
        {
            aimDirection =
                (
                    (Vector2)aimTarget.position -
                    (Vector2)gunPivot.position
                ).normalized;
        }

        // =====================================
        // PLAYER AIMING
        // =====================================

        else
        {
            Vector3 mouseWorld =
                Camera.main.ScreenToWorldPoint(
                    Input.mousePosition
                );

            mouseWorld.z = 0f;

            aimDirection =
                (
                    (Vector2)mouseWorld -
                    (Vector2)gunPivot.position
                ).normalized;
        }

        // =====================================
        // ROTATE GUN
        // =====================================

        gunPivot.right = aimDirection;

        // =====================================
        // SHOOT INPUT
        // =====================================

        if (Time.time >= nextFireTime)
        {
            // AI shooting
            if (aimTarget != null)
            {
                Shoot(aimDirection);

                nextFireTime =
                    Time.time + 1f / fireRate;
            }

            // Player shooting
            else if (Input.GetMouseButton(0))
            {
                Shoot(aimDirection);

                nextFireTime =
                    Time.time + 1f / fireRate;
            }
        }
    }

    // ==================================================
    // SHOOT
    // ==================================================

    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null)
            return;

        GameObject bullet =
            Instantiate(
                projectilePrefab,
                shootPoint.position,
                Quaternion.identity
            );

        // =====================================
        // BULLET SCRIPT
        // =====================================

        BulletScript bulletScript =
            bullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);

            // VERY IMPORTANT
            // prevents self collision
            bulletScript.SetOwner(transform.root.gameObject);
        }

        // =====================================
        // RIGIDBODY MOVEMENT
        // =====================================

        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                direction * bulletSpeed;
        }
    }

    // ==================================================
    // PUBLIC METHODS
    // ==================================================

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    public void ResetCooldown()
    {
        nextFireTime = 0f;
    }
}
