using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null) Debug.LogError("Game Manager is null, first awake it!");
            
            return _instance;
        }
    }

    private void Awake()
    {
        // Setup singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}