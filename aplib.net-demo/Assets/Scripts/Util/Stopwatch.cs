/// <summary>
/// Basic stopwatch class 
/// </summary>
public class Stopwatch
{
    private float _time;
    private readonly float _duration;

    public Stopwatch(float duration)
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
