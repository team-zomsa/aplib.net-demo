using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayButton : MonoBehaviour
{
    /// <summary>
    /// Reference to the menu canvas.
    /// </summary>
    public GameObject menuCanvas;

    /// <summary>
    /// Reference to the settings canvas.
    /// </summary>
    public GameObject settingCanvas;

    /// <summary>
    /// To ensure the settings and menu UI aren't on on the same time.
    /// </summary>
    public bool isOnSettings = false;

    private void Start()
    {
        menuCanvas.SetActive(true);
        isOnSettings = false;
        settingCanvas.SetActive(false);
    }

    /// <summary>
    /// When the play button in the main menu is clicked, it wil teleport the player to the game.
    /// Right now it is the melee weapon scene to prove concept. THIS NEEDS TO BE CHANGED.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("MeleeWeapon");
    }

    /// <summary>
    /// This button toggles the menu canvas off and the setting canvas on.
    /// </summary>
    public void SettingToggle()
    {
        isOnSettings = !isOnSettings;
        settingCanvas.SetActive(isOnSettings);
        menuCanvas.SetActive(!isOnSettings);
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
