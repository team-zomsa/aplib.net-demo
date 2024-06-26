// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Entities;
using UnityEngine;

/// <summary>
/// This component respawns the entity when the player presses the respawn button.
/// </summary>
[RequireComponent(typeof(RespawnableComponent))]
public class RespawnWhenButtonPressed : MonoBehaviour
{
    private RespawnableComponent _respawnableComponent;

    private void Awake() => _respawnableComponent = GetComponent<RespawnableComponent>();

    private void OnEnable() => InputManager.Instance.Respawned += _respawnableComponent.Respawn;

    private void OnDisable() => InputManager.Instance.Respawned -= _respawnableComponent.Respawn;
}
