// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages the main menu canvas, including the settings and help menu.
/// </summary>
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
    /// Reference to the configuration settings.
    /// </summary>
    [field: SerializeField]
    public GameObject ConfigCanvas { get; set; }

    /// <summary>
    /// Reference to the help/keybinds canvas.
    /// </summary>
    [field: SerializeField]
    public GameObject HelpCanvas { get; set; }

    /// <summary>
    /// Name of the game scene.
    /// </summary>
    [SerializeField]
    private string _gameSceneName = "LevelGeneration";

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
        HideCanvas(ConfigCanvas);
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

    /// <summary>
    /// When the play button is pressed it will take the player to the configuration scene.
    /// If the back button in this canvas is pressed it will go back to main menu.
    /// </summary>
    public void ProceedToConfig()
    {
        // Set all canvases to inactive.
        HideCanvas(MenuCanvas);

        // Set config or menu active and disable the other.
        ShowCanvas(ConfigCanvas);
    }

    /// <summary>
    /// Toggle for when the user wants to go back to the main menu.
    /// </summary>
    public void BackToMenu()
    {
        HideCanvas(ConfigCanvas);
        ShowCanvas(MenuCanvas);
    }
}
