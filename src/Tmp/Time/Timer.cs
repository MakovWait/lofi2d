namespace Tmp.Time;

public class Timer(float duration, bool repeating = false)
{
    private float _sec;
    private bool _justFinished;
    
    public bool Tick(float delta)
    {
        _justFinished = false;
        _sec += delta;
        if (Finished())
        {
            _justFinished = true;
            if (repeating)
            {
                _sec -= duration;
            }
        }
        return _justFinished;
    }

    public void Reset()
    {
        _sec = 0;
    }
    
    public bool JustFinished()
    {
        return _justFinished;
    }
    
    public bool Finished()
    {
        return _sec >= duration;
    }
}
