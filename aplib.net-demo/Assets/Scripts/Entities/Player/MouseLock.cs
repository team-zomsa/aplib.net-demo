using UnityEngine;

public class MouseLock : MonoBehaviour
{
    private bool _showMouse = false;

    /// <summary>
    /// Manages all the UI canvases.
    /// </summary>
    public CanvasManager CanvasManager;

    /// <summary>
    /// Locks the cursor and hides it when the game starts.
    /// </summary>
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Shows the mouse and unlocks it. Useful on websites.
    /// </summary>
    public void OnShowMousePressed() => EnableMouseCursor();

    /// <summary>
    /// On left mouse click, go back into the game and lock the cursor.
    /// </summary>
    public void OnLeftMousePressed()
    {
        if (CanvasManager.IsCursurNeeded) // if curser is needed, enable curser
        {
            EnableMouseCursor();
        }
        else if (_showMouse && !CanvasManager.IsCursurNeeded) // if curser is showing and is not needed, disable
        {
            DisableMouseCursor();
        }
    }

    /// <summary>
    /// Enables curser.
    /// </summary>
    private void EnableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _showMouse = true;
    }

    /// <summary>
    /// Disables curser.
    /// </summary>
    private void DisableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _showMouse = false;
    }
}
