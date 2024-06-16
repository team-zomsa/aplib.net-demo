using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCanvas : Singleton<GameCanvas>
{
    /// <summary>
    /// Reference to the game over canvas.
    /// </summary>
    [field: SerializeField]
    public GameObject GameOverCanvas { get; set; }

    /// <summary>
    /// Reference to the win screen.
    /// </summary>
    [field: SerializeField]
    public GameObject WinScreenCanvas { get; set; }

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
    private string _mainMenuSceneName = "Settings";

    /// <summary>
    /// Show the game over screen.
    /// </summary>
    public void OnGameOver()
    {
        HideAllCanvases();
        ShowCanvas(GameOverCanvas);
    }

    /// <summary>
    /// Show the win screen.
    /// </summary>
    public void OnGameWon()
    {
        HideAllCanvases();
        ShowCanvas(WinScreenCanvas);
    }

    /// <summary>
    /// Show/hide the help screen.
    /// </summary>
    public void ToggleHelp()
    {
        if (IsActive(GameOverCanvas) || IsActive(WinScreenCanvas)) return;

        ToggleCanvas(HelpCanvas);
    }

    /// <summary>
    /// Show/hide the help screen.
    /// </summary>
    public void ToggleSettings()
    {
        if (IsActive(GameOverCanvas) || IsActive(WinScreenCanvas) || IsActive(HelpCanvas)) return;

        ToggleCanvas(SettingCanvas);
    }

    /// <summary>
    /// Load the main menu.
    /// </summary>
    public void ToMainMenu() => SceneManager.LoadScene(_mainMenuSceneName);

    /// <summary>
    /// Check if one of the menu canvases is active.
    /// </summary>
    /// <returns>Whether one of the menu canvases is active.</returns>
    public bool IsOnMenu() => IsActive(SettingCanvas) || IsActive(HelpCanvas) || IsActive(GameOverCanvas) || IsActive(WinScreenCanvas);

    private void Start() => HideAllCanvases();

    private void OnEnable()
    {
        GameManager.Instance.GameOver += OnGameOver;
        GameManager.Instance.GameWon += OnGameWon;
        InputManager.Instance.ToggledSettings += ToggleSettings;
        InputManager.Instance.ToggledHelp += ToggleHelp;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameOver -= OnGameOver;
        GameManager.Instance.GameWon -= OnGameWon;
        InputManager.Instance.ToggledSettings -= ToggleSettings;
        InputManager.Instance.ToggledHelp -= ToggleHelp;
    }

    /// <summary>
    /// Disable all canvases.
    /// </summary>
    private void HideAllCanvases()
    {
        HideCanvas(GameOverCanvas);
        HideCanvas(WinScreenCanvas);
        HideCanvas(SettingCanvas);
        HideCanvas(HelpCanvas);
    }

    /// <summary>
    /// Show the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to show.</param>
    private void ShowCanvas(GameObject canvas)
    {
        if (canvas == null) return;

        canvas.SetActive(true);

        GameManager.Instance.Pause();
        MouseLock.Instance.EnableMouseCursor();
        InputManager.Instance.DisablePlayerInput();
    }

    /// <summary>
    /// Hide the canvas.
    /// </summary>
    /// <param name="canvas">The canvas to hide.</param>
    private void HideCanvas(GameObject canvas)
    {
        if (canvas == null) return;

        canvas.SetActive(false);

        if (!IsOnMenu())
        {
            MouseLock.Instance.DisableMouseCursor();
            GameManager.Instance.Resume();
            InputManager.Instance.EnablePlayerInput();
        }
    }

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
