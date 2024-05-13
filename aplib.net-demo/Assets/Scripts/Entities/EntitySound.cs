using UnityEngine;

/// <summary>
/// Sound Effect class for Entities (Player + Enemies).
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(HealthComponent))]
public class EntitySound : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _footSteps;

    [SerializeField]
    private AudioClip[] _knifeSwings;

    [SerializeField]
    private AudioClip[] _crossbowShots;

    [SerializeField]
    private AudioClip[] _deathSounds;

    private AudioSource _audioSource;

    private HealthComponent _healthComponent;

    /// <summary>
    /// Sets up the AudioSource component.
    /// </summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _healthComponent = GetComponent<HealthComponent>();

        _healthComponent.Death += OnDeath;
    }

    /// <summary>
    /// Returns a random AudioClip from a given array containing AudioClips.
    /// </summary>
    /// <param name="source">The AudioClip array.</param>
    /// <returns>A random AudioClip from the array.</returns>
    private AudioClip GetRandomClip(AudioClip[] source)
        => source[Random.Range(0, source.Length)];

    /// <summary>
    /// Plays a random footstep sound.
    /// </summary>
    public void Step() => _audioSource.PlayOneShot(GetRandomClip(_footSteps));

    /// <summary>
    /// Plays a random knife swing sound.
    /// </summary>
    public void Swing() => _audioSource.PlayOneShot(GetRandomClip(_knifeSwings));

    /// <summary>
    /// Plays a random crossbow shot sound.
    /// </summary>
    public void Shoot() => _audioSource.PlayOneShot(GetRandomClip(_crossbowShots));

    /// <summary>
    /// Plays a random death sound.
    /// </summary>
    public void OnDeath(HealthComponent _) => _audioSource.PlayOneShot(GetRandomClip(_deathSounds));
}
