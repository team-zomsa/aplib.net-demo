using System;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action Paused;
    public event Action Resumed;

    public event Action GameOver;
    public event Action GameWon;

    private void Start() => Resume();

    /// <summary>
    /// Pause the game and disable player input.
    /// </summary>
    public void Pause()
    {
        if (Time.timeScale == 0) return;

        Time.timeScale = 0;
        Paused?.Invoke();
    }

    /// <summary>
    /// Unpause the game and enable player input.
    /// </summary>
    public void Resume()
    {
        if (Time.timeScale == 1) return;

        Time.timeScale = 1;
        Resumed?.Invoke();
    }

    public void TriggerGameOver()
    {
        Pause();
        GameOver?.Invoke();
    }

    public void TriggerGameWon()
    {
        Pause();
        GameWon?.Invoke();
    }
}
