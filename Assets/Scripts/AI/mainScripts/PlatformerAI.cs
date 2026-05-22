using System.Collections.Generic;
using UnityEngine;

public class PlatformerEnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;

    private Rigidbody2D rb;
    private bool active = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!active) return;

        Transform target = LatchScript.ControlledBody;
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);
        float dir = Mathf.Sign(target.position.x - transform.position.x);

        if (dist > stopDistance)
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        transform.localScale = scale;
    }

    public void Disable() => active = false;
    public void Enable() => active = true;
}

