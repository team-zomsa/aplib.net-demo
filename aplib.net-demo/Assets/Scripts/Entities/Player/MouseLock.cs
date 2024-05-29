using UnityEngine;

public class MouseLock : MonoBehaviour
{
    private bool _isOnMenu = false;

    /// <summary>
    /// Locks the cursor and hides it when the game starts.
    /// Should be start to ensure the canvas manager is loaded.
    /// </summary>
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Compatability for when no CanvasManager is present.
        if (CanvasManager.Instance != null)
            CanvasManager.Instance.MenuOpenedEvent += OnGameSettingsToggled;
    }

    /// <summary>
    /// Shows the mouse and unlocks it. Useful on websites.
    /// </summary>
    public void OnShowMousePressed() => EnableMouseCursor();

    /// <summary>
    /// Enables or disables the cursor based on the game settings visibility.
    /// </summary>
    /// <param name="isOnGameSettings"></param>
    public void OnGameSettingsToggled(bool isOnGameSettings)
    {
        _isOnMenu = isOnGameSettings;
        if (isOnGameSettings)
            EnableMouseCursor();
        else
            DisableMouseCursor();
    }

    /// <summary>
    /// On left mouse click, go back into the game and lock the cursor.
    /// </summary>
    public void OnLeftMousePressed()
    {
        if (_isOnMenu) return;
        DisableMouseCursor();
    }

    /// <summary>
    /// Enables cursor.
    /// </summary>
    private void EnableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Disables cursor.
    /// </summary>
    private void DisableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
