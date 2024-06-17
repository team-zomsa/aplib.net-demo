using UnityEngine;

public class MouseLock : Singleton<MouseLock>
{
    /// <summary>
    /// Toggles the mouse cursor.
    /// </summary>
    public void ToggleMouseCursor()
    {
        if (Cursor.visible) DisableMouseCursor();
        else EnableMouseCursor();
    }

    /// <summary>
    /// Enables cursor.
    /// </summary>
    public void EnableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Mouse cursor enabled.");
    }

    /// <summary>
    /// Disables cursor.
    /// </summary>
    public void DisableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Mouse cursor disabled.");
    }

    /// <summary>
    /// Locks the cursor and hides it when the game starts.
    /// </summary>
    private void Start() => DisableMouseCursor();

    private void OnEnable() => InputManager.Instance.MouseShown += ToggleMouseCursor;

    private void OnDisable() => InputManager.Instance.MouseShown -= ToggleMouseCursor;
}
