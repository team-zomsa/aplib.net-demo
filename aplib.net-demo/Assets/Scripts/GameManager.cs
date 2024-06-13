using System;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action Paused;
    public event Action Resumed;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            _instance ??= new GameObject("GameManager").AddComponent<GameManager>();
            return _instance;
        }
    }

    private void Start() => Resume();

    /// <summary>
    /// Pause the game and disable player input.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        Paused?.Invoke();
        if (InputManager.Instance) InputManager.Instance.DisablePlayerInput();
    }

    /// <summary>
    /// Unpause the game and enable player input.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1;
        Resumed?.Invoke();
        if (InputManager.Instance) InputManager.Instance.EnablePlayerInput();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Some warning text", MessageType.Warning);
    }
}
