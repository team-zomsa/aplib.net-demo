using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        HideAllCanvases();

        ShowCanvas(MenuCanvas);
    }

    /// <summary>
    /// Disable all canvases.
    /// </summary>
    private void HideAllCanvases()
    {
        HideCanvas(MenuCanvas);
        HideCanvas(SettingCanvas);
        HideCanvas(HelpCanvas);
    }

    /// <summary>
    /// Show the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to show.</param>
    private void ShowCanvas(GameObject canvas) => canvas.SetActive(true);

    /// <summary>
    /// Hide the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to hide.</param>
    private void HideCanvas(GameObject canvas) => canvas.SetActive(false);

    /// <summary>
    /// Load the game scene.
    /// </summary>
    public void PlayGame() => SceneManager.LoadScene(_gameSceneName);

    /// <summary>
    /// Quit the application.
    /// </summary>
    public void QuitApplication() => Application.Quit();
}
