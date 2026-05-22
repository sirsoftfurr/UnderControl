using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform bodySprite;
    public Transform gunPivot;
    public Transform shootPoint;
    public GameObject projectilePrefab;

    [Header("Settings")]
    public float fireRate = 5f;

    [Header("AI Target (optional)")]
    public Transform aimTarget;

    private float nextFireTime = 0f;

    private bool canShoot = true;

    private SpriteRenderer sr;

    void Start()
    {
        if (bodySprite != null)
            sr = bodySprite.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!enabled || !canShoot)
            return;

        Vector2 aimDirection;

        // =========================
        // AIM
        // =========================

        if (aimTarget != null)
        {
            aimDirection =
                (
                    (Vector2)aimTarget.position -
                    (Vector2)gunPivot.position
                ).normalized;
        }
        else
        {
            Vector2 mousePos =
                Camera.main.ScreenToWorldPoint(
                    Input.mousePosition
                );

            aimDirection =
                (
                    mousePos -
                    (Vector2)gunPivot.position
                ).normalized;
        }

        // Rotate gun
        gunPivot.up = aimDirection;

        // =========================
        // FLIP SPRITE
        // =========================

        if (sr != null && aimDirection.x != 0)
        {
            sr.flipX = aimDirection.x < 0;
        }

        // =========================
        // SHOOT
        // =========================

        if (Time.time >= nextFireTime)
        {
            if (aimTarget != null ||
                Input.GetKey(KeyCode.Mouse0))
            {
                Shoot(aimDirection);

                nextFireTime =
                    Time.time + 1f / fireRate;
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null ||
            shootPoint == null)
            return;

        GameObject bullet =
            Instantiate(
                projectilePrefab,
                shootPoint.position,
                Quaternion.identity
            );

        BulletScript bulletScript =
            bullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);

            // IMPORTANT
            // Uses ROOT object
            bulletScript.SetOwner(
                transform.root.gameObject
            );
        }
    }

    // =========================
    // PUBLIC METHODS
    // =========================

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    public void ResetCooldown()
    {
        nextFireTime = 0f;
    }
}
