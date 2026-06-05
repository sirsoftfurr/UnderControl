using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MusicSlider : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;

    [Header("Mixer")]
    public AudioMixer mixer;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        float savedVolume =
            PlayerPrefs.GetFloat(
                "MusicVolume",
                1f
            );

        slider.value = savedVolume;

        SetVolume(savedVolume);

        slider.onValueChanged.AddListener(
            SetVolume
        );
    }

    // ==================================================
    // SET VOLUME
    // ==================================================

    public void SetVolume(float value)
    {
        // Convert slider value to decibels
        float volume =
            Mathf.Log10(value) * 20;

        mixer.SetFloat(
            "MusicVolume",
            volume
        );

        PlayerPrefs.SetFloat(
            "MusicVolume",
            value
        );

        PlayerPrefs.Save();
    }
}
