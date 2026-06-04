using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
     [Header("References")]
    public Transform bodySprite;
    public Transform gunPivot;
    public Transform shootPoint;

    public GameObject projectilePrefab;

    [Header("Settings")]
    public float bulletSpeed = 10f;
    public float fireRate = 5f;

    [Header("Ammo")]
    public int maxAmmo = 12;

    private int currentAmmo;

    [Header("Shotgun")]
    public bool isShotgun = false;

    public int pelletCount = 6;

    public float spreadAngle = 20f;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;

    public float muzzleFlashTime = 0.05f;

    [Header("AI Target (optional)")]
    public Transform aimTarget;

    private float nextFireTime = 0f;

    private bool canShoot = true;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        currentAmmo = maxAmmo;

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }

    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
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
            // AI SHOOTING
            if (aimTarget != null)
            {
                TryShoot(aimDirection);
            }

            // PLAYER SHOOTING
            else if (Input.GetMouseButton(0))
            {
                TryShoot(aimDirection);
            }
        }
    }

    // ==================================================
    // TRY SHOOT
    // ==================================================

    void TryShoot(Vector2 direction)
    {
        // No ammo
        if (currentAmmo <= 0)
            return;

        Shoot(direction);

        currentAmmo--;

        nextFireTime =
            Time.time + 1f / fireRate;
    }

    // ==================================================
    // SHOOT
    // ==================================================

    void Shoot(Vector2 direction)
    {
        GameStats.instance.bulletsFired++;
        
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

            // Prevent self collision
            bulletScript.SetOwner(
                transform.root.gameObject
            );
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

    // Refill ammo when possessing new enemy
    public void RefillAmmo()
    {
        currentAmmo = maxAmmo;
    }

    // Get current ammo for UI
    public int GetAmmo()
    {
        return currentAmmo;
    }
}
