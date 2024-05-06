using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _minTime = 0.1f;
    [SerializeField] private float _maxTime = 0.15f;
    [HideInInspector] public float Countdown = 0f;

    /// <summary>
    /// Set the timer when the object is created.
    /// </summary>
    void Awake() => Reset();

    /// <summary>
    /// Reduce the timer by the time passed since the last frame.
    /// </summary>
    void Update() => Countdown -= Time.deltaTime;

    /// <summary>
    /// Reset the timer after the condition we want from outside this class has been reached.
    /// </summary>
    public void Reset() => Countdown = Random.Range(_minTime, _maxTime);
}
