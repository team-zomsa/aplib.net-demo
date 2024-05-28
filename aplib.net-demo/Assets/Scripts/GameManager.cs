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

    public void Pause()
    {
        Time.timeScale = 0;
        InputManager.Instance.DisablePlayerInput();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        InputManager.Instance.EnablePlayerInput();
    }
}
