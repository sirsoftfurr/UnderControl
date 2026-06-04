using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwap : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "Game";

    [Header("Objects")]
    public GameObject objectToHide;

    public GameObject objectToShow;

    [Header("Delay")]
    public float loadDelay = 2f;

    private bool loading = false;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(false);
        }
    }

    // ==================================================
    // PLAY GAME
    // ==================================================

    public void PlayGame()
    {
        if (!loading)
        {
            StartCoroutine(
                PlayRoutine()
            );
        }
    }

    // ==================================================
    // ROUTINE
    // ==================================================

    IEnumerator PlayRoutine()
    {
        loading = true;

        // Hide first object
        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
        }

        // Show second object
        if (objectToShow != null)
        {
            objectToShow.SetActive(true);
        }

        // Wait
        yield return new WaitForSeconds(
            loadDelay
        );

        // Load scene
        SceneManager.LoadScene(
            gameSceneName
        );
    }

    // ==================================================
    // QUIT
    // ==================================================

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Quit Game");
    }
}

