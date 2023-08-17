using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CustomCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    [Tooltip("Custom texture for the cursor.")]
    public Texture2D cursorTexture;

    [Tooltip("Cursor's click point within the texture (in pixels from the bottom-left).")]
    public Vector2 hotSpot = Vector2.zero;

    [Tooltip("Should the custom cursor be used?")]
    public bool useCustomCursor = true;

    private void Start()
    {
        SetCustomCursor();
    }

    private void SetCustomCursor()
    {
        if (useCustomCursor && cursorTexture)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnValidate()
    {
        // This function will be called when any property of this script is changed in the inspector.
        // Ensures that the cursor updates in real-time when properties are modified in the editor.
        SetCustomCursor();
    }
}
