using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class houses all the settings and menu UI buttons/methods.
/// It is called CanvasManager as it manages all the UI canvi.
/// </summary>
public class CanvasManager : MonoBehaviour
{
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

    /// <summary>
    /// To ensure the menu settings and menu UI aren't on on the same time.
    /// </summary>
    public bool isOnMenuSettings = false;

    /// <summary>
    /// To ensure the game settings and menu UI aren't on on the same time.
    /// </summary>
    public bool isOnGameSettings = false;

    /// <summary>
    /// This bool communicates with the mouse lock script to enable and disable the curser.
    /// </summary>
    public bool IsCursurNeeded => isOnGameSettings;

    /// <summary>
    /// This string keeps track of what scene we are in.
    /// </summary>
    private string _currentSceneName = "";

    /// <summary>
    /// Name of the start screen
    /// </summary>
    private string _sceneNameStartingMenu = "Settings"; // TODO:: Load main start screen

    /// <summary>
    /// Name of the game scene
    /// </summary>
    private string _sceneNameGame = "MeleeWeaponMenu"; // TODO:: Load main game screen

    // Looks at which canvas is needed by checking the current scene.
    private void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;

        if (_currentSceneName == _sceneNameStartingMenu) // Are we at the starting screen?
        {
            menuCanvas.SetActive(true);
            settingMenuCanvas.SetActive(false);
            settingGameCanvas.SetActive(false);
            isOnMenuSettings = false;
            isOnGameSettings = false;
        }
        else if (_currentSceneName == _sceneNameGame) // Are we at the main gaming scene?
        {
            menuCanvas.SetActive(false);
            settingMenuCanvas.SetActive(false);
            settingGameCanvas.SetActive(false);
            isOnMenuSettings = false;
            isOnGameSettings = false;
        }
        else // TODO:: Remove when game is done. This is for future ease.
        {
            Debug.Log("Scene names are wrong. Check MenuButtons script");
        }
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
    public void ToggleSettings()
    {
        if (_currentSceneName == _sceneNameStartingMenu) // Toggle from menu to menu settings and back
        {
            isOnMenuSettings = !isOnMenuSettings;
            settingMenuCanvas.SetActive(isOnMenuSettings);
            menuCanvas.SetActive(!isOnMenuSettings);
        }
        else if (_currentSceneName == _sceneNameGame) // Toggle from game to game settings and back
        {
            isOnGameSettings = !isOnGameSettings;
            settingGameCanvas.SetActive(isOnGameSettings);
            if (isOnGameSettings) { Time.timeScale = 0; InputManager.Instance.DisablePlayerInput(); }
            else { Time.timeScale = 1; InputManager.Instance.EnablePlayerInput(); }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Toggle settings
        {
            ToggleSettings();
        }
    }
}
