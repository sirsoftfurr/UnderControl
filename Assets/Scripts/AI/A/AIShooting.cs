using UnityEngine;

public class AIShooting : MonoBehaviour
{
    public Transform gunPivot;
    public Transform bodySprite;
    public Transform target;
    public GunController gun;
    void Update()
    {
        if (target == null) return;

        Vector2 dir = ((Vector2)target.position - (Vector2)gunPivot.position).normalized;

        gunPivot.up = dir;

        if (dir.x != 0)
            bodySprite.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);

        if (canShoot)
        {
            gun.TryShoot(dir);
        }
    }

    private bool canShoot = false;

    public void SetShootingEnabled(bool value)
    {
        canShoot = value;
    }
}
