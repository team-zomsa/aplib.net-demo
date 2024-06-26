// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;

/// <summary>
/// This component provides methods for locking and unlocking the mouse cursor.
/// It also locks and hides the mouse cursor when the game starts.
/// </summary>
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
        GameManager.Instance.Pause();
    }

    /// <summary>
    /// Disables cursor.
    /// </summary>
    public void DisableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.Resume();
    }

    /// <summary>
    /// Locks the cursor and hides it when the game starts.
    /// </summary>
    private void Start() => DisableMouseCursor();

    private void OnEnable()
    {
        InputManager.Instance.MouseShown += ToggleMouseCursor;
        InputManager.Instance.Fired += DisableMouseCursor;
    }

    private void OnDisable()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.MouseShown -= ToggleMouseCursor;
            InputManager.Instance.Fired -= DisableMouseCursor;
        }
    }
}
