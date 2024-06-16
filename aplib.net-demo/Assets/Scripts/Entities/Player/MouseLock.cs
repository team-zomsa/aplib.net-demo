using UnityEngine;

public class MouseLock : Singleton<MouseLock>
{
    /// <summary>
    /// Locks the cursor and hides it when the game starts.
    /// </summary>
    private void Start() => DisableMouseCursor();

    /// <summary>
    /// Enables cursor.
    /// </summary>
    public void EnableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Disables cursor.
    /// </summary>
    public void DisableMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
