using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Time;

namespace Tmp.Animation;

public class Tween(bool defaultRunning = true)
{
    public Signal Finished { get; } = new();
    public Signal<int> StepFinished { get; } = new(-1);
    public Signal<int> LoopFinished { get; } = new(-1);

    public int LoopsLeft => _loops == 0 ? -1 : _loops - _loopsDone;
    public float ElapsedTime => _elapsedTime;

    private readonly List<TweenStep> _steps = [];
    private float _elapsedTime;
    private int _currentStepIdx;
    private float _speedScale = 1f;
    private bool _ignoreTimeScale;
    private bool _running = defaultRunning;
    private bool _parallel;
    private bool _finished;
    private int _loopsDone;
    private int _loops = 1;
    private Easing.EaseType _easeType = Easing.EaseType.In;
    private Easing.TransitionType _transitionType = Easing.TransitionType.Linear;

    private TweenStep LastStep => _steps[^1];
    private TweenStep CurrentStep => _steps[_currentStepIdx];

    public bool Step(FrameTime time)
    {
        var delta = time.DeltaByScale(scaled: !_ignoreTimeScale) * _speedScale;
        return StepWithForcedDelta(delta);
    }

    public bool StepWithForcedDelta(float delta)
    {
        if (_finished)
        {
            return true;
        }

        if (!_running)
        {
            return false;
        }

        var remainingDelta = delta;
        _elapsedTime += remainingDelta;
#if DEBUG
        var initialDelta = remainingDelta;
        var potentialInfinite = false;
#endif
        while (remainingDelta > 0 && _running)
        {
            var stepDelta = remainingDelta;
            var stepFinished = CurrentStep.Step(ref stepDelta);
            remainingDelta = stepDelta;

            if (stepFinished)
            {
                StepFinished.Emit(_currentStepIdx);
                _currentStepIdx++;
                var hasMoreSteps = _currentStepIdx < _steps.Count;
                if (!hasMoreSteps)
                {
                    _loopsDone++;
                    if (_loopsDone == _loops)
                    {
                        _running = false;
                        MarkFinished(true);
                        break;
                    }
                    else
                    {
                        LoopFinished.Emit(_loopsDone);
                        _currentStepIdx = 0;
                        foreach (var step in _steps)
                        {
                            step.PrepareForReuse();
                        }
#if DEBUG
                        if (_loops <= 0 && Mathf.IsEqualApprox(remainingDelta, initialDelta))
                        {
                            if (!potentialInfinite)
                            {
                                potentialInfinite = true;
                            }
                            else
                            {
                                // Looped twice without using any time, this is 100% certain infinite loop.
                                throw new Exception(
                                    "Infinite loop detected. Check set_loops() description for more info."
                                );
                            }
                        }
#endif
                    }
                }
            }
        }

        return _finished;
    }
    
    public Tween SetEase(Easing.EaseType easeType)
    {
        _easeType = easeType;
        return this;
    }

    public Tween SetTrans(Easing.TransitionType transitionType)
    {
        _transitionType = transitionType;
        return this;
    }
    
    public Tween Chain()
    {
        _parallel = false;
        return this;
    }

    public Tween Parallel()
    {
        _parallel = true;
        return this;
    }

    public Tween SetSpeedScale(float scale)
    {
        _speedScale = scale;
        return this;
    }

    public Tween SetIgnoreTimeScale(bool ignoreTimeScale)
    {
        _ignoreTimeScale = ignoreTimeScale;
        return this;
    }

    public Tween SetLoops(int loops = 0)
    {
        _loops = loops;
        return this;
    }

    public void Play()
    {
        _running = true;
    }

    public void Pause()
    {
        _running = false;
    }

    public void Replay()
    {
        PrepareForReuse();
        Play();
    }
    
    public Tween Reset()
    {
        if (IsRunning())
        {
            Stop();
        }
        
        Finished.Reset();
        LoopFinished.Reset();
        StepFinished.Reset();

        _finished = false;
        _speedScale = 1f;
        _ignoreTimeScale = false;
        _running = defaultRunning;
        _elapsedTime = 0;
        _currentStepIdx = 0;
        _loopsDone = 0;
        _steps.Clear();
        _easeType = Easing.EaseType.In;
        _transitionType = Easing.TransitionType.Linear;
        _parallel = false;
        _loops = 1;
        
        return this;
    }
    
    public void PrepareForReuse()
    {
        MarkFinished(true);
        _running = true;
        _elapsedTime = 0;
        _currentStepIdx = 0;
        _loopsDone = 0;
        foreach (var step in _steps)
        {
            step.PrepareForReuse();
        }
        MarkFinished(false);
    }

    public void Stop()
    {
        if (_finished) return;
        _running = false;
        _elapsedTime = 0;
        MarkFinished(true);
    }

    public bool IsRunning()
    {
        return _running;
    }

    public void AddTweener(ITweener tweener)
    {
        if (!_parallel || _steps.Count == 0)
        {
            _steps.Add(new TweenStep());
        }

        LastStep.Add(tweener);
    }

    public IntervalTweener TweenInterval(float time)
    {
        var tweener = new IntervalTweener(time);
        AddTweener(tweener);
        return tweener;
    }

    public CallbackTweener TweenCallback(Action callback)
    {
        var tweener = new CallbackTweener(callback);
        AddTweener(tweener);
        return tweener;
    }
    
    public SubTweenTweener TweenSubTween(Tween subTween)
    {
        var tweener = new SubTweenTweener(subTween);
        AddTweener(tweener);
        return tweener;
    }
    
    public RangeTweener TweenRange(Action<float> callback, float duration)
    {
        var tweener = new RangeTweener(callback, duration, _transitionType, _easeType);
        AddTweener(tweener);
        return tweener;
    }

    private void MarkFinished(bool finished)
    {
        var shouldEmit = !_finished && finished;
        _finished = finished;
        if (shouldEmit)
        {
            Finished.Emit();
        }
    }

    private class TweenStep
    {
        private readonly List<ITweener> _tweeners = [];
        private bool _started;

        public void Add(ITweener tweener)
        {
            _tweeners.Add(tweener);
        }

        public bool Step(ref float stepDelta)
        {
            if (!_started)
            {
                Start();
            }

            var stepDeltaCopy = stepDelta;
            var finished = true;
            foreach (var tweener in _tweeners)
            {
                var tmpDelta = stepDeltaCopy;
                finished = tweener.Step(ref tmpDelta) && finished;
                stepDelta = Mathf.Min(tmpDelta, stepDelta);
            }

            return finished;
        }

        public void PrepareForReuse()
        {
            _started = false;
        }

        private void Start()
        {
            foreach (var tweener in _tweeners)
            {
                tweener.Start();
            }

            _started = true;
        }
    }
}


public static class TweenEx
{
    private static Tween BindTween(this INodeInit self, Tween tween, bool oneShot, FrameTime? time = null)
    {
        if (self.State == Node.NodeState.Building)
        {
            Setup();
        }
        else
        {
            self.CallDeferred(Setup);
        }
        
        return tween;

        void Setup()
        {
            time ??= self.UseTime();
            var callback = self.On<Update>(_ => tween.Step(time));
            if (oneShot)
            {
                var disposeCallback = new SignalTarget(() =>
                {
                    callback.Dispose();
                }).Deferred(self).OneShot();
                tween.Finished.Connect(disposeCallback);   
            }
        } 
    }
    
    public static Tween UseTween(this INodeInit self, FrameTime? time = null)
    {
        var tween = new Tween(false);
        return self.BindTween(tween, false, time);
    }
    
    public static Tween CreateOneShotTween(this INodeInit self, FrameTime? time = null)
    {
        var tween = new Tween(true);
        return self.BindTween(tween, true, time);
    }
    
    public static RangeTweener TweenMethod(this Tween self, Action<Vector2> setter, Vector2 from, Vector2 to, float duration)
    {
        return self.TweenRange(x => setter(from.Lerp(to, x)), duration);
    }
    
    public static RangeTweener TweenMethod(this Tween self, Action<float> setter, float from, float to, float duration)
    {
        return self.TweenRange(x => setter(Mathf.Lerp(from, to, x)), duration);
    }
    
    public static RangeTweener TweenMethod(this Tween self, Action<Color> setter, Color from, Color to, float duration)
    {
        return self.TweenRange(x => setter(from.Lerp(to, x)), duration);
    }
    
    public static RangeTweener TweenMethodAngle(this Tween self, Action<Degrees> setter, Degrees from, Degrees to, float duration)
    {
        return self.TweenRange(x => setter(Mathf.LerpAngle(from, to, x)), duration);
    }
    
    public static RangeTweener TweenMethodAngle(this Tween self, Action<Radians> setter, Radians from, Radians to, float duration)
    {
        return self.TweenRange(x => setter(Mathf.LerpAngle(from, to, x)), duration);
    }
}

public interface ITweener
{
    void Start();

    bool Step(ref float remainingDelta);
}

public class IntervalTweener(float duration) : ITweener
{
    private bool _finished;
    private float _elapsedTime = 0f;

    public void Start()
    {
        _finished = false;
        _elapsedTime = 0f;
    }

    public bool Step(ref float remainingDelta)
    {
        if (_finished)
        {
            return true;
        }

        _elapsedTime += remainingDelta;

        if (_elapsedTime < duration)
        {
            remainingDelta = 0;
            return false;
        }
        else
        {
            remainingDelta = _elapsedTime - duration;
            _finished = true;
            return true;
        }
    }
}

public class CallbackTweener(Action callback) : ITweener
{
    private bool _finished;
    private float _elapsedTime = 0f;
    private float _delay = 0f;

    public void Start()
    {
        _finished = false;
        _elapsedTime = 0f;
    }

    public CallbackTweener SetDelay(float delay)
    {
        _delay = delay;
        return this;
    }

    public bool Step(ref float remainingDelta)
    {
        if (_finished)
        {
            return true;
        }

        _elapsedTime += remainingDelta;

        if (_elapsedTime >= _delay)
        {
            callback();
            remainingDelta = _elapsedTime - _delay;
            _finished = true;
            return true;
        }
        else
        {
            remainingDelta = 0;
            return false;
        }
    }
}

public class RangeTweener(
    Action<float> action, 
    float duration,
    Easing.TransitionType initialTransitionType = Easing.TransitionType.Linear,
    Easing.EaseType initialeEaseType = Easing.EaseType.In
) : ITweener
{
    private bool _finished;
    private float _elapsedTime = 0f;
    private float _delay = 0f;
    private Easing.EaseType _easeType = initialeEaseType;
    private Easing.TransitionType _transitionType = initialTransitionType;

    public void Start()
    {
        _finished = false;
        _elapsedTime = 0f;
    }

    public RangeTweener SetDelay(float delay)
    {
        _delay = delay;
        return this;
    }

    public RangeTweener SetEase(Easing.EaseType value)
    {
        _easeType = value;
        return this;
    }

    public RangeTweener SetTrans(Easing.TransitionType transitionType)
    {
        _transitionType = transitionType;
        return this;
    }
    
    public bool Step(ref float remainingDelta)
    {
        if (_finished)
        {
            return true;
        }

        _elapsedTime += remainingDelta;

        if (_elapsedTime < _delay)
        {
            remainingDelta = 0;
            return false;
        }

        float currentValue;
        var time = Mathf.Min(_elapsedTime - _delay, duration);
        if (time < duration)
        {
            currentValue = Easing.Ease(_transitionType, _easeType, time, 0, 1, duration);
        }
        else
        {
            currentValue = 1;
        }

        action(currentValue);

        if (time < duration)
        {
            remainingDelta = 0;
            return false;
        }
        else
        {
            remainingDelta = _elapsedTime - _delay - duration;
            _finished = true;
            return true;
        }
    }
}

public class SubTweenTweener(Tween tween) : ITweener
{
    private bool _finished;
    private float _elapsedTime = 0f;
    private float _delay = 0f;

    public SubTweenTweener SetDelay(float delay)
    {
        _delay = delay;
        return this;   
    }
    
    public void Start() 
    {
        _finished = false;
        _elapsedTime = 0f;
        tween.PrepareForReuse();
    }

    public bool Step(ref float remainingDelta)
    {
        if (_finished)
        {
            return true;
        }
        
        _elapsedTime += remainingDelta;

        if (_elapsedTime < _delay)
        {
            remainingDelta = 0;
            return false;
        }

        if (tween.StepWithForcedDelta(remainingDelta))
        {
            remainingDelta = _elapsedTime - _delay - tween.ElapsedTime;;
            _finished = true;
            return true;
        }
        else
        {
            remainingDelta = 0;
            return false;
        }
    }
}