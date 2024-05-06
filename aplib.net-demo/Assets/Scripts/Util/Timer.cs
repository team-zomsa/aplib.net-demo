
/// <summary>
/// Basic timer class 
/// </summary>
public class Timer
{
    private float _time;
    private readonly float _duration;

    public Timer(float duration)
    {
        _duration = duration;
    }

    public float GetTime()
    {
        return _time;
    }

    public void Update(float deltaTime)
    {
        _time += deltaTime;
    }

    public void Reset()
    {
        _time = 0;
    }

    public bool IsFinished()
    {
        return _time >= _duration;
    }
}
