using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null) Debug.LogError("Game Manager is null, awake it first!");

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    private void Start() => Resume();

    /// <summary>
    /// Pause the game and disable player input.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        InputManager.Instance.DisablePlayerInput();
    }

    /// <summary>
    /// Pause the game and enable player input.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1;
        InputManager.Instance.EnablePlayerInput();
    }
}
