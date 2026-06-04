using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathScreenUI;

    public GameObject greenScreenEffect;

    [Header("Animation")]
    public Animator greenAnimator;

    public float animationLength = 1f;

    [Header("Stats")]
    public TMP_Text roundReachedText;

    public TMP_Text killsText;

    public TMP_Text possessionsText;

    public TMP_Text bulletsText;

    private bool isShowing = false;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }

        if (greenScreenEffect != null)
        {
            greenScreenEffect.SetActive(false);
        }
    }

    // ==================================================
    // SHOW DEATH SCREEN
    // ==================================================

    public void ShowDeathScreen(int round)
    {
        if (!isShowing)
        {
            StartCoroutine(
                ShowDeathRoutine(round)
            );
        }
    }

    // ==================================================
    // ROUTINE
    // ==================================================

    IEnumerator ShowDeathRoutine(int round)
    {
        isShowing = true;

        // Slow freeze feeling
        Time.timeScale = 0f;

        // Enable greenscreen effect
        if (greenScreenEffect != null)
        {
            greenScreenEffect.SetActive(true);
        }

        // Play popup animation
        if (greenAnimator != null)
        {
            greenAnimator.ResetTrigger("Hide");
            greenAnimator.SetTrigger("Show");
        }

        // Wait for animation
        yield return new WaitForSecondsRealtime(
            animationLength
        );

        // Show death UI
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }

        // =========================================
        // SET STATS
        // =========================================

        if (roundReachedText != null)
        {
            roundReachedText.text =
                "Round Reached: " + round;
        }

        if (killsText != null)
        {
            killsText.text =
                "Enemies Killed: " +
                GameStats.instance.enemiesKilled;
        }

        if (possessionsText != null)
        {
            possessionsText.text =
                "Enemies Possessed: " +
                GameStats.instance.enemiesPossessed;
        }

        if (bulletsText != null)
        {
            bulletsText.text =
                "Bullets Fired: " +
                GameStats.instance.bulletsFired;
        }
    }

    // ==================================================
    // RESTART
    // ==================================================

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // ==================================================
    // QUIT
    // ==================================================

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();

        Debug.Log("Quit Game");
    }
}

