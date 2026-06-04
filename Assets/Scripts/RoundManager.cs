using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
     [Header("Round")]
    public int currentRound = 1;
    
    [Header("Round Popup")]
    public RoundPopup roundPopup;

    [Header("Timer")]
    public float roundLength = 300f;

    private float timer;

    [Header("Spawner")]
    public EnemySpawner spawner;

    [Header("Difficulty")]
    public int enemiesAddedPerRound = 3;

    public float spawnMultiplierIncrease = 0.25f;

    [Header("UI")]
    public TMP_Text roundText;

    public TMP_Text timerText;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        timer = roundLength;

        UpdateUI();
    }

    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            NextRound();
        }

        UpdateUI();
    }

    // ==================================================
    // NEXT ROUND
    // ==================================================

    void NextRound()
    {
        currentRound++;
        
        if (roundPopup != null)
        {
            roundPopup.ShowRound(currentRound);
        }

        timer = roundLength;

        // Increase max enemies
        spawner.maxEnemies +=
            enemiesAddedPerRound;

        // Increase spawn weighting
        spawner.spawnRateMultiplier +=
            spawnMultiplierIncrease;

        spawner.currentRound =
            currentRound;

        Debug.Log(
            "Round " +
            currentRound +
            " started!"
        );
    }

    // ==================================================
    // UI
    // ==================================================

    void UpdateUI()
    {
        if (roundText != null)
        {
            roundText.text =
                "Round: " + currentRound;
        }

        if (timerText != null)
        {
            int minutes =
                Mathf.FloorToInt(timer / 60);

            int seconds =
                Mathf.FloorToInt(timer % 60);

            timerText.text =
                string.Format(
                    "{0:00}:{1:00}",
                    minutes,
                    seconds
                );
        }
    }
}
