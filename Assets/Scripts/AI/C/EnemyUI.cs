using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;

    public void SetHealthBarVisible(bool value)
    {
        if (healthBar != null)
            healthBar.SetActive(value);
    }
}
