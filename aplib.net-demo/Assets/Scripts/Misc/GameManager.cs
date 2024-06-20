using System;
using UnityEngine;

/// <summary>
/// The game manager, which controls the game state.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// Event triggered when the game is paused.
    /// </summary>
    public event Action Paused;

    /// <summary>
    /// Event triggered when the game is resumed.
    /// </summary>
    public event Action Resumed;

    /// <summary>
    /// Event triggered when the game is lost.
    /// </summary>
    public event Action GameOver;

    /// <summary>
    /// Event triggered when the game is won.
    /// </summary>
    public event Action GameWon;

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

    /// <summary>
    /// Trigger the game over event and pause the game.
    /// </summary>
    public void TriggerGameOver()
    {
        Pause();
        GameOver?.Invoke();
    }

    /// <summary>
    /// Trigger the game won event and pause the game.
    /// </summary>
    public void TriggerGameWon()
    {
        Pause();
        GameWon?.Invoke();
    }

    private void Start() => Resume();
}
