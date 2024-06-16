using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisableOnPause : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour _componentToDisable;

    private void OnEnable()
    {
        GameManager.Instance.Paused += OnPaused;
        GameManager.Instance.Resumed += OnResumed;
    }

    private void OnDisable()
    {
        GameManager.Instance.Paused -= OnPaused;
        GameManager.Instance.Resumed -= OnResumed;
    }

    private void OnPaused() => _componentToDisable.enabled = false;

    private void OnResumed() => _componentToDisable.enabled = true;
}
