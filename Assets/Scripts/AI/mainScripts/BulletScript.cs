using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;

    public LayerMask hitLayers;
    public LayerMask groundLayer;

    private Vector2 direction;
    private GameObject owner;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void SetOwner(GameObject shooter)
    {
        owner = shooter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore shooter and ALL child colliders
        if (collision.transform.root.gameObject == owner)
            return;

        int layer = collision.gameObject.layer;

        // =========================
        // HIT DAMAGEABLE TARGET
        // =========================

        if (((1 << layer) & hitLayers) != 0)
        {
            NewEnemyHealth enemyHealth =
                collision.GetComponentInParent<NewEnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Health playerHealth =
                collision.GetComponentInParent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Blood effect
            Raycaster blood =
                FindObjectOfType<Raycaster>();

            if (blood != null)
            {
                blood.SpawnBlood(
                    transform.position,
                    false
                );
            }

            Destroy(gameObject);
            return;
        }

        // =========================
        // HIT GROUND
        // =========================

        if (((1 << layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}


