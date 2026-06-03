using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwap : MonoBehaviour
{
    [Header("Scene Name")]
    public string gameSceneName = "Game";

    // ==================================================
    // PLAY GAME
    // ==================================================

    public void PlayGame()
    {
        SceneManager.LoadScene(
            gameSceneName
        );
    }

    // ==================================================
    // QUIT GAME
    // ==================================================

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Quit Game");
    }
}

