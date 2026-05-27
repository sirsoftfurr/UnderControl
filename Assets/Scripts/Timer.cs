using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 300f; // 5 minutes = 300 seconds

    [Header("UI")]
    public TMP_Text timerText;

    private float currentTime;
    private bool timerRunning = true;

    void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            timerRunning = false;

            TimerFinished();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimerFinished()
    {
        Debug.Log("Time's up!");

        // Add lose condition or event here
    }
}

