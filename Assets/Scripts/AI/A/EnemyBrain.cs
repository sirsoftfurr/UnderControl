using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    public Transform target;
    public ChaseAI movement;
    public AIShooting shooting;

    public float detectionRange = 15f;
    public float attackRange = 8f;

    private AIState state;

    void Start()
    {
        state = AIState.Idle;
    }

    void Update()
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);

        switch (state)
        {
            case AIState.Idle:
                if (dist < detectionRange)
                    state = AIState.Chase;
                break;

            case AIState.Chase:
                movement.target = target; // 👈 THIS is the correct way
                shooting.SetShootingEnabled(false);

                if (dist < attackRange)
                    state = AIState.Attack;
                break;

            case AIState.Attack:
                shooting.SetShootingEnabled(true);

                if (dist > attackRange + 2f)
                    state = AIState.Chase;

                if (Random.value < 0.005f)
                    state = AIState.Reposition;
                break;

            case AIState.Reposition:
                Vector2 dir = (transform.position - target.position).normalized;
                movement.MoveToPosition((Vector2)transform.position + dir * 3f);

                shooting.SetShootingEnabled(false);

                state = AIState.Chase;
                break;
        }
    }

    public void OnHit()
    {
        state = AIState.Reposition;
    }
}

public enum AIState
{
    Idle,
    Chase,
    Attack,
    Reposition
}
