using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class houses all the settings and menu UI buttons/methods.
/// It is called CanvasManager as it manages all the UI canvases.
/// </summary>
public class CanvasManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the menu canvas.
    /// </summary>
    public GameObject MenuCanvas;

    /// <summary>
    /// Reference to the settings canvas of the menu.
    /// </summary>
    public GameObject SettingMenuCanvas;

    /// <summary>
    /// Reference to the settings canvas of the game.
    /// </summary>
    public GameObject SettingGameCanvas;

    /// <summary>
    /// Reference to the game over canvas.
    /// </summary>
    public GameObject GameOverCanvas;

    /// <summary>
    /// Reference to the win screen.
    /// </summary>
    public GameObject WinScreenCanvas;

    /// <summary>
    /// Name of the start screen.
    /// </summary>
    [SerializeField]
    private string _sceneNameStartingMenu = "Settings"; // TODO:: Load main start screen

    /// <summary>
    /// Name of the game scene
    /// </summary>
    [SerializeField]
    private string _sceneNameGame = "InGameSettings"; // TODO:: Load main game screen

    /// <summary>
    /// This string keeps track of what scene we are in.
    /// </summary>
    private string _currentSceneName = "";

    /// <summary>
    /// To ensure the game settings and menu UI aren't on at the same time.
    /// </summary>
    private bool _isOnGameSettings;

    /// <summary>
    /// To ensure the menu settings and menu UI aren't on at the same time.
    /// </summary>
    private bool _isOnMenuSettings;

    /// <summary>
    /// Prevent the settings toggle when the variable is true.
    /// </summary>
    private bool _preventSettingsToggle;

    public static CanvasManager Instance { get; private set; }

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
        WinArea winArea = FindObjectOfType<WinArea>();
        if (winArea != null)
            winArea.OnWin += ShowWinScreen;
        else
            Debug.LogWarning("No WinArea found in scene!");

        if (_currentSceneName == _sceneNameStartingMenu) // Are we at the starting screen?
        {
            SetAllCanvasesToInactive();
            MenuCanvas.SetActive(true);
        }
        else if (_currentSceneName == _sceneNameGame) // Are we at the main gaming scene?
        {
            HealthComponent playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
            playerHealth.Death += OnPlayerDeath;

            SetAllCanvasesToInactive();
        }
        else // TODO:: Remove when game is done. This is for future ease.
        {
            Debug.Log("Scene names are wrong. Check MenuButtons script");
        }
    }

    /// <summary>
    /// Event that is fired when the game settings are toggled.
    /// </summary>
    public event Action<bool> MenuOpenedEvent;

    /// <summary>
    /// Sets all UI canvases to false.
    /// </summary>
    private void SetAllCanvasesToInactive()
    {
        MenuCanvas.SetActive(false);
        SettingMenuCanvas.SetActive(false);
        SettingGameCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
        WinScreenCanvas.SetActive(false);
        _isOnMenuSettings = false;
        _isOnGameSettings = false;
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
        _preventSettingsToggle = true;

        // On death ui.
        GameOverCanvas.SetActive(true);
    }

    /// <summary>
    /// When the play button in the main menu is clicked, it wil teleport the player to the game.
    /// </summary>
    public void PlayGame() => SceneManager.LoadScene(_sceneNameGame);

    /// <summary>
    /// When the Quit To Menu button in the game setting is clicked, it will take you back to the menu.
    /// </summary>
    public void ToMenu() => SceneManager.LoadScene(_sceneNameStartingMenu);

    /// <summary>
    /// This button toggles the menu canvas off and the setting canvas on or game settings on and off.
    /// </summary>
    public void OnToggleSettings()
    {
        if (_preventSettingsToggle) return;

        if (_currentSceneName == _sceneNameStartingMenu) // Toggle from menu to menu settings and back.
        {
            _isOnMenuSettings = !_isOnMenuSettings;
            SettingMenuCanvas.SetActive(_isOnMenuSettings);
            MenuCanvas.SetActive(!_isOnMenuSettings);
        }
        else if (_currentSceneName == _sceneNameGame) // Toggle from game to game settings and back.
        {
            _isOnGameSettings = !_isOnGameSettings;
            SettingGameCanvas.SetActive(_isOnGameSettings);

            if (_isOnGameSettings) GameManager.Instance.Pause();
            else GameManager.Instance.Resume();

            MenuOpenedEvent?.Invoke(_isOnGameSettings);
        }
    }

    /// <summary>
    /// Show the win screen.
    /// </summary>
    public void ShowWinScreen()
    {
        // Mouse visible.
        MenuOpenedEvent?.Invoke(true);

        // Pause game.
        GameManager.Instance.Pause();

        // Set all off.
        SetAllCanvasesToInactive();
        _preventSettingsToggle = true;

        // On win ui.
        WinScreenCanvas.SetActive(true);
    }

    /// <summary>
    /// Set the points text on the win screen.
    /// Call from the PointsManager after updating final score to circumvent race conditions of events.
    /// </summary>
    public void SetPointsText(int points)
    {
        List<TextMeshProUGUI> textMeshes = WinScreenCanvas.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        TextMeshProUGUI pointsText = textMeshes.Find(textMesh => textMesh.name == "Points");
        pointsText.text = "Points: " + points;
    }

    /// <summary>
    /// When player presses the quit button the application closes.
    /// This works with the build not in the editor. Keep this in mind.
    /// </summary>
    public void QuitApplication() => Application.Quit();
}
