using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float fireRate = 1f;
    private float nextFireTime;

    public float detectionRange = 10f;

    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    public int damage = 20;

    private Transform player;

    private bool canShoot = true;

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    void Start()
    {
        GameObject p =
            GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (!enabled || !canShoot)
            return;

        // ALWAYS use current possession target
        player = LatchScript.currentTarget;

        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        if (distance <= detectionRange)
        {
            Vector2 direction =
                (player.position - firePoint.position).normalized;

            RaycastHit2D hit =
                Physics2D.Raycast(
                    firePoint.position,
                    direction,
                    detectionRange,
                    obstacleLayer | targetLayer
                );

            if (hit.collider != null &&
                hit.transform == player)
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot(direction);

                    nextFireTime =
                        Time.time + 1f / fireRate;
                }
            }
        }
    }

    void Shoot(Vector2 direction)
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

            // IMPORTANT
            // Uses ROOT object
            bulletScript.SetOwner(
                transform.root.gameObject
            );
        }
    }
}


