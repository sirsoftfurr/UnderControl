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
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!enabled || !canShoot)
            return;
        
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(
                firePoint.position,
                direction,
                detectionRange,
                obstacleLayer | targetLayer
            );

            if (hit.collider != null && hit.transform == player)
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot(direction);
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }

            if (hit.collider != null)
            {
                if (((1 << hit.collider.gameObject.layer) & targetLayer) != 0)
                {
                    if (Time.time >= nextFireTime)
                    {
                        Shoot(direction);
                        nextFireTime = Time.time + 1f / fireRate;
                    }
                }
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        bulletScript.SetDirection(direction);
        bulletScript.damage = damage;
        bulletScript.SetOwner(gameObject);
    }
}


