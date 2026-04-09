using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("References")]
    public Transform shootPoint;
    public GameObject projectilePrefab;

    [Header("Settings")]
    public float bulletSpeed = 10f;
    public float fireRate = 5f;

    private float nextFireTime = 0f;

    public void TryShoot(Vector2 direction)
    {
        if (Time.time < nextFireTime) return;

        Shoot(direction);
        nextFireTime = Time.time + 1f / fireRate;
    }

    private void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;
    }

    public void ResetCooldown()
    {
        nextFireTime = Time.time;
    }
}
