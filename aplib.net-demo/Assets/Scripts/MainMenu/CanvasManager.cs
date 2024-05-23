using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class houses all the settings and menu UI buttons/methods.
/// It is called CanvasManager as it manages all the UI canvases.
/// </summary>
public class CanvasManager : MonoBehaviour
{
    /// <summary>
    /// Event that is fired when the game settings are toggled.
    /// </summary>
    public event Action<bool> GameSettingsToggled;

    /// <summary>
    /// Reference to the menu canvas.
    /// </summary>
    public GameObject menuCanvas;

    /// <summary>
    /// Reference to the settings canvas of the menu.
    /// </summary>
    public GameObject settingMenuCanvas;

    /// <summary>
    /// Reference to the settings canvas of the game.
    /// </summary>
    public GameObject settingGameCanvas;

    public static CanvasManager Instance { get; private set; }

    /// <summary>
    /// Reference to the death canvas.
    /// </summary>
    public GameObject deathCanvas;

    /// <summary>
    /// To ensure the menu settings and menu UI aren't on on the same time.
    /// </summary>
    private bool _isOnMenuSettings = false;

    /// <summary>
    /// To ensure the game settings and menu UI aren't on on the same time.
    /// </summary>
    private bool _isOnGameSettings = false;

    public bool IsPlayerDead = false;

    /// <summary>
    /// This string keeps track of what scene we are in.
    /// </summary>
    private string _currentSceneName = "";

    /// <summary>
    /// Name of the start screen.
    /// </summary>
    [SerializeField]
    private string _sceneNameStartingMenu = "Settings"; // TODO:: Load main start screen

    /// <summary>
    /// Name of the game scene
    /// </summary>
    [SerializeField]
    private string _sceneNameGame = "GameSettings"; // TODO:: Load main game screen

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Looks at which canvas is needed by checking the current scene.
    private void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;

        if (_currentSceneName == _sceneNameStartingMenu) // Are we at the starting screen?
        {
            menuCanvas.SetActive(true);
            settingMenuCanvas.SetActive(false);
            settingGameCanvas.SetActive(false);
            _isOnMenuSettings = false;
            _isOnGameSettings = false;
        }
        else if (_currentSceneName == _sceneNameGame) // Are we at the main gaming scene?
        {
            menuCanvas.SetActive(false);
            settingMenuCanvas.SetActive(false);
            settingGameCanvas.SetActive(false);
            _isOnMenuSettings = false;
            _isOnGameSettings = false;
        }
        else // TODO:: Remove when game is done. This is for future ease.
        {
            Debug.Log("Scene names are wrong. Check MenuButtons script");
        }

        deathCanvas.SetActive(false);
    }

    /// <summary>
    /// When the play button in the main menu is clicked, it wil teleport the player to the game.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(_sceneNameGame);
    }

    /// <summary>
    /// When the Quit To Menu button in the game setting is clicked, it will take you back to the menu.
    /// </summary>
    public void ToMenu()
    {
        SceneManager.LoadScene(_sceneNameStartingMenu);
    }

    /// <summary>
    /// This button toggles the menu canvas off and the setting canvas on or game settings on and off.
    /// </summary>
    public void OnToggleSettings()
    {
        if (_currentSceneName == _sceneNameStartingMenu) // Toggle from menu to menu settings and back.
        {
            _isOnMenuSettings = !_isOnMenuSettings;
            settingMenuCanvas.SetActive(_isOnMenuSettings);
            menuCanvas.SetActive(!_isOnMenuSettings);
        }
        else if (_currentSceneName == _sceneNameGame) // Toggle from game to game settings and back.
        {
            _isOnGameSettings = !_isOnGameSettings;
            settingGameCanvas.SetActive(_isOnGameSettings);
            if (_isOnGameSettings)
            {
                Time.timeScale = 0;
                InputManager.Instance.DisablePlayerInput();
            }
            else
            {
                Time.timeScale = 1;
                InputManager.Instance.EnablePlayerInput();
            }
            GameSettingsToggled?.Invoke(_isOnGameSettings);
        }
    }

    /// <summary>
    /// When player presses the quit button the application closes.
    /// This works with the build not in the editor. Keep this in mind.
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }
}
