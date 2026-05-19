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

    [Header("AI Target (optional)")]
    public Transform aimTarget;

    private float nextFireTime = 0f;
    private bool canShoot = true;

    private void Update()
    {
        if (!enabled)
            return;

        //------------------------------------------------
        // GET AIM DIRECTION
        //------------------------------------------------

        Vector2 aimDirection;

        if (aimTarget != null)
        {
            aimDirection =
                ((Vector2)aimTarget.position -
                (Vector2)gunPivot.position).normalized;
        }
        else
        {
            Vector2 mousePos =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            aimDirection =
                (mousePos -
                (Vector2)gunPivot.position).normalized;
        }

        //------------------------------------------------
        // ROTATE GUN
        //------------------------------------------------

        gunPivot.up = aimDirection;

        //------------------------------------------------
        // FLIP CHARACTER
        // (SAFE FOR RIGGED CHARACTERS)
        //------------------------------------------------

        if (bodySprite != null && aimDirection.x != 0)
        {
            Vector3 scale = bodySprite.localScale;

            scale.x =
                Mathf.Abs(scale.x) * -Mathf.Sign(aimDirection.x);

            bodySprite.localScale = scale;
        }

        //------------------------------------------------
        // SHOOT
        //------------------------------------------------

        if (canShoot && Time.time >= nextFireTime)
        {
            if (aimTarget != null || Input.GetKey(KeyCode.Mouse0))
            {
                Shoot(aimDirection);

                nextFireTime =
                    Time.time + 1f / fireRate;
            }
        }
    }

    private void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null || shootPoint == null)
            return;

        GameObject bullet = Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        Rigidbody2D rb =
            bullet.GetComponent<Rigidbody2D>();

        BulletScript bulletScript =
            bullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetOwner(gameObject);
        }

        if (rb != null)
        {
            rb.linearVelocity =
                direction * bulletSpeed;
        }
    }

    //------------------------------------------------
    // POSSESSION METHODS
    //------------------------------------------------

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    public void ResetCooldown()
    {
        nextFireTime = 0f;
    }
}
