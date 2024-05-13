using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayButton : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject settingCanvas;
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
        if (isOnSettings)
        {
            settingCanvas.SetActive(false);
            isOnSettings = false;
            menuCanvas.SetActive(true);
        }
        else
        {
            menuCanvas.SetActive(false);
            isOnSettings = true;
            settingCanvas.SetActive(true);
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
