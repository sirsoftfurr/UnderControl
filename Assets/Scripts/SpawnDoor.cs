using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDoor : MonoBehaviour
{
    [Header("Door Objects")] public GameObject closedDoor;

    public GameObject openDoor;

    [Header("Settings")] public float openTime = 1f;

    void Start()
    {
        // Start closed
        if (closedDoor != null)
            closedDoor.SetActive(true);

        if (openDoor != null)
            openDoor.SetActive(false);
    }

    // ==================================================
    // OPEN DOOR
    // ==================================================

    public IEnumerator OpenDoor()
    {
        // =====================================
        // OPEN
        // =====================================

        if (closedDoor != null)
            closedDoor.SetActive(false);

        if (openDoor != null)
            openDoor.SetActive(true);

        // =====================================
        // WAIT
        // =====================================

        yield return new WaitForSeconds(
            openTime
        );

        // =====================================
        // CLOSE
        // =====================================

        if (closedDoor != null)
            closedDoor.SetActive(true);

        if (openDoor != null)
            openDoor.SetActive(false);
    }
}

