using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] _footSteps;
    [SerializeField] private AudioClip[] _knifeSwings;
    [SerializeField] private AudioClip[] _crossbowShots;
    private AudioSource _audioSource;

    /// <summary>
    /// Sets up the AudioSource component.
    /// </summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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
}
