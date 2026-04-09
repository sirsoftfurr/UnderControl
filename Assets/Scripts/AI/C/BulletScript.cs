using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;

    public LayerMask hitLayers;
    public LayerMask groundLayer;

    private Vector2 direction;
    private GameObject owner;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void SetOwner(GameObject shooter)
    {
        owner = shooter;

    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner)
            return;

        int layer = collision.gameObject.layer;

        if (((1 << layer) & hitLayers) != 0)
        {
            NewEnemyHealth enemyHealth = collision.GetComponent<NewEnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // 💥 Spawn blood
            Raycaster blood = FindObjectOfType<Raycaster>();
            if (blood != null)
            {
                blood.SpawnBlood(transform.position, false);
            }

            Destroy(gameObject);
            return;
        }

        if (((1 << layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}


