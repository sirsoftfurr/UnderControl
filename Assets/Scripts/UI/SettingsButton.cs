using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [Header("Menus")]
    public GameObject currentMenu;

    public GameObject settingsMenu;

    // ==================================================
    // OPEN SETTINGS
    // ==================================================

    public void OpenSettings()
    {
        // Hide current menu
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
        }

        // Show settings menu
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);
        }
    }
}
