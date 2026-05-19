using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float fireRate = 1f;
    public float detectionRange = 10f;

    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    public int damage = 20;

    private float nextFireTime;
    private bool canShoot = true;

    public void SetShootingEnabled(bool value)
    {
        canShoot = value;
    }

    void Update()
    {
        if (!canShoot) return;

        Transform target = LatchScript.ControlledBody;
        if (target == null || firePoint == null) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > detectionRange) return;

        Vector2 dir = (target.position - firePoint.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            firePoint.position,
            dir,
            detectionRange,
            obstacleLayer | targetLayer
        );

        if (hit.collider != null &&
            Time.time >= nextFireTime)
        {
            Shoot(dir);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot(Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BulletScript b = bullet.GetComponent<BulletScript>();
        if (b != null)
        {
            b.SetDirection(dir);
            b.damage = damage;
            b.SetOwner(gameObject);
        }
    }
}