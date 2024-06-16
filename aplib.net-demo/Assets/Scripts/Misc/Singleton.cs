using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    /// <summary>
    /// The instance of the singleton.
    /// </summary>
    public static T Instance
    {
        get
        {
            _instance ??= new GameObject(typeof(T).Name).AddComponent<T>();
            return _instance;
        }
    }

    private static T _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (this != _instance)
        {
            Debug.LogWarning($"Another instance of {typeof(T)} already exists. Destroying this instance.");
            Destroy(gameObject);
        }
    }
}
