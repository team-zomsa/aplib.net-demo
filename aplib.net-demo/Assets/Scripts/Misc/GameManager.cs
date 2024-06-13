using System;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action Paused;
    public event Action Resumed;

    private void Start() => Resume();

    /// <summary>
    /// Pause the game and disable player input.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        Paused?.Invoke();
        if (InputManager.Instance) InputManager.Instance.DisablePlayerInput();
    }

    /// <summary>
    /// Unpause the game and enable player input.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1;
        Resumed?.Invoke();
        if (InputManager.Instance) InputManager.Instance.EnablePlayerInput();
    }
}
