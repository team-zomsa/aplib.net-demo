using Entities;
using UnityEngine;

[RequireComponent(typeof(RespawnableComponent))]
public class RespawnWhenButtonPressed : MonoBehaviour
{
    private RespawnableComponent _respawnableComponent;

    private void Awake() => _respawnableComponent = GetComponent<RespawnableComponent>();

    private void OnEnable() => InputManager.Instance.Respawned += _respawnableComponent.Respawn;

    private void OnDisable() => InputManager.Instance.Respawned -= _respawnableComponent.Respawn;
}
