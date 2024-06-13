using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputSettings;

public class MainMenuCanvas : Singleton<MainMenuCanvas>
{
    /// <summary>
    /// Reference to the menu canvas.
    /// </summary>
    public GameObject MenuCanvas;

    /// <summary>
    /// Reference to the settings canvas of the menu.
    /// </summary>
    public GameObject SettingCanvas;

    /// <summary>
    /// Reference to the help/keybinds canvas.
    /// </summary>
    public GameObject HelpCanvas;


    /// <summary>
    /// Name of the game scene.
    /// </summary>
    [SerializeField]
    private string _gameSceneName = "GridSystem";

    private void Start()
    {
        SetAllCanvasesToInactive();
        MenuCanvas.SetActive(true);
    }

    /// <summary>
    /// Sets all UI canvases to false.
    /// </summary>
    private void SetAllCanvasesToInactive()
    {
        MenuCanvas.SetActive(false);
        SettingCanvas.SetActive(false);
        HelpCanvas.SetActive(false);
    }
}
