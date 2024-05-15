using UnityEngine;

public class MouseLock : MonoBehaviour
{
    private bool _showMouse = false;

    /// <summary>
    /// Managed all the UI canvases.
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
    /// Shows the mouse and unlocks it. Usefull on websites.
    /// </summary>
    public void OnShowMousePressed()
    {
        EnableMouseCurser();
    }

    /// <summary>
    /// On left mouse click, go back into the game and lock the cursor.
    /// </summary>
    public void OnLeftMousePressed()
    {
        if (CanvasManager.IsCursurNeeded) // if curser is needed, enable curser
        {
            EnableMouseCurser();
        }
        else if (_showMouse && !CanvasManager.IsCursurNeeded) // if curser is showing and is not needed, disable
        {
            DisableMouseCurser();
        }
    }

    /// <summary>
    /// Enables curser.
    /// </summary>
    void EnableMouseCurser()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _showMouse = true;
    }

    /// <summary>
    /// Disables curser.
    /// </summary>
    void DisableMouseCurser()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _showMouse = false;
    }
}
