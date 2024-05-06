using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _minTime = 0.1f;
    [SerializeField] private float _maxTime = 0.15f;
    private float _countdown = 0f;

    /// <summary>
    /// Set the timer when the object is created.
    /// </summary>
    void Awake() => Reset();

    /// <summary>
    /// Reduce the timer by the time passed since the last frame.
    /// </summary>
    void Update() => _countdown -= Time.deltaTime;

    /// <summary>
    /// Reset the timer after the condition we want from outside this class has been reached.
    /// </summary>
    public void Reset() => _countdown = Random.Range(_minTime, _maxTime);

    /// <summary>
    /// Check if the timer has reached zero.
    /// </summary>
    public bool IsFinished() => _countdown <= 0;

    public void SetExactTime(float time) => _minTime = _maxTime = time;
}
