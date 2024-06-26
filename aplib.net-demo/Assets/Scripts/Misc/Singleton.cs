// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton class that makes a single static instance of the class available.
/// </summary>
/// <typeparam name="T">
/// The type of the singleton.
/// </typeparam>
public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    /// <summary>
    /// The instance of the singleton.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (!_isInitialized)
            {
                _instance = FindObjectOfType<T>() ?? new GameObject(typeof(T).Name).AddComponent<T>();
                SceneManager.sceneUnloaded += _instance.OnSceneUnload;
                _isInitialized = true;
            }

            return _instance;
        }
    }

    private static T _instance;

    private static bool _isInitialized = false;

    public void Reset() => _isInitialized = false;

    protected virtual void Awake()
    {
        T[] instances = FindObjectsOfType<T>();
        if (instances.Length > 1)
            Debug.LogWarning($"There are {instances.Length} instances of {typeof(T).Name} in the scene, but only one is expected.");
    }

    private void OnSceneUnload(Scene scene)
    {
        SceneManager.sceneUnloaded -= OnSceneUnload;
        Reset();
    }
}
