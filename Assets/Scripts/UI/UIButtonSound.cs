using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private Button button;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(
                PlayClickSound
            );
        }
    }

    // ==================================================
    // PLAY SOUND
    // ==================================================

    void PlayClickSound()
    {
        if (audioSource != null &&
            clickSound != null)
        {
            audioSource.pitch =
                UnityEngine.Random.Range(
                    0.98f,
                    1.02f
                );

            audioSource.PlayOneShot(
                clickSound
            );
        }
    }
}
