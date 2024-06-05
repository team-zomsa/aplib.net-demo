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
    public event Action<bool> MenuOpenedEvent;

    /// <summary>
    /// Reference to the menu canvas.
    /// </summary>
    public GameObject MenuCanvas;

    /// <summary>
    /// Reference to the settings canvas of the menu.
    /// </summary>
    public GameObject SettingCanvas;

    /// <summary>
    /// Reference to the game over canvas.
    /// </summary>
    public GameObject GameOverCanvas;

    /// <summary>
    /// Reference to the help/keybinds canvas.
    /// </summary>
    public GameObject HelpCanvas;

    public static CanvasManager Instance { get; private set; }

    /// <summary>
    /// Is Settings on or off.
    /// </summary>
    private bool _isOnSettings = false;

    /// <summary>
    /// Is Help screen on or off.
    /// </summary>
    private bool _isOnHelp = false;

    /// <summary>
    /// This string keeps track of what scene we are in.
    /// </summary>
    private string _currentSceneName = "";

    /// <summary>
    /// Name of the start screen.
    /// </summary>
    [SerializeField]
    private const string _sceneNameStartingMenu = "Settings"; // TODO:: Load main start screen

    /// <summary>
    /// Name of the game scene
    /// </summary>
    [SerializeField]
    private const string _sceneNameGame = "InGameSettings"; // TODO:: Load main game screen

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

        switch (_currentSceneName)
        {
            case _sceneNameStartingMenu:    // Are we at the starting screen?
                SetAllCanvasesToInactive();
                MenuCanvas.SetActive(true);
                break;
            case _sceneNameGame:    // Are we at the main gaming scene?
                HealthComponent playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
                playerHealth.Death += OnPlayerDeath;

                SetAllCanvasesToInactive();
                break;
            default:
                Debug.Log("Scene names are wrong. Check MenuButtons script");   // TODO:: Remove when game is done. This is for future ease.
                break;
        }
    }

    /// <summary>
    /// Sets all UI canvases to false.
    /// </summary>
    private void SetAllCanvasesToInactive()
    {
        MenuCanvas.SetActive(false);
        SettingCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
        HelpCanvas.SetActive(false);
        _isOnSettings = false;
    }

    /// <summary>
    /// When the player dies, the game over ui is shown.
    /// </summary>
    public void OnPlayerDeath(HealthComponent _)
    {
        // Mouse visible.
        MenuOpenedEvent?.Invoke(true);

        // Pause game.
        GameManager.Instance.Pause();

        // Set all off.
        SetAllCanvasesToInactive();

        // On death ui.
        GameOverCanvas.SetActive(true);
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
        _isOnSettings = !_isOnSettings;

        switch (_currentSceneName)
        {
            case _sceneNameStartingMenu:
                MenuCanvas.SetActive(!_isOnSettings);
                break;
            case _sceneNameGame:    // Toggle from game to game settings and back.
                if (_isOnSettings) GameManager.Instance.Pause();
                else GameManager.Instance.Resume();

                MenuOpenedEvent?.Invoke(_isOnSettings);
                break;
            default:
                break;
        }

        SettingCanvas.SetActive(_isOnSettings);
    }

    /// <summary>
    /// This button toggles the menu canvas off and the help canvas on or help canvas in game on and off.
    /// </summary>
    public void OnToggleHelp()
    {
        _isOnHelp = !_isOnHelp;

        switch (_currentSceneName)
        {
            case _sceneNameStartingMenu:    // Toggle from menu to helpscreen and back.
                SettingCanvas.SetActive(!_isOnHelp);
                break;
            case _sceneNameGame:    // Toggle from game to helpscreen and back.
                if (_isOnHelp)
                {
                    GameManager.Instance.Pause();
                    MenuOpenedEvent?.Invoke(_isOnHelp);
                }
                else if (!_isOnSettings)
                {
                    GameManager.Instance.Resume();
                    MenuOpenedEvent?.Invoke(_isOnHelp);
                }
                break;
            default:
                break;
        }

        HelpCanvas.SetActive(_isOnHelp);
    }

    /// <summary>
    /// When player presses the quit button the application closes.
    /// This works with the build not in the editor. Keep this in mind.
    /// </summary>
    public void Quit()
    {
        switch (_currentSceneName)
        {
            case _sceneNameStartingMenu:    // From menu quit game.
                Application.Quit();
                break;
            case _sceneNameGame:    // From game quit to menu. 
                ToMenu();
                break;
            default:
                break;
        }
    }
}
