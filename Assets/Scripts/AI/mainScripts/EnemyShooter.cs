using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
     [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Shooting")]
    public float fireRate = 1f;
    private float nextFireTime;

    public int damage = 20;

    public float bulletSpeed = 10f;

    [Header("Shotgun")]
    public bool isShotgun = false;

    public int pelletCount = 6;

    public float spreadAngle = 20f;

    [Header("Detection")]
    public float detectionRange = 10f;

    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;

    public float muzzleFlashTime = 0.05f;

    // Current target
    private Transform target;

    private bool canShoot = true;

    // ==================================================
    // ENABLE / DISABLE SHOOTING
    // ==================================================

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
    {
        if (!enabled || !canShoot)
            return;

        // =========================================
        // GET CURRENT TARGET
        // =========================================

        target = LatchScript.currentTarget;

        if (target == null)
            return;

        // =========================================
        // DEBUG LINE
        // =========================================

        Debug.DrawLine(
            firePoint.position,
            target.position,
            Color.red
        );

        // =========================================
        // DISTANCE CHECK
        // =========================================

        float distance =
            Vector2.Distance(
                firePoint.position,
                target.position
            );

        if (distance > detectionRange)
            return;

        // =========================================
        // AIM DIRECTION
        // =========================================

        Vector2 direction =
            (
                (Vector2)target.position -
                (Vector2)firePoint.position
            ).normalized;

        // =========================================
        // RAYCAST CHECK
        // =========================================

        RaycastHit2D hit =
            Physics2D.Raycast(
                firePoint.position,
                direction,
                detectionRange,
                obstacleLayer | targetLayer
            );

        Debug.DrawRay(
            firePoint.position,
            direction * detectionRange,
            Color.red
        );

        // =========================================
        // HIT NOTHING
        // =========================================

        if (hit.collider == null)
            return;

        // =========================================
        // MUST HIT TARGET
        // =========================================

        bool hitTarget =
            hit.transform == target ||
            hit.transform.root == target;

        if (!hitTarget)
            return;

        // =========================================
        // FIRE
        // =========================================

        if (Time.time >= nextFireTime)
        {
            Shoot(direction);

            nextFireTime =
                Time.time + 1f / fireRate;
        }
    }

    // ==================================================
    // SHOOT
    // ==================================================

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null ||
            firePoint == null)
            return;

        // =====================================
        // NORMAL GUN
        // =====================================

        if (!isShotgun)
        {
            SpawnBullet(direction);
        }

        // =====================================
        // SHOTGUN
        // =====================================

        else
        {
            float startAngle =
                -spreadAngle / 2f;

            float angleStep =
                spreadAngle /
                (pelletCount - 1);

            for (int i = 0; i < pelletCount; i++)
            {
                float angle =
                    startAngle +
                    angleStep * i;

                Vector2 spreadDirection =
                    Quaternion.Euler(
                        0,
                        0,
                        angle
                    ) * direction;

                SpawnBullet(spreadDirection);
            }
        }

        StartCoroutine(FlashMuzzle());
    }

    // ==================================================
    // SPAWN BULLET
    // ==================================================

    void SpawnBullet(Vector2 direction)
    {
        GameObject bullet =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

        BulletScript bulletScript =
            bullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);

            bulletScript.damage = damage;

            // Prevent self-hit
            bulletScript.SetOwner(
                transform.root.gameObject
            );
        }

        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                direction * bulletSpeed;
        }
    }

    // ==================================================
    // MUZZLE FLASH
    // ==================================================

    IEnumerator FlashMuzzle()
    {
        if (muzzleFlash == null)
            yield break;

        muzzleFlash.SetActive(true);

        yield return new WaitForSeconds(
            muzzleFlashTime
        );

        muzzleFlash.SetActive(false);
    }
}


