using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputSettings;

public class GameCanvas : Singleton<GameCanvas>
{
    /// <summary>
    /// Event that is fired when the game settings are toggled.
    /// </summary>
    public event Action<bool> MenuOpenedEvent;

    /// <summary>
    /// Reference to the game over canvas.
    /// </summary>
    public GameObject GameOverCanvas;

    /// <summary>
    /// Reference to the win screen.
    /// </summary>
    public GameObject WinScreenCanvas;

    /// <summary>
    /// Reference to the settings canvas of the menu.
    /// </summary>
    public GameObject SettingCanvas;

    /// <summary>
    /// Reference to the help/keybinds canvas.
    /// </summary>
    public GameObject HelpCanvas;

    private void Start()
    {
        SetAllCanvasesToInactive();

        HealthComponent playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
        playerHealth.Death += OnPlayerDeath;
    }

    /// <summary>
    /// Sets all UI canvases to false.
    /// </summary>
    private void SetAllCanvasesToInactive()
    {
        SettingCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
        WinScreenCanvas.SetActive(false);
        HelpCanvas.SetActive(false);
    }
}
