using System.Collections;
using UnityEngine;
using TMPro;

public class RoundPopup : MonoBehaviour
{
    [Header("UI")]
    public GameObject popupObject;
    
    [Header("Text Delay")]
    public float textDelay = 0.5f;

    public TMP_Text roundText;

    [Header("Animation")]
    public Animator animator;

    public float popupDuration = 2f;

    private bool showingPopup = false;
    
    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip openSound;

    public AudioClip closeSound;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        if (popupObject != null)
        {
            popupObject.SetActive(false);
        }
    }

    // ==================================================
    // SHOW ROUND
    // ==================================================

    public void ShowRound(int round)
    {
        if (!showingPopup)
        {
            StartCoroutine(
                ShowRoundRoutine(round)
            );
        }
    }

    // ==================================================
    // ROUTINE
    // ==================================================

    IEnumerator ShowRoundRoutine(int round)
    {
        showingPopup = true;

        // Enable popup
        popupObject.SetActive(true);
        
        if (audioSource != null &&
            openSound != null)
        {
            audioSource.pitch =
                Random.Range(
                    0.98f,
                    1.02f
                );

            audioSource.PlayOneShot(
                openSound
            );
        }

        // Hide text initially
        if (roundText != null)
        {
            roundText.gameObject.SetActive(false);
        }

        // Play popup animation
        if (animator != null)
        {
            animator.ResetTrigger("Hide");
            animator.SetTrigger("Show");
        }

        // Wait before showing text
        yield return new WaitForSeconds(
            textDelay
        );

        // Show text
        if (roundText != null)
        {
            roundText.gameObject.SetActive(true);

            roundText.text =
                "ROUND " + round;
        }

        // Stay visible
        yield return new WaitForSeconds(
            popupDuration
        );

        // Hide animation
        if (animator != null)
        {
            animator.ResetTrigger("Show");
            animator.SetTrigger("Hide");
        }

        // Hide text immediately
        if (roundText != null)
        {
            roundText.gameObject.SetActive(false);
        }

        // Wait for hide animation
        yield return new WaitForSeconds(1f);
        
        if (audioSource != null &&
            closeSound != null)
        {
            audioSource.pitch =
                UnityEngine.Random.Range(
                    0.98f,
                    1.02f
                );

            audioSource.PlayOneShot(
                closeSound
            );
        }

        // Disable popup
        popupObject.SetActive(false);

        showingPopup = false;
    }
}

