using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject greenScreenEffect;

    public GameObject pauseMenuUI;

    [Header("Animation")]
    public Animator greenAnimator;

    public float animationLength = 1f;

    private bool isPaused = false;
    private DeathScreen deathScreen;
    // Prevent double animations
    private bool isTransitioning = false;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        if (greenScreenEffect != null)
            greenScreenEffect.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        
        deathScreen =
            FindObjectOfType<DeathScreen>();
    }

    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
    {
        // Prevent spam
        if (isTransitioning)
            return;

        // =========================================
        // BLOCK ESC DURING DEATH SCREEN
        // =========================================

        if (deathScreen != null)
        {
            if (deathScreen.deathScreenUI.activeSelf)
                return;
        }

        // =========================================
        // NORMAL ESC
        // =========================================

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                StartCoroutine(ClosePauseMenu());
            }
            else
            {
                StartCoroutine(OpenPauseMenu());
            }
        }
    }

    // ==================================================
    // OPEN
    // ==================================================

    IEnumerator OpenPauseMenu()
    {
        isTransitioning = true;

        isPaused = true;

        greenScreenEffect.SetActive(true);

        if (greenAnimator != null)
        {
            greenAnimator.ResetTrigger("Hide");
            greenAnimator.SetTrigger("Show");
        }

        yield return new WaitForSecondsRealtime(
            animationLength
        );

        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;

        isTransitioning = false;
    }

    // ==================================================
    // CLOSE
    // ==================================================

    IEnumerator ClosePauseMenu()
    {
        isTransitioning = true;

        isPaused = false;

        Time.timeScale = 1f;

        pauseMenuUI.SetActive(false);

        if (greenAnimator != null)
        {
            greenAnimator.ResetTrigger("Show");
            greenAnimator.SetTrigger("Hide");
        }

        yield return new WaitForSecondsRealtime(
            animationLength
        );

        greenScreenEffect.SetActive(false);

        isTransitioning = false;
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
    
    public void ShowDeathMenu()
    {
        if (!isPaused)
        {
            StartCoroutine(OpenPauseMenu());
        }
    }
    
   
}
