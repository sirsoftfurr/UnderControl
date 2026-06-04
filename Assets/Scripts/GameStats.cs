using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats instance;

    [Header("Stats")]
    public int enemiesKilled = 0;

    public int enemiesPossessed = 0;

    public int bulletsFired = 0;

    // ==================================================
    // AWAKE
    // ==================================================

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==================================================
    // RESET
    // ==================================================

    public void ResetStats()
    {
        enemiesKilled = 0;
        enemiesPossessed = 0;
        bulletsFired = 0;
    }
}

