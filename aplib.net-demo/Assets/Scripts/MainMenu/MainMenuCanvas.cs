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
    [field: SerializeField]
    public GameObject MenuCanvas { get; set; }

    /// <summary>
    /// Reference to the settings canvas of the menu.
    /// </summary>
    [field: SerializeField]
    public GameObject SettingCanvas { get; set; }

    /// <summary>
    /// Reference to the help/keybinds canvas.
    /// </summary>
    [field: SerializeField]
    public GameObject HelpCanvas { get; set; }

    /// <summary>
    /// Name of the game scene.
    /// </summary>
    [SerializeField]
    private string _gameSceneName = "GridSystem";

    /// <summary>
    /// Show/hide the help screen.
    /// </summary>
    public void ToggleHelp() => ToggleCanvas(HelpCanvas);

    /// <summary>
    /// Show/hide the settings screen.
    /// </summary>
    public void ToggleSettings()
    {
        if (IsActive(HelpCanvas)) return;

        ToggleCanvas(SettingCanvas);
        ToggleCanvas(MenuCanvas);
    }

    /// <summary>
    /// Load the game scene.
    /// </summary>
    public void PlayGame() => SceneManager.LoadScene(_gameSceneName);

    /// <summary>
    /// Quit the application.
    /// </summary>
    public void QuitApplication() => Application.Quit();

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
    /// Show/hide the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to toggle.</param>
    private void ToggleCanvas(GameObject canvas)
    {
        if (IsActive(canvas))
            HideCanvas(canvas);
        else
            ShowCanvas(canvas);
    }

    /// <summary>
    /// Check if the canvas is active.
    /// </summary>
    /// <param name="canvas">The canvas to check.</param>
    /// <returns>True if the canvas is active, false otherwise.</returns>
    private bool IsActive(GameObject canvas) => canvas && canvas.activeSelf;
}
