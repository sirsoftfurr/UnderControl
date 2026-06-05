using UnityEngine;

public class BackButton : MonoBehaviour
{
    [Header("Menus")]
    public GameObject settingsMenu;

    public GameObject mainMenu;

    // ==================================================
    // BACK
    // ==================================================

    public void GoBack()
    {
        // Hide settings
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }

        // Show main menu
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
    }
}
