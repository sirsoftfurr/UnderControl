using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        // Load saved volume
        float volume =
            PlayerPrefs.GetFloat(
                "MasterVolume",
                1f
            );

        slider.value = volume;

        AudioListener.volume = volume;

        // Listen for changes
        slider.onValueChanged.AddListener(
            SetVolume
        );
    }

    // ==================================================
    // SET VOLUME
    // ==================================================

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        PlayerPrefs.SetFloat(
            "MasterVolume",
            volume
        );

        PlayerPrefs.Save();
    }
}
