using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text ammoText;

    void Update()
    {
        if (LatchScript.currentTarget == null)
        {
            ammoText.enabled = false;
            return;
        }

        PlayerShooting shooting =
            LatchScript.currentTarget
                .GetComponent<PlayerShooting>();

        // No gun = hide UI
        if (shooting == null)
        {
            ammoText.enabled = false;
            return;
        }

        // Show UI
        ammoText.enabled = true;

        ammoText.text =
            shooting.GetAmmo() +
            " / " +
            shooting.maxAmmo;
    }
}

