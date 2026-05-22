using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    public float smoothSpeed = 5f;

    [Header("Offset")]
    public Vector3 offset =
        new Vector3(0f, 0f, -10f);

    void LateUpdate()
    {
        // Get current target
        Transform target =
            LatchScript.currentTarget;

        if (target == null)
            return;

        // Desired camera position
        Vector3 desiredPosition =
            target.position + offset;

        // Smooth movement
        Vector3 smoothedPosition =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

        transform.position = smoothedPosition;
    }
}

