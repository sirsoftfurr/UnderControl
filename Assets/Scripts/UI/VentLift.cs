using UnityEngine;

public class VentLift : MonoBehaviour
{
    public float liftSpeed = 8f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            if (rb.linearVelocity.y < liftSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, liftSpeed);
            }
        }
    }
}
