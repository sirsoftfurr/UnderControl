using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private RectTransform rectTransform;

    private Canvas canvas;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        rectTransform =
            GetComponent<RectTransform>();

        canvas =
            GetComponentInParent<Canvas>();

        // Hide default cursor
        Cursor.visible = false;
    }

    // ==================================================
    // UPDATE
    // ==================================================

    void Update()
    {
        Vector2 mousePos =
            Input.mousePosition;

        rectTransform.position =
            mousePos;
    }

    // ==================================================
    // ON DESTROY
    // ==================================================

    void OnDestroy()
    {
        Cursor.visible = true;
    }
}
