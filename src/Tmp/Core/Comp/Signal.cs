namespace Tmp.Core.Comp;

public class Signal<T>(T initial = default!)
{
    private T _state = initial;
    private readonly Targets _targets = new();

    public T Value => _state;
    
    public void Emit(T state)
    {
        _state = state;
        _targets.Handle(state);
    }

    public void Connect(ISignalTarget<T> target)
    {
        _targets.Add(target);
    }

    public void Disconnect(ISignalTarget<T> target)
    {
        _targets.Remove(target);
    }

    public void Reset()
    {
        _state = initial;
        _targets.Clear();
    }
    
    private class Targets
    {
        private readonly List<ISignalTarget<T>> _targets = [];
        private bool _freeze;
        
        public void Handle(T state)
        {
            Freeze(true);
            foreach (var target in _targets)
            {
                target.Handle(state);
            }
            Freeze(false);
        }
        
        public void Add(ISignalTarget<T> target)
        {
            EnsureIsUnfrozen();
            _targets.Add(target);
        }
        
        public void Remove(ISignalTarget<T> target)
        {
            EnsureIsUnfrozen();
            _targets.Remove(target);
        }
        
        private void Freeze(bool freeze)
        {
            _freeze = freeze;
        }
        
        private void EnsureIsUnfrozen()
        {
            if (_freeze)
            {
                throw new InvalidOperationException("Cannot modify signal targets while it is frozen.");
            }
        }

        public void Clear()
        {
            EnsureIsUnfrozen();
            _targets.Clear();       
        }
    }
}

public static class SignalNodeEx
{
    public static void UseSignal<T>(this INodeInit self, Signal<T> signal, ISignalTarget<T> target)
    {
        signal.Connect(target);
        self.OnCleanup(() => signal.Disconnect(target));
    }
}

public interface ISignalTarget<T>
{
    public void Handle(T state);
    
    public class Deferred(ISignalTarget<T> origin, ICallDeferredSource deferredSource) : ISignalTarget<T>
    {
        public void Handle(T state)
        {
            deferredSource.CallDeferred(origin.Handle, state);
        }
    }
    
    public class Throttled(ISignalTarget<T> origin, ICallDeferredSource deferredSource) : ISignalTarget<T>
    {
        private T _lastState;
        private bool _queued;
        
        public void Handle(T state)
        {
            _lastState = state;
            if (_queued) return;
            _queued = true;
            deferredSource.CallDeferred(ThrottleHandle, new Empty());
        }

        private void ThrottleHandle(Empty _)
        {
            origin.Handle(_lastState);
            _lastState = default;
            _queued = false;
        }
    }
    
    // TODO impl it in a proper way
    public class OneShot(ISignalTarget<T> origin) : ISignalTarget<T>
    {
        private bool _handled;
        
        public void Handle(T state)
        {
            if (_handled) return;
            origin.Handle(state);
            _handled = true;
        }
    }
}

public static class SignalEx
{
    public static ISignalTarget<T> Connect<T>(this Signal<T> origin, Action<T> handler)
    {
        var target = new SignalTarget<T>(handler);
        origin.Connect(target);
        return target;
    }
    
    public static ISignalTarget<Empty> Connect(this Signal origin, Action handler)
    {
        var target = new SignalTarget(handler);
        origin.Connect(target);
        return target;
    }
}

public static class ISignalTargetEx
{
    public static ISignalTarget<T> Throttled<T>(this ISignalTarget<T> origin, ICallDeferredSource deferredSource)
    {
        return new ISignalTarget<T>.Throttled(origin, deferredSource);
    }
    
    public static ISignalTarget<T> Deferred<T>(this ISignalTarget<T> origin, ICallDeferredSource deferredSource)
    {
        return new ISignalTarget<T>.Deferred(origin, deferredSource);
    }    
    
    public static ISignalTarget<T> OneShot<T>(this ISignalTarget<T> origin)
    {
        return new ISignalTarget<T>.OneShot(origin);
    }
}

public class SignalTarget<T>(Action<T> action) : ISignalTarget<T>
{
    public void Handle(T state)
    {
        action(state);
    }
}

public class SignalTarget(Action action) : ISignalTarget<Empty>
{
    public void Handle(Empty state)
    {
        action();
    }
}

public class Signal : Signal<Empty>
{
    public new void Emit(Empty state = default)
    {
        base.Emit(state);
    }
}
